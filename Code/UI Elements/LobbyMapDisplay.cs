using System;
using System.Collections;
using System.Collections.Generic;
using Celeste.Mod.XaphanHelper.Controllers;
using Celeste.Mod.XaphanHelper.Data;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.UI_Elements
{
    [Tracked(true)]
    public class LobbyMapDisplay : Entity
    {
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

        private List<LobbyMapIconsData> LobbyMapIconsData = new();

        private Vector2 basePos;

        public float Scale;

        public int TotalMaps;

        public LobbyHeartsDisplay HeartDisplay;

        private WarpScreen warpScreen;

        public Coroutine ZoomRoutine = new();

        private int areaId;

        private string room;

        private LevelData levelData;

        public LobbyMapDisplay(WarpScreen warpScreen, int areaId, string room)
        {
            this.warpScreen = warpScreen;
            this.areaId = areaId;
            this.room = room;
            Tag = Tags.HUD;
            Scale = 1f;
            playerIcon = new Image(GFX.Gui["maps/player"]);
            playerIconHair = new Image(GFX.Gui["maps/player_hair"]);
            CustomImagesTilesSizeX = 4;
            CustomImagesTilesSizeY = 4;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            Level level = SceneAs<Level>();

            AreaKey area = new(areaId);
            MapData mapData = AreaData.Areas[areaId].Mode[0].MapData;
            levelData = mapData.Get(room);
            int lobbyIndex = 0;

            if (levelData?.GetEntityData("XaphanHelper/LobbyMapController") is EntityData lobbyMapControllerData)
            {
                CustomImageDirectory = lobbyMapControllerData.Attr("directory");
                TotalMaps = lobbyMapControllerData.Int("totalMaps");
                lobbyIndex = lobbyMapControllerData.Int("lobbyIndex");
            }
            else
            {
                RemoveSelf();
                return;
            }

            level.Add(HeartDisplay = new LobbyHeartsDisplay(new Vector2(100, 180), area.LevelSet, TotalMaps, lobbyIndex));

            Add(CustomImage = new Sprite(GFX.Gui, CustomImageDirectory));
            CustomImage.AddLoop("idle", "", 1f);
            CustomImage.Play("idle");
            CustomImage.Position = basePos;
            CustomImage.Visible = false;

            XaphanModule.ModSaveData.GeneratedVisitedLobbyMapTiles.Clear();
            LobbyMapController.GenerateLobbyTiles(areaId, CustomImage.Texture);
            CustomImageTilesCoordinates = XaphanModule.ModSaveData.GeneratedVisitedLobbyMapTiles;
        }

        public IEnumerator GenerateMap(Vector2 initialPos)
        {
            GetIcons();
            basePos = new Vector2(-(initialPos.X / 2) * Scale + Engine.Width / 2, -(initialPos.Y / 2) * Scale + 600);
            yield return null;
        }

        private void GetIcons()
        {
            foreach (EntityData entity in levelData.Entities)
            {
                if (entity.Name == "XaphanHelper/WarpStation")
                {
                    LobbyMapIconsData.Add(new LobbyMapIconsData("lobbies/bench", levelData.Name, new Vector2(entity.Position.X + entity.Width / 2f, entity.Position.Y + entity.Height / 2f - 16f)));
                }
                if (entity.Name == "CollabUtils2/MiniHeartDoor")
                {
                    LobbyMapIconsData.Add(new LobbyMapIconsData("lobbies/heartgate", levelData.Name, new Vector2(entity.Position.X + entity.Width / 2f, entity.Position.Y)));
                }
                if (entity.Name == "CollabUtils2/RainbowBerry")
                {
                    LobbyMapIconsData.Add(new LobbyMapIconsData("lobbies/berry", levelData.Name, new Vector2(entity.Position.X + entity.Width / 2f, entity.Position.Y + entity.Height / 2f)));
                }
                if (entity.Name == "SJ2021/StrawberryJamJar")
                {
                    bool mapCompleted = SaveData.Instance.Areas[areaId].Modes[0].HeartGem;
                    LobbyMapIconsData.Add(new LobbyMapIconsData(mapCompleted ? "lobbies/jarfull" : "lobbies/jar", levelData.Name, new Vector2(entity.Position.X + entity.Width / 2f, entity.Position.Y + entity.Height / 2f - 16f)));
                }
            }

            foreach (EntityData trigger in levelData.Triggers)
            {
                if (trigger.Name == "CollabUtils2/ChapterPanelTrigger")
                {
                    bool mapCompleted = SaveData.Instance.Areas[areaId].Modes[0].HeartGem;
                    LobbyMapIconsData.Add(new LobbyMapIconsData(mapCompleted ? "lobbies/jarfull" : "lobbies/jar", levelData.Name, new Vector2(trigger.Position.X + trigger.Width / 2f, trigger.Position.Y + trigger.Height / 2f)));
                }
                if (trigger.Name == "CollabUtils2/JournalTrigger")
                {
                    LobbyMapIconsData.Add(new LobbyMapIconsData("lobbies/journal", levelData.Name, new Vector2(trigger.Position.X + trigger.Width / 2f, trigger.Position.Y + trigger.Height / 2f)));
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

            if (CustomImage != null && CustomImagesTilesSizeX != 0 && CustomImagesTilesSizeY != 0)
            {
                // Camera move to selected warp                 
                Vector2 target = warpScreen.SelectedWarp.Position;
                if (Scale > 0.5f)
                {
                    basePos = new Vector2(Calc.Approach(basePos.X, -(target.X / 2) * Scale + Engine.Width / 2, ZoomRoutine.Active ? 500f : 10f * (0.75f + Scale)), Calc.Approach(basePos.Y, -(target.Y / 2) * Scale + 600, ZoomRoutine.Active ? 500f : 10f * (0.75f + Scale)));
                    if (basePos != new Vector2(-(target.X / 2) * Scale + Engine.Width / 2, -(target.Y / 2) * Scale + 600))
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

            // Draw map base
            Draw.Rect(new Vector2(100, 180), 1720, 840, Color.Black * 0.9f);

            if (CustomImage != null && CustomImagesTilesSizeX != 0 && CustomImagesTilesSizeY != 0)
            {
                foreach (Vector2 tile in CustomImageTilesCoordinates)
                {
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
                ActiveFont.DrawOutline(Dialog.Clean(warpScreen.SelectedWarp.DialogKey), new Vector2(Celeste.TargetCenter.X, Celeste.TargetHeight - 110f), new Vector2(0.5f, 0.5f), Vector2.One, Color.White, 2f, Color.Black);

                Vector2 target = warpScreen.SelectedWarp.Position;
                if (playerIcon != null && playerIconHair != null && currentPositionIndicator)
                {
                    playerIcon.CenterOrigin();
                    playerIconHair.CenterOrigin();
                    playerIcon.Position = playerIconHair.Position = new Vector2(basePos.X + target.X / 8f * CustomImagesTilesSizeX * Scale, basePos.Y + (target.Y - 16f) / 8f * CustomImagesTilesSizeY * Scale);
                    playerIconHair.Color = Scene.Tracker.GetEntity<Player>()?.Hair.Color ?? Color.White;
                    playerIcon.Render();
                    playerIconHair.Render();
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
