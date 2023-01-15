using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste.Mod.XaphanHelper.UI_Elements.LobbyMap
{
    public class LobbyMapOverlay : Component
    {
        public new LobbyMapDisplay Entity => base.Entity as LobbyMapDisplay;

        private ByteArray2D visitedTiles;
        private Texture2D overlay;

        public bool IsVisited(int x, int y, byte threshold = 0x7F) =>
            visitedTiles.TryGet(x, y, out var value) && value < threshold;

        public LobbyMapOverlay() : base(false, true)
        {
        }

        public override void Added(Entity entity)
        {
            base.Added(entity);

            const int viewRadiusInTiles = 20;

            var visitManager = LobbyVisitManager.ForLobby(new AreaKey(Entity.AreaId), Entity.LobbyIndex);
            if (visitManager == null) return;

            int widthInTiles = (int) Entity.Sprite.Width / Entity.Sprite.ImageScaleX;
            int heightInTiles = (int) Entity.Sprite.Height / Entity.Sprite.ImageScaleY;

            visitedTiles = new ByteArray2D(widthInTiles, heightInTiles, byte.MaxValue);

            var circle = CreateCircleData(viewRadiusInTiles - 1, viewRadiusInTiles + 1);

            foreach (var pos in visitManager.VisitedPoints)
            {
                visitedTiles.Min(circle, (int) pos.Point.X - circle.Width / 2, (int) pos.Point.Y - circle.Height / 2);
            }

            overlay = new Texture2D(Engine.Instance.GraphicsDevice, widthInTiles, heightInTiles, false, SurfaceFormat.Alpha8);
            overlay.SetData(visitedTiles.Data);
        }

        /// <summary>
        /// Generates a 2D array of bytes containing an antialiased inverted circle.
        /// </summary>
        private static ByteArray2D CreateCircleData(int hardRadius, int softRadius)
        {
            var diameter = 2 * softRadius;
            var array = new ByteArray2D(diameter, diameter);

            for (int y = 0; y < diameter; y++)
            {
                for (int x = 0; x < diameter; x++)
                {
                    var lenSq = (softRadius - x) * (softRadius - x) + (softRadius - y) * (softRadius - y);
                    float alpha =
                        lenSq < hardRadius * hardRadius ? 0f :
                        lenSq > softRadius * softRadius ? 1f :
                        ((float) lenSq - hardRadius * hardRadius) / (softRadius * softRadius - hardRadius * hardRadius);
                    array[x, y] = (byte) (alpha * byte.MaxValue);
                }
            }

            return array;
        }

        public override void Render()
        {
            var origin = new Vector2(Entity.Origin.X * overlay.Width, Entity.Origin.Y * overlay.Height);
            var position = new Vector2(Engine.Width / 2f, Engine.Height / 2f);
            var scale = new Vector2(Entity.Scale * Entity.Sprite.ImageScaleX, Entity.Scale * Entity.Sprite.ImageScaleY);
            Draw.SpriteBatch.Draw(overlay, position, null, Color.Black, 0, origin, scale, SpriteEffects.None, 0);
        
            // draw a thin border to make sure we cover any flickering lines
            const float thickness = 2f;
            var lineColor = Color.Black;
            var topLeft = position - origin * scale;
            var bottomRight = topLeft + new Vector2(overlay.Width * scale.X, overlay.Height * scale.Y);
        
            Draw.Line(topLeft.X, topLeft.Y, topLeft.X, bottomRight.Y, lineColor, thickness);
            Draw.Line(topLeft.X, topLeft.Y, bottomRight.X, topLeft.Y, lineColor, thickness);
            Draw.Line(bottomRight.X, topLeft.Y, bottomRight.X, bottomRight.Y, lineColor, thickness);
            Draw.Line(topLeft.X, bottomRight.Y, bottomRight.X, bottomRight.Y, lineColor, thickness);
        }
    }
}
