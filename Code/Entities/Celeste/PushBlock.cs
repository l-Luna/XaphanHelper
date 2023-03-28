using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.Entities;
using Celeste.Mod.XaphanHelper.Upgrades;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [Tracked(true)]
    [CustomEntity("XaphanHelper/PushBlock")]
    class PushBlock : Solid
    {
        public ParticleType P_Move;

        public ParticleType P_Activate;

        private Level level;

        private string directory;

        private float alpha = 0f;

        private MTexture[,] nineSlice;

        private MTexture[,] dashNineSlice;

        private MTexture[,] glowNineSlice;

        private MTexture[,] glowDashNineSlice;

        private bool canPush;

        public bool canKill;

        private bool mustDash;

        private int playerDashes;

        private int playerJumps;

        public PushBlock(EntityData data, Vector2 offset) : base(data.Position + offset, data.Width, data.Height, safe: false)
        {
            OnDashCollide = OnDashed;
            canPush = true;
            SurfaceSoundIndex = data.Int("soundIndex", 8);
            directory = data.Attr("directory");
            canKill = data.Bool("mustDash");
            mustDash = data.Bool("mustDash");
            if (string.IsNullOrEmpty(directory))
            {
                directory = "objects/XaphanHelper/PushBlock";
            }
            MTexture mTexture = GFX.Game[directory + "/block"];
            nineSlice = new MTexture[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    nineSlice[i, j] = mTexture.GetSubtexture(new Rectangle(i * 8, j * 8, 8, 8));
                }
            }
            mTexture = GFX.Game[directory + "/blockDash"];
            dashNineSlice = new MTexture[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    dashNineSlice[i, j] = mTexture.GetSubtexture(new Rectangle(i * 8, j * 8, 8, 8));
                }
            }
            mTexture = GFX.Game[directory + "/glow"];
            glowNineSlice = new MTexture[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    glowNineSlice[i, j] = mTexture.GetSubtexture(new Rectangle(i * 8, j * 8, 8, 8));
                }
            }
            mTexture = GFX.Game[directory + "/glowDash"];
            glowDashNineSlice = new MTexture[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    glowDashNineSlice[i, j] = mTexture.GetSubtexture(new Rectangle(i * 8, j * 8, 8, 8));
                }
            }
            P_Activate = new ParticleType
            {
                Color = Calc.HexToColor(data.Attr("particleColor2", "1F2E2D")),
                Size = 1f,
                FadeMode = ParticleType.FadeModes.Late,
                DirectionRange = 1.74532926f,
                SpeedMin = 30f,
                SpeedMax = 50f,
                SpeedMultiplier = 0.7f,
                LifeMin = 0.3f,
                LifeMax = 0.7f
            };
            P_Move = new ParticleType
            {
                Source = GFX.Game["particles/rect"],
                Color = Calc.HexToColor(data.Attr("particleColor1", "4D6B68")),
                Color2 = Calc.HexToColor(data.Attr("particleColor2", "1F2E2D")),
                ColorMode = ParticleType.ColorModes.Choose,
                RotationMode = ParticleType.RotationModes.SameAsDirection,
                Size = 0.4f,
                SizeRange = 0.1f,
                DirectionRange = (float)Math.PI * 2f,
                FadeMode = ParticleType.FadeModes.None,
                LifeMin = 0.1f,
                LifeMax = 0.2f,
                SpeedMin = 30f,
                SpeedMax = 40f,
                SpeedMultiplier = 0.8f
            };
            Add(new LightOcclude(0.2f));
            Depth = -9999;
        }

        public static void Load()
        {
            On.Celeste.Player.Update += onPlayerUpdate;
        }


        public static void Unload()
        {
            On.Celeste.Player.Update -= onPlayerUpdate;
        }

        private static void onPlayerUpdate(On.Celeste.Player.orig_Update orig, Player self)
        {
            List<Entity> pushBlocks = self.Scene.Tracker.GetEntities<PushBlock>().ToList();
            foreach (PushBlock pushBlock in pushBlocks)
            {
                if (!SaveData.Instance.Assists.Invincible && pushBlock.canKill && (!self.DashAttacking || (self.DashAttacking && (self.Bottom < pushBlock.Top - 5 && self.DashDir.Y <= 0))))
                {
                    pushBlock.Collidable = false;
                }
            }
            orig(self);
            foreach (PushBlock pushBlock in pushBlocks)
            {
                if (pushBlock.canKill)
                {
                    pushBlock.Collidable = true;
                }
            }
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            level = SceneAs<Level>();
        }

        public override void Update()
        {
            base.Update();
            alpha += Engine.DeltaTime * 4f;
            Player player = SceneAs<Level>().Tracker.GetEntity<Player>();
            if (player != null)
            {
                playerDashes = player.DashAttacking ? player.Dashes : 0;
                playerJumps = player.DashAttacking ? SpaceJump.GetJumpBuffer() : 0;
                if (!player.DashAttacking && canKill)
                {
                    KillPlayer(player);
                }
                if (SceneAs<Level>().Session.GetFlag("Xaphan_Helper_Shinesparking") && (CollideCheck(player, Position + Vector2.UnitX) || (CollideCheck(player, Position - Vector2.UnitX))))
                {
                    OnDashed(player, new Vector2(player.Facing == Facings.Left ? -1 : 1, 0));
                    if (!MoveHCheck(player.Facing == Facings.Left ? -1 : 1))
                    {
                        player.Rebound(player.Facing == Facings.Left ? 1 : -1);
                    }
                    else if (canKill)
                    {
                        player.Die(new Vector2(player.Facing == Facings.Left ? 1f : -1f, 0f));
                    }
                }
            }
        }

        public void KillPlayer(Player player)
        {
            if (HasPlayerOnTop())
            {
                player.Die(new Vector2(0f, -1f));
            }
            if ((CollideCheck(player, Position + Vector2.UnitX) && player.Speed.X < 0) || (HasPlayerClimbing() && player.Left >= Right))
            {
                player.Die(new Vector2(1f, 0f));
            }
            if ((CollideCheck(player, Position - Vector2.UnitX) && player.Speed.X > 0) || (HasPlayerClimbing() && player.Right <= Left))
            {
                player.Die(new Vector2(-1f, 0f));
            }
            if (CollideCheck(player, Position + Vector2.UnitY) && player.Speed.Y < 0)
            {
                player.Die(new Vector2(0f, 1f));
            }
        }

        private DashCollisionResults OnDashed(Player player, Vector2 direction)
        {
            if (canKill)
            {
                Add(new Coroutine(KillDelay()));
            }
            bool onTrackOrSolidEdge = (direction.X == 0f) ? MoveVCheck(direction.Y) : MoveHCheck(direction.X);
            if (canPush && !onTrackOrSolidEdge)
            {
                Add(new Coroutine(PushedRoutine(direction)));
                player.Dashes = playerDashes;
                SpaceJump.SetJumpBuffer(playerJumps);
                return DashCollisionResults.Rebound;
            }
            if (canKill)
            {
                KillPlayer(player);
            }
            return DashCollisionResults.NormalCollision;
        }

        private IEnumerator KillDelay()
        {
            canKill = false;
            float timer = 0.05f;
            while (timer > 0)
            {
                timer -= Engine.DeltaTime;
                yield return null;
            }
            canKill = true;
        }

        private IEnumerator PushedRoutine(Vector2 direction)
        {
            canPush = false;
            Audio.Play("event:/game/06_reflection/crushblock_activate", Center);
            Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
            ActivateParticles(direction);
            float speed = 0f;
            while (true)
            {
                speed = Calc.Approach(speed, 225f, 500f * Engine.DeltaTime);
                bool flag = (direction.X == 0f) ? MoveVCheck(direction.Y) : MoveHCheck(direction.X);
                if (flag)
                {
                    break;
                }
                if (direction.X != 0)
                {
                    MoveHCollideSolids(direction.X * speed * Engine.DeltaTime, true);
                    yield return null;
                }
                if (direction.Y != 0f)
                {
                    MoveVCollideSolids(direction.Y * speed * Engine.DeltaTime, true);
                    yield return null;
                }
                if (Scene.OnInterval(0.02f))
                {
                    Vector2 position;
                    float particleDirection;
                    if (direction == Vector2.UnitX)
                    {
                        position = new Vector2(Left + 1f, Calc.Random.Range(Top + 3f, Bottom - 3f));
                        particleDirection = (float)Math.PI;
                    }
                    else if (direction == -Vector2.UnitX)
                    {
                        position = new Vector2(Right - 1f, Calc.Random.Range(Top + 3f, Bottom - 3f));
                        particleDirection = 0f;
                    }
                    else if (direction == Vector2.UnitY)
                    {
                        position = new Vector2(Calc.Random.Range(Left + 3f, Right - 3f), Top + 1f);
                        particleDirection = -(float)Math.PI / 2f;
                    }
                    else
                    {
                        position = new Vector2(Calc.Random.Range(Left + 3f, Right - 3f), Bottom - 1f);
                        particleDirection = (float)Math.PI / 2f;
                    }
                    level.Particles.Emit(P_Move, position, particleDirection);
                }
            }
            if (direction.X != 0f)
            {
                CorrectionH(direction.X);
            }
            if (direction.Y != 0f)
            {
                CorrectionV(direction.Y);
            }
            if (direction == -Vector2.UnitX)
            {
                Vector2 vector = new(0f, 2f);
                for (int i = 0; i < Height / 8f; i++)
                {
                    Vector2 vector2 = new(Left - 1f, Top + 4f + (i * 8));
                    if (Scene.CollideCheck<Solid>(vector2))
                    {
                        SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, vector2 + vector, 0f);
                        SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, vector2 - vector, 0f);
                    }
                }
            }
            else if (direction == Vector2.UnitX)
            {
                Vector2 vector3 = new(0f, 2f);
                for (int j = 0; j < Height / 8f; j++)
                {
                    Vector2 vector4 = new(Right + 1f, Top + 4f + (j * 8));
                    if (Scene.CollideCheck<Solid>(vector4))
                    {
                        SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, vector4 + vector3, (float)Math.PI);
                        SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, vector4 - vector3, (float)Math.PI);
                    }
                }
            }
            else if (direction == -Vector2.UnitY)
            {
                Vector2 vector5 = new(2f, 0f);
                for (int k = 0; k < Width / 8f; k++)
                {
                    Vector2 vector6 = new(Left + 4f + (k * 8), Top - 1f);
                    if (Scene.CollideCheck<Solid>(vector6))
                    {
                        SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, vector6 + vector5, (float)Math.PI / 2f);
                        SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, vector6 - vector5, (float)Math.PI / 2f);
                    }
                }
            }
            else if (direction == Vector2.UnitY)
            {
                Vector2 vector7 = new(2f, 0f);
                for (int l = 0; l < Width / 8f; l++)
                {
                    Vector2 vector8 = new(Left + 4f + (l * 8), Bottom + 1f);
                    if (Scene.CollideCheck<Solid>(vector8))
                    {
                        SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, vector8 + vector7, -(float)Math.PI / 2f);
                        SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, vector8 - vector7, -(float)Math.PI / 2f);
                    }
                }
            }
            speed = 0f;
            Audio.Play("event:/game/06_reflection/crushblock_impact", Center);
            StartShaking(0.2f);
            yield return 0.2f;
            canPush = true;
        }

        private bool MoveHCheck(float direction)
        {
            if (CollideCheck<PushBlockTrack>())
            {
                if ((!Scene.CollideCheck<PushBlockTrack>(new Rectangle((int)(X - 4f), (int)Y, 1, 1)) || !Scene.CollideCheck<PushBlockTrack>(new Rectangle((int)(X - 4f), (int)(Y + Height - 1), 1, 1)) || CollideCheck<Solid>(Position - Vector2.UnitX * 4)) && direction == -1)
                {
                    return true;
                }
                if ((!Scene.CollideCheck<PushBlockTrack>(new Rectangle((int)(X + Width - 1f + 4f), (int)Y, 1, 1)) || !Scene.CollideCheck<PushBlockTrack>(new Rectangle((int)(X + Width - 1f + 4f), (int)(Y + Height - 1), 1, 1)) || CollideCheck<Solid>(Position + Vector2.UnitX * 4)) && direction == 1)
                {
                    return true;
                }
                return false;
            }
            return true;
        }

        private bool MoveVCheck(float direction)
        {
            if (CollideCheck<PushBlockTrack>())
            {
                if ((!Scene.CollideCheck<PushBlockTrack>(new Rectangle((int)X, (int)(Y - 4f), 1, 1)) || !Scene.CollideCheck<PushBlockTrack>(new Rectangle((int)(X + Width - 1), (int)(Y - 4f), 1, 1)) || CollideCheck<Solid>(Position - Vector2.UnitY * 4)) && direction == -1)
                {
                    return true;
                }
                if ((!Scene.CollideCheck<PushBlockTrack>(new Rectangle((int)X, (int)(Y + Height - 1f + 4f), 1, 1)) || !Scene.CollideCheck<PushBlockTrack>(new Rectangle((int)(X + Width - 1), (int)(Y + Height - 1f + 4f), 1, 1)) || CollideCheck<Solid>(Position + Vector2.UnitY * 4)) && direction == 1)
                {
                    return true;
                }
                return false;
            }
            return true;
        }

        private void CorrectionH(float direction)
        {
            if (Scene.CollideCheck<PushBlockTrack>(new Rectangle((int)(X - 1f), (int)Y, 1, 1)) && Scene.CollideCheck<PushBlockTrack>(new Rectangle((int)(X - 1f), (int)(Y + Height - 1), 1, 1)) && !CollideCheck<Solid>(Position - Vector2.UnitX) && direction == -1)
            {
                MoveHExact(-1);
                CorrectionH(direction);
            }
            if (Scene.CollideCheck<PushBlockTrack>(new Rectangle((int)(X + Width), (int)Y, 1, 1)) && Scene.CollideCheck<PushBlockTrack>(new Rectangle((int)(X + Width), (int)(Y + Height - 1), 1, 1)) && !CollideCheck<Solid>(Position + Vector2.UnitX) && direction == 1)
            {
                MoveHExact(1);
                CorrectionH(direction);
            }
        }

        private void CorrectionV(float direction)
        {
            if (Scene.CollideCheck<PushBlockTrack>(new Rectangle((int)X, (int)(Y - 1f), 1, 1)) && Scene.CollideCheck<PushBlockTrack>(new Rectangle((int)(X + Width - 1), (int)(Y - 1f), 1, 1)) && !CollideCheck<Solid>(Position - Vector2.UnitY) && direction == -1)
            {
                MoveVExact(-1);
                CorrectionV(direction);
            }
            if (Scene.CollideCheck<PushBlockTrack>(new Rectangle((int)X, (int)(Y + Height), 1, 1)) && Scene.CollideCheck<PushBlockTrack>(new Rectangle((int)(X + Width - 1f), (int)(Y + Height), 1, 1)) && !CollideCheck<Solid>(Position + Vector2.UnitY) && direction == 1)
            {
                MoveVExact(1);
                CorrectionV(direction);
            }
        }

        private void ActivateParticles(Vector2 dir)
        {
            float direction;
            Vector2 position;
            Vector2 positionRange;
            int num;
            if (dir == Vector2.UnitX)
            {
                direction = (float)Math.PI;
                position = CenterLeft + Vector2.UnitX;
                positionRange = Vector2.UnitY * (Height - 2f) * 0.5f;
                num = (int)(Height / 8f) * 4;
            }
            else if (dir == -Vector2.UnitX)
            {
                direction = 0f;
                position = CenterRight - Vector2.UnitX;
                positionRange = Vector2.UnitY * (Height - 2f) * 0.5f;
                num = (int)(Height / 8f) * 4;
            }
            else if (dir == Vector2.UnitY)
            {
                direction = -(float)Math.PI / 2f;
                position = TopCenter + Vector2.UnitY;
                positionRange = Vector2.UnitX * (Width - 2f) * 0.5f;
                num = (int)(Width / 8f) * 4;
            }
            else
            {
                direction = (float)Math.PI / 2f;
                position = BottomCenter - Vector2.UnitY;
                positionRange = Vector2.UnitX * (Width - 2f) * 0.5f;
                num = (int)(Width / 8f) * 4;
            }
            num += 2;
            level.Particles.Emit(P_Activate, num, position, positionRange, direction);
        }

        public override void Render()
        {
            base.Render();
            float opacity = (0.9f * (0.9f + ((float)Math.Sin(alpha) + 1f) * 0.125f));
            float num = Collider.Width / 8f - 1f;
            float num2 = Collider.Height / 8f - 1f;
            for (int i = 0; i <= num; i++)
            {
                for (int j = 0; j <= num2; j++)
                {
                    int num3 = ((i < num) ? Math.Min(i, 1) : 2);
                    int num4 = ((j < num2) ? Math.Min(j, 1) : 2);

                    if (mustDash)
                    {
                        dashNineSlice[num3, num4].Draw(Position + Shake + new Vector2(i * 8, j * 8));
                        glowNineSlice[num3, num4].Draw(Position + Shake + new Vector2(i * 8, j * 8), Vector2.Zero, Color.White * opacity);
                        glowDashNineSlice[num3, num4].Draw(Position + Shake + new Vector2(i * 8, j * 8), Vector2.Zero, Color.White * opacity);
                    }
                    else
                    {
                        nineSlice[num3, num4].Draw(Position + Shake + new Vector2(i * 8, j * 8));
                        glowNineSlice[num3, num4].Draw(Position + Shake + new Vector2(i * 8, j * 8), Vector2.Zero, Color.White * opacity);
                    }
                }
            }
        }
    }
}
