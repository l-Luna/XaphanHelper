using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Data
{
    public class InGameMapTilesData
    {
        public string Room;

        public Image Image;

        public Vector2 Position;

        public Color RoomBorderColor;

        public string BackgroundPattern;

        public Color BackgroundColor;

        public string ElevatorPattern;

        public Color ElevatorColor;

        public InGameMapTilesData(string room, Image image, Vector2 position, Color roomBorderColor, string backgroundPattern, Color backgroundColor, string elevatorPattern, Color elevatorColor)
        {
            Room = room;
            Image = image;
            Position = position;
            RoomBorderColor = roomBorderColor;
            BackgroundPattern = backgroundPattern;
            BackgroundColor = backgroundColor;
            ElevatorPattern = elevatorPattern;
            ElevatorColor = elevatorColor;
        }
    }
}
