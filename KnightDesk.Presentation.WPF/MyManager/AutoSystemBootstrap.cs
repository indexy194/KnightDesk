using System;

public class AutoSystemBootstrap
{
    private static AutoSystemBootstrap instance;
    private bool isInitialized = false;

    public static AutoSystemBootstrap gI()
    {
        if (instance == null)
            instance = new AutoSystemBootstrap();
        return instance;
    }

    public void InitializeAutoSystem(int accountId)
    {
        if (isInitialized)
        {
            MyLogTxt.WriteLog(string.Format("Auto system already initialized for account {0}", accountId));
            return;
        }

        try
        {
            MyLogTxt.WriteLog(string.Format("Starting Auto System Bootstrap for account {0}...", accountId));

            // Initialize AutoManager with account ID
            AutoManager.gI().Initialize(accountId);

            isInitialized = true;
            MyLogTxt.WriteLog(string.Format("Auto System Bootstrap completed successfully for account {0}", accountId));
            MyLogTxt.WriteLog(string.Format("TCP Login Enabled: {0}", MyVariable.IpcLoginEnabled));
            MyLogTxt.WriteLog(string.Format("TCP Auto Start: {0}", MyVariable.IpcAutoStart));
        }
        catch (Exception e)
        {
            MyLogTxt.WriteLog(string.Format("Error during Auto System Bootstrap: {0}", e.Message));
        }
    }

    public void Shutdown()
    {
        try
        {
            if (isInitialized)
            {
                MyLogTxt.WriteLog("Starting Auto System Bootstrap shutdown...");

                // Shutdown AutoManager first
                AutoManager.gI().Shutdown();

                // Force garbage collection to clean up resources
                GC.Collect();
                GC.WaitForPendingFinalizers();

                isInitialized = false;
                MyLogTxt.WriteLog("Auto System Bootstrap shutdown completed successfully");
            }
            else
            {
                MyLogTxt.WriteLog("Auto System Bootstrap was not initialized, skipping shutdown");
            }
        }
        catch (Exception e)
        {
            MyLogTxt.WriteLog(string.Format("Error during Auto System Bootstrap shutdown: {0}", e.Message));
        }
    }
}