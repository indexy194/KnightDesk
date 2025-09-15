namespace KnightDesk.Presentation.WPF.Constants
{
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

        /// <summary>
        /// Default buffer size for TCP communication
        /// </summary>
        public const int DEFAULT_BUFFER_SIZE = 1024;

        /// <summary>
        /// Maximum retry attempts for connection
        /// </summary>
        public const int MAX_RETRY_ATTEMPTS = 3;

        /// <summary>
        /// Retry delay in milliseconds
        /// </summary>
        public const int RETRY_DELAY_MS = 1000;
    }
}
