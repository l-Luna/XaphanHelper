using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.UI_Elements
{
    public static class DoubleButtonUI
    {
        public static float Width(string label, VirtualButton button1, VirtualButton button2)
        {
            MTexture mTexture1 = Input.GuiButton(button1, "controls/keyboard/oemquestion");
            MTexture mTexture2 = Input.GuiButton(button2, "controls/keyboard/oemquestion");
            return ActiveFont.Measure(label).X + 8f + mTexture1.Width + mTexture2.Width;
        }

        public static void Render(Vector2 position, string label, VirtualButton button1, VirtualButton button2, float scale, bool displayButton1, bool displayButton2, float justifyX = 0.5f, float wiggle = 0f, float alpha = 1f)
        {
            MTexture mTexture1 = Input.GuiButton(button1, "controls/keyboard/oemquestion");
            MTexture mTexture2 = Input.GuiButton(button2, "controls/keyboard/oemquestion");
            float num = ActiveFont.Measure(label).X + 8f + mTexture1.Width;
            position.X -= scale * num * (justifyX - 0.5f) + mTexture2.Width / 2;
            DrawText(label, position, num / 2f, scale + wiggle, alpha);
            if (displayButton1 && !displayButton2)
            {
                mTexture1.Draw(position, new Vector2(mTexture1.Width - num / 2f, mTexture1.Height / 2f), Color.White * alpha, scale + wiggle);
            }
            if (!displayButton1 && displayButton2)
            {
                mTexture2.Draw(position, new Vector2(mTexture2.Width - num / 2f, mTexture2.Height / 2f), Color.White * alpha, scale + wiggle);
            }
            if (displayButton1 && displayButton2)
            {
                mTexture1.Draw(position, new Vector2(mTexture1.Width - num / 2f, mTexture1.Height / 2f), Color.White * alpha, scale + wiggle);
                mTexture2.Draw(position + new Vector2(mTexture1.Width / 2, 0f), new Vector2(mTexture2.Width - num / 2f, mTexture2.Height / 2f), Color.White * alpha, scale + wiggle);
            }
        }

        private static void DrawText(string text, Vector2 position, float justify, float scale, float alpha)
        {
            float x = ActiveFont.Measure(text).X;
            ActiveFont.DrawOutline(text, position, new Vector2(justify / x, 0.5f), Vector2.One * scale, Color.White * alpha, 2f, Color.Black * alpha);
        }
    }
}
