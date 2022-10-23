using Celeste.Mod.Entities;
using Celeste.Mod.XaphanHelper.Controllers;
using Celeste.Mod.XaphanHelper.UI_Elements;
using Celeste.Mod.XaphanHelper.Upgrades;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [Tracked(true)]
    [CustomEntity("XaphanHelper/Slope")]
    class FakePlayerPlatform : Solid
    {
        protected XaphanModuleSettings Settings => XaphanModule.Settings;

        private Vector2 StartPosition;

        private Vector2 EndPosition;

        public string Side;

        private bool Gentle;

        public bool UpsideDown;

        public bool StickyDash;

        public bool CanJumpThrough;

        private int SlopeHeight;

        public float platfromWidth;

        public float slopeTop;

        public FakePlayerPlatform(Vector2 position, int width, bool gentle, string side, int soundIndex, int slopeHeight, float top, bool upsideDown = false, bool stickyDash = false, bool canJumpThrough = false) : base(position, width, 4, true)
        {
            AllowStaticMovers = false;
            Gentle = gentle;
            Side = side;
            UpsideDown = upsideDown;
            Collidable = false;
            Collider = new Hitbox(width, 4, Gentle ? (Side == "Left" ? 4 : -12) : (Side == "Left" ? 4 : -4), UpsideDown ? 4 : 8);
            SurfaceSoundIndex = soundIndex;
            SlopeHeight = slopeHeight;
            platfromWidth = width;
            slopeTop = top;
            StickyDash = stickyDash;
            CanJumpThrough = canJumpThrough;
        }

        public static void Load()
        {
            On.Celeste.Solid.MoveVExact += OnSolidMoveVExact;
        }

        public static void Unload()
        {
            On.Celeste.Solid.MoveVExact -= OnSolidMoveVExact;
        }

        private static void OnSolidMoveVExact(On.Celeste.Solid.orig_MoveVExact orig, Solid self, int move)
        {
            if (self.GetType() == typeof(FakePlayerPlatform))
            {
                FakePlayerPlatform platform = (FakePlayerPlatform)self;
                FakePlayer player = self.Scene.Tracker.GetEntity<FakePlayer>();
                if (!platform.UpsideDown)
                {
                    //if (self.Collidable)
                    {
                        if (player != null)
                        {
                            if (move < 0)
                            {
                                if (player.IsRiding(self))
                                {
                                    self.Collidable = false;
                                    if (player.TreatNaive)
                                    {
                                        player.NaiveMove(Vector2.UnitY * move);
                                    }
                                    else
                                    {
                                        player.MoveVExact(move);
                                    }
                                    self.Collidable = true;
                                }
                                else if (!player.TreatNaive && self.CollideCheck(player, self.Position + Vector2.UnitY * move) && !self.CollideCheck(player))
                                {
                                    self.Collidable = false;
                                    player.MoveVExact((int)(self.Top + move - player.Bottom));
                                    self.Collidable = true;
                                }
                            }
                            else
                            {
                                if ((player.IsRiding(self) && (platform.StickyDash || player.StateMachine.State != 2)))
                                {
                                    self.Collidable = false;
                                    if (player.TreatNaive)
                                    {
                                        player.NaiveMove(Vector2.UnitY * move);
                                    }
                                    else
                                    {
                                        player.MoveVExact(move);
                                    }
                                    self.Collidable = true;
                                }
                            }
                        }
                    }
                    self.Y += move;
                    self.MoveStaticMovers(Vector2.UnitY * move);
                }
                else
                {
                    orig(self, move);
                }
            }
            else
            {
                orig(self, move);
            }
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            StartPosition = Position;
        }

        public override void Update()
        {
            base.Update();
            FakePlayer player = SceneAs<Level>().Tracker.GetEntity<FakePlayer>();
            Drone drone = SceneAs<Level>().Tracker.GetEntity<Drone>();
            if (player != null)
            {
                if (player.Right <= Left - 16 || player.Left >= Right + 16)
                {
                    Position = StartPosition;
                    return;
                }
                if (!UpsideDown)
                {
                    if (Position.Y > StartPosition.Y)
                    {
                        Position.Y = StartPosition.Y;
                    }
                }
                else
                {
                    if (Position.Y < StartPosition.Y)
                    {
                        Position.Y = StartPosition.Y;
                    }
                }
                if ((player.Sprite.Rate != 2 || (player.Sprite.Rate == 2 && player.Sprite.CurrentAnimationID == "wakeUp")))
                {
                    Collidable = false;
                    if (!UpsideDown)
                    {
                        if (XaphanModule.PlayerIsControllingRemoteDrone() && CollideCheck(player))
                        {
                            player.MoveToY(player.Position.Y - 1);
                        }
                        if (Side == "Left")
                        {
                            if (player.BottomCenter.X < Right + 16 && Position.Y >= StartPosition.Y - 8 * SlopeHeight - 4)
                            {
                                EndPosition = new Vector2(StartPosition.X, StartPosition.Y - (Right - (Gentle ? 0 : 4) - player.BottomCenter.X + (((XaphanModule.useMetroidGameplay && MetroidGameplayController.Shinesparking) || (!XaphanModule.useMetroidGameplay && SceneAs<Level>().Session.GetFlag("Xaphan_Helper_Shinesparking"))) ? 16f : 4f)) / (Gentle ? 2 : 1));
                                Add(new Coroutine(MoveSlope(player)));
                            }
                        }
                        else if (Side == "Right")
                        {
                            if (player.BottomCenter.X > Left - 16 && Position.Y >= StartPosition.Y - 8 * SlopeHeight - 4)
                            {
                                EndPosition = new Vector2(StartPosition.X, StartPosition.Y + (Left + (Gentle ? 0 : 4) - player.BottomCenter.X - (((XaphanModule.useMetroidGameplay && MetroidGameplayController.Shinesparking) || (!XaphanModule.useMetroidGameplay && SceneAs<Level>().Session.GetFlag("Xaphan_Helper_Shinesparking"))) ? 16f : 4f)) / (Gentle ? 2 : 1));
                                Add(new Coroutine(MoveSlope(player)));
                            }
                        }
                        if (player.Top > StartPosition.Y && !player.Ducking)
                        {
                            Position.Y = StartPosition.Y;
                        }
                    }
                    else
                    {
                        if (XaphanModule.PlayerIsControllingRemoteDrone() && CollideCheck(player))
                        {
                            player.MoveToY(player.Position.Y + 1);
                        }
                        if (Side == "Left")
                        {
                            if (player.BottomCenter.X < Right && player.BottomCenter.X > Left && Position.Y >= StartPosition.Y - 8 * SlopeHeight - 4)
                            {
                                EndPosition = new Vector2(StartPosition.X, StartPosition.Y - (Right - (Gentle ? 0 : 4) - player.BottomCenter.X + (((XaphanModule.useMetroidGameplay && MetroidGameplayController.Shinesparking) || (!XaphanModule.useMetroidGameplay && SceneAs<Level>().Session.GetFlag("Xaphan_Helper_Shinesparking"))) ? (Gentle ? 16f : 8f) : (Gentle ? 8f : 4f))) / (Gentle ? 2 : 1) * -1 - (Gentle ? 2 : 0));
                                Add(new Coroutine(MoveSlope(player)));
                            }
                        }
                        else if (Side == "Right")
                        {
                            if (player.BottomCenter.X > Left && player.BottomCenter.X < Right && Position.Y >= StartPosition.Y - 8 * SlopeHeight - 4)
                            {
                                EndPosition = new Vector2(StartPosition.X, StartPosition.Y + (Left + (Gentle ? 0 : 4) - player.BottomCenter.X - (((XaphanModule.useMetroidGameplay && MetroidGameplayController.Shinesparking) || (!XaphanModule.useMetroidGameplay && SceneAs<Level>().Session.GetFlag("Xaphan_Helper_Shinesparking"))) ? (Gentle ? 16f : 8f) : (Gentle ? 8f : 4f))) / (Gentle ? 2 : 1) * -1 - (Gentle ? 2 : 0));
                                Add(new Coroutine(MoveSlope(player)));
                            }
                        }
                        if (Position.Y > StartPosition.Y + SlopeHeight * 8 + 4)
                        {
                            Position.Y = StartPosition.Y + SlopeHeight * 8 + 4;
                        }
                    }
                }
            }
        }

        public IEnumerator MoveSlope(FakePlayer player)
        {
            MoveToY(Math.Max(EndPosition.Y, StartPosition.Y - 8 * SlopeHeight - 4), 0);
            yield return null;
        }

        // Remove debug render

        public override void DebugRender(Camera camera)
        {

        }
    }
}