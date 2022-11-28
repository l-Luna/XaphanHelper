using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.UI_Elements
{
    public class NormalText : Entity
    {
        public string Text;

        public string Prefix;

        public Color Color;

        public float Scale;

        public float Alpha;

        public NormalText(string text, Vector2 position, Color color, float alpha, float scale, string prefix = "")
        {
            Tag = Tags.HUD;
            Prefix = Dialog.Clean(prefix);
            Text = Dialog.Clean(text);
            Alpha = alpha;
            Color = color;
            Scale = scale;
            Depth = -20000;
            Position = position;
        }

        public void Appear()
        {
            Alpha = Calc.Approach(Alpha, 1, Engine.DeltaTime * 2f);
        }

        public void UpdateAlpha(float alpha)
        {
            Alpha = alpha;
        }

        public void UpdatePosition(Vector2 position)
        {
            Position = position;
        }

        public override void Render()
        {
            base.Render();
            ActiveFont.DrawOutline(!string.IsNullOrEmpty(Prefix) ? Prefix + " " + Text : Text, Position, new Vector2(0.5f, 0.5f), Vector2.One * Scale, Color * Alpha, 2f, Color.Black);
        }
    }

}
