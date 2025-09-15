public static class MyVariable
{
    // GLOBAL VARIABLE
    // -- TCP Settings (replacing Named Pipes)
    public static bool IpcLoginEnabled { get; set; } = true; // Keep same name for compatibility
    public static bool IpcAutoStart { get; set; } = true; // Keep same name for compatibility
    public static int MaxAccounts { get; set; } = 10; // Maximum number of simultaneous accounts

    // -- Auto Tool State
    public static bool AutoToolEnabled { get; set; } = false;
    // -- Auto Features
    public static bool AutoChangeEquipEnabled { get; set; } = false;
    // Auto Event Settings
    public static bool AutoEventEnabled { get; set; } = false; // event 2
    public static string AutoEventType { get; set; } = "Trung Thu";
    // Auto Equip Settings  
    public static bool AutoEquipEnabled { get; set; } = false;
    public static string AutoEquipType { get; set; } = "Sát Thủ";
    // Auto Mount Settings - Store mount IDs directly (mapped from MountType enum)
    public static int AutoMount1Type { get; set; } = 1; // Heo
    public static int AutoMount2Type { get; set; } = 4; // Hỏa Kì Lân
    // Focus Name Setting
    public static string FocusName { get; set; } = "";
    // Server Message Setting
    public static string ServerMessage { get; set; } = "";
    // Tree Fruit Scan Setting
    public static int TreeFruitCount { get; set; } = 0;
    public static string TreeFruitInfo { get; set; } = "";
    // Select Char Screen
    public static bool StateSelectChar { get; set; } = false;
    public static int SelectChar { get; set; } = 0;
    // Auto Delays (in game ticks)
    public static int AutoMoveDelay { get; set; } = 500;
    public static int AutoRespawnDelay { get; set; } = 150;
    public static int AutoMineDelay { get; set; } = 10;
    public static int AutoPhoBanMoveDelay { get; set; } = 50;
    public static int AutoPhoBanAttackDelay { get; set; } = 200;
    // Auto HP/MP Settings
    public static bool AutoHPMPEnabled { get; set; } = false;
    public static int AutoHPPercentage { get; set; } = 50;
    public static int AutoMPPercentage { get; set; } = 50;
    // Auto Item Settings

    // Tree Fruit Event Timing (30 min spawn cycle, 45 min duration)
    public static bool TreeFruitEventActive { get; set; } = false;
    public static long TreeFruitEventStartTime { get; set; } = 0; // When event started (ms)
    public static long TreeFruitNextSpawnTime { get; set; } = 0; // Next spawn time (ms)
    public static long TreeFruitEndTime { get; set; } = 0; // When current trees expire (ms)
    public static bool AutoHarvestEnabled { get; set; } = false; // Auto harvest when near tree
    public static int AutoHarvestRange { get; set; } = 50; // Range to auto harvest

    // Tree Fruit Event Constants (in milliseconds)
    public static readonly int TREE_SPAWN_INTERVAL = 30 * 60 * 1000; // 30 minutes
    public static readonly int TREE_DURATION = 45 * 60 * 1000; // 45 minutes
    public static readonly int TREE_COUNT_PER_SPAWN = 500; // 500 trees per spawn

    // Method to get current mount ID for specific slot
    public static int GetMountId(int slotNumber)
    {
        return slotNumber == 1 ? AutoMount1Type : AutoMount2Type;
    }

    // Method to set mount ID for specific slot  
    public static void SetMountId(int slotNumber, int mountId)
    {
        if (slotNumber == 1)
            AutoMount1Type = mountId;
        else if (slotNumber == 2)
            AutoMount2Type = mountId;
    }
    //Paint
    public static string LastEquippedItem = "";
}

public class LoginData
{
    public string Username { get; set; }
    public string Password { get; set; }
    public sbyte ServerIndex { get; set; }
    public int CharacterIndex { get; set; }
}

