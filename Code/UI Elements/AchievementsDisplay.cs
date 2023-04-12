using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;
using static Celeste.Mod.XaphanHelper.UI_Elements.StatusDisplay;

namespace Celeste.Mod.XaphanHelper.UI_Elements
{
    class AchievementsDisplay : Entity
    {
        [Tracked(true)]
        public class CategoryDisplay : Entity
        {
            public int ID;

            public float width = 550f;

            public float height = 85f;

            private string Name;

            private AchievementsScreen AchievementsScreen;

            public bool Selected;

            private float selectedAlpha = 0;

            private int alphaStatus = 0;

            public CategoryDisplay(Level level, Vector2 position, int id, string name, bool noDialog = false) : base(position)
            {
                Tag = Tags.HUD;
                ID = id;
                Name = noDialog ? name : Dialog.Clean(name);
                AchievementsScreen = level.Tracker.GetEntity<AchievementsScreen>();
                Depth = -10001;
            }

            public override void Update()
            {
                base.Update();
                if (AchievementsScreen.categorySelection == ID)
                {
                    Selected = true;
                    if (AchievementsScreen.prompt == null)
                    {
                        if (Input.MenuConfirm.Pressed)
                        {
                            Audio.Play("event:/ui/main/message_confirm");
                        }
                    }
                    if (alphaStatus == 0 || (alphaStatus == 1 && selectedAlpha != 0.9f))
                    {
                        alphaStatus = 1;
                        selectedAlpha = Calc.Approach(selectedAlpha, 0.9f, Engine.DeltaTime);
                        if (selectedAlpha == 0.9f)
                        {
                            alphaStatus = 2;
                        }
                    }
                    if (alphaStatus == 2 && selectedAlpha != 0.1f)
                    {
                        selectedAlpha = Calc.Approach(selectedAlpha, 0.1f, Engine.DeltaTime);
                        if (selectedAlpha == 0.1f)
                        {
                            alphaStatus = 1;
                        }
                    }
                }
                else
                {
                    Selected = false;
                }
            }

            public override void Render()
            {
                base.Render();
                float lenght = ActiveFont.Measure(Name).X * 0.8f;
                if (Selected)
                {
                    Draw.Rect(Position, width, height, Color.Yellow * selectedAlpha);
                }
                ActiveFont.DrawOutline(Name, Position + new Vector2(60 + lenght / 2 - 10, height / 2), new Vector2(0.5f, 0.5f), Vector2.One * 0.8f, Name != "???" ? Color.White : Color.Gray, 2f, Color.Black);
            }
        }

        private Level level;

        public AchievementsDisplay(Level level)
        {
            this.level = level;
            Tag = Tags.HUD;
            Depth = -10001;
        }

        public IEnumerator GennerateAchievementsDisplay()
        {
            Scene.Add(new CategoryDisplay(level, new Vector2(155f, 470f), 0, "XaphanHelper_UI_General"));
            bool visitedChapter = XaphanModule.ModSaveData.VisitedChapters.Contains("Xaphan/0_Ch1_0");
            Scene.Add(new CategoryDisplay(level, new Vector2(155f, 555f), 1, visitedChapter ? "Xaphan_0_1_AncientRuins" : "???", visitedChapter ? false : true));
            visitedChapter = XaphanModule.ModSaveData.VisitedChapters.Contains("Xaphan/0_Ch2_0");
            Scene.Add(new CategoryDisplay(level, new Vector2(155f, 640f), 2, visitedChapter ? "Xaphan_0_2_ForgottenAbyss" : "???", visitedChapter ? false : true));
            visitedChapter = XaphanModule.ModSaveData.VisitedChapters.Contains("Xaphan/0_Ch3_0");
            Scene.Add(new CategoryDisplay(level, new Vector2(155f, 725f), 3, visitedChapter ? "Xaphan_0_3_ExoticUndergrowdth" : "???", visitedChapter ? false : true));
            visitedChapter = XaphanModule.ModSaveData.VisitedChapters.Contains("Xaphan/0_Ch4_0");
            Scene.Add(new CategoryDisplay(level, new Vector2(155f, 810f), 4, visitedChapter ? "Xaphan_0_4_DevilBasin": "???", visitedChapter ? false : true));
            visitedChapter = XaphanModule.ModSaveData.VisitedChapters.Contains("Xaphan/0_Ch5_0");
            Scene.Add(new CategoryDisplay(level, new Vector2(155f, 895f), 5, visitedChapter ? "Xaphan_0_5_SubterraneanTerminal" : "???", visitedChapter ? false : true));
            yield return null;
        }

        public override void Removed(Scene scene)
        {
            base.Removed(scene);
            foreach (CategoryDisplay display in level.Tracker.GetEntities<CategoryDisplay>())
            {
                display.RemoveSelf();
            }
        }

        public override void Render()
        {
            base.Render();
            Level level = Scene as Level;
            if (level != null && (level.FrozenOrPaused || level.RetryPlayerCorpse != null || level.SkippingCutscene))
            {
                return;
            }
            Draw.Rect(new Vector2(100, 180), 1720, 840, Color.Black * 0.85f);
            float SectionTitleLenght;
            float SectionTitleHeight;
            string SectionName;
            Vector2 SectionPosition;
            int SectionMaxItems;

            SectionName = Dialog.Clean("XaphanHelper_UI_Medals");
            SectionPosition = new Vector2(430f, 225f);
            SectionMaxItems = 1;
            ActiveFont.DrawOutline(SectionName, Position + SectionPosition, new Vector2(0.5f, 0.5f), Vector2.One * 1f, Color.Yellow, 2f, Color.Black);
            SectionTitleLenght = ActiveFont.Measure(SectionName).X;
            SectionTitleHeight = ActiveFont.Measure(SectionName).Y;
            Draw.Rect(Position + SectionPosition + new Vector2(SectionTitleLenght / 2 + 10, -4), 275f - (SectionTitleLenght / 2 + 10) + 15, 8f, Color.White);
            Draw.Rect(Position + SectionPosition + new Vector2(-275f - 15, -4), 275f - (SectionTitleLenght / 2 + 10) + 15, 8f, Color.White);
            Draw.Rect(Position + SectionPosition + new Vector2(SectionTitleLenght / 2 + 10 + 275f - (SectionTitleLenght / 2 + 10) + 5, -4), 10f, SectionMaxItems * 150f + SectionTitleHeight / 2 + 4, Color.White);
            Draw.Rect(Position + SectionPosition + new Vector2(-275f - 15, -4), 10f, SectionMaxItems * 150f + SectionTitleHeight / 2 + 4, Color.White);
            Draw.Rect(Position + SectionPosition + new Vector2(-275f - 15, SectionMaxItems * 150f + SectionTitleHeight / 2 - 8), 580f, 8f, Color.White);

            SectionName = Dialog.Clean("XaphanHelper_UI_Categories");
            SectionPosition = new Vector2(430f, 450f);
            SectionMaxItems = 6;
            ActiveFont.DrawOutline(SectionName, Position + SectionPosition, new Vector2(0.5f, 0.5f), Vector2.One * 1f, Color.Yellow, 2f, Color.Black);
            SectionTitleLenght = ActiveFont.Measure(SectionName).X;
            SectionTitleHeight = ActiveFont.Measure(SectionName).Y;
            Draw.Rect(Position + SectionPosition + new Vector2(SectionTitleLenght / 2 + 10, -4), 275f - (SectionTitleLenght / 2 + 10) + 15, 8f, Color.White);
            Draw.Rect(Position + SectionPosition + new Vector2(-275f - 15, -4), 275f - (SectionTitleLenght / 2 + 10) + 15, 8f, Color.White);
            Draw.Rect(Position + SectionPosition + new Vector2(SectionTitleLenght / 2 + 10 + 275f - (SectionTitleLenght / 2 + 10) + 5, -4), 10f, SectionMaxItems * 85f + SectionTitleHeight / 2 + 4, Color.White);
            Draw.Rect(Position + SectionPosition + new Vector2(-275f - 15, -4), 10f, SectionMaxItems * 85f + SectionTitleHeight / 2 + 4, Color.White);
            Draw.Rect(Position + SectionPosition + new Vector2(-275f - 15, SectionMaxItems * 85f + SectionTitleHeight / 2 - 8), 580f, 8f, Color.White);

            SectionName = Dialog.Clean("XaphanHelper_UI_Achievements").ToUpper();
            SectionPosition = new Vector2(1030f, 225f);
            SectionMaxItems = 7;
            ActiveFont.DrawOutline(SectionName, Position + SectionPosition + new Vector2(230f, 0f), new Vector2(0.5f, 0.5f), Vector2.One * 1f, Color.Yellow, 2f, Color.Black);
            SectionTitleLenght = ActiveFont.Measure(SectionName).X;
            SectionTitleHeight = ActiveFont.Measure(SectionName).Y;
            Draw.Rect(Position + SectionPosition + new Vector2(SectionTitleLenght / 2 + 10, -4) + new Vector2(230f, 0f), 505f - (SectionTitleLenght / 2 + 10) + 15, 8f, Color.White);
            Draw.Rect(Position + SectionPosition + new Vector2(-505f - 15, -4) + new Vector2(230f, 0f), 505f - (SectionTitleLenght / 2 + 10) + 15, 8f, Color.White);
            Draw.Rect(Position + SectionPosition + new Vector2(SectionTitleLenght / 2 + 10 + 505f - (SectionTitleLenght / 2 + 10) + 5, -4) + new Vector2(230f, 0f), 10f, SectionMaxItems * 105f + SectionTitleHeight / 2 + 4, Color.White);
            Draw.Rect(Position + SectionPosition + new Vector2(-505f - 15, -4) + new Vector2(230f, 0f), 10f, SectionMaxItems * 105f + SectionTitleHeight / 2 + 4, Color.White);
            Draw.Rect(Position + SectionPosition + new Vector2(-505f - 15, SectionMaxItems * 105f + SectionTitleHeight / 2 - 8) + new Vector2(230f, 0f), 1040f, 8f, Color.White);

        }
    }
}
