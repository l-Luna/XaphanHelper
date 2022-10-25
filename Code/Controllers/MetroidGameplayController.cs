using Celeste.Mod.Entities;
using Celeste.Mod.XaphanHelper.Cutscenes;
using Celeste.Mod.XaphanHelper.Entities;
using Celeste.Mod.XaphanHelper.UI_Elements;
using Celeste.Mod.XaphanHelper.Upgrades;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using Monocle;
using MonoMod.Cil;
using MonoMod.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Celeste.Mod.XaphanHelper.Controllers
{
    [CustomEntity("XaphanHelper/MetroidGameplayController")]
    class MetroidGameplayController : Entity
    {
        private static FieldInfo PlayerSpriteSpriteName = typeof(PlayerSprite).GetField("spriteName", BindingFlags.Instance | BindingFlags.NonPublic);

        private static FieldInfo PlayerOnGround = typeof(Player).GetField("onGround", BindingFlags.Instance | BindingFlags.NonPublic);

        private static FieldInfo PlayerMoveX = typeof(Player).GetField("moveX", BindingFlags.Instance | BindingFlags.NonPublic);

        private static FieldInfo PlayerFastJump = typeof(Player).GetField("fastJump", BindingFlags.Instance | BindingFlags.NonPublic);

        private static FieldInfo playerFlash = typeof(Player).GetField("flash", BindingFlags.NonPublic | BindingFlags.Instance);

        private static Coroutine ChargedShinesparkTimerRoutine;

        public static int morphCurrentFrame = -1;

        public static float dashTrailTimer = 0f;

        public static float MaxRunSpeed = 1f;

        public static float BeamDelay;

        public static float BombDelay;

        public static bool ShinesparkCharged;

        public static bool JustChargedShinespark;

        public static bool ShinesparkCooldown;

        public static bool ShinesparkInitCooldown;

        public static bool StartedShinesparking;

        public static bool Shinesparking;

        public static string ShinesparkDirection;

        public static bool MorphMode;

        public static bool BeamAnimation;

        public static bool jumpSfxPlay;

        public static bool shinesparkSfxPlay;

        public static bool isSpaceJumping;

        public static string jumpSfxCurrentEventName;

        public static EventInstance jumpSfx;

        public static EventInstance shinesparkSfx;

        public static bool isLoadingFromSave;

        public MetroidGameplayController(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            
        }

        public static void Load()
        {
            Everest.Events.Level.OnLoadLevel += onLevelLoad;
            Everest.Events.Player.OnSpawn += onPlayerSpawn;
            On.Celeste.LevelEnter.Go += onLevelEnterGo;
            On.Celeste.Level.Update += onLevelUpdate;
            On.Celeste.PlayerSprite.ctor += onPlayerSpriteCtor;
            On.Celeste.Player.Jump += onPlayerJump;
            On.Celeste.Player.UpdateSprite += onPlayerUpdateSprite;
            On.Celeste.Player.Update += onPlayerUpdate;
            On.Celeste.Player.NormalUpdate += onPlayerNormalUpdate;
            On.Celeste.Player.DummyUpdate += onPlayerDummyUpdate;
            On.Celeste.Player.Render += onPlayerRender;
            On.Celeste.SpeedrunTimerDisplay.Render += onSpeedRunTimerDisplayRender;
            On.Celeste.TotalStrawberriesDisplay.Render += onTotalStrawberriesDisplayRender;
            IL.Celeste.Player.Jump += ilPlayerJump;
            IL.Celeste.Player.NormalUpdate += ilPlayerNormalUpdate;
            IL.Celeste.Player.Render += ilPlayerRender;
        }

        public static void Unload()
        {
            Everest.Events.Level.OnLoadLevel -= onLevelLoad;
            Everest.Events.Player.OnSpawn -= onPlayerSpawn;
            On.Celeste.LevelEnter.Go -= onLevelEnterGo;
            On.Celeste.Level.Update -= onLevelUpdate;
            On.Celeste.PlayerSprite.ctor -= onPlayerSpriteCtor;
            On.Celeste.Player.Jump -= onPlayerJump;
            On.Celeste.Player.UpdateSprite -= onPlayerUpdateSprite;
            On.Celeste.Player.Update -= onPlayerUpdate;
            On.Celeste.Player.NormalUpdate -= onPlayerNormalUpdate;
            On.Celeste.Player.DummyUpdate -= onPlayerDummyUpdate;
            On.Celeste.Player.Render -= onPlayerRender;
            On.Celeste.SpeedrunTimerDisplay.Render -= onSpeedRunTimerDisplayRender;
            On.Celeste.TotalStrawberriesDisplay.Render -= onTotalStrawberriesDisplayRender;
            IL.Celeste.Player.Jump -= ilPlayerJump;
            IL.Celeste.Player.NormalUpdate -= ilPlayerNormalUpdate;
            IL.Celeste.Player.Render -= ilPlayerRender;
        }

        private static void onLevelLoad(Level level, Player.IntroTypes playerIntro, bool isFromLoader)
        {
            BeamDelay = 0f;
        }

        private static void onPlayerSpawn(Player player)
        {
            if (player.SceneAs<Level>().Session.GetFlag("Xaphan_Helper_Shinesparking"))
            {
                player.SceneAs<Level>().Session.SetFlag("Xaphan_Helper_Shinesparking", false);
            }
            MorphMode = ShinesparkCharged = JustChargedShinespark = ShinesparkCooldown = ShinesparkInitCooldown = StartedShinesparking = Shinesparking = false;
        }

        private static void onLevelEnterGo(On.Celeste.LevelEnter.orig_Go orig, Session session, bool fromSaveData)
        {
            if (!fromSaveData && XaphanModule.useMetroidGameplaySessionCheck(session))
            {
                if (!XaphanModule.ModSaveData.IgnoreSavedChapter)
                {
                    AreaKey area = session.Area;
                    int currentChapter = area.ChapterIndex == -1 ? 0 : area.ChapterIndex;
                    int toChapter = 1;
                    foreach (KeyValuePair<string, int> savedChapter in XaphanModule.ModSaveData.SavedChapter)
                    {
                        if (savedChapter.Key == area.LevelSet)
                        {
                            toChapter = savedChapter.Value;
                            break;
                        }
                    }
                    int chapterOffset = toChapter - currentChapter;
                    int currentChapterID = session.Area.ID;
                    session = new Session(new AreaKey(currentChapterID + chapterOffset));
                }
            }
            orig.Invoke(session, fromSaveData);
        }

        private static void onLevelUpdate(On.Celeste.Level.orig_Update orig, Level self)
        {
            if (XaphanModule.useMetroidGameplay)
            {
                Player player = self.Tracker.GetEntity<Player>();
                if (!XaphanModule.ModSaveData.SavedRoom.ContainsKey(self.Session.Area.LevelSet) && !XaphanModule.ModSaveData.SavedChapter.ContainsKey(self.Session.Area.LevelSet))
                {
                    XaphanModule.ModSaveData.SavedRoom.Add(self.Session.Area.LevelSet, self.Session.MapData.StartLevel().Name);
                    XaphanModule.ModSaveData.SavedChapter.Add(self.Session.Area.LevelSet, self.Session.Area.ChapterIndex);
                }
                if (XaphanModule.ModSaveData.SavedRoom.ContainsKey(self.Session.Area.LevelSet) && XaphanModule.ModSaveData.SavedChapter.ContainsKey(self.Session.Area.LevelSet) && !self.Session.GetFlag("XaphanHelper_Changed_Start_Room"))
                {
                    IsLoadingFromSave(true);
                    self.Session.SetFlag("XaphanHelper_Changed_Start_Room", true);
                    string DestinationRoom = XaphanModule.ModSaveData.SavedRoom[self.Session.Area.LevelSet];
                    int chapterIndex = XaphanModule.ModSaveData.SavedChapter[self.Session.Area.LevelSet];
                    if (chapterIndex == (self.Session.Area.ChapterIndex == -1 ? 0 : self.Session.Area.ChapterIndex))
                    {
                        if (DestinationRoom != self.Session.Level)
                        {
                            Vector2 spawnPoint = Vector2.Zero;
                            AreaKey area = self.Session.Area;
                            MapData MapData = AreaData.Areas[area.ID].Mode[(int)area.Mode].MapData;
                            foreach (LevelData levelData in MapData.Levels)
                            {
                                if (levelData.Name == DestinationRoom)
                                {
                                    foreach (EntityData entity in levelData.Entities)
                                    {
                                        if (entity.Name == "XaphanHelper/SaveStation")
                                        {
                                            spawnPoint = new Vector2(entity.Position.X, entity.Position.Y);
                                        }
                                    }
                                    break;
                                }
                            }
                            self.Add(new TeleportCutscene(player, DestinationRoom, spawnPoint, 0, 0, true, 0f, "Fade", 1.35f, false, true));
                        }
                        else
                        {
                            if (self.Wipe != null)
                            {
                                self.Wipe.Cancel();
                                self.Add(new FadeWipe(self, true)
                                {
                                    Duration = 1.35f
                                });
                            }
                        }
                    }
                    else
                    {
                        int chapterOffset = chapterIndex - (self.Session.Area.ChapterIndex == -1 ? 0 : self.Session.Area.ChapterIndex);
                        int currentChapterID = self.Session.Area.ID;
                        XaphanModule.ModSaveData.DestinationRoom = DestinationRoom;
                        AreaKey area = self.Session.Area;
                        MapData MapData = AreaData.Areas[currentChapterID + chapterOffset].Mode[(int)area.Mode].MapData;
                        foreach (LevelData levelData in MapData.Levels)
                        {
                            if (levelData.Name == DestinationRoom)
                            {
                                foreach (EntityData entity in levelData.Entities)
                                {
                                    if (entity.Name == "XaphanHelper/WarpStation")
                                    {
                                        XaphanModule.ModSaveData.Spawn = new Vector2(entity.Position.X, entity.Position.Y);
                                    }
                                }
                                break;
                            }
                        }
                        XaphanModule.ModSaveData.Wipe = "Fade";
                        XaphanModule.ModSaveData.WipeDuration = 1.35f;
                        self.Add(new FadeWipe(self, false, () => LevelEnter.Go(new Session(new AreaKey(currentChapterID + chapterOffset)), fromSaveData: false))
                        {
                            Duration = 1.35f
                        });
                    }
                }
            }
            orig(self);
        }

        public static void IsLoadingFromSave(bool value)
        {
            isLoadingFromSave = value;
        }

        private static void onPlayerSpriteCtor(On.Celeste.PlayerSprite.orig_ctor orig, PlayerSprite self, PlayerSpriteMode mode)
        {
            if (XaphanModule.useMetroidGameplay)
            {
                orig(self, mode);
                PlayerSpriteSpriteName.SetValue(self, "XaphanHelper_samus");
                GFX.SpriteBank.CreateOn(self, "XaphanHelper_samus");
            }
            else
            {
                orig(self, mode);
            }
        }

        private static void onPlayerJump(On.Celeste.Player.orig_Jump orig, Player self, bool particles, bool playSfx)
        {
            if (XaphanModule.useMetroidGameplay)
            {
                if (!StartedShinesparking && !Shinesparking)
                {
                    playSfx = false;
                    if (self.Sprite.CurrentAnimationID.Contains("jumpFastSpace"))
                    {
                        particles = false;
                        isSpaceJumping = true;
                    }
                    else
                    {
                        isSpaceJumping = false;
                    }
                }
            }
            if (!StartedShinesparking && !Shinesparking)
            {
                orig(self, particles, playSfx);
            }
        }

        private static void onPlayerUpdateSprite(On.Celeste.Player.orig_UpdateSprite orig, Player self)
        {
            if (XaphanModule.useMetroidGameplay)
            {
                self.Sprite.Scale = Vector2.One;
                int CurrentAnimationFrame = self.Sprite.CurrentAnimationFrame;
                if (self.Sprite.CurrentAnimationID.Contains("run") || self.Sprite.CurrentAnimationID.Contains("jumpFast"))
                {
                    self.Sprite.Rate = MaxRunSpeed;
                }
                else
                {
                    self.Sprite.Rate = 1f;
                }
                if (self.InControl && self.StateMachine.State != 20 && self.StateMachine.State != 18 && self.StateMachine.State != 19 && self.StateMachine.State != 21)
                {
                    if (Shinesparking && self.Speed.Y <= 0)
                    {
                        if (ShinesparkDirection == "Left" || ShinesparkDirection == "Right")
                        {
                            if (self.Speed.X != 0)
                            {
                                self.Sprite.Play("dash");
                            }
                            else if (self.Sprite.CurrentAnimationID != "dashImpact")
                            {
                                self.Sprite.Play("dashImpact");
                            }
                        }
                        else if (ShinesparkDirection == "Up")
                        {
                            if (self.Speed.Y < 0)
                            {
                                self.Sprite.Play("dashUp");
                            }
                            else if (self.Sprite.CurrentAnimationID != "dashImpact")
                            {
                                self.Sprite.Play("dashImpact");
                            }
                        }
                    }
                    else
                    {
                        if (MorphMode)
                        {
                            if (self.Sprite.LastAnimationID != "duck" && self.Sprite.LastAnimationID != "morphLoop")
                            {
                                self.Sprite.Play("duck");
                            }
                            else if (self.Sprite.LastAnimationID.Contains("morphLoop") && self.Speed.X == 0)
                            {
                                morphCurrentFrame = self.Sprite.CurrentAnimationFrame;
                                self.Sprite.Stop();
                            }
                            else if (self.Sprite.LastAnimationID.Contains("morphLoop") && (int)PlayerMoveX.GetValue(self) != 0)
                            {
                                self.Sprite.Play("morphLoop");
                                if (morphCurrentFrame != -1)
                                {
                                    self.Sprite.SetAnimationFrame(morphCurrentFrame);
                                    morphCurrentFrame = -1;
                                }
                            }
                            else if (self.Sprite.CurrentAnimationID == "runStumble")
                            {
                                self.Sprite.Play("morphLoop");
                            }
                        }
                        else if ((bool)PlayerOnGround.GetValue(self))
                        {
                            PlayerFastJump.SetValue(self, false);
                            if ((int)PlayerMoveX.GetValue(self) != 0 && self.CollideCheck<Solid>(self.Position + Vector2.UnitX * (int)PlayerMoveX.GetValue(self)) && !XaphanModule.onSlope)
                            {
                                CurrentAnimationFrame = -1;
                                if (BeamAnimation)
                                {
                                    if (!self.Sprite.CurrentAnimationID.Contains("Aim"))
                                    {
                                        CurrentAnimationFrame = self.Sprite.CurrentAnimationFrame;
                                    }
                                    self.Sprite.Play("idleAim");
                                    if (CurrentAnimationFrame != -1)
                                    {
                                        self.Sprite.SetAnimationFrame(CurrentAnimationFrame);
                                    }
                                }
                                else
                                {
                                    if (self.Sprite.CurrentAnimationID.Contains("Aim"))
                                    {
                                        CurrentAnimationFrame = self.Sprite.CurrentAnimationFrame;
                                    }
                                    self.Sprite.Play("idle");
                                    if (CurrentAnimationFrame != -1)
                                    {
                                        self.Sprite.SetAnimationFrame(CurrentAnimationFrame);
                                    }
                                }
                            }
                            else if (Math.Abs(self.Speed.X) <= 25f && (int)PlayerMoveX.GetValue(self) == 0)
                            {
                                self.Sprite.Rate = 1f;
                                if (Input.MoveY.Value == -1 && !MorphMode)
                                {
                                    if (self.Speed.X == 0 && self.Sprite.LastAnimationID != "lookUp" && !MorphMode)
                                    {
                                        self.Sprite.Play("lookUp");
                                    }
                                }
                                else if (self.Speed.X == 0 && !MorphMode)
                                {
                                    CurrentAnimationFrame = -1;
                                    if (BeamAnimation)
                                    {
                                        if (!self.Sprite.CurrentAnimationID.Contains("Aim"))
                                        {
                                            CurrentAnimationFrame = self.Sprite.CurrentAnimationFrame;
                                        }
                                        self.Sprite.Play("idleAim");
                                        if (CurrentAnimationFrame != -1)
                                        {
                                            self.Sprite.SetAnimationFrame(CurrentAnimationFrame);
                                        }
                                    }
                                    else
                                    {
                                        if (self.Sprite.CurrentAnimationID.Contains("Aim"))
                                        {
                                            CurrentAnimationFrame = self.Sprite.CurrentAnimationFrame;
                                        }
                                        self.Sprite.Play("idle");
                                        if (CurrentAnimationFrame != -1)
                                        {
                                            self.Sprite.SetAnimationFrame(CurrentAnimationFrame);
                                        }
                                    }
                                }
                            }
                            else if ((int)PlayerMoveX.GetValue(self) != 0 && !MorphMode)
                            {
                                CurrentAnimationFrame = -1;
                                if (Math.Abs(self.Speed.X) <= 90f)
                                {
                                    if (BeamAnimation)
                                    {
                                        if (!self.Sprite.CurrentAnimationID.Contains("runAim"))
                                        {
                                            CurrentAnimationFrame = self.Sprite.CurrentAnimationFrame;
                                        }
                                        self.Sprite.Play("runAimH");
                                        if (CurrentAnimationFrame != -1)
                                        {
                                            self.Sprite.SetAnimationFrame(CurrentAnimationFrame);
                                        }
                                    }
                                    else
                                    {
                                        if (self.Sprite.CurrentAnimationID.Contains("runAim"))
                                        {
                                            CurrentAnimationFrame = self.Sprite.CurrentAnimationFrame;
                                        }
                                        self.Sprite.Play("run");
                                        if (CurrentAnimationFrame != -1)
                                        {
                                            self.Sprite.SetAnimationFrame(CurrentAnimationFrame);
                                        }
                                    }
                                }
                                else
                                {
                                    if (BeamAnimation)
                                    {
                                        if (!self.Sprite.CurrentAnimationID.Contains("runAim"))
                                        {
                                            CurrentAnimationFrame = self.Sprite.CurrentAnimationFrame;
                                        }
                                        self.Sprite.Play("runAimH");
                                        if (CurrentAnimationFrame != -1)
                                        {
                                            self.Sprite.SetAnimationFrame(CurrentAnimationFrame);
                                        }
                                    }
                                    else
                                    {
                                        if (self.Sprite.CurrentAnimationID.Contains("runAim"))
                                        {
                                            CurrentAnimationFrame = self.Sprite.CurrentAnimationFrame;
                                        }
                                        self.Sprite.Play("run");
                                        if (CurrentAnimationFrame != -1)
                                        {
                                            self.Sprite.SetAnimationFrame(CurrentAnimationFrame);
                                        }
                                    }
                                }
                            }
                        }
                        else if (self.Speed.Y < 0f && !MorphMode)
                        {
                            if (Input.MoveY.Value == -1 &&  Input.MoveX.Value == 0)
                            {
                                if (!self.Sprite.LastAnimationID.Contains("jumpLookUp"))
                                {
                                    self.Sprite.Play("jumpLookUp");
                                }
                            }
                            else if ((Input.MoveY.Value == 0 || (Input.MoveY.Value == -1 && Input.MoveX.Value != 0)) && BeamAnimation)
                            {
                                if (!self.Sprite.LastAnimationID.Contains("jumpAim"))
                                {
                                    self.Sprite.Play("jumpAim");
                                }
                            }
                            else if ((Input.MoveY.Value == 1 || self.Sprite.LastAnimationID.Contains("jumpLookDown")) && !MorphMode)
                            {
                                if (!self.Sprite.LastAnimationID.Contains("jumpLookDown"))
                                {
                                    self.Sprite.Play("jumpLookDown");
                                }
                            }
                            else
                            {
                                if ((int)PlayerMoveX.GetValue(self) != 0 && (self.OnGround() || !self.Sprite.LastAnimationID.Contains("jumpSlow")))
                                {
                                    if (Input.Dash.Check && BeamDelay <= 0)
                                    {
                                        self.Sprite.Play("jumpAim");
                                    }
                                    else if (!self.Sprite.LastAnimationID.Contains("jumpAim") && !self.OnGround())
                                    {
                                        PlayerFastJump.SetValue(self, true);
                                        if (SpaceJump.Active(self.SceneAs<Level>()) && (GravityJacket.determineIfInLiquid() ? GravityJacket.Active(self.SceneAs<Level>()) : true))
                                        {
                                            self.Sprite.Play("jumpFastSpace");
                                        }
                                        else
                                        {
                                            self.Sprite.Play("jumpFast");
                                        }
                                    }
                                }
                                else
                                {
                                    if (self.Sprite.LastAnimationID.Contains("jumpFast"))
                                    {
                                        if (Input.Dash.Check && BeamDelay <= 0)
                                        {
                                            self.Sprite.Play("jumpAim");
                                        }
                                    }
                                    else if (!self.Sprite.LastAnimationID.Contains("jumpSlow"))
                                    {
                                        self.Sprite.Play("jumpSlow");
                                    }
                                }
                            }
                        }
                        else if ((bool)PlayerFastJump.GetValue(self))
                        {
                            PlayerFastJump.SetValue(self, true);
                            if (Input.Dash.Check && BeamDelay <= 0 && !self.Sprite.LastAnimationID.Contains("jumpLookDown") && !self.Sprite.LastAnimationID.Contains("fallLookDown"))
                            {
                                self.Sprite.Play("fallAim");
                            }
                            if (Input.MoveY.Value == -1)
                            {
                                if (!self.Sprite.LastAnimationID.Contains("jumpLookUp"))
                                {
                                    self.Sprite.Play("jumpLookUp");
                                }
                            }
                            else if ((Input.MoveY.Value == 1 || self.Sprite.LastAnimationID.Contains("jumpLookDown")) && !MorphMode)
                            {
                                if (!self.Sprite.LastAnimationID.Contains("jumpLookDown"))
                                {
                                    self.Sprite.Play("jumpLookDown");
                                }
                            }
                            else if (self.Sprite.LastAnimationID == "jumpFastSpace" && GravityJacket.determineIfInLiquid() && !GravityJacket.Active(self.SceneAs<Level>()))
                            {
                                CurrentAnimationFrame = self.Sprite.CurrentAnimationFrame;
                                self.Sprite.Play("jumpFast");
                                self.Sprite.SetAnimationFrame(CurrentAnimationFrame);
                            }
                        }
                        else
                        {
                            if (Input.MoveY.Value == -1 && Input.MoveX.Value == 0)
                            {
                                if (!self.Sprite.LastAnimationID.Contains("fallLookUp"))
                                {
                                    self.Sprite.Play("fallLookUp");
                                }
                            }
                            else if ((Input.MoveY.Value == 0 || (Input.MoveY.Value == -1 && Input.MoveX.Value != 0)) && BeamAnimation)
                            {
                                if (!self.Sprite.LastAnimationID.Contains("fallAim"))
                                {
                                    self.Sprite.Play("fallAim");
                                }
                            }
                            else if ((Input.MoveY.Value == 1 || self.Sprite.LastAnimationID.Contains("fallLookDown")) && !MorphMode)
                            {
                                if (!self.Sprite.LastAnimationID.Contains("fallLookDown"))
                                {
                                    self.Sprite.Play("fallLookDown");
                                }
                            }
                            else if (!self.Sprite.LastAnimationID.Contains("fallSlow"))
                            {
                                self.Sprite.Play("fallSlow");
                            }
                        }
                    }
                }
            }
            else
            {
                orig(self);
            }
        }

        private static void onPlayerUpdate(On.Celeste.Player.orig_Update orig, Player self)
        {
            if (XaphanModule.useMetroidGameplay)
            {
                HealthDisplay healthDisplay = self.SceneAs<Level>().Tracker.GetEntity<HealthDisplay>();
                if (healthDisplay != null && healthDisplay.PlayerWasKilled)
                {
                    self.Sprite.Color = Color.White;
                    Audio.Stop(jumpSfx, false);
                    Audio.Stop(shinesparkSfx, false);
                    return;
                }
                self.Sprite.OnFrameChange = delegate (string anim)
                {
                    if (self.SceneAs<Level>() != null && !self.Dead && self.Sprite.Visible)
                    {
                        int currentAnimationFrame = self.Sprite.CurrentAnimationFrame;
                        if (anim.Contains("run") && (currentAnimationFrame == 0 || currentAnimationFrame == 6))
                        {
                            Platform platformByPriority = SurfaceIndex.GetPlatformByPriority(self.CollideAll<Platform>(self.Position + Vector2.UnitY));
                            if (platformByPriority != null)
                            {
                                Audio.Play("event:/char/madeline/footstep", "surface_index", platformByPriority.GetStepSoundIndex(self));
                            }
                        }
                    }
                };
                DynData<Player> playerData = new DynData<Player>(self);
                Hitbox normalHitbox = playerData.Get<Hitbox>("normalHitbox");
                Hitbox normalHurtbox = playerData.Get<Hitbox>("normalHurtbox");
                if (MorphMode)
                {
                    normalHitbox.Height = 6f;
                    normalHitbox.Top = -6f;
                    normalHurtbox.Height = 4f;
                    normalHurtbox.Top = -6f;
                }
                else
                {
                    normalHitbox.Height = 11f;
                    normalHitbox.Top = -11f;
                    normalHurtbox.Height = 9f;
                    normalHurtbox.Top = -11f;
                }
                if (MaxRunSpeed >= 1.5f || Shinesparking)
                {
                    dashTrailTimer -= Engine.DeltaTime;
                    if (dashTrailTimer <= 0f)
                    {
                        createTrail(self);
                        dashTrailTimer = Shinesparking ? 0.04f : 0.05f;
                    }
                }
                if (!Input.Grab.Check || MorphMode)
                {
                    MaxRunSpeed = MaxRunSpeed > 1f ? MaxRunSpeed -= 0.05f : MaxRunSpeed = 1f;
                }
                if (StartedShinesparking)
                {
                    self.Speed.X = 0;
                    self.Speed.Y = 0;
                }
                else
                {
                    if (self.Speed.X == 0 || (self.OnGround() && self.Sprite.CurrentAnimationID.Contains("fallSlow")))
                    {
                        MaxRunSpeed = 1f;
                    }
                    else if ((Input.MoveX == 1 && self.Facing == Facings.Left) || (Input.MoveX == -1 && self.Facing == Facings.Right))
                    {
                        MaxRunSpeed = 1f;
                    }
                    else if (MaxRunSpeed >= 1.5f)
                    {
                        if (self.OnGround() && Input.MenuDown.Pressed)
                        {
                            if (ShinesparkCooldown)
                            {
                                ShinesparkCooldown = false;
                            }
                            ShinesparkCharged = true;
                            JustChargedShinespark = true;
                        }
                    }
                    else if (self.OnGround() && Input.Grab.Check && (int)PlayerMoveX.GetValue(self) != 0 && !MorphMode)
                    {
                        if (MaxRunSpeed < (SpeedBooster.Active(self.SceneAs<Level>()) && (GravityJacket.determineIfInLiquid() ? GravityJacket.Active(self.SceneAs<Level>()) : true) ? 1.5f : 1.25f))
                        {
                            MaxRunSpeed += MaxRunSpeed <= 1.25f ? 0.0125f : 0.0075f;
                        }
                    }
                    if (MaxRunSpeed >= 1.5f || Shinesparking)
                    {
                        self.SceneAs<Level>().Session.SetFlag("Xaphan_Helper_Shinesparking", true);
                    }
                    else
                    {
                        self.SceneAs<Level>().Session.SetFlag("Xaphan_Helper_Shinesparking", false);
                    }
                }
                if (Input.Dash.Check && BeamDelay <= 0 && !self.SceneAs<Level>().Transitioning && !Shinesparking && !self.Sprite.CurrentAnimationID.Contains("jumpFast") && !MorphMode && !self.Dead && self.Holding == null && self.StateMachine.State == 0)
                {
                    self.Add(new Coroutine(Shoot(self)));
                    self.Add(new Coroutine(ShootAnimationDelay()));
                }
                else if (Input.Dash.Pressed && BombDelay <= 0 && !self.SceneAs<Level>().Transitioning && MorphMode && self.SceneAs<Level>().Tracker.GetEntities<MorphBomb>().Count <= 4 && self.StateMachine.State != 11)
                {
                    self.Add(new Coroutine(UseBomb(self)));
                }
                if (JustChargedShinespark)
                {
                    self.Add(new Coroutine(ShinesparkDelay(self)));
                }
                if (ShinesparkCharged && !ShinesparkCooldown)
                {
                    ChargedShinesparkTimerRoutine = new Coroutine(ChargedShinesparkTimer(self));
                }
                if (StartedShinesparking && !ShinesparkInitCooldown)
                {
                    self.Add(new Coroutine(InitShinespark(self)));
                }
                if (ChargedShinesparkTimerRoutine != null)
                {
                    ChargedShinesparkTimerRoutine.Update();
                }
            }
            orig(self);
        }

        private static int onPlayerNormalUpdate(On.Celeste.Player.orig_NormalUpdate orig, Player self)
        {
            if (XaphanModule.useMetroidGameplay)
            {
                if (ShinesparkCharged && !MorphMode)
                {
                    if (Input.Jump.Pressed && Input.MoveX == 0 && !StartedShinesparking && !self.Sprite.CurrentAnimationID.Contains("Fast"))
                    {
                        StartedShinesparking = true;
                        self.Speed.Y = -100f;
                        Audio.Play("event:/game/xaphan/shinespark_start");
                    }
                }
                if (MorphingBall.Active(self.SceneAs<Level>()) && MaxRunSpeed < 1.5f)
                {
                    if (Input.MenuDown.Pressed && !Input.MenuDown.Repeating && ((self.OnGround() || self.Sprite.LastAnimationID.Contains("Down")) && !MorphMode))
                    {
                        MorphMode = true;
                        self.Ducking = true;
                    }
                    else if (Input.MenuUp.Pressed && MorphMode && !self.CollideCheck<Solid>(self.Position + new Vector2(0, -8)))
                    {
                        MorphMode = false;
                        self.Ducking = false;
                    }
                }
                else if (MorphMode && !self.CollideCheck<Solid>(self.Position + new Vector2(0, -8)))
                {
                    MorphMode = false;
                    self.Ducking = false;
                }
                if (MorphMode && self.Speed.Y > 0)
                {
                    self.Ducking = true;
                }
                if ((!ScrewAttack.Active(self.SceneAs<Level>()) || (ScrewAttack.Active(self.SceneAs<Level>()) && !GravityJacket.Active(self.SceneAs<Level>()) && GravityJacket.determineIfInLiquid())) && self.SceneAs<Level>().FormationBackdrop.Display == false)
                {
                    if (self.Sprite.CurrentAnimationID == "jumpFast" && !jumpSfxPlay)
                    {
                        jumpSfxPlay = true;
                        jumpSfx = Audio.Play("event:/game/xaphan/spin_jump");
                        jumpSfxCurrentEventName = "spin_jump";
                    }
                    else if (self.Sprite.CurrentAnimationID == "jumpFastSpace" && !jumpSfxPlay)
                    {
                        jumpSfxPlay = true;
                        jumpSfx = Audio.Play("event:/game/xaphan/space_jump");
                        jumpSfxCurrentEventName = "space_jump";
                    }
                    else if (self.Sprite.CurrentAnimationID == "jumpFastSpace" && jumpSfxPlay && jumpSfxCurrentEventName == "spin_jump")
                    {
                        Audio.Stop(jumpSfx, false);
                        jumpSfx = Audio.Play("event:/game/xaphan/space_jump");
                        jumpSfxCurrentEventName = "space_jump";
                    }
                }
                else
                {
                    jumpSfxPlay = false;
                    Audio.Stop(jumpSfx, false);
                }
                if (MaxRunSpeed >= 1.5f && !shinesparkSfxPlay)
                {
                    shinesparkSfxPlay = true;
                    shinesparkSfx = Audio.Play("event:/game/xaphan/start_speed_boost");
                }
                if (!self.Sprite.CurrentAnimationID.Contains("jumpFast") || self.Dead || self.SceneAs<Level>().Frozen)
                {
                    jumpSfxPlay = false;
                    Audio.Stop(jumpSfx, false);
                }
                if (MaxRunSpeed < 1.5f && shinesparkSfxPlay && !ShinesparkCharged && !Shinesparking)
                {
                    shinesparkSfxPlay = false;
                    Audio.Stop(shinesparkSfx, false);
                }
                if (self.OnGround())
                {
                    if (!MorphMode && Input.Jump.Pressed)
                    {
                        self.Jump();
                    }
                    else if (MorphMode && Input.Jump.Check && SpringBall.Active(self.SceneAs<Level>()))
                    {
                        self.Jump();
                    }
                    if (!JustChargedShinespark)
                    {
                        self.Speed.X = Calc.Approach(self.Speed.X, 90 * (int)PlayerMoveX.GetValue(self) * MaxRunSpeed * GravityJacket.determineSpeedXFactor(), 1000f * Engine.DeltaTime);
                    }
                    return 0;
                }
            }
            return orig(self);
        }

        private static int onPlayerDummyUpdate(On.Celeste.Player.orig_DummyUpdate orig, Player self)
        {
            if (XaphanModule.useMetroidGameplay)
            {
                if (MorphMode)
                {
                    int currentFrame = self.Sprite.CurrentAnimationFrame;
                    orig(self);
                    self.Sprite.Play("morphLoop");
                    self.Sprite.SetAnimationFrame(currentFrame);
                    self.Ducking = true;
                }
                return 11;
            }
            else
            {
                return orig(self);
            }
        }

        private static void onPlayerRender(On.Celeste.Player.orig_Render orig, Player self)
        {
            if (XaphanModule.useMetroidGameplay)
            {
                orig(self);
                if (MorphMode)
                {
                    Draw.Rect(self.Position + new Vector2(-2f, 0f), 4f, 1f, Color.Black);
                }
                HeatController controller = self.SceneAs<Level>().Tracker.GetEntity<HeatController>();
                if (controller != null && controller.FlashingRed)
                {
                    if (self.Scene.OnRawInterval(0.06f))
                    {
                        if (self.Sprite.Color == Color.Red)
                        {
                            self.Sprite.Color = Color.White;
                        }
                        else if (self.Sprite.Color == Color.White)
                        {
                            self.Sprite.Color = Color.Red;
                        }
                    }
                }
                else
                {
                    if ((bool)playerFlash.GetValue(self))
                    {
                        self.Sprite.Color = Color.LightBlue;
                    }
                    else
                    {
                        self.Sprite.Color = Color.White;
                    }
                }
            }
            else
            {
                orig(self);
            }
        }

        private static void onSpeedRunTimerDisplayRender(On.Celeste.SpeedrunTimerDisplay.orig_Render orig, SpeedrunTimerDisplay self)
        {
            if (XaphanModule.useMetroidGameplay)
            {
                if (Engine.Scene is Level)
                {
                    Level level = (Level)Engine.Scene;
                    if (level.Paused)
                    {
                        self.Y = 60f;
                    }
                    else
                    {
                        self.Y = 130f;
                    }
                }
            }
            orig(self);
        }

        private static void onTotalStrawberriesDisplayRender(On.Celeste.TotalStrawberriesDisplay.orig_Render orig, TotalStrawberriesDisplay self)
        {
            if (!XaphanModule.useMetroidGameplay)
            {
                orig(self);
            }
        }

        private static void ilPlayerJump(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);

            while (cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(-105f)))
            {
                cursor.EmitDelegate<Func<float>>(determineJumpHeightFactor);
                cursor.Emit(OpCodes.Mul);
            }
        }

        private static void ilPlayerNormalUpdate(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);

            // Increase X speed based on MaxRunSpeed value

            if (cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(90f))
                && cursor.TryGotoNext(MoveType.Before, instr => instr.OpCode == OpCodes.Stloc_S
                    && (((VariableDefinition)instr.Operand).Index == 6 || ((VariableDefinition)instr.Operand).Index == 31)))
            {
                VariableDefinition variable = (VariableDefinition)cursor.Next.Operand;
                if (cursor.TryGotoNext(MoveType.Before, instr => instr.OpCode == OpCodes.Ldflda))
                {
                    cursor.Emit(OpCodes.Pop);
                    cursor.Emit(OpCodes.Ldloc_S, variable);
                    cursor.EmitDelegate<Func<float>>(determineSpeedXFactor);
                    cursor.Emit(OpCodes.Mul);
                    cursor.Emit(OpCodes.Stloc_S, variable);
                    cursor.Emit(OpCodes.Ldarg_0);
                }
            }

            // Change falling Speed if holding down

            while (cursor.TryGotoNext(MoveType.After, instr => instr.OpCode == OpCodes.Ldc_R4 && (float)instr.Operand == 240f))
            {
                cursor.EmitDelegate<Func<float>>(determineFallSpeedFactor);
                cursor.Emit(OpCodes.Mul);
            }

            // Force player to stay duck if MorphMode is true

            if (cursor.TryGotoNext(MoveType.After,
                instr => instr.MatchCallvirt<Player>("get_Ducking"),
                instr => (instr.OpCode == OpCodes.Brfalse || instr.OpCode == OpCodes.Stloc_S))
                && (cursor.Prev.OpCode == OpCodes.Brfalse || cursor.Next.Next.OpCode == OpCodes.Brfalse))
            {
                if (cursor.Prev.OpCode == OpCodes.Stloc_S)
                {
                    cursor.Index += 2;
                }
                ILLabel target = (ILLabel)cursor.Prev.Operand;
                cursor.EmitDelegate<Func<bool>>(ForceDuck);
                cursor.Emit(OpCodes.Brtrue, target);
                cursor.GotoLabel(target);
                if (cursor.TryGotoNext(MoveType.Before, instr => instr.MatchLdsfld(typeof(Input), "MoveY")))
                {
                    ILCursor cursorAfterCondition = cursor.Clone();
                    if (cursorAfterCondition.TryGotoNext(MoveType.After, instr => (instr.OpCode == OpCodes.Bne_Un || instr.OpCode == OpCodes.Bne_Un_S)))
                    {
                        cursor.EmitDelegate<Func<bool>>(ForceDuck);
                        cursor.Emit(OpCodes.Brtrue, cursorAfterCondition.Next);
                    }
                }
            }

            // Change gravity while Shinesparking

            while (cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(900f)))
            {
                cursor.EmitDelegate<Func<float>>(determineGravityFactor);
                cursor.Emit(OpCodes.Mul);
            }
        }

        private static void ilPlayerRender(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);

            if (cursor.TryGotoNext(instr => instr.MatchCallvirt<StateMachine>("get_State"), instr => instr.MatchLdcI4(19)))
            {
                cursor.Index++;
                cursor.EmitDelegate<Func<int, int>>(orig => {
                    if (determineifMetGameplay())
                    {
                        return 19;
                    }
                    return orig;
                });
            }
        }

        private static float determineJumpHeightFactor()
        {
            if (Engine.Scene is Level)
            {
                Level level = (Level)Engine.Scene;
                if (XaphanModule.useMetroidGameplay)
                {
                    return isSpaceJumping ? 1.15f : (HighJumpBoots.Active(level) ? 1.66f : 1.33f) - (MorphMode ? 0.33f : 0f);
                }
            }
            return 1f;
        }

        private static float determineSpeedXFactor()
        {
            return XaphanModule.useMetroidGameplay ? MaxRunSpeed : 1f;
        }

        private static float determineFallSpeedFactor()
        {
            return XaphanModule.useMetroidGameplay ? 0.66f : 1f;
        }

        private static bool ForceDuck()
        {
            if (XaphanModule.useMetroidGameplay && MorphMode)
            {
                return true;
            }
            return false;
        }

        private static float determineGravityFactor()
        {
            return XaphanModule.useMetroidGameplay ? ((StartedShinesparking || Shinesparking) ? 0 : 1f) : 1f;
        }

        private static bool determineifMetGameplay()
        {
            if (XaphanModule.useMetroidGameplay)
            {
                if (Engine.Scene is Level)
                {
                    Level level = (Level)Engine.Scene;
                    HeatController controller = level.Tracker.GetEntity<HeatController>();
                    if (controller != null)
                    {
                        if (controller.FlashingRed)
                        {
                            return true;
                        }
                    }
                }
                if (MaxRunSpeed >= 1.5f || ShinesparkCharged || StartedShinesparking || Shinesparking)
                {
                    return true;
                }
            }
            return false;
        }

        private static void createTrail(Player player)
        {
            Vector2 scale = new Vector2(Math.Abs(player.Sprite.Scale.X) * (float)player.Facing, player.Sprite.Scale.Y);
            if (player.StateMachine.State != 14)
            {
                TrailManager.Add(player.Position + new Vector2(0, -20), player.Get<PlayerSprite>(), player.Get<PlayerHair>(), scale, Calc.HexToColor(GravityJacket.Active(player.SceneAs<Level>()) ? "9E68A5" : VariaJacket.Active(player.SceneAs<Level>()) ? "F8930E" : "D6AD00"), player.Depth + 1, 0.5f);
            }
        }

        private static IEnumerator Shoot(Player player)
        {
            AmmoDisplay ammoDisplay = player.SceneAs<Level>().Tracker.GetEntity<AmmoDisplay>();
            if (ammoDisplay != null && !ammoDisplay.MissileSelected && !ammoDisplay.SuperMissileSelected)
            {
                string beamType;
                if (PlasmaBeam.Active(player.SceneAs<Level>()))
                {
                    beamType = "Plasma";
                }
                else if (Spazer.Active(player.SceneAs<Level>()))
                {
                    beamType = "Spazer";
                }
                else
                {
                    beamType = "Power";
                }
                if (WaveBeam.Active(player.SceneAs<Level>()))
                {
                    beamType += "Wave";
                }
                if (IceBeam.Active(player.SceneAs<Level>()))
                {
                    beamType += "Ice";
                }
                string beamSound = "power";
                if (beamType.Contains("Wave"))
                {
                    beamSound = "wave";
                }
                if (beamType.Contains("Spazer"))
                {
                    beamSound = "spazer";
                }
                if (beamType.Contains("Plasma"))
                {
                    beamSound = "plasma";
                }
                if (beamType.Contains("Ice"))
                {
                    beamSound = "ice";
                }
                if ((beamType.Contains("Spazer") || beamType.Contains("Plasma")) && beamType.Contains("Ice"))
                {
                    beamSound = "ice_spazer_plasma";
                }
                beamSound = "event:/game/xaphan/" + beamSound + "_beam_fire";
                if (beamType.Contains("Plasma") && beamType.Contains("Wave") && !Spazer.Active(player.SceneAs<Level>()))
                {
                    player.SceneAs<Level>().Add(new Beam(player, beamType, beamSound, player.Position, 3, true));
                    player.SceneAs<Level>().Add(new Beam(player, beamType, beamSound, player.Position, -3, false));
                }
                else
                {
                    if (Spazer.Active(player.SceneAs<Level>()))
                    {
                        player.SceneAs<Level>().Add(new Beam(player, beamType, beamSound, player.Position, 5, true));

                    }
                    player.SceneAs<Level>().Add(new Beam(player, beamType, beamSound, player.Position, (beamType == "PowerWave" || beamType == "PowerWaveIce") ? 4 : 0));
                    if (Spazer.Active(player.SceneAs<Level>()))
                    {
                        player.SceneAs<Level>().Add(new Beam(player, beamType, beamSound, player.Position, -5, true));
                    }
                }
                BeamDelay = 0.35f;
            }
            else if (ammoDisplay != null && ammoDisplay.MissileSelected)
            {
                player.SceneAs<Level>().Add(new Missile(player, player.Position, false));
                ammoDisplay.CurrentMissiles -= 1;
                BeamDelay = 0.25f;
            }
            else if (ammoDisplay != null && ammoDisplay.SuperMissileSelected)
            {
                player.SceneAs<Level>().Add(new Missile(player, player.Position, true));
                ammoDisplay.CurrentSuperMissiles -= 1;
                BeamDelay = 0.5f;
            }
            while (BeamDelay > 0f)
            {
                BeamDelay -= Engine.DeltaTime;
                yield return null;
            }

        }

        private static IEnumerator ShootAnimationDelay()
        {
            while (Input.Dash.Check)
            {
                BeamAnimation = true;
                yield return null;
            }
            BeamAnimation = false;
        }

        private static IEnumerator UseBomb(Player player)
        {
            AmmoDisplay ammoDisplay = player.SceneAs<Level>().Tracker.GetEntity<AmmoDisplay>();
            if (ammoDisplay != null && !ammoDisplay.PowerBombSelected && MorphBombs.Active(player.SceneAs<Level>()))
            {
                player.SceneAs<Level>().Add(new MorphBomb(player.Center));
                BombDelay = 0.25f;
                while (BombDelay > 0f)
                {
                    BombDelay -= Engine.DeltaTime;
                    yield return null;
                }
            }
            else if (ammoDisplay != null && ammoDisplay.PowerBombSelected)
            {
                player.SceneAs<Level>().Add(new PowerBomb(player.Center));
                ammoDisplay.CurrentPowerBombs -= 1;
                BombDelay = 2.5f;
                while (BombDelay > 0f)
                {
                    BombDelay -= Engine.DeltaTime;
                    yield return null;
                }
            }
        }

        private static IEnumerator ShinesparkDelay(Player player)
        {
            while (Input.MenuDown.Check)
            {
                player.StateMachine.State = Player.StDummy;
                player.Sprite.Play("Idle");
                player.Speed.X = 0;
                yield return null;
            }
            JustChargedShinespark = false;
            player.StateMachine.State = Player.StNormal;
        }

        private static IEnumerator ChargedShinesparkTimer(Player player)
        {
            float timer = 3f;
            ShinesparkCooldown = true;
            Audio.Stop(shinesparkSfx, false);
            shinesparkSfx = Audio.Play("event:/game/xaphan/shinespark_charged_loop");
            while (timer > 0 && !player.Dead)
            {
                timer -= Engine.DeltaTime;
                yield return null;
            }
            ShinesparkCharged = ShinesparkCooldown = false;
            if (!Shinesparking)
            {
                Audio.Stop(shinesparkSfx, false);
            }
        }

        private static IEnumerator InitShinespark(Player player)
        {
            DynData<Player> playerData = new DynData<Player>(player);
            bool playerInControl = playerData.Get<bool>("InControl");
            ShinesparkInitCooldown = true;
            ShinesparkDirection = "Up";
            float timer = 0.5f;
            while (timer > 0)
            {
                timer -= Engine.DeltaTime;
                player.Speed.X = 0f;
                player.Speed.Y = 0f;
                if (Input.MenuUp.Check && !Input.MenuLeft.Check && !Input.MenuRight.Check)
                {
                    ShinesparkDirection = "Up";
                    timer = 0;
                }
                else if (!Input.MenuUp.Check && Input.MenuLeft.Check && !Input.MenuRight.Check)
                {
                    ShinesparkDirection = "Left";
                    timer = 0;
                }
                else if (!Input.MenuUp.Check && !Input.MenuLeft.Check && Input.MenuRight.Check)
                {
                    ShinesparkDirection = "Right";
                    timer = 0;
                }
                else if (player == null)
                {
                    yield break;
                }
                yield return null;
            }
            Audio.Stop(shinesparkSfx, false);
            shinesparkSfx = Audio.Play("event:/game/xaphan/shinespark_loop");
            Facings ShinesparkFacing = player.Facing;
            ShinesparkCharged = StartedShinesparking = ShinesparkInitCooldown = false;
            Shinesparking = true;
            if (ShinesparkDirection == "Up")
            {
                float playerXPosition = player.Position.X;
                while (player != null && !player.Dead && !player.CollideCheck<Solid>(player.Position + new Vector2(0, -1)))
                {
                    player.Speed.Y = Calc.Approach(player.Speed.Y, -400f, 100f);
                    player.Speed.X = 0f;
                    player.Facing = ShinesparkFacing;
                    yield return null;
                }
            }
            else if (ShinesparkDirection == "Left")
            {
                while (player != null && !player.Dead && !player.CollideCheck<Solid>(player.Position + new Vector2(-1, 0)))
                {
                    player.Speed.X = Calc.Approach(player.Speed.X, -400f, 100f);
                    player.Speed.Y = 0f;
                    player.Facing = Facings.Left;
                    yield return null;
                }
            }
            else if (ShinesparkDirection == "Right")
            {
                while (player != null && !player.Dead && !player.CollideCheck<Solid>(player.Position + new Vector2(1, 0)))
                {
                    player.Speed.X = Calc.Approach(player.Speed.X, 400f, 100f);
                    player.Speed.Y = 0f;
                    player.Facing = Facings.Right;
                    yield return null;
                }
            }
            Audio.Stop(shinesparkSfx, false);
            shinesparkSfx = Audio.Play("event:/game/xaphan/shinespark_end");
            player.SceneAs<Level>().Shake(0.3f);
            timer = 0.75f;
            while (timer > 0)
            {
                timer -= Engine.DeltaTime;
                player.Facing = ShinesparkFacing;
                player.StateMachine.State = Player.StDummy;
                player.DummyAutoAnimate = player.DummyGravity = false;
                player.Speed = Vector2.Zero;
                yield return null;
            }
            player.StateMachine.State = Player.StNormal;
            player.DummyAutoAnimate = player.DummyGravity = true;
            Shinesparking = false;
            player.Sprite.Play("fallSlow");
        }
    }
}
