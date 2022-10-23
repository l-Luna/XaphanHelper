namespace Celeste.Mod.XaphanHelper.Data
{
    public class LobbyMapsLobbiesData
    {
        public int LobbyIndex;

        public string ImageDicrectory;

        public int GlobalLobbyID;

        public string LevelSet;

        public int TotalMaps;

        public LobbyMapsLobbiesData(int lobbyIndex, string imageDicrectory, int globalLobbyID, string levelSet, int totalMaps)
        {
            LobbyIndex = lobbyIndex;
            ImageDicrectory = imageDicrectory;
            GlobalLobbyID = globalLobbyID;
            LevelSet = levelSet;
            TotalMaps = totalMaps;
        }
    }
}