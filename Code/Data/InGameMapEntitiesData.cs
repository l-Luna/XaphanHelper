using Microsoft.Xna.Framework;

namespace Celeste.Mod.XaphanHelper.Data
{
    public class InGameMapEntitiesData
    {
        public int ChapterIndex;

        public string Room;

        public LevelData LevelData;

        public string Type;

        public Vector2 Position;

        public Vector2 MapTilesPosition;

        public AreaKey StrawberryArea;

        public int ID;

        public string UpgradeCollectableUpgrade;

        public int UpgradeCollectableMapShardIndex;

        public string CustomCollectableMapIcon;

        public string CustomCollectableFlag;

        public string CollectableDoorFlagID;

        public string CollectableDoorInteriorColor;

        public string CollectableDoorEdgesColor;

        public string CollectableDoorOrientation;

        public string CollectableDoorMapIcon;

        public int CollectableDoorRequires;

        public InGameMapEntitiesData(int chapterIndex, string room, LevelData levelData, string type, Vector2 position, Vector2 mapTilesPosition, AreaKey strawberryArea = new AreaKey(), int id = 0, string upgradeCollectableUpgrade = null, int upgradeCollectableMapShardIndex = 0, string customCollectableMapIcon = null, string customCollectableFlag = null, string collectableDoorFlagID = null, string collectableDoorInteriorColor = null, string collectableDoorEdgesColor = null, string collectableDoorOrientation = null, string collectableDoorMapIcon = null, int collectableDoorRequires = 0)
        {
            ChapterIndex = chapterIndex;
            Room = room;
            LevelData = levelData;
            Type = type;
            Position = position;
            MapTilesPosition = mapTilesPosition;
            StrawberryArea = strawberryArea;
            ID = id;
            UpgradeCollectableUpgrade = upgradeCollectableUpgrade;
            UpgradeCollectableMapShardIndex = upgradeCollectableMapShardIndex;
            CustomCollectableMapIcon = customCollectableMapIcon;
            CustomCollectableFlag = customCollectableFlag;
            CollectableDoorFlagID = collectableDoorFlagID;
            CollectableDoorInteriorColor = collectableDoorInteriorColor;
            CollectableDoorEdgesColor = collectableDoorEdgesColor;
            CollectableDoorOrientation = collectableDoorOrientation;
            CollectableDoorMapIcon = collectableDoorMapIcon;
            CollectableDoorRequires = collectableDoorRequires;
        }
    }
}
