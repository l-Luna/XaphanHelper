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

        public WorkRobot(EntityData data, Vector2 offset) : base(data.Position + offset, 10f, 17f, false)
        {
            OnDashCollide = OnDashed;
            Collider = new Hitbox(10f, 17f, -5f, -1f);
            Add(sprite = new Sprite(GFX.Game, "enemies/Xaphan/WorkRobot/"));
            sprite.Position = new Vector2(-8);
            sprite.AddLoop("idle", "idle", 0.08f);
            sprite.Play("idle");
            goLeft = true;
        }

        public override void Update()
        {
            base.Update();
            if (!pushed)
            {
                if (goLeft && Scene.CollideCheck<Solid>(new Rectangle((int)Position.X - 6, (int)(Position.Y + 18), 1, 1)) && !CollideCheck<Solid>(Position + Vector2.UnitX * -1f))
                {
                    Speed.X = -20;
                }
                else if (!goLeft && Scene.CollideCheck<Solid>(new Rectangle((int)Position.X + 5, (int)(Position.Y + 18), 1, 1)) && !CollideCheck<Solid>(Position + Vector2.UnitX * 1f))
                {
                    Speed.X = 20;
                }
                else
                {
                    Speed.X = 0f;
                }
                if (goLeft && (!Scene.CollideCheck<Solid>(new Rectangle((int)Position.X - 6, (int)(Position.Y + 18), 1, 1)) || CollideCheck<Solid>(Position + Vector2.UnitX * -1f)))
                {
                    goLeft = false;
                }
                else if (!goLeft && (!Scene.CollideCheck<Solid>(new Rectangle((int)Position.X + 5, (int)(Position.Y + 18), 1, 1)) || CollideCheck<Solid>(Position + Vector2.UnitX * 1f)))
                {
                    goLeft = true;
                }
                sprite.FlipX = !goLeft;
            }
            if (!CollideCheck<Solid>(Position + Vector2.UnitY))
            {
                float num = 800f * 1;
                if (Math.Abs(Speed.Y) <= 30f)
                {
                    num *= 0.5f;
                }
                Speed.Y = Calc.Approach(Speed.Y, 200f * 1, num * Engine.DeltaTime);
            }
            else
            {
                Speed.Y = 0f;
            }
        }

        private DashCollisionResults OnDashed(Player player, Vector2 direction)
        {
            if (direction.X != 0 && direction.Y == 0)
            {
                pushed = true;
                Add(new Coroutine(PushedRoutine(direction)));
                return DashCollisionResults.Rebound;
            }
            return DashCollisionResults.NormalCollision;
        }

        private IEnumerator PushedRoutine(Vector2 direction)
        {
            Speed.X = 20 * 4 * direction.X;
            while (Speed.X > 20 || Speed.X < -20)
            {
                if (Speed.X > 0)
                {
                    Speed.X -= Engine.DeltaTime * 20;
                }
                else
                {
                    Speed.X += Engine.DeltaTime * 20;
                }
                if (CollideCheck<Solid>(Position + Vector2.UnitX * direction.X))
                {
                    Speed.X = 0f;
                    yield return 0.3f;
                }
                yield return null;
            }
            pushed = false;
        }
    }
}
