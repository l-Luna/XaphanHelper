using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.XaphanHelper.Triggers;
using Celeste.Mod.XaphanHelper.Managers;
using Celeste.Mod.XaphanHelper.Entities;

namespace Celeste.Mod.XaphanHelper.UI_Elements
{
    [Tracked(true)]
    public class MiniMap : Entity
    {
        protected XaphanModuleSettings Settings => XaphanModule.Settings;

        private Level level;

        private Player player;

        public MapDisplay mapDisplay;

        private float Opacity;

        private string lastGeneratedRoom;

        private string currentRoom;

        public int chapterIndex;

        public int alphaStatus;

        public float positionIndicatorAlpha;

        public MiniMap(Level level)
        {
            this.level = level;
            Tag = (Tags.HUD | Tags.Persistent | Tags.PauseUpdate);
            Depth = -10002;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            chapterIndex = level.Session.Area.ChapterIndex == -1 ? 0 : level.Session.Area.ChapterIndex;
            currentRoom = level.Session.Level;
            positionIndicatorAlpha = 1f;
        }

        private IEnumerator MapRoutine(Level level, string originRoom)
        {
            Scene.Add(mapDisplay = new MapDisplay(SceneAs<Level>(), "minimap")
            {
                currentRoom = originRoom
            });
            yield return mapDisplay.GenerateMap();
        }

        private float GetMapOpacity()
        {
            return Settings.MiniMapOpacity / 10f;
        }

        public override void Update()
        {
            base.Update();
            player = level.Tracker.GetEntity<Player>();
            if (player != null && player.Center.X > level.Camera.Right - 64f && player.Center.Y < level.Camera.Top + 52)
            {
                if (GetMapOpacity() > 0.3f)
                {
                    Opacity = Calc.Approach(Opacity, 0.3f, Engine.RawDeltaTime * 3f);
                }
            }
            else
            {
                if (Opacity != GetMapOpacity())
                {
                    Opacity = Calc.Approach(Opacity, GetMapOpacity(), Engine.RawDeltaTime * 3f);
                }
            }
            bool sliding = false;
            foreach (PlayerPlatform slope in SceneAs<Level>().Tracker.GetEntities<PlayerPlatform>())
            {
                if (slope.Sliding)
                {
                    sliding = true;
                    break;
                }
            }
            if (level != null && (level.FrozenOrPaused || level.RetryPlayerCorpse != null || level.SkippingCutscene || level.InCutscene) || (player != null && !player.Sprite.Visible && !SceneAs<Level>().Session.GetFlag("Xaphan_Helper_Ceiling") && !sliding && (level.Tracker.GetEntity<ScrewAttackManager>() != null ? !level.Tracker.GetEntity<ScrewAttackManager>().StartedScrewAttack : true)) || (level.Tracker.GetEntity<MapScreen>() != null && level.Tracker.GetEntity<MapScreen>().ShowUI)
                || (level.Tracker.GetEntity<StatusScreen>() != null && level.Tracker.GetEntity<StatusScreen>().ShowUI) || (level.Tracker.GetEntity<WarpScreen>() != null && level.Tracker.GetEntity<WarpScreen>().ShowUI) || playerIsInHideTrigger() || !XaphanModule.CanOpenMap(level))
            {
                RemoveSelf();
            }
            else
            {
                Visible = true;
            }
            if (mapDisplay != null)
            {
                mapDisplay.Visible = Visible;
                mapDisplay.Opacity = Opacity;
            }
            currentRoom = level.Session.Level;
            if (lastGeneratedRoom == null)
            {
                Add(new Coroutine(MapRoutine(level, currentRoom)));
                lastGeneratedRoom = "Ch" + chapterIndex + "_" + currentRoom;
            }
            else if ("Ch" + chapterIndex + "_" + currentRoom != lastGeneratedRoom)
            {
                if (mapDisplay != null)
                {
                    mapDisplay.currentRoom = currentRoom;
                    mapDisplay.SetCurrentRoomCoordinates(Vector2.Zero);
                    Vector2 playerPosition = new Vector2(Math.Min((float)Math.Floor((player.Center.X - level.Bounds.X) / mapDisplay.ScreenTilesX), (float)Math.Round(level.Bounds.Width / (float)mapDisplay.ScreenTilesX, MidpointRounding.AwayFromZero) - 1), Math.Min((float)Math.Floor((player.Center.Y - level.Bounds.Y) / mapDisplay.ScreenTilesY), (float)Math.Round(level.Bounds.Height / (float)mapDisplay.ScreenTilesY, MidpointRounding.AwayFromZero) + 1));
                    if (playerPosition.Y == -1)
                    {
                        playerPosition.Y = 0;
                    }
                    mapDisplay.setMapPosition(playerPosition * -40);
                }
                lastGeneratedRoom = "Ch" + chapterIndex + "_" + currentRoom;
            }
            if (alphaStatus == 0 || (alphaStatus == 1 && positionIndicatorAlpha != 1f))
            {
                alphaStatus = 1;
                positionIndicatorAlpha = Calc.Approach(positionIndicatorAlpha, 1f, Engine.DeltaTime / 2);
                if (positionIndicatorAlpha == 1f)
                {
                    alphaStatus = 2;
                }
            }
            if (alphaStatus == 2 && positionIndicatorAlpha != 0.7f)
            {
                positionIndicatorAlpha = Calc.Approach(positionIndicatorAlpha, 0.7f, Engine.DeltaTime / 2);
                if (positionIndicatorAlpha == 0.7f)
                {
                    alphaStatus = 1;
                }
            }
        }

        public bool playerIsInHideTrigger()
        {
            foreach (HideMiniMapTrigger trigger in level.Tracker.GetEntities<HideMiniMapTrigger>())
            {
                if (trigger.playerInside)
                {
                    return true;
                }
            }
            return false;
        }

        public override void Render()
        {
            base.Render();
            if (mapDisplay != null)
            {
                Draw.Rect(new Vector2(mapDisplay.Grid.Left - 2, mapDisplay.Grid.Top - 2), 204, 2, mapDisplay.GridColor * Opacity);
                Draw.Rect(new Vector2(mapDisplay.Grid.Left - 2, mapDisplay.Grid.Top), 2, 120, mapDisplay.GridColor * Opacity);
                Draw.Rect(new Vector2(mapDisplay.Grid.Right, mapDisplay.Grid.Top), 2, 120, mapDisplay.GridColor * Opacity);
                Draw.Rect(new Vector2(mapDisplay.Grid.Left - 2, mapDisplay.Grid.Bottom), 204, 2, mapDisplay.GridColor * Opacity);
                Draw.Rect(new Vector2(mapDisplay.Grid.Left + 81, mapDisplay.Grid.Top + 41), 40, 40, mapDisplay.RoomBorderColor * 0.5f * positionIndicatorAlpha * Opacity);
            }
        }

        public override void Removed(Scene scene)
        {
            base.Removed(scene);
            if (mapDisplay != null)
            {
                mapDisplay.RemoveSelf();
            }
        }
    }
}
