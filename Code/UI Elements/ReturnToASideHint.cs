using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.UI_Elements
{
    public class ReturnToASideHint : Entity
    {
        public ReturnToASideHint()
        {
            Tag = Tags.HUD;
        }

        public override void Render()
        {
            MTexture mTexture = GFX.Gui["XaphanHelper/skull" + (SceneAs<Level>().Session.Area.Mode == AreaMode.BSide ? "Red" : "Gold")];
            string text = Dialog.Clean("XaphanHelper_UI_GiveUp" + (SceneAs<Level>().Session.Area.Mode == AreaMode.BSide ? "B" : "C") + "Side_info");
            float width = ActiveFont.Measure(text).X * 0.75f;
            float textureWidth = mTexture.Width * 0.75f;
            Vector2 value2 = new((1920f - width - textureWidth - 64f) / 2f, 730f);
            ActiveFont.DrawOutline(text, value2 + new Vector2(width / 2f, 0f), new Vector2(0.5f, 0.5f), Vector2.One * 0.75f, Color.LightGray, 2f, Color.Black);
            value2.X += width + 64f;
            mTexture.DrawCentered(value2 + new Vector2(textureWidth * 0.5f, 0f), Color.White, 0.75f);
        }
    }
}
