namespace KnightDesk.Presentation.WPF.Constants
{
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
        /// Auto state command
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

        /// <summary>
        /// Name focus command
        /// </summary>
        public const string NAME_FOCUS_COMMAND = "FOCUS_NAME";

        /// <summary>
        /// Mount command
        /// </summary>
        public const string MOUNT_COMMAND = "MOUNT";

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

        #region Game Status Messages

        /// <summary>
        /// Game starting status
        /// </summary>
        public const string STATUS_STARTING = "Starting...";

        /// <summary>
        /// Game logging in status
        /// </summary>
        public const string STATUS_LOGGING_IN = "Game started, logging in...";

        /// <summary>
        /// Game online status
        /// </summary>
        public const string STATUS_IN_GAME = "In-game";

        /// <summary>
        /// Game offline status
        /// </summary>
        public const string STATUS_OFFLINE = "Offline";

        /// <summary>
        /// Game running but no connection status
        /// </summary>
        public const string STATUS_NO_CONNECTION = "Running (No connection)";

        /// <summary>
        /// Game disconnected status
        /// </summary>
        public const string STATUS_DISCONNECTED = "Disconnected";

        /// <summary>
        /// Game shutting down status
        /// </summary>
        public const string STATUS_SHUTTING_DOWN = "Game is shutting down...";

        /// <summary>
        /// Connection error status
        /// </summary>
        public const string STATUS_CONNECTION_ERROR = "Connection error";

        /// <summary>
        /// Monitoring error status
        /// </summary>
        public const string STATUS_MONITORING_ERROR = "Monitoring error";

        /// <summary>
        /// Login error status
        /// </summary>
        public const string STATUS_LOGIN_ERROR = "Login error";

        /// <summary>
        /// Login failed status
        /// </summary>
        public const string STATUS_LOGIN_FAILED = "Login failed";

        /// <summary>
        /// Failed to start game status
        /// </summary>
        public const string STATUS_START_FAILED = "Failed to start game";

        /// <summary>
        /// Failed to send shutdown command status
        /// </summary>
        public const string STATUS_SHUTDOWN_FAILED = "Failed to send shutdown command";

        #endregion
    }
}
