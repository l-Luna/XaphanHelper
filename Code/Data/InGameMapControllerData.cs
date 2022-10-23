namespace Celeste.Mod.XaphanHelper.Data
{
    public class InGameMapControllerData
    {
        public string ExploredRoomColor;

        public string UnexploredRoomColor;

        public string SecretRoomColor;

        public string HeatedRoomColor;

        public string RoomBorderColor;

        public string ElevatorColor;

        public string GridColor;

        public bool RevealUnexploredRooms;

        public bool HideIconsInUnexploredRooms;

        public string ShowProgress;

        public bool HideMapProgress;

        public bool HideStrawberryProgress;

        public bool HideMoonberryProgress;

        public bool HideUpgradeProgress;

        public bool HideHeartProgress;

        public bool HideCassetteProgress;

        public string CustomCollectablesProgress;

        public string SecretsCustomCollectablesProgress;

        public int WorldmapOffsetX;

        public int WorldmapOffsetY;

        public InGameMapControllerData(string exploredRoomColor, string unexploredRoomColor, string secretRoomColor, string heatedRoomColor, string roomBorderColor, string elevatorColor, string gridColor, bool revealUnexploredRooms, bool hideIconsInUnexploredRooms, string showProgress, bool hideMapProgress, bool hideStrawberryProgress, bool hideMoonberryProgress, bool hideUpgradeProgress, bool hideHeartProgress, bool hideCassetteProgress, string customCollectablesProgress, string secretsCustomCollectablesProgress, int worldmapOffsetX, int worldmapOffsetY)
        {
            ExploredRoomColor = exploredRoomColor;
            UnexploredRoomColor = unexploredRoomColor;
            SecretRoomColor = secretRoomColor;
            HeatedRoomColor = heatedRoomColor;
            RoomBorderColor = roomBorderColor;
            ElevatorColor = elevatorColor;
            GridColor = gridColor;
            RevealUnexploredRooms = revealUnexploredRooms;
            HideIconsInUnexploredRooms = hideIconsInUnexploredRooms;
            ShowProgress = showProgress;
            HideMapProgress = hideMapProgress;
            HideStrawberryProgress = hideStrawberryProgress;
            HideMoonberryProgress = hideMoonberryProgress;
            HideUpgradeProgress = hideUpgradeProgress;
            HideHeartProgress = hideHeartProgress;
            HideCassetteProgress = hideCassetteProgress;
            CustomCollectablesProgress = customCollectablesProgress;
            SecretsCustomCollectablesProgress = secretsCustomCollectablesProgress;
            WorldmapOffsetX = worldmapOffsetX;
            WorldmapOffsetY = worldmapOffsetY;
        }
    }
}
