using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [Pooled]
    [Tracked(true)]
    public class DroneDebris : Actor
    {
        public static ParticleType P_Dust;

        private Image image;

        private float percent;

        private float duration;

        private Vector2 speed;

        private Collision collideH;

        private Collision collideV;

        private Color color;

        public DroneDebris() : base(Vector2.Zero)
        {
            Depth = -9990;
            Collider = new Hitbox(2f, 2f, -1f, -1f);
            collideH = OnCollideH;
            collideV = OnCollideV;
            image = new Image(GFX.Game["particles/shard"]);
            image.CenterOrigin();
            Add(image);
            P_Dust = new ParticleType
            {
                Color = Calc.HexToColor("D17A45"),
                Color2 = Calc.HexToColor("9C2632"),
                ColorMode = ParticleType.ColorModes.Blink,
                FadeMode = ParticleType.FadeModes.Late,
                LifeMin = 0.25f,
                LifeMax = 0.6f,
                Size = 1f,
                SpeedMin = 2f,
                SpeedMax = 8f,
                DirectionRange = (float)Math.PI * 2f
            };
        }

        private void Init(Vector2 position, Color color)
        {
            Position = position;
            image.Color = (this.color = color);
            image.Scale = Vector2.One;
            percent = 0f;
            duration = Calc.Random.Range(0.8f, 1.6f);
            speed = Calc.AngleToVector(Calc.Random.NextAngle(), Calc.Random.Range(40, 120));
        }

        public override void Update()
        {
            base.Update();
            speed.X = Calc.Clamp(speed.X, -100000f, 100000f);
            speed.Y = Calc.Clamp(speed.Y, -100000f, 100000f);
            if (percent > 1f)
            {
                RemoveSelf();
                return;
            }
            percent += Engine.DeltaTime / duration;
            speed.X = Calc.Approach(speed.X, 0f, Engine.DeltaTime * 20f);
            speed.Y += 200f * Engine.DeltaTime;
            if (speed.Length() > 0f)
            {
                image.Rotation = speed.Angle();
            }
            image.Scale = Vector2.One * Calc.ClampedMap(percent, 0.8f, 1f, 1f, 0f);
            image.Scale.X *= Calc.ClampedMap(speed.Length(), 0f, 400f, 1f, 2f);
            image.Scale.Y *= Calc.ClampedMap(speed.Length(), 0f, 400f, 1f, 0.2f);
            MoveH(speed.X * Engine.DeltaTime, collideH);
            MoveV(speed.Y * Engine.DeltaTime, collideV);
            if (Scene.OnInterval(0.05f))
            {
                (Scene as Level).ParticlesFG.Emit(P_Dust, Position);
            }
        }

        public override void Render()
        {
            Color color = image.Color;
            image.Color = Color.Black;
            image.Position = new Vector2(-1f, 0f);
            image.Render();
            image.Position = new Vector2(0f, -1f);
            image.Render();
            image.Position = new Vector2(1f, 0f);
            image.Render();
            image.Position = new Vector2(0f, 1f);
            image.Render();
            image.Position = Vector2.Zero;
            image.Color = color;
            base.Render();
        }

        private void OnCollideH(CollisionData hit)
        {
            speed.X *= -0.4f;
        }

        private void OnCollideV(CollisionData hit)
        {
            if (Math.Sign(speed.X) != 0)
            {
                speed.X -= Math.Sign(speed.X) * 2;
            }
            else
            {
                speed.X -= Calc.Random.Choose(-1, 1) * 2;
            }
            speed.Y *= -0.6f;
        }

        public static void Burst(Vector2 position, Color color, int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                DroneDebris debris = Engine.Pooler.Create<DroneDebris>();
                Vector2 position2 = position + new Vector2(Calc.Random.Range(-4, 4), Calc.Random.Range(-4, 4));
                debris.Init(position2, color);
                Engine.Scene.Add(debris);
            }
        }
    }
}
