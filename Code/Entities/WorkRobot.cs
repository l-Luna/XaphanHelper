using System;
using System.Collections;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

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

        private bool active;

        private string flag;

        private string directory;

        public WorkRobot(EntityData data, Vector2 offset) : base(data.Position + offset, 10f, 17f, false)
        {
            Tag = Tags.TransitionUpdate;
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
            sprite.AddLoop("walk", "walk", 0.1f);
            sprite.Add("turn", "turn", 0.08f);
            sprite.Add("activate", "turn", 0.08f, 7, 7, 6, 5, 4, 3, 2, 1, 0);
            sprite.Add("deactivate", "turn", 0.08f, 0, 1, 2, 3, 4, 5, 6, 7, 7);
            goLeft = sprite.FlipX = data.Bool("startWalkLeft", false);
            Add(new ClimbBlocker(edge: true));
            sprite.OnFrameChange = onFrameChange;
            sprite.Play("walk");
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            if (!string.IsNullOrEmpty(flag))
            {
                if (SceneAs<Level>().Session.GetFlag(flag))
                {
                    Add(new Coroutine(Activate()));
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

        public override void Update()
        {
            base.Update();
            if (ActiveRoutine.Active && !pushed)
            {
                Speed.X = 0;
            }
            if (!TurnAroundRoutine.Active && !ActiveRoutine.Active && active && (!string.IsNullOrEmpty(flag) ? SceneAs<Level>().Session.GetFlag(flag) : true))
            {
                sprite.FlipX = goLeft;
                if (!pushed && CollideCheck<Solid>(Position + Vector2.UnitY))
                {
                    Rectangle LeftHit = size == "Small" ? new Rectangle((int)Position.X + 2, (int)Position.Y + 24, 1, 1) : size == "Medium" ? new Rectangle((int)Position.X - 1, (int)Position.Y + 24, 1, 1) : new Rectangle((int)Position.X - 3, (int)Position.Y + 24, 1, 1);
                    Rectangle RightHit = size == "Small" ? new Rectangle((int)Position.X + 13, (int)Position.Y + 24, 1, 1) : size == "Medium" ? new Rectangle((int)Position.X + 15, (int)Position.Y + 24, 1, 1) : new Rectangle((int)Position.X + 18, (int)Position.Y + 24, 1, 1);
                    if (goLeft && Scene.CollideCheck<Solid>(LeftHit) && !CollideCheck<Solid>(Position + Vector2.UnitX * -1f))
                    {
                        Speed.X = -speed;
                    }
                    else if (!goLeft && Scene.CollideCheck<Solid>(RightHit) && !CollideCheck<Solid>(Position + Vector2.UnitX * 1f))
                    {
                        Speed.X = speed;
                    }
                    else
                    {
                        Speed.X = 0f;
                    }
                    if (goLeft && (!Scene.CollideCheck<Solid>(LeftHit) || CollideCheck<Solid>(Position + Vector2.UnitX * -1f)))
                    {
                        goLeft = false;
                        Add(TurnAroundRoutine = new Coroutine(TurnAround()));
                    }
                    else if (!goLeft && (!Scene.CollideCheck<Solid>(RightHit) || CollideCheck<Solid>(Position + Vector2.UnitX * 1f)))
                    {
                        goLeft = true;
                        Add(TurnAroundRoutine = new Coroutine(TurnAround()));
                    }
                }
            }
            if (!string.IsNullOrEmpty(flag) && !TurnAroundRoutine.Active && !ActiveRoutine.Active)
            {
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
            if (CollideCheck<Solid>(Position))
            {
                Rectangle LeftHit = size == "Small" ? new Rectangle((int)Position.X + 1, (int)Position.Y + 7, 1, height) : size == "Medium" ? new Rectangle((int)Position.X - 1, (int)Position.Y - 2, 1, height) : new Rectangle((int)Position.X - 3, (int)Position.Y - 11, 1, height);
                Rectangle RightHit = size == "Small" ? new Rectangle((int)Position.X + 13, (int)Position.Y + 7, 1, height) : size == "Medium" ? new Rectangle((int)Position.X + 15, (int)Position.Y - 2, 1, height) : new Rectangle((int)Position.X + 18, (int)Position.Y - 11, 1, height);
                Rectangle BottomHit = size == "Small" ? new Rectangle((int)Position.X + 4, (int)Position.Y + 24, width - 2, 1) : size == "Medium" ? new Rectangle((int)Position.X + 1, (int)Position.Y + 24, width - 2, 1) : new Rectangle((int)Position.X - 1, (int)Position.Y + 24, width - 2, 1);
                if (Scene.CollideCheck<Solid>(LeftHit))
                {
                    MoveH(1);
                }
                else if (Scene.CollideCheck<Solid>(RightHit))
                {
                    MoveH(-1);
                }
                if (Scene.CollideCheck<Solid>(BottomHit))
                {
                    MoveV(-1);
                }
            }
            if (!CollideCheck<Solid>(Position + Vector2.UnitY))
            {
                float num = 800f;
                if (Math.Abs(Speed.Y) <= 30f)
                {
                    num *= 0.5f;
                }
                Speed.Y = Calc.Approach(Speed.Y, 200f , num * Engine.DeltaTime);
            }
            else
            {
                if (Speed.Y > 0)
                {
                    Speed.X = 0;
                }
                Speed.Y = 0f;
            }
        }

        private IEnumerator TurnAround()
        {
            if (!CollideCheck<Solid>(Position + Vector2.UnitY) || (Speed.Y == 0 && CollideCheck<Solid>(Position + Vector2.UnitY)))
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
                        MoveV(1, 0);
                        yield return 0.08f;
                    }
                    if (sprite.CurrentAnimationFrame == 9 && (!string.IsNullOrEmpty(flag) ? !SceneAs<Level>().Session.GetFlag(flag) : true))
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
                    Logger.Log(LogLevel.Info, "XH", "Frame : " + sprite.CurrentAnimationFrame);
                    Collider.Height -= 1;
                    sprite.Position.Y -= 1;
                    MoveV(1, 0);
                }
            }
            else if(sprite.CurrentAnimationID == "activate")
            {
                int value = size == "Small" ? 2 : size == "Medium" ? 3 : 4;
                if (sprite.CurrentAnimationFrame >= 2 && sprite.CurrentAnimationFrame < 2 + value)
                {
                    Logger.Log(LogLevel.Info, "XH", "Frame : " + sprite.CurrentAnimationFrame);
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
            Add(new Coroutine(PushedRoutine(force, direction)));
        }

        private IEnumerator PushedRoutine(Vector2 force, Vector2 direction)
        {
            Speed.X = force.X * direction.X;
            Speed.Y = force.Y;
            if (sprite.CurrentAnimationID == "walk")
            {
                sprite.Rate = 2.5f;
            }
            while (Speed.X > speed || Speed.X < -speed)
            {
                if (Speed.X > 0)
                {
                    Speed.X -= Engine.DeltaTime * 300;
                }
                else
                {
                    Speed.X += Engine.DeltaTime * 300;
                }
                if (CollideCheck<Solid>(Position + Vector2.UnitX * direction.X))
                {
                    Speed.X = 0f;
                    yield return 0.3f;
                }
                yield return null;
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
