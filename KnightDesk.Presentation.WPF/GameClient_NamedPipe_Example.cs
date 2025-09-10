// This is an example of how the game client should implement named pipes
// This file should NOT be included in the actual build - it's for reference only

using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameClient.Examples
{
    public class NamedPipeGameClient
    {
        private NamedPipeServerStream _pipeServer;
        private CancellationTokenSource _cancellationTokenSource;
        private int _accountId;

        public NamedPipeGameClient(int accountId)
        {
            _accountId = accountId;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task StartListening()
        {
            var pipeName = $"KnightDesk_Account_{_accountId}";
            
            try
            {
                _pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.In);
                
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    await _pipeServer.WaitForConnectionAsync(_cancellationTokenSource.Token);
                    
                    // Read command from pipe
                    var buffer = new byte[1024];
                    int bytesRead = await _pipeServer.ReadAsync(buffer, 0, buffer.Length);
                    
                    if (bytesRead > 0)
                    {
                        string command = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        await ProcessCommand(command);
                    }
                    
                    _pipeServer.Disconnect();
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Named pipe error: {ex.Message}");
            }
        }

        private async Task ProcessCommand(string command)
        {
            var parts = command.Split(':');
            if (parts.Length < 2) return;

            string action = parts[0];
            string value = parts[1];

            switch (action)
            {
                case "AUTO_STATE":
                    bool autoState = bool.Parse(value);
                    // Implement auto state logic
                    SetAutoState(autoState);
                    break;

                case "AUTO_EVENT":
                    bool autoEvent = bool.Parse(value);
                    // Implement auto event logic
                    SetAutoEvent(autoEvent);
                    break;

                case "AUTO_EQUIP":
                    bool autoEquip = bool.Parse(value);
                    // Implement auto equip logic
                    SetAutoEquip(autoEquip);
                    break;

                case "PING":
                    // Respond to ping - connection check
                    break;

                case "SHUTDOWN":
                    // Graceful shutdown
                    Environment.Exit(0);
                    break;
            }
        }

        private void SetAutoState(bool enabled)
        {
            // Game-specific implementation
            // Example: Enable/disable auto state functionality
        }

        private void SetAutoEvent(bool enabled)
        {
            // Game-specific implementation
            // Example: Enable/disable auto event functionality
        }

        private void SetAutoEquip(bool enabled)
        {
            // Game-specific implementation
            // Example: Enable/disable auto equip functionality
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
            _pipeServer?.Close();
            _pipeServer?.Dispose();
        }
    }

    // Example usage in game client main:
    public class GameMain
    {
        public static void Main(string[] args)
        {
            int accountId = 0;
            
            // Parse account ID from command line arguments
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-account" && i + 1 < args.Length)
                {
                    int.TryParse(args[i + 1], out accountId);
                    break;
                }
            }

            if (accountId > 0)
            {
                var pipeClient = new NamedPipeGameClient(accountId);
                
                // Start listening for commands in background
                Task.Run(() => pipeClient.StartListening());
            }

            // Continue with normal game initialization...
        }
    }
}
