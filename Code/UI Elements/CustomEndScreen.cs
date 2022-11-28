using System;
using Celeste.Mod.Meta;
using Celeste.Mod.XaphanHelper.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;

namespace Celeste.Mod.XaphanHelper.UI_Elements
{
    public class CustomEndScreen : Scene
    {
        public Session Session;

        private bool finishedSlide;

        private bool canConfirm = true;

        private HiresSnow snow;

        private float speedrunTimerDelay = 1.1f;

        private float speedrunTimerEase;

        private string speedrunTimerChapterString;

        private string speedrunTimerFileString;

        private string chapterSpeedrunText = Dialog.Get("OPTIONS_SPEEDRUN_CHAPTER") + ":";

        private AreaCompleteTitle title;

        private CompleteRenderer complete;

        private string version;

        private static float everestTime;

        private float buttonTimerDelay;

        private float buttonTimerEase;

        private static bool ShowTitle;

        private static bool HideVanillaTimer;

        private static bool ShowTime;

        private static string ClearTime;

        private static int Strawberries;

        private static int MaxStrawberries;

        private static string StrawberriesColor;

        private static string StrawberriesMaxColor;

        private static float ItemPercent;

        private static string ItemPercentColor;

        private static string ItemPercentMaxColor;

        private static float MapPercent;

        private static string MapPercentColor;

        private static string MapPercentMaxColor;

        private static string SubText1;

        private static string SubText1Color;

        private static string SubText2;

        private static string SubText2Color;

        public CustomEndScreen(Session session, Atlas atlas, HiresSnow snow, MapMetaCompleteScreen meta, string clearTime, int strawberries, int maxStrawberries, float itemPercent, float mapPercent, CustomEndScreenControllerData screenData)
        {
            Session = session;
            version = Celeste.Instance.Version.ToString();
            if (session.Area.ID != 7)
            {
                string text = Dialog.Clean(screenData.Title);
                if (string.IsNullOrEmpty(text))
                {
                    text = Dialog.Clean(string.Concat("areacomplete_", session.Area.Mode, session.FullClear ? "_fullclear" : ""));
                }
                Vector2 origin = new(960f, 200f);
                float scale = Math.Min(1600f / ActiveFont.Measure(text).X, 3f);
                title = new AreaCompleteTitle(origin, text, scale);
            }
            Add(complete = new CompleteRenderer(null, atlas, 1f, delegate
            {
                finishedSlide = true;
            }, meta));
            if (title != null)
            {
                complete.RenderUI = delegate
                {
                    title.DrawLineUI();
                };
            }
            ShowTitle = screenData.ShowTitle;
            HideVanillaTimer = screenData.HideVanillaTimer;
            ShowTime = screenData.ShowTime;
            ClearTime = clearTime;
            Strawberries = strawberries;
            MaxStrawberries = maxStrawberries;
            StrawberriesColor = screenData.StrawberriesColor;
            StrawberriesMaxColor = screenData.StrawberriesMaxColor;
            ItemPercent = itemPercent;
            MapPercent = mapPercent;
            SubText1 = screenData.SubText1;
            SubText1Color = screenData.SubText1Color;
            SubText2 = screenData.SubText2;
            SubText2Color = screenData.SubText2Color;
            ItemPercentColor = screenData.ItemPercentColor;
            ItemPercentMaxColor = screenData.ItemPercentMaxColor;
            MapPercentColor = screenData.MapPercentColor;
            MapPercentMaxColor = screenData.MapPercentMaxColor;
            complete.RenderPostUI = RenderUI;
            speedrunTimerChapterString = TimeSpan.FromTicks(Session.Time).ShortGameplayFormat();
            speedrunTimerFileString = Dialog.FileTime(SaveData.Instance.Time);
            SpeedrunTimerDisplay.CalculateBaseSizes();
            Add(this.snow = snow);
            RendererList.UpdateLists();
        }

        public override void End()
        {
            base.End();
            complete.Dispose();
        }

        public override void Update()
        {
            base.Update();
            if (Input.MenuConfirm.Pressed && finishedSlide && canConfirm)
            {
                canConfirm = false;
                HiresSnow outSnow = new();
                outSnow.Alpha = 0f;
                outSnow.AttachAlphaTo = new FadeWipe(this, false, delegate
                {
                    Engine.Scene = new OverworldLoader(Overworld.StartMode.AreaComplete, outSnow);
                });
                Add(outSnow);
            }
            snow.Alpha = Calc.Approach(snow.Alpha, 0f, Engine.DeltaTime * 0.5f);
            snow.Direction.Y = Calc.Approach(snow.Direction.Y, 1f, Engine.DeltaTime * 24f);
            speedrunTimerDelay -= Engine.DeltaTime;
            if (speedrunTimerDelay <= 0f)
            {
                speedrunTimerEase = Calc.Approach(speedrunTimerEase, 1f, Engine.DeltaTime * 2f);
            }
            if (title != null)
            {
                title.Update();
            }
            if (Celeste.PlayMode == Celeste.PlayModes.Debug)
            {
                if (MInput.Keyboard.Pressed(Keys.F2))
                {
                    Celeste.ReloadAssets(levels: false, graphics: true, hires: false);
                    Engine.Scene = new LevelExit(LevelExit.Mode.Completed, Session);
                }
                else if (MInput.Keyboard.Pressed(Keys.F3))
                {
                    Celeste.ReloadAssets(levels: false, graphics: true, hires: true);
                    Engine.Scene = new LevelExit(LevelExit.Mode.Completed, Session);
                }
            }
            buttonTimerDelay -= Engine.DeltaTime;
            if (buttonTimerDelay <= 0f)
            {
                buttonTimerEase = Calc.Approach(buttonTimerEase, 1f, Engine.DeltaTime * 4f);
            }
        }

        private void RenderUI()
        {
            Entities.Render();
            Info(speedrunTimerEase, speedrunTimerChapterString, speedrunTimerFileString, chapterSpeedrunText, version);
            if (complete.HasUI && title != null)
            {
                title.Render();
            }
            if (!(buttonTimerEase > 0f) || Settings.Instance.SpeedrunClock != 0)
            {
                return;
            }

            MTexture mTexture = Input.GuiButton(Input.MenuConfirm, "controls/keyboard/oemquestion");
            Vector2 vector = new(1860f - mTexture.Width, 1020f - mTexture.Height);
            float num = buttonTimerEase * buttonTimerEase;
            float num2 = 0.9f + buttonTimerEase * 0.1f;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i != 0 && j != 0)
                    {
                        mTexture.DrawCentered(vector + new Vector2(i, j), Color.Black * num * num * num * num, Vector2.One * num2);
                    }
                }
            }
            mTexture.DrawCentered(vector, Color.White * num, Vector2.One * num2);
        }

        public static void Info(float ease, string speedrunTimerChapterString, string speedrunTimerFileString, string chapterSpeedrunText, string versionText)
        {
            if (ease > 0f && Settings.Instance.SpeedrunClock != 0)
            {
                if (!HideVanillaTimer)
                {
                    Vector2 vector = new(80f - 300f * (1f - Ease.CubeOut(ease)), 1000f);
                    if (Settings.Instance.SpeedrunClock == SpeedrunType.Chapter)
                    {
                        SpeedrunTimerDisplay.DrawTime(vector, speedrunTimerChapterString);
                    }
                    else
                    {
                        vector.Y -= 16f;
                        SpeedrunTimerDisplay.DrawTime(vector, speedrunTimerFileString);
                        ActiveFont.DrawOutline(chapterSpeedrunText, vector + new Vector2(0f, 40f), new Vector2(0f, 1f), Vector2.One * 0.6f, Color.White, 2f, Color.Black);
                        SpeedrunTimerDisplay.DrawTime(vector + new Vector2(ActiveFont.Measure(chapterSpeedrunText).X * 0.6f + 8f, 40f), speedrunTimerChapterString, 0.6f);
                    }
                }
                VersionNumberAndVariants(versionText, ease, 1f);
            }
            Vector2 vectorV = new(Engine.Width / 2, Engine.Height - 30f + 300f * (1f - Ease.CubeOut(ease)));
            float paddingText = ((!ShowTime && Strawberries == -1 && MaxStrawberries == -1) ? 50 : 0) + ((ItemPercent == -1 && MapPercent == -1) ? 50 : 0);
            if (!string.IsNullOrEmpty(SubText1))
            {
                ActiveFont.DrawOutline(Dialog.Clean(SubText1), vectorV - new Vector2(0f, 175f - paddingText), new Vector2(0.5f, 1f), Vector2.One, Calc.HexToColor(SubText1Color), 2f, Color.Black);
            }
            if (!string.IsNullOrEmpty(SubText2))
            {
                ActiveFont.DrawOutline(Dialog.Clean(SubText2), vectorV - new Vector2(0f, 125f - paddingText), new Vector2(0.5f, 1f), Vector2.One * 0.6f, Calc.HexToColor(SubText2Color), 2f, Color.Black);
            }
            if (ShowTime || (Strawberries != -1 && MaxStrawberries != -1))
            {
                float padding = (ItemPercent == -1 && MapPercent == -1) ? 50 : 0;
                MTexture iconTime = MTN.Journal["time"];
                float TimeWidth = SpeedrunTimerDisplay.GetTimeWidth(ClearTime) * 0.8f;
                float iconTimeWidth = iconTime.Width;
                float totalTimeWidth = iconTimeWidth + TimeWidth;
                MTexture iconStrawberry = MTN.Journal["strawberry"];
                string strawberryCounter = Strawberries.ToString() + "/" + MaxStrawberries.ToString();
                float iconStrawberryWidth = iconStrawberry.Width;
                float counterWidth = ActiveFont.Measure(strawberryCounter).X * 0.8f;
                float totalStrawberriesWidth = iconStrawberryWidth + counterWidth;
                Vector2 adjust = new(15f, 0f);
                if (ShowTime && Strawberries == -1 && MaxStrawberries == -1)
                {
                    adjust = new Vector2(2f, 0f);
                    iconTime.DrawCentered(vectorV - new Vector2(0f, 75f - padding) - new Vector2(totalTimeWidth / 2, 0f) + new Vector2(iconTimeWidth / 2, 0f) - adjust, Color.White, 1f);
                    SpeedrunTimerDisplay.DrawTime(vectorV - new Vector2(0f, 50f - padding) - new Vector2(totalTimeWidth / 2, 0f) + new Vector2(iconTimeWidth, 0f) - adjust, ClearTime, 0.8f);
                }
                else if (!ShowTime && Strawberries != -1 && MaxStrawberries != -1)
                {
                    adjust = new Vector2(4f, 0f);
                    iconStrawberry.DrawCentered(vectorV - new Vector2(0f, 75f - padding) - new Vector2(totalStrawberriesWidth / 2, 0f) + new Vector2(iconStrawberryWidth / 2, 0f) - adjust, Color.White, 1f);
                    ActiveFont.DrawOutline(strawberryCounter, vectorV - new Vector2(0f, 75f - padding) - new Vector2(totalStrawberriesWidth / 2, 0f) + new Vector2(iconStrawberryWidth, 0f) - adjust, new Vector2(0f, 0.5f), Vector2.One * 0.8f, Strawberries < MaxStrawberries ? Calc.HexToColor(StrawberriesColor) : Calc.HexToColor(StrawberriesMaxColor), 2f, Color.Black);
                }
                else if (ShowTime && Strawberries != -1 && MaxStrawberries != -1)
                {
                    adjust = new Vector2(2f, 0f);
                    iconTime.DrawCentered(vectorV - new Vector2(250f, 0f) - new Vector2(0f, 75f - padding) - new Vector2(totalTimeWidth / 2, 0f) + new Vector2(iconTimeWidth / 2, 0f) - adjust, Color.White, 1f);
                    SpeedrunTimerDisplay.DrawTime(vectorV - new Vector2(250f, 0f) - new Vector2(0f, 50f - padding) - new Vector2(totalTimeWidth / 2, 0f) + new Vector2(iconTimeWidth, 0f) - adjust, ClearTime, 0.8f);
                    adjust = new Vector2(4f, 0f);
                    iconStrawberry.DrawCentered(vectorV + new Vector2(250f, 0f) - new Vector2(0f, 75f - padding) - new Vector2(totalStrawberriesWidth / 2, 0f) + new Vector2(iconStrawberryWidth / 2, 0f) - adjust, Color.White, 1f);
                    ActiveFont.DrawOutline(strawberryCounter, vectorV + new Vector2(250f, 0f) - new Vector2(0f, 75f - padding) - new Vector2(totalStrawberriesWidth / 2, 0f) + new Vector2(iconStrawberryWidth, 0f) - adjust, new Vector2(0f, 0.5f), Vector2.One * 0.8f, Strawberries < MaxStrawberries ? Calc.HexToColor(StrawberriesColor) : Calc.HexToColor(StrawberriesMaxColor), 2f, Color.Black);
                }
            }
            if (ItemPercent != -1 && MapPercent == -1)
            {
                ActiveFont.DrawOutline(Dialog.Clean("XaphanHelper_UI_itemsCollected") + " " + ItemPercent + " %", vectorV, new Vector2(0.5f, 1f), Vector2.One * 0.8f, ItemPercent < 100 ? Calc.HexToColor(ItemPercentColor) : Calc.HexToColor(ItemPercentMaxColor), 2f, Color.Black);
            }
            else if (ItemPercent == -1 && MapPercent != -1)
            {
                ActiveFont.DrawOutline(Dialog.Clean("XaphanHelper_UI_mapExplored") + " " + MapPercent + " %", vectorV, new Vector2(0.5f, 1f), Vector2.One * 0.8f, MapPercent < 100 ? Calc.HexToColor(MapPercentColor) : Calc.HexToColor(MapPercentMaxColor), 2f, Color.Black);
            }
            else if (ItemPercent != -1 && MapPercent != -1)
            {
                ActiveFont.DrawOutline(Dialog.Clean("XaphanHelper_UI_itemsCollected") + " " + ItemPercent + " %", vectorV - new Vector2(250f, 0f), new Vector2(0.5f, 1f), Vector2.One * 0.8f, ItemPercent < 100 ? Calc.HexToColor(ItemPercentColor) : Calc.HexToColor(ItemPercentMaxColor), 2f, Color.Black);
                ActiveFont.DrawOutline(Dialog.Clean("XaphanHelper_UI_mapExplored") + " " + MapPercent + " %", vectorV + new Vector2(250f, 0f), new Vector2(0.5f, 1f), Vector2.One * 0.8f, MapPercent < 100 ? Calc.HexToColor(MapPercentColor) : Calc.HexToColor(MapPercentMaxColor), 2f, Color.Black);
            }
        }

        public static void VersionNumberAndVariants(string version, float ease, float alpha)
        {
            everestTime += Engine.RawDeltaTime;
            Vector2 vector = new(1820f + 300f * (1f - Ease.CubeOut(ease)), 1020f);
            if (SaveData.Instance.AssistMode || SaveData.Instance.VariantMode)
            {
                MTexture mTexture = GFX.Gui[SaveData.Instance.AssistMode ? "cs_assistmode" : "cs_variantmode"];
                vector.Y -= 32f;
                mTexture.DrawJustified(vector + new Vector2(0f, -8f), new Vector2(0.5f, 1f), Color.White, 0.6f);
                ActiveFont.DrawOutline(version, vector, new Vector2(0.5f, 0f), Vector2.One * 0.5f, Color.White, 2f, Color.Black);
            }
            else
            {
                ActiveFont.DrawOutline(version, vector, new Vector2(0.5f, 0.5f), Vector2.One * 0.5f, Color.White, 2f, Color.Black);
            }
        }

        public override void Begin()
        {
            base.Begin();
            buttonTimerDelay = 2.2f;
            buttonTimerEase = 0f;
        }
    }
}
