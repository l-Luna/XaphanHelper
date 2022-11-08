using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Celeste.Mod.Entities;
using Celeste.Mod.XaphanHelper.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Controllers
{
    [Tracked(true)]
    [CustomEntity("XaphanHelper/TilesetsController")]
    class TilesetsController : Entity
    {
        public static FieldInfo coverupWallFillTile = typeof(CoverupWall).GetField("fillTile", BindingFlags.Instance | BindingFlags.NonPublic);

        private char targetTileset;

        private char replacementTileset;

        private string currentLevel;

        private List<string> generatedLevels = new();
        public TilesetsController(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            Tag = Tags.TransitionUpdate | Tags.Persistent | Tags.Global;
            targetTileset = data.Char("targetTileset", '3');
            replacementTileset = data.Char("replacementTileset", '3');
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            if (SceneAs<Level>().Tracker.GetEntities<TilesetsController>().Count > 1)
            {
                RemoveSelf();
            }
        }

        public override void Update()
        {
            base.Update();
            if (currentLevel != SceneAs<Level>().Session.Level && !generatedLevels.Contains(SceneAs<Level>().Session.Level))
            {
                Add(new Coroutine(switchTilesets()));
            }
            foreach (CoverupWall coverupWall in SceneAs<Level>().Tracker.GetEntities<CoverupWall>())
            {
                if ((char)coverupWallFillTile.GetValue(coverupWall) == targetTileset)
                {
                    SceneAs<Level>().Add(new CoverupWall(coverupWall.Position, replacementTileset, coverupWall.Width, coverupWall.Height));
                    SceneAs<Level>().Remove(coverupWall);
                }
            }
        }

        private IEnumerator switchTilesets()
        {
            Level level = Scene as Level;
            currentLevel = level.Session.Level;
            generatedLevels.Add(level.Session.Level);
            int ox = level.LevelSolidOffset.X;
            int oy = level.LevelSolidOffset.Y;
            int tw = (int)Math.Ceiling(level.Bounds.Width / 8f);
            int th = (int)Math.Ceiling(level.Bounds.Height / 8f);
            VirtualMap<char> fgData = level.SolidsData;
            VirtualMap<MTexture> fgTexes = level.SolidTiles.Tiles.Tiles;
            VirtualMap<char> newFgData = new VirtualMap<char>(tw + 2, th + 2, '0');
            for (int x = ox - 1; x < ox + tw + 1; x++)
            {
                for (int y = oy - 1; y < oy + th + 1; y++)
                {
                    if ((fgData[x, y] == replacementTileset || fgData[x, y] == targetTileset))
                    {
                        if (x > 0 && x < fgTexes.Columns && y > 0 && y < fgTexes.Rows && fgData[x, y] != '0')
                        {
                            newFgData[x - ox + 1, y - oy + 1] = replacementTileset;
                        }
                    }
                    else if (fgData[x, y] != '0')
                    {
                        newFgData[x - ox + 1, y - oy + 1] = fgData[x, y];
                    }
                }
            }
            Autotiler.Generated newFgTiles = GFX.FGAutotiler.GenerateMap(newFgData, false);
            for (int x = ox - 1; x < ox + tw + 1; x++)
            {
                for (int y = oy - 1; y < oy + th + 1; y++)
                {
                    if (x > 0 && x < fgTexes.Columns && y > 0 && y < fgTexes.Rows && x >= ox && x < ox + tw && y >= oy && y < oy + th && fgTexes[x, y] != newFgTiles.TileGrid.Tiles[x - ox + 1, y - oy + 1])
                    {
                        fgData[x, y] = newFgData[x - ox + 1, y - oy + 1];
                        fgTexes[x, y] = newFgTiles.TileGrid.Tiles[x - ox + 1, y - oy + 1];
                    }
                }
            }
            /*foreach (Slope slope in SceneAs<Level>().Tracker.GetEntities<Slope>())
            {
                if (slope.Texture != "dirt")
                {
                    SceneAs<Level>().Add(new Slope(slope.Position, Vector2.Zero, slope.Gentle, slope.Side, slope.SoundIndex, slope.SlopeHeight, slope.TilesTop, slope.TilesBottom, "dirt", slope.CanSlide, "objects/XaphanHelper/Slope", slope.UpsideDown, slope.NoRender, slope.StickyDash, slope.Rainbow, slope.CanJumpThrough, true));
                    level.Remove(slope);
                }
            }*/
            yield return null;
        }
    }
}
