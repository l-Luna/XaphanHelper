namespace Celeste.Mod.XaphanHelper.Data
{
    public class InGameMapTilesControllerData
    {
        public int ChapterIndex;

        public string Room;

        public string Tile0Cords;

        public string Tile0;

        public string Tile1Cords;

        public string Tile1;

        public string Tile2Cords;

        public string Tile2;

        public string Tile3Cords;

        public string Tile3;

        public string Tile4Cords;

        public string Tile4;

        public string Tile5Cords;

        public string Tile5;

        public string Tile6Cords;

        public string Tile6;

        public string Tile7Cords;

        public string Tile7;

        public string Tile8Cords;

        public string Tile8;

        public string Tile9Cords;

        public string Tile9;

        public InGameMapTilesControllerData(int chapterIndex, string room, string tile0Cords, string tile0, string tile1Cords, string tile1, string tile2Cords, string tile2, string tile3Cords, string tile3, string tile4Cords, string tile4, string tile5Cords, string tile5, string tile6Cords, string tile6, string tile7Cords, string tile7, string tile8Cords, string tile8, string tile9Cords, string tile9)
        {
            ChapterIndex = chapterIndex;
            Room = room;
            Tile0Cords = tile0Cords;
            Tile0 = tile0;
            Tile1Cords = tile1Cords;
            Tile1 = tile1;
            Tile2Cords = tile2Cords;
            Tile2 = tile2;
            Tile3Cords = tile3Cords;
            Tile3 = tile3;
            Tile4Cords = tile4Cords;
            Tile4 = tile4;
            Tile5Cords = tile5Cords;
            Tile5 = tile5;
            Tile6Cords = tile6Cords;
            Tile6 = tile6;
            Tile7Cords = tile7Cords;
            Tile7 = tile7;
            Tile8Cords = tile8Cords;
            Tile8 = tile8;
            Tile9Cords = tile9Cords;
            Tile9 = tile9;
        }

        public string GetTileCords(int tile)
        {
            if (tile == 0)
            {
                return Tile0Cords;
            }
            if (tile == 1)
            {
                return Tile1Cords;
            }
            if (tile == 2)
            {
                return Tile2Cords;
            }
            if (tile == 3)
            {
                return Tile3Cords;
            }
            if (tile == 4)
            {
                return Tile4Cords;
            }
            if (tile == 5)
            {
                return Tile5Cords;
            }
            if (tile == 6)
            {
                return Tile6Cords;
            }
            if (tile == 7)
            {
                return Tile7Cords;
            }
            if (tile == 8)
            {
                return Tile8Cords;
            }
            if (tile == 9)
            {
                return Tile9Cords;
            }
            return null;
        }

        public string GetTile(int tile)
        {
            if (tile == 0)
            {
                return Tile0;
            }
            if (tile == 1)
            {
                return Tile1;
            }
            if (tile == 2)
            {
                return Tile2;
            }
            if (tile == 3)
            {
                return Tile3;
            }
            if (tile == 4)
            {
                return Tile4;
            }
            if (tile == 5)
            {
                return Tile5;
            }
            if (tile == 6)
            {
                return Tile6;
            }
            if (tile == 7)
            {
                return Tile7;
            }
            if (tile == 8)
            {
                return Tile8;
            }
            if (tile == 9)
            {
                return Tile9;
            }
            return null;
        }
    }
}
