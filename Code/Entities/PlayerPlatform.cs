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
    class PlayerPlatform : Solid
    {
        protected XaphanModuleSettings Settings => XaphanModule.Settings;

        private Sprite PlayerSprite;

        private Sprite PlayerHairSprite;

        private Vector2 StartPosition;

        private Vector2 EndPosition;

        public string Side;

        private bool Gentle;

        private bool CanSlide;

        public bool UpsideDown;

        public bool StickyDash;

        public bool CanJumpThrough;

        private int SlopeHeight;

        public float platfromWidth;

        public float slopeTop;

        public bool Sliding;

        public PlayerPlatform(Vector2 position, int width, bool gentle, string side, int soundIndex, int slopeHeight, bool canSlide, float top, bool upsideDown = false, bool stickyDash = false, bool canJumpThrough = false) : base(position, width, 4, true)
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
            CanSlide = canSlide;
            slopeTop = top;
            StickyDash = stickyDash;
            CanJumpThrough = canJumpThrough;
            Add(PlayerSprite = GFX.SpriteBank.Create("XaphanHelper_player_slide"));
            PlayerSprite.Visible = false;
            Add(PlayerHairSprite = GFX.SpriteBank.Create("XaphanHelper_player_slide"));
            PlayerHairSprite.Visible = false;
        }

        public static void Load()
        {
            Everest.Events.Player.OnDie += onPlayerDie;
            On.Celeste.Solid.MoveVExact += OnSolidMoveVExact;
            On.Celeste.Solid.Update += OnSolidUpdate;
        }

        public static void Unload()
        {
            Everest.Events.Player.OnDie -= onPlayerDie;
            On.Celeste.Solid.MoveVExact -= OnSolidMoveVExact;
        }

        private static void onPlayerDie(Player player)
        {
            foreach (PlayerPlatform platform in player.SceneAs<Level>().Tracker.GetEntities<PlayerPlatform>())
            {
                if (platform.PlayerSprite.Visible)
                {
                    platform.PlayerSprite.Visible = false;
                }
                if (platform.PlayerHairSprite.Visible)
                {
                    platform.PlayerHairSprite.Visible = false;
                }
            }
        }

        private static void OnSolidMoveVExact(On.Celeste.Solid.orig_MoveVExact orig, Solid self, int move)
        {
            if (self.GetType() == typeof(PlayerPlatform))
            {
                PlayerPlatform platform = (PlayerPlatform)self;
                Player player = self.Scene.Tracker.GetEntity<Player>();
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

        private static void OnSolidUpdate(On.Celeste.Solid.orig_Update orig, Solid self)
        {
            foreach (PlayerPlatform platform in self.Scene.Tracker.GetEntities<PlayerPlatform>())
            {
                if (!platform.HasPlayerRider() && platform.CollideFirst<Player>(platform.Position + Vector2.UnitY) == null)
                {
                    platform.Collidable = false;
                }
            }
            orig(self);
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            StartPosition = Position;
        }

        public override void Update()
        {
            base.Update();
            Player player = SceneAs<Level>().Tracker.GetEntity<Player>();
            Drone drone = SceneAs<Level>().Tracker.GetEntity<Drone>();
            if (drone != null)
            {
                if (drone.FakePlayer != null && !drone.dead)
                {
                    if (drone.FakePlayer.Sprite.Rate == 2)
                    {
                        Collidable = false;
                    }
                }
            }
            if (player != null)
            {
                if (Sliding && (player.Sprite.CurrentAnimationID != "duck" || player.Speed.X == 0 || Input.Jump.Pressed))
                {
                    Sliding = false;
                    player.Sprite.Visible = true;
                    player.Hair.Visible = true;
                }
                if (player.Sprite.Visible)
                {
                    PlayerSprite.Visible = false;
                    PlayerHairSprite.Visible = false;
                }
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
                    if (!UpsideDown)
                    {
                        if (XaphanModule.PlayerIsControllingRemoteDrone() && CollideCheck(player))
                        {
                            player.MoveToY(player.Position.Y - 1);
                        }
                        if (player.Bottom > StartPosition.Y + 4)
                        {
                            Collidable = false;
                        }
                        else
                        {
                            SetCollision(player);
                        }
                        if (Side == "Left")
                        {
                            if (CanSlide && drone == null && player.IsRiding(this) && Input.MoveY == 1 && Input.MoveX != -1 && player.Left >= Left && SceneAs<Level>().Tracker.GetEntity<MapScreen>() == null && SceneAs<Level>().Tracker.GetEntity<StatusScreen>() == null)
                            {
                                Sliding = true;
                                player.Sprite.Visible = false;
                                player.Hair.Visible = false;
                                PlayerSprite.Visible = true;
                                PlayerHairSprite.Visible = true;
                                if (player.Facing != Facings.Right)
                                {
                                    player.Facing = Facings.Right;
                                }
                                if (player.Speed.X <= 250f - (Gentle ? 15f : 18f))
                                {
                                    player.Speed.X += Gentle ? 15f : 18f;
                                }
                                else
                                {
                                    player.Speed.X = 250f;
                                }
                            }
                            if (player.BottomCenter.X < Right + 16 && Position.Y >= StartPosition.Y - 8 * SlopeHeight - 4)
                            {
                                EndPosition = new Vector2(StartPosition.X, StartPosition.Y - (Right - (Gentle ? 0 : 4) - player.BottomCenter.X + (((XaphanModule.useMetroidGameplay && MetroidGameplayController.Shinesparking) || (!XaphanModule.useMetroidGameplay && SceneAs<Level>().Session.GetFlag("Xaphan_Helper_Shinesparking"))) ? 16f : 4f)) / (Gentle ? 2 : 1));
                                Add(new Coroutine(MoveSlope(player)));
                            }
                        }
                        else if (Side == "Right")
                        {
                            if (CanSlide && drone == null && player.IsRiding(this) && Input.MoveY == 1 && Input.MoveX != 1 && player.Right <= Right && SceneAs<Level>().Tracker.GetEntity<MapScreen>() == null && SceneAs<Level>().Tracker.GetEntity<StatusScreen>() == null)
                            {
                                Sliding = true;
                                player.Sprite.Visible = false;
                                player.Hair.Visible = false;
                                PlayerSprite.Visible = true;
                                PlayerHairSprite.Visible = true;
                                if (player.Facing != Facings.Left)
                                {
                                    player.Facing = Facings.Left;
                                }
                                if (player.Speed.X >= -250f + (Gentle ? 15f : 18f))
                                {
                                    player.Speed.X -= Gentle ? 15f : 18f;
                                }
                                else
                                {
                                    player.Speed.X = -250f;
                                }
                            }
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
                        if (player.Top < StartPosition.Y + 12)
                        {
                            Collidable = false;
                        }
                        else
                        {
                            SetCollision(player);
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
                if (Collidable)
                {
                    if (!UpsideDown && player.Left >= Left && player.Right <= Right && player.Top < Bottom && player.Bottom > Top)
                    {
                        player.Position -= Vector2.UnitY;
                    }
                    if (UpsideDown && player.Left >= Left && player.Right <= Right && player.Bottom > Top && player.Top < Bottom)
                    {
                        player.Position += Vector2.UnitY;
                    }
                }
            }
        }

        public IEnumerator MoveSlope(Player player)
        {
            MoveToY(Math.Max(EndPosition.Y, StartPosition.Y - 8 * SlopeHeight - 4), 0);
            yield return null;
        }

        public void SetCollision(Player player)
        {
            if (player != null)
            {
                if (!UpsideDown)
                {
                    if (CanJumpThrough)
                    {
                        if (player.Bottom <= Top)
                        {
                            Collidable = true;
                        }
                        else
                        {
                            Collidable = false;
                        }
                    }
                    else
                    {
                        Collidable = true;
                    }
                }
                else
                {
                    if (CanJumpThrough)
                    {
                        if (player.Top >= Bottom)
                        {
                            Collidable = true;
                        }
                        else
                        {
                            Collidable = false;
                        }
                    }
                    else
                    {
                        Collidable = true;
                    }
                }
            }            
        }

        public override void Render()
        {
            if (XaphanModule.useUpgrades && (VariaJacket.Active(SceneAs<Level>()) || GravityJacket.Active(SceneAs<Level>())))
            {
                string id = "";
                if (GravityJacket.Active(SceneAs<Level>()))
                {
                    id = "gravity";
                }
                else if (VariaJacket.Active(SceneAs<Level>()))
                {
                    id = "varia";
                }
                Effect fxColorGrading = GFX.FxColorGrading;
                fxColorGrading.CurrentTechnique = fxColorGrading.Techniques["ColorGradeSingle"];
                Engine.Graphics.GraphicsDevice.Textures[1] = GFX.ColorGrades[id].Texture.Texture_Safe;
                Draw.SpriteBatch.End();
                Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, fxColorGrading, (Scene as Level).GameplayRenderer.Camera.Matrix);
            }
            base.Render();
            Player player = SceneAs<Level>().Tracker.GetEntity<Player>();
            if (player != null && PlayerSprite != null && PlayerHairSprite != null)
            {
                if (player.Facing == Facings.Left)
                {
                    PlayerSprite.FlipX = true;
                    PlayerHairSprite.FlipX = true;
                }
                else
                {
                    PlayerSprite.FlipX = false;
                    PlayerHairSprite.FlipX = false;
                }
                PlayerSprite.RenderPosition = player.Position + new Vector2(-16f, -31f);
                PlayerHairSprite.RenderPosition = player.Position + new Vector2(-16f, -31f);
                string backpack = SceneAs<Level>().Session.Inventory.Backpack ? "Backpack" : "NoBackpack";
                if (PlayerSprite.Visible && PlayerHairSprite.Visible)
                {
                    PlayerSprite.Play("slide" + backpack);
                    PlayerHairSprite.Color = player.Hair.Color;
                    PlayerHairSprite.Play("hair" + backpack);
                }
            }
            if (XaphanModule.useUpgrades && (VariaJacket.Active(SceneAs<Level>()) || GravityJacket.Active(SceneAs<Level>())))
            {
                Draw.SpriteBatch.End();
                Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, (Scene as Level).GameplayRenderer.Camera.Matrix);
            }
        }

        // Remove debug render

        public override void DebugRender(Camera camera)
        {
            
        }
    }
}