using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Celeste.Mod.XaphanHelper.Data
{
    public class LobbyMapTilesData
    {
        public string Prefix;

        public int Chapter;

        public string Room;

        public List<Vector2> Coordinates = new List<Vector2>(); 

        public LobbyMapTilesData(string prefix, int chapter, string room, Vector2 coordinates)
        {
            Prefix = prefix;
            Chapter = chapter;
            Room = room;
            Coordinates.Add(coordinates);
        }
    }
}
