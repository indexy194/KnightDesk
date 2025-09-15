using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using KnightDesk.Presentation.WPF.Constants;

namespace GameClientExample
{
    /// <summary>
    /// Example implementation of TCP client for .NET 3.5 game integration
    /// This code should be integrated into your game client
    /// </summary>
    public class KnightDeskTcpClient
    {
        private TcpClient _tcpClient;
        private NetworkStream _stream;
        private bool _isConnected = false;
        private int _accountId;
        private int _serverPort;
        private Thread _listenThread;

        public event Action<string> CommandReceived;
        public event Action<bool> ConnectionStatusChanged;

        public bool IsConnected
        {
            get { return _isConnected; }
        }

        /// <summary>
        /// Initialize TCP client with account ID and server port
        /// </summary>
        /// <param name="accountId">Account ID passed from command line arguments</param>
        /// <param name="serverPort">TCP port of KnightDesk server (default: 8888)</param>
        public void Initialize(int accountId, int serverPort = TcpConstants.DEFAULT_TCP_PORT)
        {
            _accountId = accountId;
            _serverPort = serverPort;
        }

        /// <summary>
        /// Connect to KnightDesk TCP server
        /// </summary>
        public bool Connect()
        {
            try
            {
                _tcpClient = new TcpClient();
                _tcpClient.Connect(TcpConstants.TCP_HOST, _serverPort);
                _stream = _tcpClient.GetStream();
                _isConnected = true;

                // Send registration message
                string registerMessage = string.Format("{0}{1}{2}", 
                    GameCommandConstants.REGISTER_COMMAND, 
                    GameCommandConstants.COMMAND_SEPARATOR, 
                    _accountId);
                byte[] data = Encoding.UTF8.GetBytes(registerMessage);
                _stream.Write(data, 0, data.Length);
                _stream.Flush();

                // Wait for registration confirmation
                byte[] response = new byte[GameConfigConstants.DEFAULT_BUFFER_SIZE];
                int bytesRead = _stream.Read(response, 0, response.Length);
                string responseStr = Encoding.UTF8.GetString(response, 0, bytesRead);

                if (responseStr == GameCommandConstants.REGISTERED_OK_RESPONSE)
                {
                    Console.WriteLine("Successfully registered with KnightDesk server");
                    
                    // Start listening for commands
                    _listenThread = new Thread(ListenForCommands);
                    _listenThread.IsBackground = true;
                    _listenThread.Start();

                    ConnectionStatusChanged?.Invoke(true);
                    return true;
                }
                else
                {
                    Disconnect();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to connect to KnightDesk server: " + ex.Message);
                Disconnect();
                return false;
            }
        }

        /// <summary>
        /// Disconnect from KnightDesk server
        /// </summary>
        public void Disconnect()
        {
            try
            {
                _isConnected = false;

                if (_listenThread != null && _listenThread.IsAlive)
                {
                    _listenThread.Abort();
                }

                if (_stream != null)
                {
                    _stream.Close();
                    _stream = null;
                }

                if (_tcpClient != null)
                {
                    _tcpClient.Close();
                    _tcpClient = null;
                }

                ConnectionStatusChanged?.Invoke(false);
                Console.WriteLine("Disconnected from KnightDesk server");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during disconnect: " + ex.Message);
            }
        }

        /// <summary>
        /// Send response back to KnightDesk server
        /// </summary>
        /// <param name="response">Response message</param>
        public void SendResponse(string response)
        {
            if (_isConnected && _stream != null)
            {
                try
                {
                    byte[] data = Encoding.UTF8.GetBytes(response);
                    _stream.Write(data, 0, data.Length);
                    _stream.Flush();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to send response: " + ex.Message);
                    Disconnect();
                }
            }
        }

        /// <summary>
        /// Request shutdown from KnightDesk tool (client-initiated shutdown)
        /// </summary>
        public void RequestShutdown()
        {
            if (_isConnected && _stream != null)
            {
                try
                {
                    Console.WriteLine("Requesting shutdown from KnightDesk tool...");
                    SendResponse(GameCommandConstants.CLIENT_SHUTDOWN_COMMAND);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to send shutdown request: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Listen for commands from KnightDesk server
        /// </summary>
        private void ListenForCommands()
        {
            byte[] buffer = new byte[GameConfigConstants.DEFAULT_BUFFER_SIZE];

            while (_isConnected && _stream != null)
            {
                try
                {
                    int bytesRead = _stream.Read(buffer, 0, buffer.Length);
                    
                    if (bytesRead > 0)
                    {
                        string command = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Console.WriteLine("Received command: " + command);
                        
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
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error listening for commands: " + ex.Message);
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
                if (command == GameCommandConstants.PING_COMMAND)
                {
                    return GameCommandConstants.PONG_RESPONSE;
                }
                else if (command == GameCommandConstants.SHUTDOWN_COMMAND)
                {
                    // Graceful shutdown initiated by KnightDesk tool
                    Console.WriteLine("Received shutdown command from KnightDesk tool");
                    Disconnect();
                    return GameCommandConstants.SHUTDOWN_OK_RESPONSE;
                }
                else if (command.StartsWith(GameCommandConstants.LOGIN_COMMAND + GameCommandConstants.COMMAND_SEPARATOR))
                {
                    // Handle login command
                    string[] parts = command.Split(GameCommandConstants.COMMAND_SEPARATOR[0]);
                    if (parts.Length >= 5)
                    {
                        string username = parts[1];
                        string password = parts[2];
                        string indexServer = parts[3];
                        string indexCharacter = parts[4];

                        // Perform login logic here
                        Console.WriteLine("Login attempt: " + username);
                        
                        // Simulate successful login and get character name
                        string characterName = GetCharacterName(username, indexCharacter);
                        
                        if (!string.IsNullOrEmpty(characterName))
                        {
                            return GameCommandConstants.CHARACTER_NAME_PREFIX + characterName;
                        }
                        else
                        {
                            return GameCommandConstants.LOGIN_FAILED_RESPONSE;
                        }
                    }
                }

                return GameCommandConstants.UNKNOWN_COMMAND_RESPONSE;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error processing command: " + ex.Message);
                return GameCommandConstants.ERROR_RESPONSE;
            }
        }

        /// <summary>
        /// Get character name after login (implement your game logic here)
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="indexCharacter">Character index</param>
        /// <returns>Character name</returns>
        private string GetCharacterName(string username, string indexCharacter)
        {
            // Implement your game's character selection logic here
            // This is just an example
            return "TestCharacter_" + username;
        }
    }

    /// <summary>
    /// Main entry point for game client integration
    /// Add this to your game's main initialization code
    /// </summary>
    public class GameClientIntegration
    {
        private static KnightDeskTcpClient _knightDeskClient;

        /// <summary>
        /// Initialize KnightDesk integration
        /// Call this from your game's main function or initialization code
        /// </summary>
        /// <param name="args">Command line arguments</param>
        public static void InitializeKnightDeskIntegration(string[] args)
        {
            int accountId = -1;
            int serverPort = TcpConstants.DEFAULT_TCP_PORT;

            // Parse command line arguments
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == GameConfigConstants.ACCOUNT_ARGUMENT && i + 1 < args.Length)
                {
                    int.TryParse(args[i + 1], out accountId);
                }
                else if (args[i] == GameConfigConstants.TCP_PORT_ARGUMENT && i + 1 < args.Length)
                {
                    int.TryParse(args[i + 1], out serverPort);
                }
            }

            if (accountId > 0)
            {
                _knightDeskClient = new KnightDeskTcpClient();
                _knightDeskClient.Initialize(accountId, serverPort);
                _knightDeskClient.ConnectionStatusChanged += OnConnectionStatusChanged;
                _knightDeskClient.CommandReceived += OnCommandReceived;

                // Connect to KnightDesk server
                if (_knightDeskClient.Connect())
                {
                    Console.WriteLine("KnightDesk integration initialized successfully");
                }
                else
                {
                    Console.WriteLine("Failed to initialize KnightDesk integration");
                }
            }
        }

        /// <summary>
        /// Handle connection status changes
        /// </summary>
        /// <param name="connected">Connection status</param>
        private static void OnConnectionStatusChanged(bool connected)
        {
            Console.WriteLine("KnightDesk connection status: " + (connected ? "Connected" : "Disconnected"));
        }

        /// <summary>
        /// Handle commands received from KnightDesk
        /// </summary>
        /// <param name="command">Command received</param>
        private static void OnCommandReceived(string command)
        {
            Console.WriteLine("KnightDesk command received: " + command);
            // Add your game-specific command handling here
        }

        /// <summary>
        /// Request shutdown from KnightDesk tool
        /// Call this when user wants to close the game from within the game
        /// </summary>
        public static void RequestGameShutdown()
        {
            if (_knightDeskClient != null && _knightDeskClient.IsConnected)
            {
                _knightDeskClient.RequestShutdown();
            }
        }

        /// <summary>
        /// Cleanup KnightDesk integration
        /// Call this when your game is shutting down
        /// </summary>
        public static void CleanupKnightDeskIntegration()
        {
            if (_knightDeskClient != null)
            {
                _knightDeskClient.Disconnect();
                _knightDeskClient = null;
            }
        }
    }

    /// <summary>
    /// Example usage in your game's main function
    /// </summary>
    public class ExampleGameMain
    {
        public static void Main(string[] args)
        {
            // Initialize KnightDesk integration
            GameClientIntegration.InitializeKnightDeskIntegration(args);

            // Your game initialization code here
            Console.WriteLine("Game started...");
            Console.WriteLine("Commands:");
            Console.WriteLine("  'q' - Quit game normally");
            Console.WriteLine("  's' - Request shutdown from KnightDesk tool");
            Console.WriteLine("  Any other key - Continue game");

            // Your game loop here
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                
                if (key.KeyChar == 'q')
                {
                    Console.WriteLine("Quitting game...");
                    break;
                }
                else if (key.KeyChar == 's')
                {
                    Console.WriteLine("Requesting shutdown from KnightDesk tool...");
                    GameClientIntegration.RequestGameShutdown();
                    break;
                }
                else
                {
                    // Continue game logic here
                    Console.WriteLine("Game continues...");
                }
            }

            // Cleanup KnightDesk integration
            GameClientIntegration.CleanupKnightDeskIntegration();
        }
    }
}
