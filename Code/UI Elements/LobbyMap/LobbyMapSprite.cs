using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste.Mod.XaphanHelper.UI_Elements.LobbyMap
{
    public class LobbyMapSprite : Sprite
    {
        public Texture2D MapTexture => Animations.TryGetValue(CurrentAnimationID, out var animation)
            ? animation.Frames.ElementAtOrDefault(CurrentAnimationFrame)?.Texture.Texture
            : null;
        
        public readonly int WidthInTiles;
        public readonly int HeightInTiles;

        public readonly int ImageScaleX;
        public readonly int ImageScaleY;

        public Vector2 Size => new Vector2(Width, Height);

        public LobbyMapSprite(string path, int imageScaleX, int imageScaleY) : base(GFX.Gui, path)
        {
            ImageScaleX = imageScaleX;
            ImageScaleY = imageScaleY;

            Visible = false;

            AddLoop("idle", string.Empty, 1f);
            Play("idle");

            WidthInTiles = (int) (Width / imageScaleX);
            HeightInTiles = (int) (Height / imageScaleY);
        }
    }
}
