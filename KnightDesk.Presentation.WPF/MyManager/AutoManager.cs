using System;

public class AutoManager
{
	public static AutoManager instance;
	private bool isInitialized = false;
	private bool isProcessingLogin = false;

	public static AutoManager gI()
	{
		if (instance == null)
		{
			instance = new AutoManager();
		}
		return instance;
	}
	public void Initialize(int accountId)
	{
		MyLogTxt.WriteLog($"Initializing AutoManager for account {accountId}...");
		if (isInitialized) return;

		try
		{
			// Start TCP client if enabled
			if (MyVariable.IpcLoginEnabled && MyVariable.IpcAutoStart)
			{
				try
				{
					TcpClientManager.gI().StartTcpClient(accountId);
					MyLogTxt.WriteLog("TCP Client Manager started successfully");
				}
				catch (Exception tcpEx)
				{
					MyLogTxt.WriteLog($"TCP Client Manager failed to start: {tcpEx.Message}");
					MyLogTxt.WriteLog("Continuing without TCP support...");
				}
			}
			else
			{
				MyLogTxt.WriteLog("TCP disabled - running in standalone mode");
			}

			MyLogTxt.WriteLog($"AutoManager initialized successfully for account {accountId}");
			isInitialized = true;
		}
		catch (Exception e)
		{
			MyLogTxt.WriteLog($"Error initializing AutoManager: {e.Message}");
			MyLogTxt.WriteLog($"Stack trace: {e.StackTrace}");
			
			// Still mark as initialized to prevent repeated failures
			isInitialized = true;
		}
	}

	// Method to check if TCP features are supported
	private bool CheckIpcSupport()
	{
		// TCP is supported on all platforms, so always return true
		MyLogTxt.WriteLog("TCP Support Check: TCP is available on all platforms");
		return true;
	}
	// Method for TCP login processing
	public void ProcessLogin(int accountId)
	{
		try
		{
			if (!TcpClientManager.IsDataAvailable())
			{
				MyLogTxt.WriteLog("No TCP login data available");
				return;
			}

			LoginData loginData = TcpClientManager.ReceivedLoginData;
			MyLogTxt.WriteLog(string.Format("Processing TCP login for account {0}, user: {1}", accountId, loginData.Username));
			// Set server index
			GameCanvas.IndexServer = loginData.ServerIndex;

			// Set login fields
			LoginScreen.tfusername.setText(loginData.Username);
			LoginScreen.tfpassword.setText(loginData.Password);

			// Connect to game
			GameCanvas.connect();

			// Send login request
			GlobalService.gI().login(loginData.Username, loginData.Password, GameMidlet.version, "0", "0", "0", -1, -1);

			// Save login info
			LoginScreen.saveUser_Pass();
			LoginScreen.saveversionGame();
			LoginScreen.saveIndexServer();

			// Set character selection
			MyVariable.StateSelectChar = true;
			MyVariable.SelectChar = loginData.CharacterIndex;


			// Simulate getting character name after successful login

			// TODO: Replace with actual character name retrieval
			string characterName = GetCharacterName(); // You'll implement this

			if (!string.IsNullOrEmpty(characterName))
			{
				// Send character name back to management tool
				string response = string.Format("CHARACTER_NAME|{0}", characterName);
				// Response will be sent automatically by TcpClientManager
				MyLogTxt.WriteLog(string.Format("Character name response prepared: {0}", characterName));
			}

			// Consume the login data
			TcpClientManager.ConsumeLoginData();

			MyLogTxt.WriteLog(string.Format("TCP login processed successfully for account {0}", accountId));
		}
		catch (Exception e)
		{
			MyLogTxt.WriteLog(string.Format("Error processing TCP login: {0}", e.Message));
			MyLogTxt.WriteLog(string.Format("Stack trace: {0}", e.StackTrace));
		}
	}

	// Method to handle mount actions
	public void ProcessMountAction(int slotNumber)
	{
		try
		{
			int mountId = (slotNumber == 1) ? MyVariable.AutoMount1Type : MyVariable.AutoMount2Type;
			string mountName = GetMountNameFromId(mountId);

			MyLogTxt.WriteLog(string.Format("Processing mount action: Slot {0}, Mount ID: {1}, Name: {2}", slotNumber, mountId, mountName));

			// Here you can add the actual mount logic
			// For example: ActivateMount(mountId);

		}
		catch (Exception e)
		{
			MyLogTxt.WriteLog(string.Format("Error processing mount action: {0}", e.Message));
		}
	}
	// Method to get character name - YOU NEED TO IMPLEMENT THIS
	private string GetCharacterName()
	{

		try
		{
			if (GameScreen.player != null && !string.IsNullOrEmpty(GameScreen.player.name))
			{
				return GameScreen.player.name;
			}
			return null;
		}
		catch (Exception e)
		{
			MyLogTxt.WriteLog(string.Format("Error getting character name: {0}", e.Message));
			return null;
		}
	}

	// Helper method to get mount name from ID (for logging purposes)
	private string GetMountNameFromId(int mountId)
	{
		switch (mountId)
		{
			case 1: return "Heo";
			case 2: return "Sát Thủ";
			case 3: return "Trung Thu";
			case 4: return "Hỏa Kì Lân";
			default: return "Unknown";
		}
	}
	public void Shutdown()
	{
		try
		{
			IpcManager.gI().StopIpcListener();
			MyLogTxt.WriteLog("AutoManager shutdown completed");
		}
		catch (Exception e)
		{
			MyLogTxt.WriteLog(string.Format("Error during shutdown: {0}", e.Message));
		}
	}
	// Debug method to check current state
	public void DebugState()
	{
		try
		{
			MyLogTxt.WriteLog("=== AutoManager Debug State ===");
			MyLogTxt.WriteLog(string.Format("Initialized: {0}", isInitialized));
			MyLogTxt.WriteLog(string.Format("Processing Login: {0}", isProcessingLogin));
			MyLogTxt.WriteLog(string.Format("IPC Login Enabled: {0}", MyVariable.IpcLoginEnabled));
			MyLogTxt.WriteLog(string.Format("IPC Auto Start: {0}", MyVariable.IpcAutoStart));
			MyLogTxt.WriteLog(string.Format("IPC Data Available: {0}", IpcManager.IsDataAvailable()));

			// Get detailed IPC status
			MyLogTxt.WriteLog(IpcManager.gI().GetDetailedStatus());

			if (IpcManager.IsDataAvailable())
			{
				LoginData data = IpcManager.ReceivedLoginData;
				MyLogTxt.WriteLog(string.Format("Available Login Data: {0}, Server: {1}, Char: {2}", data.Username, data.ServerIndex, data.CharacterIndex));
			}

			MyLogTxt.WriteLog(string.Format("Current Screen: {0}", GameCanvas.currentScreen != null ? GameCanvas.currentScreen.GetType().Name : "null"));
			MyLogTxt.WriteLog(string.Format("Login Screen: {0}", GameCanvas.login != null ? GameCanvas.login.GetType().Name : "null"));
			
			// Run IPC diagnostic
			IpcManager.gI().DiagnoseIpcStatus();
			
			MyLogTxt.WriteLog("=== End Debug State ===");
		}
		catch (Exception e)
		{
			MyLogTxt.WriteLog(string.Format("Error in DebugState: {0}", e.Message));
		}
	}

	// Method to manually trigger TCP diagnostics
	public void RunTcpDiagnostics()
	{
		try
		{
			MyLogTxt.WriteLog("Running TCP diagnostics...");
			
			// Test if TCP client is connected
			bool tcpConnected = TcpClientManager.gI().IsConnected();
			MyLogTxt.WriteLog(string.Format("TCP Connection Status: {0}", tcpConnected));
			
			// Test if TCP support is working
			bool tcpSupported = CheckIpcSupport(); // This now checks TCP support
			MyLogTxt.WriteLog(string.Format("TCP Support Check Result: {0}", tcpSupported));
		}
		catch (Exception e)
		{
			MyLogTxt.WriteLog(string.Format("Error running TCP diagnostics: {0}", e.Message));
		}
	}

	public void Shutdown()
	{
		try
		{
			MyLogTxt.WriteLog("Starting AutoManager shutdown...");
			
			if (isInitialized)
			{
				// Stop TCP client
				TcpClientManager.gI().StopTcpClient();
				MyLogTxt.WriteLog("TCP Client Manager stopped");
				
				isInitialized = false;
				MyLogTxt.WriteLog("AutoManager shutdown completed successfully");
			}
			else
			{
				MyLogTxt.WriteLog("AutoManager was not initialized, skipping shutdown");
			}
		}
		catch (Exception e)
		{
			MyLogTxt.WriteLog(string.Format("Error during AutoManager shutdown: {0}", e.Message));
		}
	}

}
