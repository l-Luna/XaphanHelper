using Celeste.Mod.XaphanHelper.Controllers;
using Celeste.Mod.XaphanHelper.Data;
using Celeste.Mod.XaphanHelper.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.XaphanHelper.UI_Elements
{
    [Tracked(true)]
    public class LobbyMapDisplay : Entity
    {
        private Level level;

        public Image playerIcon;

        public Image playerIconHair;

        private bool currentPositionIndicator;

        private string CustomImageDirectory;

        //private MTexture[,] SubTextures;

        private int CustomImagesTilesSizeX;

        private int CustomImagesTilesSizeY;

        private List<Vector2> CustomImageTilesCoordinates;

        //private MTexture CustomImage;

        private Sprite CustomImage;

        private List<Vector3> CustomImgTilesToDraw = new List<Vector3>();

        private List<LobbyMapWarpsData> Warps = new List<LobbyMapWarpsData>();

        private List<LobbyMapIconsData> LobbyMapIconsData = new List<Data.LobbyMapIconsData>();

        private Vector2 basePos;

        public float Scale;

        public string currentRoom;

        public int lobbyIndex;

        public int GlobalLobbyID;

        public string LevelSet;

        public int TotalMaps;

        public LobbyHeartsDisplay HeartDisplay;

        public WarpScreen WarpScreen;

        public Coroutine ZoomRoutine = new Coroutine();

        public LobbyMapDisplay(Level level, WarpScreen warpScreen, int lobbyIndex, int globalLobbyID)
        {
            Tag = Tags.HUD;
            this.level = level;
            Scale = 1f;
            WarpScreen = warpScreen;
            AreaKey area = level.Session.Area;
            currentRoom = level.Session.Level;
            this.lobbyIndex = lobbyIndex;
            GlobalLobbyID = globalLobbyID;
            playerIcon = new Image(GFX.Gui["maps/player"]);
            playerIconHair = new Image(GFX.Gui["maps/player_hair"]);
            CustomImagesTilesSizeX = 4;
            CustomImagesTilesSizeY = 4;
            //Visible = false;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            LobbyMapController LobbyMapController = level.Tracker.GetEntity<LobbyMapController>();
            foreach (LobbyMapsLobbiesData lobbiesData in LobbyMapController.LobbiesData)
            {
                if (lobbiesData.LobbyIndex == lobbyIndex)
                {
                    CustomImageDirectory = lobbiesData.ImageDicrectory;
                    LevelSet = lobbiesData.LevelSet;
                    TotalMaps = lobbiesData.TotalMaps;
                }
            }
            level.Add(HeartDisplay = new LobbyHeartsDisplay(new Vector2(100, 180), LevelSet, TotalMaps, lobbyIndex));
            XaphanModule.ModSaveData.GeneratedVisitedLobbyMapTiles.Clear();
            LobbyMapController.GenerateLobbyTiles(CustomImageDirectory, lobbyIndex);
            CustomImageTilesCoordinates = XaphanModule.ModSaveData.GeneratedVisitedLobbyMapTiles;
            Add(CustomImage = new Sprite(GFX.Gui, CustomImageDirectory));
            CustomImage.AddLoop("idle", "", 1f);
            CustomImage.Play("idle");
            CustomImage.Position = basePos;
            CustomImage.Visible = false;
            //GetCustomImage();
        }

        public IEnumerator GenerateMap()
        {
            GetWarpStations();
            GetIcons();
            Player player = level.Tracker.GetEntity<Player>();
            foreach (LobbyMapWarpsData warpData in Warps)
            {
                if (warpData.Index == level.Tracker.GetNearestEntity<WarpStation>(player.Position).index)
                {
                    basePos = new Vector2(-(warpData.Position.X / 2) * Scale + Engine.Width / 2, -(warpData.Position.Y / 2) * Scale + 600);
                }
            }
            yield return null;
        }

        /*public void GetCustomImage()
        {
            if (CustomImageDirectory != null && CustomImagesTilesSizeX != 0 && CustomImagesTilesSizeY != 0)
            {
                CustomImage = GFX.Gui[CustomImageDirectory];
                SubTextures = new MTexture[CustomImage.Width / CustomImagesTilesSizeX, CustomImage.Height / CustomImagesTilesSizeY];
                for (int i = 0; i < CustomImage.Width / CustomImagesTilesSizeX; i++)
                {
                    for (int j = 0; j < CustomImage.Height / CustomImagesTilesSizeY; j++)
                    {
                        SubTextures[i, j] = CustomImage.GetSubtexture(new Rectangle(i * CustomImagesTilesSizeX, j * CustomImagesTilesSizeY, CustomImagesTilesSizeX, CustomImagesTilesSizeY));
                    }
                }
            }
            Visible = true;
        }*/

        public void GetWarpStations()
        {
            AreaKey area = level.Session.Area;
            MapData MapData = AreaData.Areas[GlobalLobbyID].Mode[(int)area.Mode].MapData;
            foreach (LevelData levelData in MapData.Levels)
            {
                foreach (EntityData entity in levelData.Entities)
                {
                    if (entity.Name == "XaphanHelper/WarpStation")
                    {
                        Warps.Add(new LobbyMapWarpsData(levelData.Name, new Vector2(entity.Position.X + entity.Width / 2f, entity.Position.Y + entity.Height / 2f), entity.Int("index")));
                    }
                }
            }
        }

        private void GetIcons()
        {
            AreaKey area = level.Session.Area;
            MapData MapData = AreaData.Areas[GlobalLobbyID].Mode[(int)area.Mode].MapData;
            foreach (LevelData levelData in MapData.Levels)
            {
                List<AreaStats> Maps = new List<AreaStats>(SaveData.Instance.Areas_Safe).ToList();
                foreach (EntityData entity in levelData.Entities)
                {
                    if (entity.Name == "XaphanHelper/WarpStation")
                    {
                        LobbyMapIconsData.Add(new LobbyMapIconsData("lobbies/bench", level.Session.Level, new Vector2(entity.Position.X + entity.Width / 2f, entity.Position.Y + entity.Height / 2f - 16f)));
                    }
                    if (entity.Name == "CollabUtils2/MiniHeartDoor")
                    {
                        LobbyMapIconsData.Add(new LobbyMapIconsData("lobbies/heartgate", level.Session.Level, new Vector2(entity.Position.X + entity.Width / 2f, entity.Position.Y)));
                    }
                    if (entity.Name == "CollabUtils2/RainbowBerry")
                    {
                        LobbyMapIconsData.Add(new LobbyMapIconsData("lobbies/berry", level.Session.Level, new Vector2(entity.Position.X + entity.Width / 2f, entity.Position.Y + entity.Height / 2f)));
                    }
                    if (entity.Name == "SJ2021/StrawberryJamJar")
                    {
                        bool mapCompleted = false;
                        foreach (AreaStats map in Maps)
                        {
                            if (!mapCompleted)
                            {
                                AreaData areaData = AreaData.Get(map.ID_Safe);
                                if (areaData.SID == entity.Attr("map"))
                                {
                                    mapCompleted = map.Modes[0].HeartGem;
                                    continue;
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                        LobbyMapIconsData.Add(new LobbyMapIconsData(mapCompleted ? "lobbies/jarfull" : "lobbies/jar", level.Session.Level, new Vector2(entity.Position.X + entity.Width / 2f, entity.Position.Y + entity.Height / 2f - 16f)));
                    }
                }
                foreach (EntityData trigger in levelData.Triggers)
                {
                    if (trigger.Name == "CollabUtils2/ChapterPanelTrigger")
                    {
                        bool mapCompleted = false;
                        foreach (AreaStats map in Maps)
                        {
                            if (!mapCompleted)
                            {
                                AreaData areaData = AreaData.Get(map.ID_Safe);
                                if (areaData.SID == trigger.Attr("map"))
                                {
                                    mapCompleted = map.Modes[0].HeartGem;
                                    continue;
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                        LobbyMapIconsData.Add(new LobbyMapIconsData(mapCompleted ? "lobbies/jarfull" : "lobbies/jar", level.Session.Level, new Vector2(trigger.Position.X + trigger.Width / 2f, trigger.Position.Y + trigger.Height / 2f)));
                    }
                    if (trigger.Name == "CollabUtils2/JournalTrigger")
                    {
                        LobbyMapIconsData.Add(new LobbyMapIconsData("lobbies/journal", level.Session.Level, new Vector2(trigger.Position.X + trigger.Width / 2f, trigger.Position.Y + trigger.Height / 2f)));
                    }
                }
            }
        }

        public bool CameraIsMoving;

        public override void Update()
        {
            base.Update();
            if (Input.MenuJournal.Pressed && !ZoomRoutine.Active)
            {
                Add(ZoomRoutine = new Coroutine(ChangeZoom()));
            }
            if (CustomImage != null && CustomImagesTilesSizeX != 0 && CustomImagesTilesSizeY != 0 && WarpScreen.destDisplay != null)
            {
                foreach (LobbyMapWarpsData warp in Warps)
                {
                    if (warp.Index == WarpScreen.destDisplay.Index)
                    {
                        // Camera move to selected warp
                        if (Scale > 0.5f)
                        {
                            basePos = new Vector2(Calc.Approach(basePos.X, -(warp.Position.X / 2) * Scale + Engine.Width / 2, ZoomRoutine.Active ? 500f : 10f * (0.75f + Scale)), Calc.Approach(basePos.Y, -(warp.Position.Y / 2) * Scale + 600, ZoomRoutine.Active ? 500f : 10f * (0.75f + Scale)));
                            if (basePos != new Vector2(-(warp.Position.X / 2) * Scale + Engine.Width / 2, -(warp.Position.Y / 2) * Scale + 600))
                            {
                                CameraIsMoving = true;
                            }
                            else
                            {
                                CameraIsMoving = false;
                            }
                        }
                        else
                        {
                            basePos = new Vector2(Calc.Approach(basePos.X, (Engine.Width - CustomImage.Width * Scale) / 2, 30f), Calc.Approach(basePos.Y, (900 - CustomImage.Height * Scale) / 2 + 100, 30f));
                        }
                        // Instantly move to selected warp
                        /*if (Scale > 0.25f)
                        {
                            basePos = new Vector2(-warp.Position.X * Scale + Engine.Width / 2, -warp.Position.Y * Scale + 600);
                        }
                        else
                        {
                            basePos = new Vector2((Engine.Width - CustomImage.Width * Scale) / 2, (900 - CustomImage.Height * Scale) / 2 + 100);
                        }*/
                    }
                }
            }
                if (Engine.Scene.OnRawInterval(0.3f))
            {
                if (currentPositionIndicator)
                {
                    currentPositionIndicator = false;
                }
                else
                {
                    currentPositionIndicator = true;
                }
            }
        }

        private IEnumerator ChangeZoom()
        {
            if (Scale != 0.5f)
            {
                float finalScale = Scale - 0.25f;
                while (Scale > finalScale)
                {
                    Scale -= Engine.DeltaTime * 1.25f;
                    yield return null;
                }
                Scale = finalScale;
            }
            else
            {
                float finalScale = 1f;
                while (Scale < finalScale)
                {
                    Scale += Engine.DeltaTime * 2.5f;
                    yield return null;
                }
                Scale = finalScale;
            }
            yield return null;
        }


        public override void Render()
        {
            base.Render();
            Draw.Rect(new Vector2(100, 180), 1720, 840, Color.Black * 0.9f);
            if (CustomImage != null && CustomImagesTilesSizeX != 0 && CustomImagesTilesSizeY != 0 && WarpScreen.destDisplay != null)
            {
                foreach (Vector2 tile in CustomImageTilesCoordinates)
                {
                    //SubTextures[(int)tile.X, (int)tile.Y].Draw(new Vector2(basePos.X + tile.X * CustomImagesTilesSizeX * Scale, basePos.Y + tile.Y * CustomImagesTilesSizeY * Scale), Vector2.Zero, Color.White * 1, Scale);
                    CustomImage.Scale = new Vector2(Scale);
                    CustomImage.DrawSubrect(basePos + new Vector2(tile.X * CustomImagesTilesSizeX * Scale, tile.Y * CustomImagesTilesSizeY * Scale), new Rectangle((int)tile.X * CustomImagesTilesSizeX, (int)tile.Y * CustomImagesTilesSizeY, 4, 4));
                }
                foreach (LobbyMapIconsData icon in LobbyMapIconsData)
                {
                    if (CustomImageTilesCoordinates.Contains(new Vector2((float)Math.Floor(icon.Position.X / 8), (float)Math.Floor(icon.Position.Y / 8))))
                    {
                        Image iconImage = null;
                        iconImage = new Image(GFX.Gui["maps/" + icon.Type]);
                        iconImage.CenterOrigin();
                        iconImage.Position = new Vector2(basePos.X + icon.Position.X / 8f * CustomImagesTilesSizeX * Scale, basePos.Y + icon.Position.Y / 8 * CustomImagesTilesSizeY * Scale);
                        iconImage.Scale = new Vector2(Math.Max(0.4f, Scale - 0.5f), Math.Max(0.4f, Scale - 0.5f));
                        iconImage.Render();
                    }
                }
                foreach (LobbyMapWarpsData warp in Warps)
                {
                    if (warp.Index == WarpScreen.destDisplay.Index)
                    {
                        if (playerIcon != null && playerIconHair != null && WarpScreen.destDisplay != null && currentPositionIndicator)
                        {
                            if (warp.Index == WarpScreen.destDisplay.Index)
                            {
                                playerIcon.CenterOrigin();
                                playerIconHair.CenterOrigin();
                                playerIcon.Position = playerIconHair.Position = new Vector2(basePos.X + warp.Position.X / 8f * CustomImagesTilesSizeX * Scale, basePos.Y + (warp.Position.Y - 16f) / 8f * CustomImagesTilesSizeY * Scale);
                                playerIconHair.Color = (level.Tracker.GetEntity<Player>() != null ? level.Tracker.GetEntity<Player>().Hair.Color : Color.White);
                            }
                            playerIcon.Render();
                            playerIconHair.Render();
                        }
                    }
                }
            }
        }

        public override void Removed(Scene scene)
        {
            base.Removed(scene);
            if (HeartDisplay != null)
            {
                HeartDisplay.RemoveSelf();
            }
        }
    }
}
