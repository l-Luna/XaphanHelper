using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.UI_Elements
{
    public class WarpDestinationDisplay : Entity
    {
        public string Room;

        public string Label;

        public int Index;

        public float TextWidth;

        public WarpDestinationDisplay(Vector2 position, string room, string label, int index)
        {
            Tag = Tags.HUD;
            Room = room;
            Label = Dialog.Clean(label);
            Index = index;
            Depth = -20000;
            Position = position;
            TextWidth = ActiveFont.Measure(Label).X;
        }

        public void UpdateDest(string room, string label, int index)
        {
            Room = room;
            Label = Dialog.Clean(label);
            Index = index;
            TextWidth = ActiveFont.Measure(Label).X;
        }

        public override void Render()
        {
            base.Render();
            ActiveFont.DrawOutline(Label, Position, new Vector2(0.5f, 0.5f), Vector2.One, Color.White, 2f, Color.Black);
        }
    }
}
