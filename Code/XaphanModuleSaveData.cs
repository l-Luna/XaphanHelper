using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Celeste.Mod.XaphanHelper.Data;

namespace Celeste.Mod.XaphanHelper
{
    public class XaphanModuleSaveData : EverestModuleSaveData
    {
        // Flags

        public HashSet<string> SavedFlags = new HashSet<string>();

        public HashSet<string> GlobalFlags = new HashSet<string>();

        public HashSet<string> WatchedCutscenes = new HashSet<string>();

        // In-Game Map

        public List<string> VisitedChapters = new List<string>();

        public HashSet<string> VisitedRooms = new HashSet<string>();

        public HashSet<string> VisitedRoomsTiles = new HashSet<string>();

        public HashSet<string> ExtraUnexploredRooms = new HashSet<string>();

        public Dictionary<string, bool> ShowHints = new Dictionary<string, bool>();

        public Dictionary<string, int> ProgressMode = new Dictionary<string, int>();

        public Dictionary<string, int> WorldMapProgressMode = new Dictionary<string, int>();

        // Warps

        public HashSet<string> UnlockedWarps = new();

        public HashSet<string> SpeedrunModeUnlockedWarps = new();

        // Celeste Upgrades

        public HashSet<string> PowerGripInactive = new HashSet<string>();

        public HashSet<string> ClimbingKitInactive = new HashSet<string>();

        public HashSet<string> SpiderMagnetInactive = new HashSet<string>();

        public HashSet<string> LightningDashInactive = new HashSet<string>();

        public HashSet<string> DroneTeleportInactive = new HashSet<string>();

        public HashSet<string> JumpBoostInactive = new HashSet<string>();

        public HashSet<string> BombsInactive = new HashSet<string>();

        public HashSet<string> MegaBombsInactive = new HashSet<string>();

        public HashSet<string> RemoteDroneInactive = new HashSet<string>();

        public HashSet<string> GoldenFeatherInactive = new HashSet<string>();

        public HashSet<string> BinocularsInactive = new HashSet<string>();

        public HashSet<string> EtherealDashInactive = new HashSet<string>(); 

        public HashSet<string> PortableStationInactive = new HashSet<string>();

        public HashSet<string> PulseRadarInactive = new HashSet<string>();

        public HashSet<string> DashBootsInactive = new HashSet<string>();

        public HashSet<string> HoverBootsInactive = new HashSet<string>();

        // Metroid Upgrades

        public HashSet<string> SpazerInactive = new HashSet<string>();

        public HashSet<string> PlasmaBeamInactive = new HashSet<string>();

        public HashSet<string> MorphingBallInactive = new HashSet<string>();

        public HashSet<string> MorphBombsInactive = new HashSet<string>();

        public HashSet<string> SpringBallInactive = new HashSet<string>();

        public HashSet<string> HighJumpBootsInactive = new HashSet<string>();

        public HashSet<string> SpeedBoosterInactive = new HashSet<string>();

        // Common Upgrades

        public HashSet<string> LongBeamInactive = new HashSet<string>();

        public HashSet<string> IceBeamInactive = new HashSet<string>();

        public HashSet<string> WaveBeamInactive = new HashSet<string>();

        public HashSet<string> VariaJacketInactive = new HashSet<string>();

        public HashSet<string> GravityJacketInactive = new HashSet<string>();

        public HashSet<string> ScrewAttackInactive = new HashSet<string>();

        public HashSet<string> SpaceJumpInactive = new HashSet<string>();

        // Other Upgrades

        public HashSet<string> StaminaUpgrades = new HashSet<string>();

        public HashSet<string> SpeedrunModeStaminaUpgrades = new HashSet<string>();

        public HashSet<string> DroneFireRateUpgrades = new HashSet<string>();

        public HashSet<string> SpeedrunModeDroneFireRateUpgrades = new HashSet<string>();

        // Countdown

        public long CountdownCurrentTime = -1;

        public bool CountdownShake = false;

        public bool CountdownExplode = false;

        public string CountdownActiveFlag = "";

        public int CountdownStartChapter = -1;

        public string CountdownStartRoom = "";

        public Vector2 CountdownSpawn = new Vector2();

        public bool CountdownIntroType = false;

        public bool CountdownUseLevelWipe = false;

        // Teleport variables

        public bool TeleportFromElevator = false;

        public string DestinationRoom = "";

        public Vector2 Spawn = new Vector2();

        public string Wipe = "";

        public float WipeDuration = 1.35f;

        public bool ConsiderBeginning = false;

        public Dictionary<string, int> SavedChapter = new Dictionary<string, int>();

        public Dictionary<string, string> SavedRoom = new Dictionary<string, string>();

        public Dictionary<string, Vector2> SavedSpawn = new Dictionary<string, Vector2>();

        public Dictionary<string, float> SavedLightingAlphaAdd = new Dictionary<string, float>();

        public Dictionary<string, float> SavedBloomBaseAdd = new Dictionary<string, float>();

        public Dictionary<string, Session.CoreModes> SavedCoreMode = new Dictionary<string, Session.CoreModes>();

        public Dictionary<string, string> SavedMusic = new Dictionary<string, string>();

        public Dictionary<string, string> SavedAmbience = new Dictionary<string, string>();

        public Dictionary<string, HashSet<EntityID>> SavedNoLoadEntities = new Dictionary<string, HashSet<EntityID>>();

        public Dictionary<string, long> SavedTime = new Dictionary<string, long>();

        public Dictionary<string, bool> SavedFromBeginning = new Dictionary<string, bool>();

        public Dictionary<string, string> SavedSesionFlags = new Dictionary<string, string>();

        public Dictionary<string, HashSet<EntityID>> SavedSessionStrawberries = new Dictionary<string, HashSet<EntityID>>();

        // Lobbies variables

        public HashSet<string> VisitedLobbyMapTiles = new HashSet<string>();

        public List<Vector2> GeneratedVisitedLobbyMapTiles = new List<Vector2>();

        public List<Vector2> GeneratedVisitedLobbyMapTiles2 = new List<Vector2>();

        // Metroid variables

        public Dictionary<string, int> MaxHealth = new Dictionary<string, int>();

        public Dictionary<string, int> MaxMissiles = new Dictionary<string, int>();

        public Dictionary<string, int> MaxSuperMissiles = new Dictionary<string, int>();

        public Dictionary<string, int> MaxPowerBombs = new Dictionary<string, int>();

        public HashSet<string> DoorsOpened = new HashSet<string>();

        public HashSet<string> AmmoCollected = new HashSet<string>();

        public bool IgnoreSavedChapter = false;

        public bool IgnoreSavedRoom = false;

        // Sides Unlock variables

        public HashSet<string> CSideUnlocked = new HashSet<string>();

        public int LastPlayedSide = 0;

        // Checkpoints (Merged chapters)

        public HashSet<string> Checkpoints = new HashSet<string>();

        // Others

        public string CurrentSubArea = "";

        public bool SpeedrunMode = false;

        public int BagUIId1 = 0;

        public int BagUIId2 = 0;

        public bool LoadedPlayer = false;
    }
}