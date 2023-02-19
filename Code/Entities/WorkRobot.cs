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

        public WorkRobot(EntityData data, Vector2 offset) : base(data.Position + offset, 10f, 17f, false)
        {
            SurfaceSoundIndex = 7;
            size = data.Attr("size", "Medium");
            OnDashCollide = OnDashed;
            width = size == "Small" ? 10 : size == "Medium" ? 15 : 20;
            height = size == "Small" ? 17 : size == "Medium" ? 26 : 35;
            float x = size == "Small" ? 3f : size == "Medium" ? 0f : -2f;
            float y = size == "Small" ? 7f : size == "Medium" ? -2f : -11f;
            Collider = new Hitbox(width, height, x, y);
            Add(sprite = new Sprite(GFX.Game, "enemies/Xaphan/WorkRobot-" + size + "/"));
            sprite.Position = new Vector2(-8, -16);
            sprite.AddLoop("idle", "idle", 0f);
            sprite.AddLoop("walk", "walk", 0.1f);
            sprite.Add("turn", "turn", 0.08f);
            sprite.Play("walk");
            goLeft = sprite.FlipX = data.Bool("startWalkLeft", false);
            speed = 20;
            Add(new ClimbBlocker(edge: true));
        }

        public override void Update()
        {
            base.Update();
            if (!TurnAroundRoutine.Active)
            {
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
            if (CollideCheck<Solid>(Position))
            {
                Rectangle LeftHit = size == "Small" ? new Rectangle((int)Position.X + 1, (int)Position.Y + 7, 1, height) : size == "Medium" ? new Rectangle((int)Position.X - 1, (int)Position.Y - 2, 1, height) : new Rectangle((int)Position.X - 3, (int)Position.Y - 11, 1, height);
                Rectangle RightHit = size == "Small" ? new Rectangle((int)Position.X + 13, (int)Position.Y + 7, 1, height) : size == "Medium" ? new Rectangle((int)Position.X + 15, (int)Position.Y - 2, 1, height) : new Rectangle((int)Position.X + 18, (int)Position.Y - 11, 1, height);
                Rectangle BottomHit = size == "Small" ? new Rectangle((int)Position.X + 4, (int)Position.Y + 24, width - 2, 1) : size == "Medium" ? new Rectangle((int)Position.X + 1, (int)Position.Y + 24, width - 2, 1) : new Rectangle((int)Position.X - 1, (int)Position.Y + 24, width - 2, 1);
                if (Scene.CollideCheck<Solid>(LeftHit))
                {
                    MoveH(1);
                }
                else if (Scene.CollideCheck<Solid>(RightHit) && !TurnAroundRoutine.Active)
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
                    if (sprite.CurrentAnimationFrame >= upFirstFrame && sprite.CurrentAnimationFrame <= upLastFrame)
                    {
                        Collider.Height += 1;
                        sprite.Position.Y += 1;
                        MoveV(-1, 0);
                        yield return 0.08f;
                    }
                    yield return null;
                }
                Collider.Height = Height;
                sprite.FlipX = goLeft;
                sprite.Play("walk");
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
