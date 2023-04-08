using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Effects
{
    public class HeatParticles : Backdrop
    {
        private struct Particle
        {
            public Vector2 Position;

            public float Percent;

            public float Duration;

            public Vector2 Direction;

            public float Speed;

            public float Spin;

            public int Color;
        }

        public Color[] Colors;

        private Color[] currentColors;

        private Particle[] particles;

        private float fade;

        private bool NoMist;

        private Parallax mist1;

        private Parallax mist2;

        private bool show;

        private bool wasShow;

        public HeatParticles(string particlesColors, int particlesAmount = 50, bool noMist = false)
        {
            particles = new Particle[particlesAmount];
            string[] colors = particlesColors.Split(',');
            Colors = new Color[2]
            {
                Calc.HexToColor(colors.GetLength(0) >= 1 ? colors[0] : "FFFFFF") * (Color.A / 255),
                Calc.HexToColor(colors.GetLength(0) >= 2 ? colors[1] : "FFFFFF") * (Color.A / 255)
            };
            for (int i = 0; i < particles.Length; i++)
            {
                Reset(i, Calc.Random.NextFloat());
            }
            NoMist = noMist;
            currentColors = new Color[Colors.Length];
            if (!NoMist)
            {
                mist1 = new Parallax(GFX.Misc["mist"]);
                mist2 = new Parallax(GFX.Misc["mist"]);
            }
        }

        private void Reset(int i, float p)
        {
            particles[i].Percent = p;
            particles[i].Position = new Vector2(Calc.Random.Range(0, 320), Calc.Random.Range(0, 180));
            particles[i].Speed = Calc.Random.Range(4, 14);
            particles[i].Spin = Calc.Random.Range(0.25f, (float)Math.PI * 6f);
            particles[i].Duration = Calc.Random.Range(1f, 4f);
            particles[i].Direction = Calc.AngleToVector(Calc.Random.NextFloat((float)Math.PI * 2f), 1f);
            particles[i].Color = Calc.Random.Next(Colors.Length) * (Color.A / 255);
        }

        public override void Update(Scene scene)
        {
            Level level = scene as Level;
            show = (IsVisible(level));
            if (show)
            {
                for (int i = 0; i < currentColors.Length; i++)
                {
                    currentColors[i] = Colors[i];
                }
            }
            for (int j = 0; j < particles.Length; j++)
            {
                if (particles[j].Percent >= 1f)
                {
                    Reset(j, 0f);
                }
                float scaleFactor = 1f;
                scaleFactor = 0.25f;
                particles[j].Percent += Engine.DeltaTime / particles[j].Duration;
                particles[j].Position += particles[j].Direction * particles[j].Speed * scaleFactor * Engine.DeltaTime;
                particles[j].Direction.Rotate(particles[j].Spin * Engine.DeltaTime);
                particles[j].Position.Y -= 10f * Engine.DeltaTime;
            }
            fade = Calc.Approach(fade, 0.5f, Engine.DeltaTime) * (Color.A / 255);
            if (!NoMist)
            {
                mist1.Color = Calc.HexToColor("f1b22b") * fade * 0.7f * (Color.A / 255);
                mist2.Color = Calc.HexToColor("f12b3a") * fade * 0.7f * (Color.A / 255);
                mist1.Speed = new Vector2(4f, -20f);
                mist2.Speed = new Vector2(4f, -40f);
                mist1.Update(scene);
                mist2.Update(scene);
            }
            wasShow = show;
        }

        public override void Render(Scene scene)
        {
            if (show)
            {
                Camera camera = (scene as Level).Camera;
                for (int i = 0; i < particles.Length; i++)
                {
                    Vector2 vector = default(Vector2);
                    vector.X = Mod(particles[i].Position.X - camera.X, 320f);
                    vector.Y = Mod(particles[i].Position.Y - camera.Y, 180f);
                    float percent = particles[i].Percent;
                    float num = 0f;
                    num = ((!(percent < 0.7f)) ? Calc.ClampedMap(percent, 0.7f, 1f, 1f, 0f) : Calc.ClampedMap(percent, 0f, 0.3f));
                    Draw.Rect(vector, 1f, 1f, currentColors[particles[i].Color] * (fade * num));
                }
                if (!NoMist)
                {
                    mist1.Render(scene);
                    mist2.Render(scene);
                }
            }
        }

        private float Mod(float x, float m)
        {
            return (x % m + m) % m;
        }
    }
}


