namespace Celeste.Mod.XaphanHelper.Data
{
    public class InGameMapSubAreaControllerData
    {
        public string ExploredRoomColor;

        public string UnexploredRoomColor;

        public string SecretRoomColor;

        public string HeatedRoomColor;

        public string RoomBorderColor;

        public string ElevatorColor;

        public string SubAreaName;

        public int SubAreaIndex;

        public InGameMapSubAreaControllerData(string exploredRoomColor, string unexploredRoomColor, string secretRoomColor, string heatedRoomColor, string roomBorderColor, string elevatorColor, string subAreaName, int subAreaIndex)
        {
            ExploredRoomColor = exploredRoomColor;
            UnexploredRoomColor = unexploredRoomColor;
            SecretRoomColor = secretRoomColor;
            HeatedRoomColor = heatedRoomColor;
            RoomBorderColor = roomBorderColor;
            ElevatorColor = elevatorColor;
            SubAreaName = subAreaName;
            SubAreaIndex = subAreaIndex;
        }
    }
}
