namespace Celeste.Mod.XaphanHelper.Data
{
    public class InGameMapRoomControllerData
    {
        public string Room;

        public bool ShowUnexplored;

        public int MapShardIndex;

        public bool Secret;

        public string Entrance0Position;

        public string Entrance0Cords;

        public string Entrance1Position;

        public string Entrance1Cords;

        public string Entrance2Position;

        public string Entrance2Cords;

        public string Entrance3Position;

        public string Entrance3Cords;

        public string Entrance4Position;

        public string Entrance4Cords;

        public string Entrance5Position;

        public string Entrance5Cords;

        public string Entrance6Position;

        public string Entrance6Cords;

        public string Entrance7Position;

        public string Entrance7Cords;

        public string Entrance8Position;

        public string Entrance8Cords;

        public string Entrance9Position;

        public string Entrance9Cords;

        public int Entrance0Offset;

        public int Entrance1Offset;

        public int Entrance2Offset;

        public int Entrance3Offset;

        public int Entrance4Offset;

        public int Entrance5Offset;

        public int Entrance6Offset;

        public int Entrance7Offset;

        public int Entrance8Offset;

        public int Entrance9Offset;

        public int SubAreaIndex;

        public InGameMapRoomControllerData(string room, bool showUnexplored, int mapShardIndex, bool secret, string entrance0Position, string entrance0Cords, string entrance1Position, string entrance1Cords, string entrance2Position, string entrance2Cords, string entrance3Position, string entrance3Cords, string entrance4Position, string entrance4Cords, string entrance5Position, string entrance5Cords, string entrance6Position, string entrance6Cords, string entrance7Position, string entrance7Cords, string entrance8Position, string entrance8Cords, string entrance9Position, string entrance9Cords, int entrance0Offset = 0, int entrance1Offset = 0, int entrance2Offset = 0, int entrance3Offset = 0, int entrance4Offset = 0, int entrance5Offset = 0, int entrance6Offset = 0, int entrance7Offset = 0, int entrance8Offset = 0, int entrance9Offset = 0, int subAreaIndex = 0)
        {
            Room = room;
            ShowUnexplored = showUnexplored;
            MapShardIndex = mapShardIndex;
            Secret = secret;
            Entrance0Position = entrance0Position;
            Entrance0Cords = entrance0Cords;
            Entrance1Position = entrance1Position;
            Entrance1Cords = entrance1Cords;
            Entrance2Position = entrance2Position;
            Entrance2Cords = entrance2Cords;
            Entrance3Position = entrance3Position;
            Entrance3Cords = entrance3Cords;
            Entrance4Position = entrance4Position;
            Entrance4Cords = entrance4Cords;
            Entrance5Position = entrance5Position;
            Entrance5Cords = entrance5Cords;
            Entrance6Position = entrance6Position;
            Entrance6Cords = entrance6Cords;
            Entrance7Position = entrance7Position;
            Entrance7Cords = entrance7Cords;
            Entrance8Position = entrance8Position;
            Entrance8Cords = entrance8Cords;
            Entrance9Position = entrance9Position;
            Entrance9Cords = entrance9Cords;
            Entrance0Offset = entrance0Offset;
            Entrance1Offset = entrance1Offset;
            Entrance2Offset = entrance2Offset;
            Entrance3Offset = entrance3Offset;
            Entrance4Offset = entrance4Offset;
            Entrance5Offset = entrance5Offset;
            Entrance6Offset = entrance6Offset;
            Entrance7Offset = entrance7Offset;
            Entrance8Offset = entrance8Offset;
            Entrance9Offset = entrance9Offset;
            SubAreaIndex = subAreaIndex;
        }

        public string GetEntrancePositionField(int entrance)
        {
            if (entrance == 0)
            {
                return Entrance0Position;
            }
            if (entrance == 1)
            {
                return Entrance1Position;
            }
            if (entrance == 2)
            {
                return Entrance2Position;
            }
            if (entrance == 3)
            {
                return Entrance3Position;
            }
            if (entrance == 4)
            {
                return Entrance4Position;
            }
            if (entrance == 5)
            {
                return Entrance5Position;
            }
            if (entrance == 6)
            {
                return Entrance6Position;
            }
            if (entrance == 7)
            {
                return Entrance7Position;
            }
            if (entrance == 8)
            {
                return Entrance8Position;
            }
            if (entrance == 9)
            {
                return Entrance9Position;
            }
            return null;
        }

        public string GetEntranceCordsField(int entrance)
        {
            if (entrance == 0)
            {
                return Entrance0Cords;
            }
            if (entrance == 1)
            {
                return Entrance1Cords;
            }
            if (entrance == 2)
            {
                return Entrance2Cords;
            }
            if (entrance == 3)
            {
                return Entrance3Cords;
            }
            if (entrance == 4)
            {
                return Entrance4Cords;
            }
            if (entrance == 5)
            {
                return Entrance5Cords;
            }
            if (entrance == 6)
            {
                return Entrance6Cords;
            }
            if (entrance == 7)
            {
                return Entrance7Cords;
            }
            if (entrance == 8)
            {
                return Entrance8Cords;
            }
            if (entrance == 9)
            {
                return Entrance9Cords;
            }
            return null;
        }

        public int GetEntranceOffsetField(int entrance)
        {
            if (entrance == 0)
            {
                return Entrance0Offset;
            }
            if (entrance == 1)
            {
                return Entrance1Offset;
            }
            if (entrance == 2)
            {
                return Entrance2Offset;
            }
            if (entrance == 3)
            {
                return Entrance3Offset;
            }
            if (entrance == 4)
            {
                return Entrance4Offset;
            }
            if (entrance == 5)
            {
                return Entrance5Offset;
            }
            if (entrance == 6)
            {
                return Entrance6Offset;
            }
            if (entrance == 7)
            {
                return Entrance7Offset;
            }
            if (entrance == 8)
            {
                return Entrance8Offset;
            }
            if (entrance == 9)
            {
                return Entrance9Offset;
            }
            return 0;
        }
    }
}
