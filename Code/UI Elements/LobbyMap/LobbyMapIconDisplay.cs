using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.XaphanHelper.Data;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.UI_Elements.LobbyMap
{
    public class LobbyMapIconDisplay : Component
    {
        public new LobbyMapDisplay Entity => base.Entity as LobbyMapDisplay;

        private readonly List<LobbyMapIconsData> iconData = new();
        private readonly Dictionary<LobbyMapIconsData, Image> iconImages = new();

        public Image playerIcon;
        public Image playerIconHair;

        private float playerVisibleIntervalOffset;
        private float playerVisibleForceTime;

        public void ResetPlayerVisible()
        {
            playerIcon.Visible = playerIconHair.Visible = true;
            playerVisibleForceTime = 0.8f;
        }

        public LobbyMapIconDisplay(LevelData levelData, AreaStats stats, string miniHeartDoorIcon, string journalIcon, string mapIcon, string rainbowsBerryIcon, string warpIcon, string extraEntitiesNames, string extraEntitiesIcons)
            : base(true, true)
        {
            foreach (EntityData entity in levelData.Entities)
            {
                if (entity.Name == "XaphanHelper/WarpStation")
                {
                    iconData.Add(new LobbyMapIconsData((string.IsNullOrEmpty(warpIcon) ? "lobbies/warp" : warpIcon), levelData.Name, new Vector2(entity.Position.X + entity.Width / 2f, entity.Position.Y + entity.Height / 2f - 16f)));
                }

                if (entity.Name == "CollabUtils2/MiniHeartDoor")
                {
                    iconData.Add(new LobbyMapIconsData((string.IsNullOrEmpty(miniHeartDoorIcon) ? "lobbies/heartgate" : miniHeartDoorIcon), levelData.Name, new Vector2(entity.Position.X + entity.Width / 2f, entity.Position.Y)));
                }

                if (entity.Name == "CollabUtils2/RainbowBerry")
                {
                    iconData.Add(new LobbyMapIconsData((string.IsNullOrEmpty(rainbowsBerryIcon) ? "lobbies/rainbowBerry" : rainbowsBerryIcon), levelData.Name, new Vector2(entity.Position.X + entity.Width / 2f, entity.Position.Y + entity.Height / 2f)));
                }

                if (entity.Name == "SJ2021/StrawberryJamJar")
                {
                    bool mapCompleted = GetMapCompleted(entity);
                    string mapDifficulty = GetMapIcon(entity);
                    iconData.Add(new LobbyMapIconsData((string.IsNullOrEmpty(mapIcon) ? "lobbies/map" : mapIcon) + (mapCompleted ? "Completed" : ""), levelData.Name, new Vector2(entity.Position.X + entity.Width / 2f, entity.Position.Y + entity.Height / 2f - 16f)));
                }

                if (!string.IsNullOrEmpty(extraEntitiesNames) && !string.IsNullOrEmpty(extraEntitiesIcons))
                {
                    string[] EntitiesNames = extraEntitiesNames.Split(',');
                    string[] EntitiesIcons = extraEntitiesIcons.Split(',');
                    foreach (string EntityName in EntitiesNames)
                    {
                        if (entity.Name == EntityName)
                        {
                            if (EntitiesIcons.Count() >= Array.IndexOf(EntitiesNames, EntityName) + 1)
                            {
                                iconData.Add(new LobbyMapIconsData(EntitiesIcons[Array.IndexOf(EntitiesNames, EntityName)], levelData.Name, new Vector2(entity.Position.X + entity.Width / 2f, entity.Position.Y + entity.Height / 2f)));
                            }
                            else
                            {
                                Logger.Log(LogLevel.Warn, "XaphanHelper/LobbyMapIconDisplay", $"No icon found for {EntityName}! {EntityName} will not be displayed...");
                            }
                        }
                    }
                }
            }

            foreach (EntityData trigger in levelData.Triggers)
            {
                if (trigger.Name == "CollabUtils2/ChapterPanelTrigger")
                {
                    bool mapCompleted = GetMapCompleted(trigger);
                    string mapDifficulty = GetMapIcon(trigger);
                    iconData.Add(new LobbyMapIconsData((string.IsNullOrEmpty(mapIcon) ? "lobbies/map" : mapIcon) + (mapCompleted ? "Completed" : ""), levelData.Name, new Vector2(trigger.Position.X + trigger.Width / 2f, trigger.Position.Y + trigger.Height / 2f)));
                }

                if (trigger.Name == "CollabUtils2/JournalTrigger")
                {
                    iconData.Add(new LobbyMapIconsData((string.IsNullOrEmpty(journalIcon) ? "lobbies/journal" : journalIcon), levelData.Name, new Vector2(trigger.Position.X + trigger.Width / 2f, trigger.Position.Y + trigger.Height / 2f)));
                }
            }
        }

        public bool GetMapCompleted(EntityData entity)
        {
            AreaData areaData = AreaData.Get(entity.Attr("map"));
            if (areaData != null)
            {
                AreaStats areaStatsFor = SaveData.Instance.GetAreaStatsFor(areaData.ToKey());
                return areaStatsFor != null && areaStatsFor.Modes[0].Completed;
            }
            return false;
        }

        public string GetMapIcon(EntityData entity)
        {
            return AreaData.Get(entity.Attr("map"))?.Icon;
        }

        public override void Added(Entity entity)
        {
            base.Added(entity);

            foreach (LobbyMapIconsData icon in iconData)
            {
                var tileX = (int)Math.Floor(icon.Position.X / 8);
                var tileY = (int)Math.Floor(icon.Position.Y / 8);
                if (Entity.Overlay.IsVisited(tileX, tileY))
                {
                    var image = new Image(GFX.Gui[$"maps/{icon.Type}"]);
                    image.CenterOrigin();
                    iconImages[icon] = image;
                }
            }

            playerIcon = new Image(GFX.Gui["maps/player"]);
            playerIcon.CenterOrigin();

            playerIconHair = new Image(GFX.Gui["maps/player_hair"]);
            playerIconHair.CenterOrigin();
        }

        public override void Update()
        {
            base.Update();

            playerIconHair.Color = Scene?.Tracker.GetEntity<Player>()?.Hair.Color ?? Color.White;

            if (playerVisibleForceTime > 0)
            {
                playerVisibleForceTime -= Engine.RawDeltaTime;
                if (playerVisibleForceTime <= 0)
                {
                    playerVisibleIntervalOffset = Scene?.RawTimeActive ?? 0f;
                }
            }
            else if (Engine.Scene.OnRawInterval(0.3f, playerVisibleIntervalOffset))
            {
                playerIcon.Visible = !playerIcon.Visible;
            }
        }

        public override void Render()
        {
            var offset = new Vector2(Engine.Width / 2f, Engine.Height / 2f) - Entity.Origin * Entity.Sprite.Size * Entity.Scale;
            var scale = new Vector2(Entity.Scale / 8f * Entity.Sprite.ImageScaleX, Entity.Scale / 8f * Entity.Sprite.ImageScaleY);
            var iconScale = Calc.LerpClamp(0.4f, 0.6f, Entity.ScaleRatio);

            foreach (var pair in iconImages)
            {
                pair.Value.Position = offset + pair.Key.Position * scale;
                pair.Value.Scale = new Vector2(iconScale);
                pair.Value.Render();
            }

            if (playerIcon.Visible)
            {
                playerIcon.Position = playerIconHair.Position = offset + (Entity.SelectedWarp.Position - Vector2.UnitY * 16f) * scale;
                playerIcon.Render();
                playerIconHair.Render();
            }
        }
    }
}
