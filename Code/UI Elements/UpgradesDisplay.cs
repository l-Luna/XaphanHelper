using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Reflection;
using System.Xml.Linq;
using Celeste.Mod.XaphanHelper.Entities;
using Celeste.Mod.XaphanHelper.Managers;
using Celeste.Mod.XaphanHelper.Upgrades;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;

namespace Celeste.Mod.XaphanHelper.UI_Elements
{
    [Tracked(true)]
    class UpgradesDisplay : Entity
    {
        private static ILHook playerUpdateHook;

        private static ILHook summitGemSmashRoutineHook;

        private MTexture bg = GFX.Gui["upgrades/displayBG"];

        private MTexture staminaIcon = GFX.Gui["upgrades/stamina/icon"];

        private MTexture missileIcon = GFX.Gui["upgrades/ammo/missile"];

        private MTexture superMissileIcon = GFX.Gui["upgrades/ammo/superMissile"];

        private Image section = new(GFX.Gui["upgrades/stamina/section"]);

        public HashSet<Image> Sections = new();

        public static Player player;

        public int TotalSections;

        public static float BaseStamina;

        public static bool ShowStaminaBar;

        public static string Prefix;

        public static void getStaminaData(Level level)
        {
            AreaKey area = level.Session.Area;
            MapData MapData = AreaData.Areas[area.ID].Mode[(int)area.Mode].MapData;
            string entity = "XaphanHelper/UpgradeController";
            if (MapData.HasEntity(entity))
            {
                BaseStamina = MapData.GetEntityData(entity).Float("baseStamina");
                ShowStaminaBar = MapData.GetEntityData(entity).Bool("showStaminaBar", false);
                Prefix = area.LevelSet;
            }
        }

        public UpgradesDisplay()
        {
            Tag = (Tags.HUD | Tags.Persistent | Tags.PauseUpdate | Tags.TransitionUpdate);
            Position.Y = -50f;
            Depth = -99;
        }

        public static void Load()
        {
            Everest.Events.Level.OnLoadLevel += onLevelLoad;
            IL.Celeste.Player.ClimbUpdate += patchOutStamina;
            IL.Celeste.Player.SwimBegin += patchOutStamina;
            IL.Celeste.Player.DreamDashBegin += patchOutStamina;
            IL.Celeste.Player.ctor += patchOutStamina;
            On.Celeste.Player.RefillStamina += modRefillStamina;
            playerUpdateHook = new ILHook(typeof(Player).GetMethod("orig_Update"), patchOutStamina);
            summitGemSmashRoutineHook = new ILHook(typeof(SummitGem).GetMethod("SmashRoutine", BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(), patchOutStamina);
            On.Celeste.Level.Update += onLevelUpdate;
            On.Celeste.TotalStrawberriesDisplay.Update += onTotalStrawberriesDisplayUpdate;
        }

        public static void Unload()
        {
            Everest.Events.Level.OnLoadLevel -= onLevelLoad;
            IL.Celeste.Player.ClimbUpdate -= patchOutStamina;
            IL.Celeste.Player.SwimBegin -= patchOutStamina;
            IL.Celeste.Player.DreamDashBegin -= patchOutStamina;
            IL.Celeste.Player.ctor -= patchOutStamina;
            On.Celeste.Player.RefillStamina -= modRefillStamina;
            if (playerUpdateHook != null) playerUpdateHook.Dispose();
            if (summitGemSmashRoutineHook != null) summitGemSmashRoutineHook.Dispose();
            On.Celeste.Level.Update -= onLevelUpdate;
            On.Celeste.TotalStrawberriesDisplay.Update -= onTotalStrawberriesDisplayUpdate;
        }

        private static void onLevelLoad(Level level, Player.IntroTypes playerIntro, bool isFromLoader)
        {
            if (XaphanModule.useUpgrades)
            {
                getStaminaData(level);
            }
        }

        private static void patchOutStamina(ILContext il)
        {
            ILCursor cursor = new(il);
            while (cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(110f)))
            {
                cursor.EmitDelegate<Func<float, float>>(orig =>
                {
                    if (XaphanModule.useUpgrades)
                    {
                        return determineBaseStamina();
                    }
                    return orig;
                });
            }
        }

        private static void modRefillStamina(On.Celeste.Player.orig_RefillStamina orig, Player self)
        {
            orig.Invoke(self);
            if (XaphanModule.useUpgrades)
            {
                self.Stamina = determineBaseStamina();
            }
        }

        private static float determineBaseStamina()
        {
            int ExtraStamina = 0;
            if (Engine.Scene is Level)
            {
                Level level = (Level)Engine.Scene;
                if (XaphanModule.PlayerHasGolden)
                {
                    foreach (string upgrade in XaphanModule.ModSaveData.SpeedrunModeStaminaUpgrades)
                    {
                        if (upgrade.Contains(Prefix))
                        {
                            ExtraStamina += 5;
                        }
                    }
                }
                else
                {
                    foreach (string upgrade in XaphanModule.ModSaveData.StaminaUpgrades)
                    {
                        if (upgrade.Contains(Prefix))
                        {
                            ExtraStamina += 5;
                        }
                    }
                }
            }
            return BaseStamina + ExtraStamina;
        }

        private static void onLevelUpdate(On.Celeste.Level.orig_Update orig, Level self)
        {
            orig(self);
            if (XaphanModule.useUpgrades)
            {
                bool sliding = false;
                player = self.Tracker.GetEntity<Player>();
                foreach (PlayerPlatform slope in self.Tracker.GetEntities<PlayerPlatform>())
                {
                    if (slope.Sliding)
                    {
                        sliding = true;
                        break;
                    }
                }
                if ((self.Frozen || self.RetryPlayerCorpse != null || self.SkippingCutscene || self.InCutscene) || (player != null && !player.Sprite.Visible && !self.Session.GetFlag("Xaphan_Helper_Ceiling") && !sliding && (self.Tracker.GetEntity<ScrewAttackManager>() != null ? !self.Tracker.GetEntity<ScrewAttackManager>().StartedScrewAttack : true)) || (self.Tracker.GetEntity<MapScreen>() != null && self.Tracker.GetEntity<MapScreen>().ShowUI)
                    || (self.Tracker.GetEntity<StatusScreen>() != null && self.Tracker.GetEntity<StatusScreen>().ShowUI) || (self.Tracker.GetEntity<WarpScreen>() != null && self.Tracker.GetEntity<WarpScreen>().ShowUI))
                {
                    if (self.Tracker.GetEntity<UpgradesDisplay>() != null)
                    {
                        self.Tracker.GetEntity<UpgradesDisplay>().RemoveSelf();
                    }
                }
                else
                {
                    if (self.Tracker.GetEntity<UpgradesDisplay>() == null)
                    {
                        self.Add(new UpgradesDisplay());
                    }
                }
            }
        }

        private static void onTotalStrawberriesDisplayUpdate(On.Celeste.TotalStrawberriesDisplay.orig_Update orig, TotalStrawberriesDisplay self)
        {
            orig(self);
            if (Engine.Scene is Level)
            {
                Level level = (Level)Engine.Scene;
                if (XaphanModule.useUpgrades && ShowStaminaBar)
                {
                    float num = self.Y = 154f;
                    if (!level.TimerHidden)
                    {
                        if (Settings.Instance.SpeedrunClock == SpeedrunType.File)
                        {
                            num += 20f;
                        }
                    }
                    self.Y = num;
                }
            }
        }

        public override void Update()
        {
            base.Update();
            if (Settings.Instance.SpeedrunClock == SpeedrunType.Off)
            {
                Position.X = 0f;
            }
            else if (!SceneAs<Level>().TimerHidden)
            {
                if (Settings.Instance.SpeedrunClock == SpeedrunType.Chapter)
                {
                    Position.X = SpeedrunTimerDisplay.GetTimeWidth(TimeSpan.FromTicks(SceneAs<Level>().Session.Time).ShortGameplayFormat()) + 30f;
                }
                else if (Settings.Instance.SpeedrunClock == SpeedrunType.File)
                {
                    Position.X = SpeedrunTimerDisplay.GetTimeWidth(TimeSpan.FromTicks(SaveData.Instance.Time).ShortGameplayFormat()) + 30f;
                }
            }
            Position.Y = 60f;
            if (player != null)
            {
                TotalSections = (int)determineBaseStamina() / 5;
                Sections = GetSections();
            }
        }

        private HashSet<Image> GetSections()
        {
            Sections = new HashSet<Image>();
            if (player != null)
            {
                for (int i = 1; i <= TotalSections; i++)
                {
                    if ((int)Math.Ceiling(player.Stamina / 5) >= i)
                    {
                        Sections.Add(new Image(GFX.Gui["upgrades/stamina/section" + (i <= 4 ? "Low" : "")]));
                        continue;
                    }
                    Sections.Add(new Image(GFX.Gui["upgrades/stamina/sectionEmpty"]));
                }
            }
            return Sections;
        }

        public override void Render()
        {
            base.Render();
            if (ShowStaminaBar && PowerGrip.isActive && !XaphanModule.PlayerIsControllingRemoteDrone())
            {
                bg.Draw(Position + new Vector2(-bg.Width + 32f + staminaIcon.Width + 15f + TotalSections * 9 + section.Width, 0f));
                staminaIcon.Draw(Position + new Vector2(32f, -1));
                int Col = 0;
                foreach (Image section in Sections)
                {
                    section.Position = Position + (new Vector2(32f + staminaIcon.Width + 15f + Col, 3f));
                    section.Color = Color.White;
                    section.Render();
                    Col += 9;
                }
            }
            else if (XaphanModule.PlayerIsControllingRemoteDrone())
            {
                int missileIconPos = 32;
                int superMissileIconPos = 145;
                bg.Draw(Position + new Vector2(-bg.Width + 100f + missileIcon.Width + superMissileIconPos, 0f));
                missileIcon.Draw(Position + new Vector2(missileIconPos, -11));
                superMissileIcon.Draw(Position + new Vector2(superMissileIconPos, -11));
                ActiveFont.DrawOutline("5", Position + new Vector2(missileIconPos + missileIcon.Width + 15f, 17f), new Vector2(0f, 0.5f), Vector2.One * 0.7f, Color.White, 2f, Color.Black);
                ActiveFont.DrawOutline("2", Position + new Vector2(superMissileIconPos + superMissileIcon.Width + 15f, 17f), new Vector2(0f, 0.5f), Vector2.One * 0.7f, Color.White, 2f, Color.Black);
            }
        }
    }
}
