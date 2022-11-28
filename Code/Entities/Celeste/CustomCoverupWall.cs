using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [Tracked(true)]
    [CustomEntity("XaphanHelper/CustomCoverupWall")]
    class CustomCoverupWall : Entity
    {
        private char fillTile;

        private char flagFillTile;

        private TileGrid tiles;

        private EffectCutout cutout;

        private string flag;

        public CustomCoverupWall(EntityData data, Vector2 position, EntityID eid) : base(data.Position + position)
        {
            fillTile = data.Char("tiletype", '3');
            flagFillTile = data.Char("flagTiletype", '3');
            flag = data.Attr("flag");
            Collider = new Hitbox(data.Width, data.Height);
            Add(cutout = new EffectCutout());
            Depth = -13000;

        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            int tilesX = (int)Width / 8;
            int tilesY = (int)Height / 8;
            Level level = SceneAs<Level>();
            Rectangle tileBounds = level.Session.MapData.TileBounds;
            VirtualMap<char> solidsData = level.SolidsData;
            int x = (int)X / 8 - tileBounds.Left;
            int y = (int)Y / 8 - tileBounds.Top;
            Add(tiles = GFX.FGAutotiler.GenerateOverlay((!string.IsNullOrEmpty(flag) && level.Session.GetFlag(flag)) ? flagFillTile : fillTile, x, y, tilesX, tilesY, solidsData).TileGrid);
            Add(new TileInterceptor(tiles, highPriority: false));
        }
    }
}
