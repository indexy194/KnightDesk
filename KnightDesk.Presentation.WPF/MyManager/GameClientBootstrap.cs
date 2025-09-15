using System;

/// <summary>
/// Bootstrap class to initialize the game client with account ID from command line
/// This should be called from your main game entry point
/// Updated to use TCP instead of Named Pipes
/// </summary>
public class GameClientBootstrap
{
    private static bool isInitialized = false;
    private static int currentAccountId = -1;
    private static int serverPort = GameClientConstants.TcpConstants.DEFAULT_TCP_PORT;

    public static void Initialize()
    {
        MyLogTxt.WriteLog("Starting game client bootstrap...");
        if (isInitialized)
        {
            MyLogTxt.WriteLog("Game client already initialized");
            return;
        }

        try
        {
            // Get command line arguments from Unity Environment
            string[] args = Environment.GetCommandLineArgs();

            // Parse account ID and TCP port from command line arguments
            int accountId = ParseAccountId(args);
            int port = ParseTcpPort(args);

            if (accountId == -1)
            {
                MyLogTxt.WriteLog("No account ID provided in command line arguments. TCP will not be available.");
                MyLogTxt.WriteLog("Command line: " + string.Join(" ", args));
                
                // Fallback: Try to initialize with default account ID for testing
                MyLogTxt.WriteLog("Attempting fallback initialization with account ID 1 for testing...");
                try
                {
                    InitializeManually(1, port);
                }
                catch (Exception fallbackEx)
                {
                    MyLogTxt.WriteLog(string.Format("Fallback initialization failed: {0}", fallbackEx.Message));
                }
                return;
            }

            currentAccountId = accountId;
            serverPort = port;
            MyLogTxt.WriteLog(string.Format("Initializing game client for account ID: {0}, TCP Port: {1}", accountId, port));

            // Initialize the auto system with account ID
            AutoSystemBootstrap.gI().InitializeAutoSystem(accountId);

            isInitialized = true;
            MyLogTxt.WriteLog(string.Format("Game client initialized successfully for account {0}", accountId));
        }
        catch (Exception e)
        {
            MyLogTxt.WriteLog(string.Format("Error initializing game client: {0}", e.Message));
        }
    }


    /// <summary>
    /// Parse account ID from command line arguments
    /// Expected format: -account {accountId}
    /// </summary>
    /// <param name="args">Command line arguments</param>
    /// <returns>Account ID or -1 if not found</returns>
    private static int ParseAccountId(string[] args)
    {
        try
        {
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (args[i].ToLower() == GameClientConstants.GameConfigConstants.ACCOUNT_ARGUMENT)
                {
                    if (int.TryParse(args[i + 1], out int accountId))
                    {
                        return accountId;
                    }
                }
            }
        }
        catch (Exception e)
        {
            MyLogTxt.WriteLog(string.Format("Error parsing account ID: {0}", e.Message));
        }

        return -1;
    }

    /// <summary>
    /// Parse TCP port from command line arguments
    /// </summary>
    /// <param name="args">Command line arguments</param>
    /// <returns>TCP port or default port if not found</returns>
    private static int ParseTcpPort(string[] args)
    {
        try
        {
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (args[i].ToLower() == GameClientConstants.GameConfigConstants.TCP_PORT_ARGUMENT)
                {
                    if (int.TryParse(args[i + 1], out int port))
                    {
                        return port;
                    }
                }
            }
        }
        catch (Exception e)
        {
            MyLogTxt.WriteLog(string.Format("Error parsing TCP port: {0}", e.Message));
        }

        return GameClientConstants.TcpConstants.DEFAULT_TCP_PORT;
    }

    /// <summary>
    /// Get the current account ID
    /// </summary>
    /// <returns>Current account ID or -1 if not set</returns>
    public static int GetCurrentAccountId()
    {
        return currentAccountId;
    }

    /// <summary>
    /// Check if the game client is initialized
    /// </summary>
    /// <returns>True if initialized</returns>
    public static bool IsInitialized()
    {
        return isInitialized;
    }

    /// <summary>
    /// Manual initialization for testing (bypasses command line parsing)
    /// </summary>
    /// <param name="accountId">Account ID to use</param>
    public static void InitializeManually(int accountId)
    {
        MyLogTxt.WriteLog(string.Format("Manual initialization requested for account ID: {0}", accountId));
        if (isInitialized)
        {
            MyLogTxt.WriteLog("Game client already initialized");
            return;
        }

        try
        {
            currentAccountId = accountId;
            MyLogTxt.WriteLog(string.Format("Manually initializing game client for account ID: {0}", accountId));

            // Initialize the auto system with account ID
            AutoSystemBootstrap.gI().InitializeAutoSystem(accountId);

            isInitialized = true;
            MyLogTxt.WriteLog(string.Format("Manual game client initialized successfully for account {0}", accountId));
        }
        catch (Exception e)
        {
            MyLogTxt.WriteLog(string.Format("Error during manual initialization: {0}", e.Message));
        }
    }

    /// <summary>
    /// Force restart IPC for testing
    /// </summary>
    public static void RestartIpc()
    {
        try
        {
            MyLogTxt.WriteLog("Restarting IPC system...");
            if (currentAccountId != -1)
            {
                AutoManager.gI().Shutdown();
                AutoManager.gI().Initialize(currentAccountId);
                MyLogTxt.WriteLog("IPC system restarted successfully");
            }
            else
            {
                MyLogTxt.WriteLog("No account ID available for IPC restart");
            }
        }
        catch (Exception e)
        {
            MyLogTxt.WriteLog(string.Format("Error restarting IPC: {0}", e.Message));
        }
    }

    /// <summary>
    /// Shutdown the game client
    /// Call this when the game is closing
    /// </summary>
    public static void Shutdown()
    {
        try
        {
            if (isInitialized)
            {
                MyLogTxt.WriteLog(string.Format("Shutting down game client for account {0}", currentAccountId));
                AutoSystemBootstrap.gI().Shutdown();
                isInitialized = false;
                currentAccountId = -1;
                MyLogTxt.WriteLog("Game client shutdown completed");
            }
        }
        catch (Exception e)
        {
            MyLogTxt.WriteLog(string.Format("Error during game client shutdown: {0}", e.Message));
        }
    }
}
