using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Celeste.Mod.Entities;
using Celeste.Mod.XaphanHelper.Data;
using Celeste.Mod.XaphanHelper.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Controllers
{
    [Tracked(true)]
    [CustomEntity("XaphanHelper/TilesetsController")]
    class TilesetsController : Entity
    {
        public static List<TilesetsControllerData> TilesetsControllerData = new();

        private char oldTileset1;

        private char newTileset1;

        private char oldTileset2;

        private char newTileset2;

        private char oldTileset3;

        private char newTileset3;

        private char oldTileset4;

        private char newTileset4;

        private char oldTileset5;

        private char newTileset5;

        private char oldTileset6;

        private char newTileset6;

        private char oldTileset7;

        private char newTileset7;

        private char oldTileset8;

        private char newTileset8;

        private char oldTileset9;

        private char newTileset9;

        private char oldTileset10;

        private char newTileset10;

        private string currentLevel;      

        private static bool storedData;

        private string flag;

        public TilesetsController(Vector2 position, Vector2 offset, char oldTileset1, char newTileset1, char oldTileset2, char newTileset2, char oldTileset3, char newTileset3, char oldTileset4, char newTileset4, char oldTileset5, char newTileset5, char oldTileset6, char newTileset6, char oldTileset7, char newTileset7, char oldTileset8, char newTileset8, char oldTileset9, char newTileset9, char oldTileset10, char newTileset10, string flag) : base(position + offset)
        {
            Tag = Tags.TransitionUpdate | Tags.Persistent | Tags.Global;
            this.oldTileset1 = oldTileset1;
            this.newTileset1 = newTileset1;
            this.oldTileset2 = oldTileset2;
            this.newTileset2 = newTileset2;
            this.oldTileset3 = oldTileset3;
            this.newTileset3 = newTileset3;
            this.oldTileset4 = oldTileset4;
            this.newTileset4 = newTileset4;
            this.oldTileset5 = oldTileset5;
            this.newTileset5 = newTileset5;
            this.oldTileset6 = oldTileset6;
            this.newTileset6 = newTileset6;
            this.oldTileset7 = oldTileset7;
            this.newTileset7 = newTileset7;
            this.oldTileset8 = oldTileset8;
            this.newTileset8 = newTileset8;
            this.oldTileset9 = oldTileset9;
            this.newTileset9 = newTileset9;
            this.oldTileset10 = oldTileset10;
            this.newTileset10 = newTileset10;
            this.flag = flag;
        }

        public TilesetsController(EntityData data, Vector2 offset) : this(data.Position, offset, data.Char("oldTileset1", '0'), data.Char("newTileset1", '0'), data.Char("oldTileset2", '0'), data.Char("newTileset2", '0'),
            data.Char("oldTileset3", '0'), data.Char("newTileset3", '0'), data.Char("oldTileset4", '0'), data.Char("newTileset4", '0'), data.Char("oldTileset5", '0'), data.Char("newTileset5", '0'),
            data.Char("oldTileset6", '0'), data.Char("newTileset6", '0'), data.Char("oldTileset7", '0'), data.Char("newTileset7", '0'), data.Char("oldTileset8", '0'), data.Char("newTileset8", '0'),
            data.Char("oldTileset9", '0'), data.Char("newTileset9", '0'), data.Char("oldTileset10", '0'), data.Char("newTileset10", '0'), data.Attr("flag"))
        {
            
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
            if (!string.IsNullOrEmpty(flag) && SceneAs<Level>().Session.GetFlag(flag))
            {
                if (currentLevel != SceneAs<Level>().Session.Level)
                {
                    if (!XaphanModule.TilesetsControllerGeneratedLevelsTiles.ContainsKey(SceneAs<Level>().Session.Level) && !XaphanModule.TilesetsControllerGeneratedLevelsData.ContainsKey(SceneAs<Level>().Session.Level))
                    {
                        switchTilesets();
                    }
                    else
                    {
                        applyGeneratedTileset(XaphanModule.TilesetsControllerGeneratedLevelsTiles[SceneAs<Level>().Session.Level], XaphanModule.TilesetsControllerGeneratedLevelsData[SceneAs<Level>().Session.Level]);
                    }
                }
            }
        }

        public static void Load()
        {
            Everest.Events.Level.OnEnter += onLevelEnter;
            Everest.Events.Level.OnExit += onLevelExit;
            On.Celeste.Level.Update += onLevelUpdate;
        }

        public static void Unload()
        {
            Everest.Events.Level.OnExit -= onLevelExit;
            On.Celeste.Level.Update -= onLevelUpdate;
        }

        private static void onLevelEnter(Session session, bool fromSaveData)
        {
            if (storedData)
            {
                TilesetsControllerData.Clear();
                storedData = false;
                getData(session);
            }
        }

        private static void onLevelExit(Level level, LevelExit exit, LevelExit.Mode mode, Session session, HiresSnow snow)
        {
            if (storedData)
            {
                TilesetsControllerData.Clear();
                storedData = false;
            }
        }

        private static void onLevelUpdate(On.Celeste.Level.orig_Update orig, Level self)
        {
            orig(self);
            if (storedData && TilesetsControllerData.Count == 0)
            {
                storedData = false;
            }
            if (!storedData)
            {
                getData(self.Session);
            }
            if (self.Tracker.GetEntities<TilesetsController>().Count == 0 && TilesetsControllerData.Count != 0)
            {
                foreach (TilesetsControllerData data in TilesetsControllerData)
                {
                    if (data.Prefix == self.Session.Area.LevelSet && data.ChapterIndex == self.Session.Area.ChapterIndex)
                    {
                        self.Add(new TilesetsController(Vector2.Zero, Vector2.Zero, data.OldTileset1, data.NewTileset1, data.OldTileset2, data.NewTileset2, data.OldTileset3, data.NewTileset3, data.OldTileset4, data.NewTileset4, data.OldTileset5, data.NewTileset5, data.OldTileset6, data.NewTileset6, data.OldTileset7, data.NewTileset7, data.OldTileset8, data.NewTileset8, data.OldTileset9, data.NewTileset9, data.OldTileset10, data.NewTileset10, data.Flag));
                    }
                }
            }
        }

        public static void getData(Session session)
        {
            MapData MapData = AreaData.Areas[session.Area.ID].Mode[0].MapData;
            if (MapData != null)
            {
                foreach (LevelData levelData in MapData.Levels)
                {
                    foreach (EntityData entity in levelData.Entities)
                    {
                        if (entity.Name == "XaphanHelper/TilesetsController")
                        {
                            TilesetsControllerData.Add(new TilesetsControllerData(session.Area.LevelSet, session.Area.ChapterIndex == - 1 ? 0 : session.Area.ChapterIndex,
                                entity.Char("oldTileset1"), entity.Char("newTileset1"), entity.Char("oldTileset2"), entity.Char("newTileset2"), entity.Char("oldTileset3"), entity.Char("newTileset3"),
                                entity.Char("oldTileset4"), entity.Char("newTileset4"), entity.Char("oldTileset5"), entity.Char("newTileset5"), entity.Char("oldTileset6"), entity.Char("newTileset6"),
                                entity.Char("oldTileset7"), entity.Char("newTileset7"), entity.Char("oldTileset8"), entity.Char("newTileset8"), entity.Char("oldTileset9"), entity.Char("newTileset9"),
                                entity.Char("oldTileset10"), entity.Char("newTileset10"), entity.Attr("flag")));
                            break;
                        }
                    }
                }
            }
            storedData = true;  
        }

        private void switchTilesets()
        {
            Level level = Scene as Level;
            currentLevel = level.Session.Level;
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
                    if (fgData[x, y] == oldTileset1)
                    {
                        if (x > 0 && x < fgTexes.Columns && y > 0 && y < fgTexes.Rows && fgData[x, y] != '0')
                        {
                            newFgData[x - ox + 1, y - oy + 1] = newTileset1;
                        }
                    }
                    else if (fgData[x, y] == oldTileset2)
                    {
                        if (x > 0 && x < fgTexes.Columns && y > 0 && y < fgTexes.Rows && fgData[x, y] != '0')
                        {
                            newFgData[x - ox + 1, y - oy + 1] = newTileset2;
                        }
                    }
                    else if (fgData[x, y] == oldTileset3)
                    {
                        if (x > 0 && x < fgTexes.Columns && y > 0 && y < fgTexes.Rows && fgData[x, y] != '0')
                        {
                            newFgData[x - ox + 1, y - oy + 1] = newTileset3;
                        }
                    }
                    else if (fgData[x, y] == oldTileset4)
                    {
                        if (x > 0 && x < fgTexes.Columns && y > 0 && y < fgTexes.Rows && fgData[x, y] != '0')
                        {
                            newFgData[x - ox + 1, y - oy + 1] = newTileset4;
                        }
                    }
                    else if (fgData[x, y] == oldTileset5)
                    {
                        if (x > 0 && x < fgTexes.Columns && y > 0 && y < fgTexes.Rows && fgData[x, y] != '0')
                        {
                            newFgData[x - ox + 1, y - oy + 1] = newTileset5;
                        }
                    }
                    else if (fgData[x, y] == oldTileset6)
                    {
                        if (x > 0 && x < fgTexes.Columns && y > 0 && y < fgTexes.Rows && fgData[x, y] != '0')
                        {
                            newFgData[x - ox + 1, y - oy + 1] = newTileset6;
                        }
                    }
                    else if(fgData[x, y] == oldTileset7)
                    {
                        if (x > 0 && x < fgTexes.Columns && y > 0 && y < fgTexes.Rows && fgData[x, y] != '0')
                        {
                            newFgData[x - ox + 1, y - oy + 1] = newTileset7;
                        }
                    }
                    else if(fgData[x, y] == oldTileset8)
                    {
                        if (x > 0 && x < fgTexes.Columns && y > 0 && y < fgTexes.Rows && fgData[x, y] != '0')
                        {
                            newFgData[x - ox + 1, y - oy + 1] = newTileset8;
                        }
                    }
                    else if(fgData[x, y] == oldTileset9)
                    {
                        if (x > 0 && x < fgTexes.Columns && y > 0 && y < fgTexes.Rows && fgData[x, y] != '0')
                        {
                            newFgData[x - ox + 1, y - oy + 1] = newTileset9;
                        }
                    }
                    else if(fgData[x, y] == oldTileset10)
                    {
                        if (x > 0 && x < fgTexes.Columns && y > 0 && y < fgTexes.Rows && fgData[x, y] != '0')
                        {
                            newFgData[x - ox + 1, y - oy + 1] = newTileset10;
                        }
                    }
                    else if (fgData[x, y] != '0')
                    {
                        newFgData[x - ox + 1, y - oy + 1] = fgData[x, y];
                    }
                }
            }
            Autotiler.Generated newFgTiles = GFX.FGAutotiler.GenerateMap(newFgData, false);
            XaphanModule.TilesetsControllerGeneratedLevelsTiles.Add(level.Session.Level, newFgTiles);
            XaphanModule.TilesetsControllerGeneratedLevelsData.Add(level.Session.Level, newFgData);
            applyGeneratedTileset(newFgTiles, newFgData);
        }

        private void applyGeneratedTileset(Autotiler.Generated newFgTiles, VirtualMap<char> newFgData)
        {
            Level level = Scene as Level;
            currentLevel = level.Session.Level;
            int ox = level.LevelSolidOffset.X;
            int oy = level.LevelSolidOffset.Y;
            int tw = (int)Math.Ceiling(level.Bounds.Width / 8f);
            int th = (int)Math.Ceiling(level.Bounds.Height / 8f);
            VirtualMap<char> fgData = level.SolidsData;
            VirtualMap<MTexture> fgTexes = level.SolidTiles.Tiles.Tiles;
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
        }
    }
}
