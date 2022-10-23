using Microsoft.Xna.Framework;
using Monocle;
using System;
using On.Celeste;
using System.Collections.Generic;
using System.Reflection;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [Tracked(true)]
    public class MovableEntity : Entity
    {
        public bool IgnoreJumpThrus;

        private Vector2 movementCounter;

        public Vector2 ExactPosition => Position + movementCounter;

        public float LiftSpeedGraceTime = 0.16f;

        private Vector2 currentLiftSpeed;

        private Vector2 lastLiftSpeed;

        private float liftSpeedTimer;

        public Vector2 LiftSpeed
        {
            get
            {
                if (currentLiftSpeed == Vector2.Zero)
                {
                    return lastLiftSpeed;
                }
                return currentLiftSpeed;
            }
            set
            {
                currentLiftSpeed = value;
                if (value != Vector2.Zero && LiftSpeedGraceTime > 0f)
                {
                    lastLiftSpeed = value;
                    liftSpeedTimer = LiftSpeedGraceTime;
                }
            }
        }

        public MovableEntity(Vector2 position) : base(position)
        {

        }

        private bool IsRiding(MovableEntity movableEntity)
        {
            return CollideCheck(movableEntity, Position + Vector2.UnitY);
        }

        public override void Update()
        {
            base.Update();
            LiftSpeed = Vector2.Zero;
            if (liftSpeedTimer > 0f)
            {
                liftSpeedTimer -= Engine.DeltaTime;
                if (liftSpeedTimer <= 0f)
                {
                    lastLiftSpeed = Vector2.Zero;
                }
            }
        }

        public void ResetLiftSpeed()
        {
            currentLiftSpeed = (lastLiftSpeed = Vector2.Zero);
            liftSpeedTimer = 0f;
        }

        public virtual bool IsRiding(JumpThru jumpThru)
        {
	        if (IgnoreJumpThrus)
	        {
		        return false;
	        }
	        return CollideCheckOutside(jumpThru, Position + Vector2.UnitY);
        }

	    public virtual bool IsRiding(Solid solid)
	    {
		    return CollideCheck(solid, Position + Vector2.UnitY);
	    }

        public bool OnGround(int downCheck = 1)
        {
            if (!CollideCheck<Solid>(Position + Vector2.UnitY * downCheck))
            {
                if (!IgnoreJumpThrus)
                {
                    return CollideCheckOutside<JumpThru>(Position + Vector2.UnitY * downCheck);
                }
                return false;
            }
            return true;
        }

        public bool OnGround(Vector2 at, int downCheck = 1)
        {
            Vector2 position = Position;
            Position = at;
            bool result = OnGround(downCheck);
            Position = position;
            return result;
        }

        public bool MoveH(float moveH, Collision onCollide = null, Solid pusher = null)
        {
            movementCounter.X += moveH;
            int num = (int)System.Math.Round(movementCounter.X, MidpointRounding.ToEven);
            if (num != 0)
            {
                movementCounter.X -= num;
                return MoveHExact(num, onCollide, pusher);
            }
            return false;
        }

        public bool MoveV(float moveV, Collision onCollide = null, Solid pusher = null)
        {
            movementCounter.Y += moveV;
            int num = (int)Math.Round(movementCounter.Y, MidpointRounding.ToEven);
            if (num != 0)
            {
                movementCounter.Y -= num;
                return MoveVExact(num, onCollide, pusher);
            }
            return false;
        }

        public bool MoveHExact(int moveH, Collision onCollide = null, Solid pusher = null)
        {
            Vector2 targetPosition = Position + Vector2.UnitX * moveH;
            int num = Math.Sign(moveH);
            int num2 = 0;
            while (moveH != 0)
            {
                Solid solid = CollideFirst<Solid>(Position + Vector2.UnitX * num);
                if (solid != null)
                {
                    movementCounter.X = 0f;
                    onCollide?.Invoke(new CollisionData
                    {
                        Direction = Vector2.UnitX * num,
                        Moved = Vector2.UnitX * num2,
                        TargetPosition = targetPosition,
                        Hit = solid,
                        Pusher = pusher
                    });
                    return true;
                }
                num2 += num;
                moveH -= num;
                base.X += num;
            }
            return false;
        }

        public bool MoveVExact(int moveV, Collision onCollide = null, Solid pusher = null)
        {
            Vector2 targetPosition = Position + Vector2.UnitY * moveV;
            int num = Math.Sign(moveV);
            int num2 = 0;
            while (moveV != 0)
            {
                Platform platform = CollideFirst<Solid>(Position + Vector2.UnitY * num);
                if (platform != null)
                {
                    movementCounter.Y = 0f;
                    onCollide?.Invoke(new CollisionData
                    {
                        Direction = Vector2.UnitY * num,
                        Moved = Vector2.UnitY * num2,
                        TargetPosition = targetPosition,
                        Hit = platform,
                        Pusher = pusher
                    });
                    return true;
                }
                if (moveV > 0 && !IgnoreJumpThrus)
                {
                    platform = CollideFirstOutside<JumpThru>(Position + Vector2.UnitY * num);
                    if (platform != null)
                    {
                        movementCounter.Y = 0f;
                        onCollide?.Invoke(new CollisionData
                        {
                            Direction = Vector2.UnitY * num,
                            Moved = Vector2.UnitY * num2,
                            TargetPosition = targetPosition,
                            Hit = platform,
                            Pusher = pusher
                        });
                        return true;
                    }
                }
                num2 += num;
                moveV -= num;
                base.Y += num;
            }
            return false;
        }

        public void MoveTowardsX(float targetX, float maxAmount, Collision onCollide = null)
        {
            float toX = Calc.Approach(ExactPosition.X, targetX, maxAmount);
            MoveToX(toX, onCollide);
        }

        public void MoveTowardsY(float targetY, float maxAmount, Collision onCollide = null)
        {
            float toY = Calc.Approach(ExactPosition.Y, targetY, maxAmount);
            MoveToY(toY, onCollide);
        }

        public void MoveToX(float toX, Collision onCollide = null)
        {
            MoveH(toX - ExactPosition.X, onCollide);
        }

        public void MoveToY(float toY, Collision onCollide = null)
        {
            MoveV(toY - ExactPosition.Y, onCollide);
        }

        public void NaiveMove(Vector2 amount)
        {
            movementCounter += amount;
            int num = (int)Math.Round(movementCounter.X);
            int num2 = (int)Math.Round(movementCounter.Y);
            Position += new Vector2(num, num2);
            movementCounter -= new Vector2(num, num2);
        }
    }
}
