using System;
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
            visitedTiles.TryGet(x, y, out var value) && value > threshold;

        public LobbyMapOverlay() : base(false, true)
        {
        }

        public override void Added(Entity entity)
        {
            base.Added(entity);

            var visitManager = LobbyVisitManager.ForLobby(new AreaKey(Entity.AreaId), Entity.LobbyIndex);
            if (visitManager == null) return;

            int viewRadiusInTiles = Entity.ExplorationRadius;
            int widthInTiles = (int) Entity.Sprite.Width / Entity.Sprite.ImageScaleX;
            int heightInTiles = (int) Entity.Sprite.Height / Entity.Sprite.ImageScaleY;

            visitedTiles = new ByteArray2D(widthInTiles, heightInTiles);

            var circle = CreateCircleData(viewRadiusInTiles - 1, viewRadiusInTiles + 1);

            foreach (var pos in visitManager.VisitedPoints)
            {
                visitedTiles.Max(circle, (int) pos.Point.X - circle.Width / 2, (int) pos.Point.Y - circle.Height / 2);
            }
            
            // fuzzy edges to remove the hard map boundary
            const byte quarter = byte.MaxValue / 4;
            for (int x = 0; x < widthInTiles; x++)
            {
                visitedTiles[x, 0] = 0;
                visitedTiles[x, 1] = Math.Min(visitedTiles[x, 1], quarter);
                visitedTiles[x, heightInTiles - 1] = 0;
                visitedTiles[x, heightInTiles - 2] = Math.Min(visitedTiles[x, heightInTiles - 2], quarter);
            }
            for (int y = 1; y < heightInTiles - 1; y++)
            {
                visitedTiles[0, y] = 0;
                visitedTiles[1, y] = Math.Min(visitedTiles[1, y], quarter);
                visitedTiles[widthInTiles - 1, y] = 0;
                visitedTiles[widthInTiles - 2, y] = Math.Min(visitedTiles[widthInTiles - 2, y], quarter);
            }
            
            overlay = new Texture2D(Engine.Instance.GraphicsDevice, widthInTiles, heightInTiles, false, SurfaceFormat.Alpha8);
            overlay.SetData(visitedTiles.Data);
        }

        /// <summary>
        /// Generates a 2D array of bytes containing an antialiased circle.
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
                    array[x, y] = (byte) ((1 - alpha) * byte.MaxValue);
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
        }
    }
}
