using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

/// <summary>
/// TCP Client Manager for communicating with KnightDesk tool
/// Replaces the old IpcManager that used Named Pipes
/// </summary>
public class TcpClientManager
{
    private static TcpClientManager instance;
    
    // Single account ID for this game client instance
    private static int currentAccountId = -1;
    private static int serverPort = GameClientConstants.TcpConstants.DEFAULT_TCP_PORT;
    
    private TcpClient tcpClient;
    private NetworkStream networkStream;
    private Thread listenerThread;
    private bool isConnected = false;
    private bool isListening = false;

    private static LoginData receivedLoginData;
    private static bool hasNewData = false;

    // Response callback for sending data back to KnightDesk tool
    private static Action<string> responseCallback;

    public static LoginData ReceivedLoginData
    {
        get { return receivedLoginData; }
    }

    public static TcpClientManager gI()
    {
        if (instance == null)
            instance = new TcpClientManager();
        return instance;
    }

    /// <summary>
    /// Start TCP client connection to KnightDesk server
    /// </summary>
    /// <param name="accountId">Account ID for this client instance</param>
    /// <param name="port">TCP port of KnightDesk server (optional)</param>
    public void StartTcpClient(int accountId, int port = -1)
    {
        try
        {
            MyLogTxt.WriteLog($"Starting TCP Client Manager for account {accountId}...");
            
            if (isConnected)
            {
                MyLogTxt.WriteLog($"TCP Client already connected for account {currentAccountId}");
                return;
            }

            currentAccountId = accountId;
            if (port > 0)
            {
                serverPort = port;
            }

            MyLogTxt.WriteLog($"Connecting to KnightDesk server at {GameClientConstants.TcpConstants.TCP_HOST}:{serverPort}...");

            // Connect to KnightDesk TCP server
            tcpClient = new TcpClient();
            tcpClient.Connect(GameClientConstants.TcpConstants.TCP_HOST, serverPort);
            networkStream = tcpClient.GetStream();
            isConnected = true;

            MyLogTxt.WriteLog($"Connected to KnightDesk server successfully!");

            // Send registration message
            string registerMessage = $"{GameClientConstants.GameCommandConstants.REGISTER_COMMAND}{GameClientConstants.GameCommandConstants.COMMAND_SEPARATOR}{accountId}";
            byte[] data = Encoding.UTF8.GetBytes(registerMessage);
            networkStream.Write(data, 0, data.Length);
            networkStream.Flush();

            MyLogTxt.WriteLog($"Sent registration message: {registerMessage}");

            // Wait for registration confirmation
            byte[] response = new byte[GameClientConstants.TcpConstants.DEFAULT_BUFFER_SIZE];
            int bytesRead = networkStream.Read(response, 0, response.Length);
            string responseStr = Encoding.UTF8.GetString(response, 0, bytesRead);

            if (responseStr == GameClientConstants.GameCommandConstants.REGISTERED_OK_RESPONSE)
            {
                MyLogTxt.WriteLog("Successfully registered with KnightDesk server");
                
                // Start listening for commands
                isListening = true;
                listenerThread = new Thread(ListenForCommands);
                listenerThread.IsBackground = true;
                listenerThread.Start();

                MyLogTxt.WriteLog($"TCP Client Manager initialized for account {accountId}");
            }
            else
            {
                MyLogTxt.WriteLog($"Registration failed. Response: {responseStr}");
                Disconnect();
            }
        }
        catch (Exception e)
        {
            MyLogTxt.WriteLog($"Error starting TCP client: {e.Message}");
            Disconnect();
        }
    }

    /// <summary>
    /// Stop TCP client connection
    /// </summary>
    public void StopTcpClient()
    {
        try
        {
            MyLogTxt.WriteLog("Starting TCP Client shutdown...");

            isListening = false;
            isConnected = false;

            // Send shutdown request to KnightDesk tool
            if (tcpClient != null && tcpClient.Connected)
            {
                try
                {
                    string shutdownMessage = GameClientConstants.GameCommandConstants.CLIENT_SHUTDOWN_COMMAND;
                    byte[] data = Encoding.UTF8.GetBytes(shutdownMessage);
                    networkStream?.Write(data, 0, data.Length);
                    networkStream?.Flush();
                    MyLogTxt.WriteLog("Sent shutdown request to KnightDesk tool");
                }
                catch (Exception e)
                {
                    MyLogTxt.WriteLog($"Error sending shutdown request: {e.Message}");
                }
            }

            // Close connection
            networkStream?.Close();
            tcpClient?.Close();

            if (listenerThread != null && listenerThread.IsAlive)
            {
                if (!listenerThread.Join(2000))
                {
                    MyLogTxt.WriteLog("TCP listener thread did not stop gracefully");
                }
                else
                {
                    MyLogTxt.WriteLog("TCP listener thread terminated gracefully");
                }
            }

            networkStream = null;
            tcpClient = null;

            MyLogTxt.WriteLog("TCP Client stopped successfully");
        }
        catch (Exception e)
        {
            MyLogTxt.WriteLog($"Error during TCP shutdown: {e.Message}");
        }
    }

    /// <summary>
    /// Disconnect from server
    /// </summary>
    private void Disconnect()
    {
        try
        {
            isListening = false;
            isConnected = false;

            networkStream?.Close();
            tcpClient?.Close();

            networkStream = null;
            tcpClient = null;

            MyLogTxt.WriteLog("Disconnected from KnightDesk server");
        }
        catch (Exception e)
        {
            MyLogTxt.WriteLog($"Error during disconnect: {e.Message}");
        }
    }

    /// <summary>
    /// Listen for commands from KnightDesk server
    /// </summary>
    private void ListenForCommands()
    {
        byte[] buffer = new byte[GameClientConstants.TcpConstants.DEFAULT_BUFFER_SIZE];

        while (isListening && isConnected && tcpClient != null && tcpClient.Connected)
        {
            try
            {
                int bytesRead = networkStream.Read(buffer, 0, buffer.Length);

                if (bytesRead > 0)
                {
                    string command = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    MyLogTxt.WriteLog($"Received command from KnightDesk: {command}");

                    // Process command and send response
                    string response = ProcessCommand(command);
                    if (!string.IsNullOrEmpty(response))
                    {
                        SendResponse(response);
                    }
                }
                else
                {
                    // Connection lost
                    MyLogTxt.WriteLog("Connection lost to KnightDesk server");
                    break;
                }
            }
            catch (Exception ex)
            {
                if (isListening)
                {
                    MyLogTxt.WriteLog($"Error reading from server: {ex.Message}");
                }
                break;
            }
        }

        // Connection lost
        Disconnect();
    }

    /// <summary>
    /// Process commands received from KnightDesk server
    /// </summary>
    /// <param name="command">Command to process</param>
    /// <returns>Response to send back</returns>
    private string ProcessCommand(string command)
    {
        try
        {
            // Handle different command types
            if (command == GameClientConstants.GameCommandConstants.PING_COMMAND)
            {
                return GameClientConstants.GameCommandConstants.PONG_RESPONSE;
            }
            else if (command == GameClientConstants.GameCommandConstants.SHUTDOWN_COMMAND)
            {
                // Graceful shutdown initiated by KnightDesk tool
                MyLogTxt.WriteLog("Received shutdown command from KnightDesk tool");
                StopTcpClient();
                return GameClientConstants.GameCommandConstants.SHUTDOWN_OK_RESPONSE;
            }
            else if (command.StartsWith(GameClientConstants.GameCommandConstants.LOGIN_COMMAND + GameClientConstants.GameCommandConstants.COMMAND_SEPARATOR))
            {
                return ProcessLoginCommand(command);
            }
            else if (command.StartsWith(GameClientConstants.GameCommandConstants.AUTO_STATE_COMMAND + GameClientConstants.GameCommandConstants.COMMAND_SEPARATOR))
            {
                return ProcessAutoStateCommand(command);
            }
            else if (command.StartsWith(GameClientConstants.GameCommandConstants.AUTO_EVENT_COMMAND + GameClientConstants.GameCommandConstants.COMMAND_SEPARATOR))
            {
                return ProcessAutoEventCommand(command);
            }
            else if (command.StartsWith(GameClientConstants.GameCommandConstants.AUTO_EQUIP_COMMAND + GameClientConstants.GameCommandConstants.COMMAND_SEPARATOR))
            {
                return ProcessAutoEquipCommand(command);
            }
            // Legacy commands for backward compatibility
            else if (command.StartsWith(GameClientConstants.GameCommandConstants.AUTO_TOOL_COMMAND + GameClientConstants.GameCommandConstants.COMMAND_SEPARATOR))
            {
                return ProcessLegacyAutoToolCommand(command);
            }
            else if (command.StartsWith(GameClientConstants.GameCommandConstants.FOCUS_NAME_COMMAND + GameClientConstants.GameCommandConstants.COMMAND_SEPARATOR))
            {
                return ProcessFocusNameCommand(command);
            }
            else if (command == GameClientConstants.GameCommandConstants.DISCONNECT_COMMAND)
            {
                MyLogTxt.WriteLog("Received disconnect command");
                return GameClientConstants.GameCommandConstants.SHUTDOWN_OK_RESPONSE;
            }

            return GameClientConstants.GameCommandConstants.UNKNOWN_COMMAND_RESPONSE;
        }
        catch (Exception ex)
        {
            MyLogTxt.WriteLog($"Error processing command: {ex.Message}");
            return GameClientConstants.GameCommandConstants.ERROR_RESPONSE;
        }
    }

    /// <summary>
    /// Process LOGIN command
    /// </summary>
    private string ProcessLoginCommand(string command)
    {
        string[] parts = command.Split(GameClientConstants.GameCommandConstants.COMMAND_SEPARATOR[0]);
        if (parts.Length >= 5)
        {
            receivedLoginData = new LoginData
            {
                Username = parts[1],
                Password = parts[2],
                CharacterIndex = int.Parse(parts[3]),
                ServerIndex = sbyte.Parse(parts[4])
            };

            hasNewData = true;
            MyLogTxt.WriteLog($"Login data received: {receivedLoginData.Username}, Server: {receivedLoginData.ServerIndex}, Char: {receivedLoginData.CharacterIndex}");

            // Process login immediately
            AutoManager.gI().ProcessLogin(currentAccountId);

            // Return character name response (you'll need to implement this based on your game logic)
            string characterName = GetCharacterName(receivedLoginData.Username, receivedLoginData.CharacterIndex);
            if (!string.IsNullOrEmpty(characterName))
            {
                return $"{GameClientConstants.GameCommandConstants.CHARACTER_NAME_PREFIX}{characterName}";
            }
            else
            {
                return GameClientConstants.GameCommandConstants.LOGIN_FAILED_RESPONSE;
            }
        }

        return GameClientConstants.GameCommandConstants.ERROR_RESPONSE;
    }

    /// <summary>
    /// Process AUTO_STATE command
    /// </summary>
    private string ProcessAutoStateCommand(string command)
    {
        string[] parts = command.Split(GameClientConstants.GameCommandConstants.COMMAND_SEPARATOR[0]);
        if (parts.Length >= 2)
        {
            bool enabled = bool.Parse(parts[1]);
            MyVariable.AutoToolEnabled = enabled;
            MyLogTxt.WriteLog($"Auto Tool state set to: {enabled}");
            return GameClientConstants.GameCommandConstants.PONG_RESPONSE;
        }
        return GameClientConstants.GameCommandConstants.ERROR_RESPONSE;
    }

    /// <summary>
    /// Process AUTO_EVENT command
    /// </summary>
    private string ProcessAutoEventCommand(string command)
    {
        string[] parts = command.Split(GameClientConstants.GameCommandConstants.COMMAND_SEPARATOR[0]);
        if (parts.Length >= 3)
        {
            bool enabled = bool.Parse(parts[1]);
            string eventType = parts[2];
            MyVariable.AutoEventEnabled = enabled;
            MyVariable.AutoEventType = eventType;
            MyLogTxt.WriteLog($"Auto Event state set to: {enabled}, Type: {eventType}");
            return GameClientConstants.GameCommandConstants.PONG_RESPONSE;
        }
        return GameClientConstants.GameCommandConstants.ERROR_RESPONSE;
    }

    /// <summary>
    /// Process AUTO_EQUIP command
    /// </summary>
    private string ProcessAutoEquipCommand(string command)
    {
        string[] parts = command.Split(GameClientConstants.GameCommandConstants.COMMAND_SEPARATOR[0]);
        if (parts.Length >= 3)
        {
            bool enabled = bool.Parse(parts[1]);
            string equipType = parts[2];
            MyVariable.AutoEquipEnabled = enabled;
            MyVariable.AutoEquipType = equipType;
            MyLogTxt.WriteLog($"Auto Equip state set to: {enabled}, Type: {equipType}");
            return GameClientConstants.GameCommandConstants.PONG_RESPONSE;
        }
        return GameClientConstants.GameCommandConstants.ERROR_RESPONSE;
    }

    /// <summary>
    /// Process legacy AUTO_TOOL command
    /// </summary>
    private string ProcessLegacyAutoToolCommand(string command)
    {
        string[] parts = command.Split(GameClientConstants.GameCommandConstants.COMMAND_SEPARATOR[0]);
        if (parts.Length >= 2)
        {
            bool enabled = bool.Parse(parts[1]);
            MyVariable.AutoToolEnabled = enabled;
            MyLogTxt.WriteLog($"Auto Tool state set to: {enabled} (legacy command)");
            return GameClientConstants.GameCommandConstants.PONG_RESPONSE;
        }
        return GameClientConstants.GameCommandConstants.ERROR_RESPONSE;
    }

    /// <summary>
    /// Process FOCUS_NAME command
    /// </summary>
    private string ProcessFocusNameCommand(string command)
    {
        string[] parts = command.Split(GameClientConstants.GameCommandConstants.COMMAND_SEPARATOR[0]);
        if (parts.Length >= 2)
        {
            string focusName = parts[1];
            MyVariable.FocusName = focusName;
            MyLogTxt.WriteLog($"Focus name set to: {focusName}");
            return GameClientConstants.GameCommandConstants.PONG_RESPONSE;
        }
        return GameClientConstants.GameCommandConstants.ERROR_RESPONSE;
    }

    /// <summary>
    /// Send response back to KnightDesk server
    /// </summary>
    /// <param name="response">Response message</param>
    private void SendResponse(string response)
    {
        if (isConnected && networkStream != null)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(response);
                networkStream.Write(data, 0, data.Length);
                networkStream.Flush();
                MyLogTxt.WriteLog($"Sent response: {response}");
            }
            catch (Exception ex)
            {
                MyLogTxt.WriteLog($"Failed to send response: {ex.Message}");
                Disconnect();
            }
        }
    }

    /// <summary>
    /// Get character name after login (implement your game logic here)
    /// </summary>
    /// <param name="username">Username</param>
    /// <param name="characterIndex">Character index</param>
    /// <returns>Character name</returns>
    private string GetCharacterName(string username, int characterIndex)
    {
        // Implement your game's character selection logic here
        // This is just an example - you'll need to integrate with your actual game logic
        return $"Character_{username}_{characterIndex}";
    }

    /// <summary>
    /// Check if login data is available
    /// </summary>
    /// <returns>True if login data is available</returns>
    public static bool IsDataAvailable()
    {
        bool available = hasNewData && receivedLoginData != null;
        if (available)
        {
            MyLogTxt.WriteLog($"TCP Data is available: {receivedLoginData.Username}");
        }
        return available;
    }

    /// <summary>
    /// Consume login data after processing
    /// </summary>
    public static void ConsumeLoginData()
    {
        if (receivedLoginData != null)
        {
            MyLogTxt.WriteLog($"Consuming login data for: {receivedLoginData.Username}");
        }
        hasNewData = false;
        receivedLoginData = null;
        MyLogTxt.WriteLog("Login data consumed and cleared");
    }

    /// <summary>
    /// Check if TCP client is connected
    /// </summary>
    /// <returns>True if connected</returns>
    public bool IsConnected()
    {
        return isConnected && tcpClient != null && tcpClient.Connected;
    }

    /// <summary>
    /// Get current account ID
    /// </summary>
    /// <returns>Current account ID</returns>
    public static int GetCurrentAccountId()
    {
        return currentAccountId;
    }
}
