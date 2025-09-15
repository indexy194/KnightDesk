namespace GameClientConstants
{
    /// <summary>
    /// Constants for game client TCP communication
    /// </summary>
    public static class TcpConstants
    {
        /// <summary>
        /// Default TCP port for connecting to KnightDesk server
        /// </summary>
        public const int DEFAULT_TCP_PORT = 8888;

        /// <summary>
        /// KnightDesk server host address
        /// </summary>
        public const string TCP_HOST = "127.0.0.1";

        /// <summary>
        /// Connection timeout in milliseconds
        /// </summary>
        public const int CONNECTION_TIMEOUT_MS = 10000;

        /// <summary>
        /// Login timeout in milliseconds
        /// </summary>
        public const int LOGIN_TIMEOUT_MS = 5000;

        /// <summary>
        /// Default buffer size for TCP communication
        /// </summary>
        public const int DEFAULT_BUFFER_SIZE = 1024;
    }

    /// <summary>
    /// Constants for game commands and responses
    /// </summary>
    public static class GameCommandConstants
    {
        #region Registration Commands
        
        /// <summary>
        /// Client registration command prefix
        /// </summary>
        public const string REGISTER_COMMAND = "REGISTER";
        
        /// <summary>
        /// Registration confirmation response
        /// </summary>
        public const string REGISTERED_OK_RESPONSE = "REGISTERED_OK";
        
        #endregion

        #region Connection Commands
        
        /// <summary>
        /// Ping command to check connection
        /// </summary>
        public const string PING_COMMAND = "PING";
        
        /// <summary>
        /// Pong response to ping
        /// </summary>
        public const string PONG_RESPONSE = "PONG";
        
        #endregion

        #region Game Control Commands
        
        /// <summary>
        /// Login command prefix
        /// </summary>
        public const string LOGIN_COMMAND = "LOGIN";
        
        /// <summary>
        /// Shutdown command
        /// </summary>
        public const string SHUTDOWN_COMMAND = "SHUTDOWN";
        
        /// <summary>
        /// Shutdown confirmation response
        /// </summary>
        public const string SHUTDOWN_OK_RESPONSE = "SHUTDOWN_OK";
        
        /// <summary>
        /// Client-initiated shutdown command
        /// </summary>
        public const string CLIENT_SHUTDOWN_COMMAND = "CLIENT_SHUTDOWN";
        
        #endregion

        #region Character Commands
        
        /// <summary>
        /// Character name response prefix
        /// </summary>
        public const string CHARACTER_NAME_PREFIX = "CHARACTER_NAME|";
        
        /// <summary>
        /// Login failed response
        /// </summary>
        public const string LOGIN_FAILED_RESPONSE = "LOGIN_FAILED";
        
        #endregion

        #region Auto Commands
        
        /// <summary>
        /// Auto state command (equivalent to AUTO_TOOL)
        /// </summary>
        public const string AUTO_STATE_COMMAND = "AUTO_STATE";
        
        /// <summary>
        /// Auto event command
        /// </summary>
        public const string AUTO_EVENT_COMMAND = "AUTO_EVENT";
        
        /// <summary>
        /// Auto equip command
        /// </summary>
        public const string AUTO_EQUIP_COMMAND = "AUTO_EQUIP";
        
        #endregion

        #region Error Responses
        
        /// <summary>
        /// Unknown command response
        /// </summary>
        public const string UNKNOWN_COMMAND_RESPONSE = "UNKNOWN_COMMAND";
        
        /// <summary>
        /// Error response
        /// </summary>
        public const string ERROR_RESPONSE = "ERROR";
        
        #endregion

        #region Command Separators
        
        /// <summary>
        /// Command parameter separator
        /// </summary>
        public const string COMMAND_SEPARATOR = "|";
        
        #endregion

        #region Legacy Commands (for backward compatibility)
        
        /// <summary>
        /// Legacy auto tool command
        /// </summary>
        public const string AUTO_TOOL_COMMAND = "AUTO_TOOL";
        
        /// <summary>
        /// Legacy focus name command
        /// </summary>
        public const string FOCUS_NAME_COMMAND = "FOCUS_NAME";
        
        /// <summary>
        /// Legacy disconnect command
        /// </summary>
        public const string DISCONNECT_COMMAND = "DISCONNECT";
        
        #endregion
    }

    /// <summary>
    /// Constants for game configuration and command line arguments
    /// </summary>
    public static class GameConfigConstants
    {
        /// <summary>
        /// Account command line argument
        /// </summary>
        public const string ACCOUNT_ARGUMENT = "-account";
        
        /// <summary>
        /// TCP port command line argument
        /// </summary>
        public const string TCP_PORT_ARGUMENT = "-tcpport";
    }
}
