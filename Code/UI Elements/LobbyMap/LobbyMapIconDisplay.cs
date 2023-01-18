using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Celeste.Mod.XaphanHelper.Data;
using Celeste.Mod.XaphanHelper.Managers;
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

        public LobbyMapIconDisplay(LevelData levelData, AreaStats stats)
            : base(true, true)
        {
            foreach (EntityData entity in levelData.Entities)
            {
                if (entity.Name == "XaphanHelper/WarpStation")
                {
                    iconData.Add(new LobbyMapIconsData("lobbies/bench", levelData.Name, new Vector2(entity.Position.X + entity.Width / 2f, entity.Position.Y + entity.Height / 2f - 16f)));
                }

                if (entity.Name == "CollabUtils2/MiniHeartDoor")
                {
                    iconData.Add(new LobbyMapIconsData("lobbies/heartgate", levelData.Name, new Vector2(entity.Position.X + entity.Width / 2f, entity.Position.Y)));
                }

                if (entity.Name == "CollabUtils2/RainbowBerry")
                {
                    iconData.Add(new LobbyMapIconsData("lobbies/berry", levelData.Name, new Vector2(entity.Position.X + entity.Width / 2f, entity.Position.Y + entity.Height / 2f)));
                }

                if (entity.Name == "SJ2021/StrawberryJamJar")
                {
                    bool mapCompleted = GetMapCompleted(entity);
                    string mapIcon = GetMapIcon(entity);
                    iconData.Add(new LobbyMapIconsData(mapCompleted ? "lobbies/jarfull" : "lobbies/jar", levelData.Name, new Vector2(entity.Position.X + entity.Width / 2f, entity.Position.Y + entity.Height / 2f - 16f)));
                }

                if (entity.Name == "MaxHelpingHand/MoreCustomNPC" || entity.Name == "MaxHelpingHand/CustomNPCSprite")
                {
                    if (entity.Attr("dialogId").Contains("Credits"))
                    {
                        iconData.Add(new LobbyMapIconsData("lobbies/berry", levelData.Name, new Vector2(entity.Position.X + entity.Width / 2f, entity.Position.Y + entity.Height / 2f)));
                    }
                }
            }

            foreach (EntityData trigger in levelData.Triggers)
            {
                if (trigger.Name == "CollabUtils2/ChapterPanelTrigger")
                {
                    bool mapCompleted = GetMapCompleted(trigger);
                    string mapIcon = GetMapIcon(trigger);
                    iconData.Add(new LobbyMapIconsData(mapCompleted ? "lobbies/jarfull" : "lobbies/jar", levelData.Name, new Vector2(trigger.Position.X + trigger.Width / 2f, trigger.Position.Y + trigger.Height / 2f)));
                }

                if (trigger.Name == "CollabUtils2/JournalTrigger")
                {
                    iconData.Add(new LobbyMapIconsData("lobbies/journal", levelData.Name, new Vector2(trigger.Position.X + trigger.Width / 2f, trigger.Position.Y + trigger.Height / 2f)));
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

            if (Engine.Scene.OnRawInterval(0.3f))
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
                playerIcon.Position = playerIconHair.Position = offset + Entity.SelectedWarp.Position * scale;
                playerIcon.Render();
                playerIconHair.Render();
            }
        }
    }
}
