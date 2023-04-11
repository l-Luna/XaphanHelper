using System;
using System.Collections;
using System.Reflection;
using Celeste.Mod.Entities;
using Celeste.Mod.XaphanHelper.Colliders;
using Microsoft.Xna.Framework;
using Monocle;
using static Celeste.GaussianBlur;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [Tracked(true)]
    [CustomEntity("XaphanHelper/WorkRobot")]
    public class WorkRobot : Solid
    {
        Sprite sprite;

        private bool goLeft;

        private bool pushed;

        private float speed;

        private string size;

        private int width;

        private int height;

        private Coroutine TurnAroundRoutine = new();

        private Coroutine ActiveRoutine = new();

        private Coroutine PushRoutine = new();

        private bool active;

        private string flag;

        private string directory;

        private float SpeedH;

        private float SpeedV;

        private float cancelYPos = 0f;

        public WorkRobot(EntityData data, Vector2 offset) : base(data.Position + offset, 10f, 17f, false)
        {
            AllowStaticMovers = false;
            SurfaceSoundIndex = 7;
            size = data.Attr("size", "Medium");
            flag = data.Attr("flag");
            speed = data.Int("speed", 20);
            directory = data.Attr("directory");
            if (string.IsNullOrEmpty(directory))
            {
                directory = "objects/XaphanHelper/WorkRobot";
            }
            OnDashCollide = OnDashed;
            width = size == "Small" ? 10 : size == "Medium" ? 15 : 20;
            height = size == "Small" ? 17 : size == "Medium" ? 26 : 35;
            float x = size == "Small" ? 3f : size == "Medium" ? 0f : -2f;
            float y = size == "Small" ? 7f : size == "Medium" ? -2f : -11f;
            Collider = new Hitbox(width, height, x, y);
            Add(sprite = new Sprite(GFX.Game, directory + "/" + size + "/"));
            sprite.Position = new Vector2(-8, -16);
            sprite.AddLoop("idle", "turn", 0f, 7);
            sprite.AddLoop("walk", "walk", 0.1f / Math.Min(2, speed / 20));
            sprite.Add("turn", "turn", 0.08f);
            sprite.Add("activate", "turn", 0.08f, 7, 7, 6, 5, 4, 3, 2, 1, 0);
            sprite.Add("deactivate", "turn", 0.08f, 0, 1, 2, 3, 4, 5, 6, 7, 7);
            sprite.Add("stun", "turn", 0.08f, 0, 1, 2, 3, 4, 5, 4, 3, 2, 1, 0);
            goLeft = sprite.FlipX = data.Bool("startWalkLeft", false);
            Add(new ClimbBlocker(edge: true));
            sprite.OnFrameChange = onFrameChange;
            sprite.Play("walk");
            Depth = -1;
            Add(new SpringCollider(OnSpring, Collider));
        }

        public static void Load()
        {
            On.Monocle.Entity.Update += OnEntityUpdate;
        }

        public static void Unload()
        {
            On.Monocle.Entity.Update -= OnEntityUpdate;
        }

        private static void OnEntityUpdate(On.Monocle.Entity.orig_Update orig, Entity self)
        {
            orig(self);
            if (self is Spring)
            {
                Spring spring = self as Spring;
                foreach (SpringCollider springCollider in spring.Scene.Tracker.GetComponents<SpringCollider>())
                {
                    springCollider.Check(spring);
                }
            }
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            if (!string.IsNullOrEmpty(flag))
            {
                if (SceneAs<Level>().Session.GetFlag(flag))
                {
                    active = true;
                }
                else
                {
                    Add(ActiveRoutine = new Coroutine(DeActivate(true)));
                }
            }
            else
            {
                active = true;
            }
        }

        private int conveyorSpeed;
        private int conveyorDir;

        public override void Update()
        {
            base.Update();
            MoveHCollideSolids(SpeedH * Engine.DeltaTime, false);
            MoveVCollideSolids(SpeedV * Engine.DeltaTime, false);
            if (ActiveRoutine.Active && !pushed)
            {
                SpeedH = 0;
            }
            Rectangle BottomHit = size == "Small" ? new Rectangle((int)Position.X + 4, (int)Position.Y + 24 - (active ? 0 : 2), width - 2, 1) : size == "Medium" ? new Rectangle((int)Position.X + 1, (int)Position.Y + 24 - (active ? 0 : 3), width - 2, 1) : new Rectangle((int)Position.X - 1, (int)Position.Y + 24 - (active ? 0 : 4), width - 2, 1);
            Rectangle LeftHit = size == "Small" ? new Rectangle((int)Position.X + 2, (int)Position.Y + 24, 1, 1) : size == "Medium" ? new Rectangle((int)Position.X - 1, (int)Position.Y + 24, 1, 1) : new Rectangle((int)Position.X - 3, (int)Position.Y + 24, 1, 1);
            Rectangle RightHit = size == "Small" ? new Rectangle((int)Position.X + 13, (int)Position.Y + 24, 1, 1) : size == "Medium" ? new Rectangle((int)Position.X + 15, (int)Position.Y + 24, 1, 1) : new Rectangle((int)Position.X + 18, (int)Position.Y + 24, 1, 1);
            foreach (Conveyor conveyor in SceneAs<Level>().Tracker.GetEntities<Conveyor>())
            {
                if (CollideCheck(conveyor, BottomCenter))
                {
                    conveyorSpeed = conveyor.conveyorSpeed;
                    conveyorDir = conveyor.direction;
                    break;
                }
            }
            int conveyorSpeedAdjust = conveyorSpeed * conveyorDir;
            if (!TurnAroundRoutine.Active && !ActiveRoutine.Active && active && (!string.IsNullOrEmpty(flag) ? SceneAs<Level>().Session.GetFlag(flag) : true))
            {
                sprite.FlipX = goLeft;
                if (!pushed && (Scene.CollideCheck<Solid>(BottomHit) || Scene.CollideCheck<JumpThru>(BottomHit)))
                {
                    if (goLeft && (Scene.CollideCheck<Solid>(LeftHit) || Scene.CollideCheck<JumpThru>(LeftHit)) && !CollideCheck<Solid>(Position + Vector2.UnitX * -1f))
                    {
                        SpeedH = -speed + conveyorSpeedAdjust;
                    }
                    else if (!goLeft && (Scene.CollideCheck<Solid>(RightHit) || Scene.CollideCheck<JumpThru>(RightHit)) && !CollideCheck<Solid>(Position + Vector2.UnitX * 1f))
                    {
                        SpeedH = speed + conveyorSpeedAdjust;
                    }
                    else
                    {
                        SpeedH = conveyorSpeedAdjust;
                        if ((goLeft && ((!Scene.CollideCheck<Solid>(LeftHit) && !Scene.CollideCheck<JumpThru>(LeftHit)) || CollideCheck<Solid>(Position + Vector2.UnitX * -1f))) || (!goLeft && ((!Scene.CollideCheck<Solid>(RightHit) && !Scene.CollideCheck<JumpThru>(RightHit)) || CollideCheck<Solid>(Position + Vector2.UnitX * 1f))))
                        {
                            goLeft = !goLeft;
                            Add(TurnAroundRoutine = new Coroutine(TurnAround()));
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(flag) && !TurnAroundRoutine.Active && !ActiveRoutine.Active)
            {
                if ((!pushed || (pushed && sprite.CurrentAnimationID == "stun")) && (Scene.CollideCheck<Solid>(BottomHit) || Scene.CollideCheck<JumpThru>(BottomHit)))
                {
                    if ((goLeft && (Scene.CollideCheck<Solid>(LeftHit) || Scene.CollideCheck<JumpThru>(LeftHit)) && !CollideCheck<Solid>(Position + Vector2.UnitX * -1f)) || (!goLeft && (Scene.CollideCheck<Solid>(RightHit) || Scene.CollideCheck<JumpThru>(RightHit)) && !CollideCheck<Solid>(Position + Vector2.UnitX * 1f)))
                    {
                        SpeedH = conveyorSpeedAdjust;
                    }
                }
                if (!active)
                {
                    if (SceneAs<Level>().Session.GetFlag(flag))
                    {
                        Add(ActiveRoutine = new Coroutine(Activate()));
                    }
                }
                else
                {
                    if (!SceneAs<Level>().Session.GetFlag(flag))
                    {
                        if (TurnAroundRoutine.Active)
                        {
                            TurnAroundRoutine.Cancel();
                        }
                        Add(ActiveRoutine = new Coroutine(DeActivate()));
                    }
                }
            }
            if (!Scene.CollideCheck<Solid>(BottomHit) && !Scene.CollideCheck<JumpThru>(BottomHit))
            {
                float num = 800f;
                if (Math.Abs(SpeedV) <= 30f)
                {
                    num *= 0.5f;
                }
                SpeedV = Calc.Approach(SpeedV, 200f, num * Engine.DeltaTime);
            }
            else
            {
                if (SpeedV > 0)
                {
                    SpeedH = 0;
                }
                SpeedV = 0f;
            }
        }

        private void OnSpring(Spring spring)
        {
            if (spring.Orientation == Spring.Orientations.Floor)
            {
                Spring_BounceAnimate.Invoke(spring, null);
                Bottom = spring.Top;
                Push(new Vector2(active ? (goLeft ? -100 : 100) : 0, -240), goLeft ? -Vector2.UnitX : Vector2.UnitX);
            }
            else if (spring.Orientation == Spring.Orientations.WallLeft)
            {
                Spring_BounceAnimate.Invoke(spring, null);
                Push(new Vector2(200, -180), Vector2.UnitX);
            }
            else if (spring.Orientation == Spring.Orientations.WallRight)
            {
                Spring_BounceAnimate.Invoke(spring, null);
                Push(new Vector2(200, -180), -Vector2.UnitX);
            }
        }

        private static MethodInfo Spring_BounceAnimate = typeof(Spring).GetMethod("BounceAnimate", BindingFlags.Instance | BindingFlags.NonPublic);

        private IEnumerator TurnAround()
        {
            if (!CollideCheck<Solid>(Position + Vector2.UnitY) || (SpeedV == 0 && CollideCheck<Solid>(Position + Vector2.UnitY)))
            {
                sprite.Play("turn");
                int downFirstFrame = size == "Small" ? 6 : size == "Medium" ? 5 : 4;
                int downLastFrame = size == "Small" ? 7 : size == "Medium" ? 7 : 7;
                int upFirstFrame = size == "Small" ? 11 : size == "Medium" ? 11 : 12;
                int upLastFrame = size == "Small" ? 12 : size == "Medium" ? 14 : 15;
                while (sprite.CurrentAnimationFrame != sprite.CurrentAnimationTotalFrames - 1)
                {
                    if (sprite.CurrentAnimationFrame >= downFirstFrame && sprite.CurrentAnimationFrame <= downLastFrame)
                    {
                        Collider.Height -= 1;
                        sprite.Position.Y -= 1;
                        cancelYPos += 1;
                        MoveV(1, 0);
                        yield return 0.08f;
                    }
                    if (sprite.CurrentAnimationFrame == 9 && (!string.IsNullOrEmpty(flag) ? !SceneAs<Level>().Session.GetFlag(flag) : false))
                    {
                        active = false;
                        sprite.Play("idle");
                        sprite.FlipX = goLeft;
                        yield break;
                    }
                    if (sprite.CurrentAnimationFrame >= upFirstFrame && sprite.CurrentAnimationFrame <= upLastFrame)
                    {
                        Collider.Height += 1;
                        sprite.Position.Y += 1;
                        cancelYPos -= 1;
                        MoveV(-1, 0);
                        yield return 0.08f;
                    }
                    yield return null;
                }
                sprite.Play("walk");
                sprite.FlipX = goLeft;
            }
        }

        private IEnumerator Activate()
        {
            sprite.Play("activate");
            while (sprite.CurrentAnimationFrame <= sprite.CurrentAnimationTotalFrames - 1)
            {
                yield return null;
            }
            active = true;
            sprite.Play("walk");
        }

        private IEnumerator DeActivate(bool immediate = false)
        {
            sprite.Play("deactivate");
            int value = size == "Small" ? 2 : size == "Medium" ? 3 : 4;
            if (immediate)
            {
                Collider.Height -= value;
                sprite.Position.Y -= value;
                MoveV(value, 0);
            }
            else
            {
                while (sprite.CurrentAnimationFrame <= sprite.CurrentAnimationTotalFrames - 1)
                {
                    yield return null;
                }
            }
            active = false;
            sprite.Play("idle");
        }

        private void onFrameChange(string s)
        {
            if (sprite.CurrentAnimationID == "deactivate")
            {
                int value = size == "Small" ? 2 : size == "Medium" ? 3 : 4;
                if (sprite.CurrentAnimationFrame >= sprite.CurrentAnimationTotalFrames - value - 1)
                {
                    Collider.Height -= 1;
                    sprite.Position.Y -= 1;
                    MoveV(1, 0);
                }
            }
            else if (sprite.CurrentAnimationID == "activate")
            {
                int value = size == "Small" ? 2 : size == "Medium" ? 3 : 4;
                if (sprite.CurrentAnimationFrame >= 2 && sprite.CurrentAnimationFrame < 2 + value)
                {
                    Collider.Height += 1;
                    sprite.Position.Y += 1;
                    MoveV(-1, 0);
                }
            }
        }

        private DashCollisionResults OnDashed(Player player, Vector2 direction)
        {
            if (direction.X != 0 && direction.Y == 0)
            {
                Push(new Vector2(150, -75), direction);
                return DashCollisionResults.Rebound;
            }
            return DashCollisionResults.NormalCollision;
        }

        public void Push(Vector2 force, Vector2 direction)
        {
            pushed = true;
            if (PushRoutine.Active)
            {
                PushRoutine.Cancel();
            }
            if (TurnAroundRoutine.Active)
            {
                TurnAroundRoutine.Cancel();
                if (cancelYPos != 0)
                {
                    Collider.Height += cancelYPos;
                    sprite.Position.Y += cancelYPos;
                    MoveV(-cancelYPos, 0);                    
                    cancelYPos = 0f;
                }
            }
            Add(PushRoutine = new Coroutine(PushedRoutine(force, direction)));
        }

        private IEnumerator PushedRoutine(Vector2 force, Vector2 direction)
        {
            SpeedH = force.X * direction.X;
            SpeedV = force.Y;
            if (active)
            {
                sprite.Play("walk");
                sprite.Rate = 2.5f;
            }
            while (SpeedH > speed || SpeedH < -speed)
            {
                if (SpeedH > 0)
                {
                    SpeedH -= Engine.DeltaTime * 300;
                }
                else
                {
                    SpeedH += Engine.DeltaTime * 300;
                }
                if (CollideCheck<Solid>(Position + Vector2.UnitX * direction.X))
                {
                    SpeedH = 0f;
                    yield return 0.3f;
                }
                yield return null;
            }
            int conveyorSpeedAdjust = conveyorSpeed * conveyorDir;
            if (active)
            {
                float stunTimer = 1f;
                SpeedH = 50 * direction.X;
                while (stunTimer > 0.5f)
                {
                    if (SpeedH > 0)
                    {
                        SpeedH -= Engine.DeltaTime * 100;
                    }
                    else
                    {
                        SpeedH += Engine.DeltaTime * 100;
                    }
                    stunTimer -= Engine.DeltaTime;
                    yield return null;
                }
                sprite.Play("stun");
                SpeedH = conveyorSpeedAdjust;
                while (stunTimer > 0)
                {
                    stunTimer -= Engine.DeltaTime;
                    yield return null;
                }
                sprite.Play("walk");
            }
            else
            {
                SpeedH = conveyorSpeedAdjust;
            }
            pushed = false;
            sprite.Rate = 1f;
        }

        public override void Render()
        {
            sprite.DrawOutline(Color.Black);
            base.Render();
        }
    }
}
