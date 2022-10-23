using Microsoft.Xna.Framework;

namespace Celeste.Mod.XaphanHelper.Data
{
    public class InGameMapIconsData
    {
        public string Type;


        public string Room;

        public Vector2 Position;

        public bool Checkmark;

        public string Color;

        public InGameMapIconsData(string type, string room, Vector2 position, bool checkmark, string color = "FFFFFF")
        {
            Type = type;
            Room = room;
            Position = position;
            Checkmark = checkmark;
            Color = color;
        }
    }
}
