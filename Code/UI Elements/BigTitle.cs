using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.UI_Elements
{
    public class BigTitle : Entity
    {
        public string Text;

        public string Prefix;

        public float Scale;

        public BigTitle(string text, Vector2 position, bool isDialog = false, float scale = 2f, string prefix = "")
        {
            Tag = Tags.HUD;
            Prefix = Dialog.Clean(prefix);
            if (isDialog)
            {
                Text = text;
            }
            else
            {
                Text = Dialog.Clean(text);
            }
            Scale = scale;
            Depth = -20000;
            Position = position;
        }

        public override void Render()
        {
            ActiveFont.DrawEdgeOutline(!string.IsNullOrEmpty(Prefix) ? Prefix + " " + Text : Text, Position, new Vector2(0.5f, 0.5f), Vector2.One * Scale, Color.Gray, Scale * 2f, Color.DarkSlateBlue, 2f, Color.Black);
        }
    }

}
