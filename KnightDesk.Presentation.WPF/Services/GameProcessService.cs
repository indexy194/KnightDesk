using KnightDesk.Presentation.WPF.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;

namespace KnightDesk.Presentation.WPF.Services
{
    public interface IGameProcessService
    {
        void StartGame(Account account, string gamePath, Action<bool> callback = null);
        void StopGame(Account account, Action<bool> callback = null);
        void SendCommandToPipe(int accountId, string command, Action<bool> callback = null);
        void CheckGameConnection(Account account, Action<bool> callback = null);
        void SendLoginCommand(Account account, Action<string> characterNameCallback = null);
        void SendCommandWithResponse(int accountId, string command, Action<string> responseCallback = null);
    }

    public class GameProcessService : IGameProcessService
    {
        public void StartGame(Account account, string gamePath, Action<bool> callback = null)
        {
            if (string.IsNullOrEmpty(gamePath) || !File.Exists(gamePath))
            {
                if (callback != null) callback(false);
                return;
            }
            
            // Check if game is already running
            if (account.IsGameRunning && account.GameProcess != null && !account.GameProcess.HasExited)
            {
                if (callback != null) callback(false);
                return;
            }

            ThreadPool.QueueUserWorkItem(_ =>
            {
                bool result = false;
                try
                {
                    var processStartInfo = new ProcessStartInfo
                    {
                        FileName = gamePath,
                        UseShellExecute = false,
                        CreateNoWindow = false,
                        Arguments = string.Format("-account {0}", account.Id) // .NET 3.5 compatible
                    };

                    var process = Process.Start(processStartInfo);
                    if (process != null)
                    {
                        account.GameProcess = process;
                        account.IsGameRunning = true;
                        account.GameStatus = "Starting...";

                        // Monitor process in background
                        ThreadPool.QueueUserWorkItem(__ => MonitorGameProcess(account));

                        // Send login command after a delay to ensure game is ready
                        ThreadPool.QueueUserWorkItem(___ => 
                        {
                            Thread.Sleep(5000); // Wait 5 seconds for game to initialize
                            SendLoginCommand(account, (characterName) =>
                            {
                                if (!string.IsNullOrEmpty(characterName))
                                {
                                    account.GameStatus = string.Format("Logged in as {0}", characterName);
                                }
                                else
                                {
                                    account.GameStatus = "Login failed";
                                }
                            });
                        });

                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    account.GameStatus = string.Format("Error: {0}", ex.Message);
                    result = false;
                }
                
                if (callback != null) callback(result);
            });
        }

        public void StopGame(Account account, Action<bool> callback = null)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                bool result = false;
                try
                {
                    if (account.GameProcess != null && !account.GameProcess.HasExited)
                    {
                        // Try graceful shutdown first
                        SendCommandToPipeSync(account.Id, "SHUTDOWN");

                        // Wait a moment for graceful shutdown
                        Thread.Sleep(2000);

                        // Force kill if still running
                        if (!account.GameProcess.HasExited)
                        {
                            account.GameProcess.Kill();
                        }

                        account.GameProcess.Dispose();
                        account.GameProcess = null;
                    }

                    account.IsGameRunning = false;
                    account.IsConnectedToGame = false;
                    account.GameStatus = "Offline";

                    result = true;
                }
                catch (Exception ex)
                {
                    account.GameStatus = string.Format("Error stopping: {0}", ex.Message);
                    result = false;
                }
                
                if (callback != null) callback(result);
            });
        }

        public void SendCommandToPipe(int accountId, string command, Action<bool> callback = null)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                bool result = SendCommandToPipeSync(accountId, command);
                if (callback != null) callback(result);
            });
        }

        public void CheckGameConnection(Account account, Action<bool> callback = null)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                bool result = false;
                try
                {
                    if (!account.IsGameRunning || account.GameProcess == null || account.GameProcess.HasExited)
                    {
                        account.IsConnectedToGame = false;
                        result = false;
                    }
                    else
                    {
                        // Try to send a ping command to check connection
                        bool connected = SendCommandToPipeSync(account.Id, "PING");
                        account.IsConnectedToGame = connected;

                        if (connected)
                        {
                            account.GameStatus = "In-game";
                        }
                        else
                        {
                            account.GameStatus = "Running (No connection)";
                        }

                        result = connected;
                    }
                }
                catch (Exception)
                {
                    account.IsConnectedToGame = false;
                    account.GameStatus = "Connection error";
                    result = false;
                }
                
                if (callback != null) callback(result);
            });
        }

        // Method to send login command and wait for character name response
        public void SendLoginCommand(Account account, Action<string> characterNameCallback = null)
        {
            if (account == null)
            {
                if (characterNameCallback != null) characterNameCallback(null);
                return;
            }

            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    // Wait a moment for game to fully start
                    Thread.Sleep(3000);

                    string loginCommand = string.Format("LOGIN|{0}|{1}|{2}|{3}", 
                        account.Username, 
                        account.Password, 
                        account.IndexServer, 
                        account.IndexCharacter);

                    SendCommandWithResponse(account.Id, loginCommand, (response) =>
                    {
                        if (!string.IsNullOrEmpty(response) && response.StartsWith("CHARACTER_NAME|"))
                        {
                            string characterName = response.Substring("CHARACTER_NAME|".Length);
                            account.CharacterName = characterName;
                            
                            // Update via API
                            UpdateCharacterNameViaAPI(account, characterName);
                            
                            if (characterNameCallback != null) characterNameCallback(characterName);
                        }
                        else
                        {
                            if (characterNameCallback != null) characterNameCallback(null);
                        }
                    });
                }
                catch
                {
                    // Log error
                    if (characterNameCallback != null) characterNameCallback(null);
                }
            });
        }

        // Method to send command and wait for response
        public void SendCommandWithResponse(int accountId, string command, Action<string> responseCallback = null)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                string response = SendCommandToPipeSyncWithResponse(accountId, command);
                if (responseCallback != null) responseCallback(response);
            });
        }

        // Synchronous version for internal use
        private bool SendCommandToPipeSync(int accountId, string command)
        {
            try
            {
                var pipeName = string.Format("KnightDesk_Account_{0}", accountId);

                using (var pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.Out))
                {
                    pipeClient.Connect(1000); // 1 second timeout

                    var commandBytes = Encoding.UTF8.GetBytes(command);
                    pipeClient.Write(commandBytes, 0, commandBytes.Length);
                    pipeClient.Flush();

                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Synchronous version with response for internal use
        private string SendCommandToPipeSyncWithResponse(int accountId, string command)
        {
            try
            {
                var pipeName = string.Format("KnightDesk_Account_{0}", accountId);

                using (var pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut))
                {
                    pipeClient.Connect(5000); // 5 second timeout for login

                    // Send command
                    var commandBytes = Encoding.UTF8.GetBytes(command);
                    pipeClient.Write(commandBytes, 0, commandBytes.Length);
                    pipeClient.Flush();

                    // Read response
                    byte[] buffer = new byte[1024];
                    int bytesRead = pipeClient.Read(buffer, 0, buffer.Length);
                    
                    if (bytesRead > 0)
                    {
                        return Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    }
                }
            }
            catch (Exception)
            {
                // Log error if needed
            }
            return null;
        }

        // Event to notify when character name is received
        public static event Action<Account, string> CharacterNameReceived;

        // Method to update character name via API
        private void UpdateCharacterNameViaAPI(Account account, string characterName)
        {
            // Raise event for ViewModel to handle API update
            if (CharacterNameReceived != null)
            {
                CharacterNameReceived(account, characterName);
            }
        }

        private void MonitorGameProcess(Account account)
        {
            try
            {
                while (account.GameProcess != null && !account.GameProcess.HasExited)
                {
                    Thread.Sleep(5000); // Check every 5 seconds
                    
                    // Simple synchronous connection check to avoid nested threading
                    try
                    {
                        bool connected = SendCommandToPipeSync(account.Id, "PING");
                        account.IsConnectedToGame = connected;
                        
                        if (connected)
                        {
                            account.GameStatus = "In-game";
                        }
                        else
                        {
                            account.GameStatus = "Running (No connection)";
                        }
                    }
                    catch (Exception)
                    {
                        account.IsConnectedToGame = false;
                        account.GameStatus = "Connection error";
                    }
                }

                // Process has exited
                if (account.GameProcess != null)
                {
                    account.GameProcess.Dispose();
                    account.GameProcess = null;
                }

                account.IsGameRunning = false;
                account.IsConnectedToGame = false;
                account.GameStatus = "Offline";
            }
            catch (Exception)
            {
                // Handle monitoring errors silently
                account.IsGameRunning = false;
                account.IsConnectedToGame = false;
                account.GameStatus = "Monitoring error";
            }
        }
    }
}
