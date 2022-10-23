using Microsoft.Xna.Framework;

namespace Celeste.Mod.XaphanHelper.Data
{
    public class InGameMapEntrancesData
    {
        public string Room;

        public Vector2 Position;

        public int Width;

        public int Height;

        public Color Color;

        public InGameMapEntrancesData(string room, Vector2 position, int width, int height, Color color)
        {
            Room = room;
            Position = position;
            Width = width;
            Height = height;
            Color = color;
        }
    }
}
