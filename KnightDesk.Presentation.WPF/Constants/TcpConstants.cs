namespace KnightDesk.Presentation.WPF.Constants
{
    /// <summary>
    /// Constants for TCP communication
    /// </summary>
    public static class TcpConstants
    {
        /// <summary>
        /// Default TCP port for game client connections
        /// </summary>
        public const int DEFAULT_TCP_PORT = 8888;

        /// <summary>
        /// TCP server host address
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
        /// Game initialization delay in milliseconds
        /// </summary>
        public const int GAME_INIT_DELAY_MS = 10000;

        /// <summary>
        /// Login command delay in milliseconds
        /// </summary>
        public const int LOGIN_DELAY_MS = 3000;

        /// <summary>
        /// Process monitoring interval in milliseconds
        /// </summary>
        public const int MONITOR_INTERVAL_MS = 5000;

        /// <summary>
        /// Graceful shutdown timeout in milliseconds
        /// </summary>
        public const int SHUTDOWN_TIMEOUT_MS = 2000;
    }
}
