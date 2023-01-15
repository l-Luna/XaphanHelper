using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.UI_Elements.LobbyMap
{
    public class LobbyMapSprite : Sprite
    {
        public new LobbyMapDisplay Entity => base.Entity as LobbyMapDisplay;

        public readonly int WidthInTiles;
        public readonly int HeightInTiles;

        public readonly int ImageScaleX;
        public readonly int ImageScaleY;

        public Vector2 Size => new Vector2(Width, Height);

        public LobbyMapSprite(string path, int imageScaleX, int imageScaleY) : base(GFX.Gui, path)
        {
            ImageScaleX = imageScaleX;
            ImageScaleY = imageScaleY;

            Position = new Vector2(Engine.Width / 2f, Engine.Height / 2f);

            AddLoop("idle", string.Empty, 1f);
            Play("idle");

            WidthInTiles = (int) (Width / imageScaleX);
            HeightInTiles = (int) (Height / imageScaleY);
        }

        public override void Render()
        {
            Scale = new Vector2(Entity.Scale);
            JustifyOrigin(Entity.Origin);
            base.Render();
        }
    }
}
