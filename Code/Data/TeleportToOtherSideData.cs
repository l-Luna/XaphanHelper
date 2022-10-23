using Microsoft.Xna.Framework;

namespace Celeste.Mod.XaphanHelper.Data
{
    public class TeleportToOtherSideData
    {
        public int Mode;

        public string Room;

        public Vector2 Position;

        public string Destination;

        public TeleportToOtherSideData(int mode, string room, Vector2 position, string destination)
        {
            Mode = mode;
            Room = room;
            Position = position;
            Destination = destination;
        }
    }
}