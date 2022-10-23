using Microsoft.Xna.Framework;

namespace Celeste.Mod.XaphanHelper.Data
{
    public class LobbyMapIconsData
    {
        public string Type;

        public string Room;

        public Vector2 Position;

        public LobbyMapIconsData(string type, string room, Vector2 position)
        {
            Type = type;
            Room = room;
            Position = position;
        }
    }
}
