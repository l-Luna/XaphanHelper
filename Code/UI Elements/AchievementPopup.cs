using System.Collections;
using System.Collections.Generic;
using Celeste.Mod.XaphanHelper.Data;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.UI_Elements
{
    [Tracked(true)]
    public class AchievementPopup : Entity
    {
        private List<AchievementData> achievements;

        private Coroutine PopupRoutine = new();

        private MTexture Icon;

        private MTexture MedalIcon;

        private string Name;

        private string Description;

        private string MedalsValue;

        private float alpha;

        public AchievementPopup()
        {
            Tag = (Tags.HUD | Tags.Persistent | Tags.PauseUpdate | Tags.TransitionUpdate);
            Position = new Vector2(0f, Engine.Height - 149f);
            MedalIcon = GFX.Gui["achievements/medal"];
            Visible = false;
            Depth = -1000000;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            achievements = Achievements.GenerateAchievementsList(SceneAs<Level>().Session);
        }

        public override void Update()
        {
            base.Update();
            if (!PopupRoutine.Active && XaphanModule.ModSaveData.CanDisplayAchievementsPopups)
            {
                foreach (AchievementData achievement in achievements)
                {
                    if (SceneAs<Level>().Session.GetFlag(achievement.Flag) && !XaphanModule.ModSaveData.Achievements.Contains(achievement.AchievementID))
                    {
                        XaphanModule.ModSaveData.Achievements.Add(achievement.AchievementID);
                        Add(PopupRoutine = new Coroutine(DisplayPopup(achievement)));
                        break;
                    }
                }
            }
        }

        private IEnumerator DisplayPopup(AchievementData data)
        {
            Icon = GFX.Gui[data.Icon];
            Name = Dialog.Clean(data.Name);
            Description = Dialog.Clean(data.Description);
            MedalsValue = "+ " + data.Medals.ToString();
            Audio.Play("event:/game/02_old_site/theoselfie_photo_filter");
            float popupTime = 5f;
            while (popupTime > 0)
            {
                if (popupTime <= 1f)
                {
                    alpha = popupTime;
                }
                else if (popupTime >= 4f)
                {
                    alpha = 5f - popupTime;
                }
                Visible = true;
                popupTime -= Engine.DeltaTime;
                yield return null;
            }
            Visible = false;
        }

        public override void Render()
        {
            base.Render();
            Draw.Rect(Position, 750f, 149f, Color.Black * alpha);
            Draw.Rect(Position, 750f, 5f, Color.Gold * alpha);
            Draw.Rect(Position + Vector2.UnitY * 5f, 5f, 139f, Color.Gold * alpha);
            Draw.Rect(Position + new Vector2(745f, 5f), 5f, 139f, Color.Gold * alpha);
            Draw.Rect(Position + Vector2.UnitY * 144f, 750f, 5f, Color.Gold * alpha);
            Icon.Draw(Position + Vector2.One * 7f, Vector2.Zero, Color.White * alpha, 0.9f);
            float lenght = ActiveFont.Measure(Name).X * 0.6f;
            float descHeight = ActiveFont.Measure(Description).Y * 0.4f;
            ActiveFont.DrawOutline(Name, Position + new Vector2(167f + lenght / 2 - 10, 50f - descHeight / 2), new Vector2(0.5f, 0.5f), Vector2.One * 0.6f, Color.White * alpha, 2f, Color.Black * alpha);
            ActiveFont.DrawOutline(Description, Position + new Vector2(158f, 70f - descHeight / 2), Vector2.Zero, Vector2.One * 0.4f, Color.Gray * alpha, 2f, Color.Black * alpha);
            ActiveFont.DrawOutline(MedalsValue, Position + new Vector2(158f, 70f + descHeight / 2 + 5f), Vector2.Zero, Vector2.One * 0.5f, Color.Gold * alpha, 2f, Color.Black * alpha);
            lenght = ActiveFont.Measure(MedalsValue).X * 0.5f;
            MedalIcon.Draw(Position + new Vector2(159f + lenght + 10f, 70f + descHeight / 2), Vector2.Zero, Color.White * alpha, 0.35f);
        }
    }
}
