using Microsoft.Xna.Framework;

namespace Celeste.Mod.XaphanHelper.Data
{
    public class LobbyMapWarpsData
    {
        public string Room;

        public Vector2 Position;

        public int Index;

        public LobbyMapWarpsData( string room, Vector2 position, int index)
        {
            Room = room;
            Position = position;
            Index = index;
        }
    }
}
