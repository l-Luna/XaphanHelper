using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [CustomEntity("XaphanHelper/CustomZipMover")]
    class CustomZipMover : Solid
    {
        private class ZipMoverPathRenderer : Entity
        {
            public CustomZipMover zipMover;

            private MTexture cog;

            private Vector2 from;

            private Vector2 to;

            private Vector2 sparkAdd;

            private Color ropeColor;

            private Color ropeLightColor;

            private float sparkDirFromA;

            private float sparkDirFromB;

            private float sparkDirToA;

            private float sparkDirToB;

            public ZipMoverPathRenderer(CustomZipMover zipMover)
            {
                Depth = 5000;
                this.zipMover = zipMover;
                ropeColor = zipMover.ropeColor;
                ropeLightColor = zipMover.ropeLightColor;
                from = zipMover.start + new Vector2(zipMover.Width / 2f, zipMover.Height / 2f);
                to = zipMover.target + new Vector2(zipMover.Width / 2f, zipMover.Height / 2f);
                sparkAdd = (from - to).SafeNormalize(5f).Perpendicular();
                float num = (from - to).Angle();
                sparkDirFromA = num + (float)Math.PI / 8f;
                sparkDirFromB = num - (float)Math.PI / 8f;
                sparkDirToA = num + (float)Math.PI - (float)Math.PI / 8f;
                sparkDirToB = num + (float)Math.PI + (float)Math.PI / 8f;
                cog = GFX.Game[zipMover.directory + "/cog"];
            }

            public void CreateSparks()
            {
                SceneAs<Level>().ParticlesBG.Emit(ZipMover.P_Sparks, from + sparkAdd + Calc.Random.Range(-Vector2.One, Vector2.One), sparkDirFromA);
                SceneAs<Level>().ParticlesBG.Emit(ZipMover.P_Sparks, from - sparkAdd + Calc.Random.Range(-Vector2.One, Vector2.One), sparkDirFromB);
                SceneAs<Level>().ParticlesBG.Emit(ZipMover.P_Sparks, to + sparkAdd + Calc.Random.Range(-Vector2.One, Vector2.One), sparkDirToA);
                SceneAs<Level>().ParticlesBG.Emit(ZipMover.P_Sparks, to - sparkAdd + Calc.Random.Range(-Vector2.One, Vector2.One), sparkDirToB);
            }

            public override void Render()
            {
                DrawCogs(Vector2.UnitY, Color.Black);
                DrawCogs(Vector2.Zero);
                if (zipMover.drawBlackBorder)
                {
                    Draw.Rect(new Rectangle((int)(zipMover.X + zipMover.Shake.X - 1f), (int)(zipMover.Y + zipMover.Shake.Y - 1f), (int)zipMover.Width + 2, (int)zipMover.Height + 2), Color.Black);
                }
            }

            private void DrawCogs(Vector2 offset, Color? colorOverride = null)
            {
                Vector2 vector = (to - from).SafeNormalize();
                Vector2 value = vector.Perpendicular() * 3f;
                Vector2 value2 = -vector.Perpendicular() * 4f;
                float rotation = zipMover.percent * (float)Math.PI * 2f;
                Draw.Line(from + value + offset, to + value + offset, colorOverride.HasValue ? colorOverride.Value : ropeColor);
                Draw.Line(from + value2 + offset, to + value2 + offset, colorOverride.HasValue ? colorOverride.Value : ropeColor);
                for (float num = 4f - zipMover.percent * (float)Math.PI * 8f % 4f; num < (to - from).Length(); num += 4f)
                {
                    Vector2 value3 = from + value + vector.Perpendicular() + vector * num;
                    Vector2 value4 = to + value2 - vector * num;
                    Draw.Line(value3 + offset, value3 + vector * 2f + offset, colorOverride.HasValue ? colorOverride.Value : ropeLightColor);
                    Draw.Line(value4 + offset, value4 - vector * 2f + offset, colorOverride.HasValue ? colorOverride.Value : ropeLightColor);
                }
                cog.DrawCentered(from + offset, colorOverride.HasValue ? colorOverride.Value : Color.White, 1f, rotation);
                cog.DrawCentered(to + offset, colorOverride.HasValue ? colorOverride.Value : Color.White, 1f, rotation);
            }
        }

        private MTexture[,] edges = new MTexture[3, 3];

        private Sprite streetlight;

        private BloomPoint bloom;

        private ZipMoverPathRenderer pathRenderer;

        private List<MTexture> innerCogs;

        private MTexture temp = new MTexture();

        private bool drawBlackBorder;

        private Vector2 start;

        private Vector2 target;

        private float percent = 0f;

        private Color ropeColor = Calc.HexToColor("663931");

        private Color ropeLightColor = Calc.HexToColor("9b6157");

        private SoundSource sfx = new SoundSource();

        public string directory = "objects/zipmover";

        public int percentage = 100;

        public CustomZipMover(EntityData data, Vector2 offset) : base(data.Position + offset, data.Width, data.Height, safe: false)
        {
            Depth = -9998;
            start = Position;
            target = data.Nodes[0] + offset;
            Add(new Coroutine(Sequence()));
            Add(new LightOcclude());
            percentage = data.Int("percentage");
            ropeColor = Calc.HexToColor(data.Attr("ropeColor"));
            ropeLightColor = Calc.HexToColor(data.Attr("ropeLightColor"));
            directory = data.Attr("directory");
            string path;
            string id;
            string key;
            path = directory + "/light";
            id = directory + "/block";
            key = directory + "/innercog";
            drawBlackBorder = true;
            innerCogs = GFX.Game.GetAtlasSubtextures(key);
            Add(streetlight = new Sprite(GFX.Game, path));
            streetlight.Add("frames", "", 1f);
            streetlight.Play("frames");
            streetlight.Active = false;
            streetlight.SetAnimationFrame(1);
            streetlight.Position = new Vector2(Width / 2f - streetlight.Width / 2f, 0f);
            Add(bloom = new BloomPoint(1f, 6f));
            bloom.Position = new Vector2(Width / 2f, 4f);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    edges[i, j] = GFX.Game[id].GetSubtexture(i * 8, j * 8, 8, 8);
                }
            }
            SurfaceSoundIndex = 7;
            sfx.Position = new Vector2(Width, Height) / 2f;
            Add(sfx);
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            scene.Add(pathRenderer = new ZipMoverPathRenderer(this));
        }

        public override void Removed(Scene scene)
        {
            scene.Remove(pathRenderer);
            pathRenderer = null;
            base.Removed(scene);
        }

        public override void Update()
        {
            base.Update();
            bloom.Y = streetlight.CurrentAnimationFrame * 3;
        }

        public override void Render()
        {
            Vector2 position = Position;
            Position += Shake;
            Draw.Rect(X + 1f, Y + 1f, Width - 2f, Height - 2f, Color.Black);
            int num = 1;
            float num2 = 0f;
            int count = innerCogs.Count;
            for (int i = 4; i <= Height - 4f; i += 8)
            {
                int num3 = num;
                for (int j = 4; j <= Width - 4f; j += 8)
                {
                    int index = (int)(mod((num2 + num * percent * (float)Math.PI * 4f) / ((float)Math.PI / 2f), 1f) * (float)count);
                    MTexture mTexture = innerCogs[index];
                    Rectangle rectangle = new Rectangle(0, 0, mTexture.Width, mTexture.Height);
                    Vector2 zero = Vector2.Zero;
                    if (j <= 4)
                    {
                        zero.X = 2f;
                        rectangle.X = 2;
                        rectangle.Width -= 2;
                    }
                    else if (j >= Width - 4f)
                    {
                        zero.X = -2f;
                        rectangle.Width -= 2;
                    }
                    if (i <= 4)
                    {
                        zero.Y = 2f;
                        rectangle.Y = 2;
                        rectangle.Height -= 2;
                    }
                    else if (i >= Height - 4f)
                    {
                        zero.Y = -2f;
                        rectangle.Height -= 2;
                    }
                    mTexture = mTexture.GetSubtexture(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, temp);
                    mTexture.DrawCentered(Position + new Vector2(j, i) + zero, Color.White * ((num < 0) ? 0.5f : 1f));
                    num = -num;
                    num2 += (float)Math.PI / 3f;
                }
                if (num3 == num)
                {
                    num = -num;
                }
            }
            for (int k = 0; k < Width / 8f; k++)
            {
                for (int l = 0; l < Height / 8f; l++)
                {
                    int num4 = (k != 0) ? ((k != Width / 8f - 1f) ? 1 : 2) : 0;
                    int num5 = (l != 0) ? ((l != Height / 8f - 1f) ? 1 : 2) : 0;
                    if (num4 != 1 || num5 != 1)
                    {
                        edges[num4, num5].Draw(new Vector2(X + (float)(k * 8), Y + (float)(l * 8)));
                    }
                }
            }
            base.Render();
            Position = position;
        }

        private void ScrapeParticlesCheck(Vector2 to)
        {
            if (!Scene.OnInterval(0.03f))
            {
                return;
            }
            bool flag = to.Y != ExactPosition.Y;
            bool flag2 = to.X != ExactPosition.X;
            if (flag && !flag2)
            {
                int num = Math.Sign(to.Y - ExactPosition.Y);
                Vector2 value = (num != 1) ? TopLeft : BottomLeft;
                int num2 = 4;
                if (num == 1)
                {
                    num2 = Math.Min((int)Height - 12, 20);
                }
                int num3 = (int)Height;
                if (num == -1)
                {
                    num3 = Math.Max(16, (int)Height - 16);
                }
                if (Scene.CollideCheck<Solid>(value + new Vector2(-2f, num * -2)))
                {
                    for (int i = num2; i < num3; i += 8)
                    {
                        SceneAs<Level>().ParticlesFG.Emit(ZipMover.P_Scrape, TopLeft + new Vector2(0f, (float)i + (float)num * 2f), (num == 1) ? (-(float)Math.PI / 4f) : ((float)Math.PI / 4f));
                    }
                }
                if (Scene.CollideCheck<Solid>(value + new Vector2(Width + 2f, num * -2)))
                {
                    for (int j = num2; j < num3; j += 8)
                    {
                        SceneAs<Level>().ParticlesFG.Emit(ZipMover.P_Scrape, TopRight + new Vector2(-1f, (float)j + (float)num * 2f), (num == 1) ? ((float)Math.PI * -3f / 4f) : ((float)Math.PI * 3f / 4f));
                    }
                }
            }
            else
            {
                if (!flag2 || flag)
                {
                    return;
                }
                int num4 = Math.Sign(to.X - ExactPosition.X);
                Vector2 value2 = (num4 != 1) ? TopLeft : TopRight;
                int num5 = 4;
                if (num4 == 1)
                {
                    num5 = Math.Min((int)Width - 12, 20);
                }
                int num6 = (int)Width;
                if (num4 == -1)
                {
                    num6 = Math.Max(16, (int)Width - 16);
                }
                if (Scene.CollideCheck<Solid>(value2 + new Vector2(num4 * -2, -2f)))
                {
                    for (int k = num5; k < num6; k += 8)
                    {
                        SceneAs<Level>().ParticlesFG.Emit(ZipMover.P_Scrape, TopLeft + new Vector2((float)k + (float)num4 * 2f, -1f), (num4 == 1) ? ((float)Math.PI * 3f / 4f) : ((float)Math.PI / 4f));
                    }
                }
                if (Scene.CollideCheck<Solid>(value2 + new Vector2(num4 * -2, Height + 2f)))
                {
                    for (int l = num5; l < num6; l += 8)
                    {
                        SceneAs<Level>().ParticlesFG.Emit(ZipMover.P_Scrape, BottomLeft + new Vector2((float)l + (float)num4 * 2f, 0f), (num4 == 1) ? ((float)Math.PI * -3f / 4f) : (-(float)Math.PI / 4f));
                    }
                }
            }
        }

        private IEnumerator Sequence()
        {
            Vector2 start = Position;
            while (true)
            {
                if (!HasPlayerRider())
                {
                    yield return null;
                    continue;
                }
                sfx.Play("event:/game/xaphan/zip_mover_start");
                Input.Rumble(RumbleStrength.Medium, RumbleLength.Short);
                StartShaking(0.1f);
                yield return 0.1f;
                streetlight.SetAnimationFrame(3);
                StopPlayerRunIntoAnimation = false;
                float at2 = 0f;
                while (at2 < 1f)
                {
                    yield return null;
                    at2 = Calc.Approach(at2, 1f, 2f * Engine.DeltaTime * (percentage / 100f));
                    percent = Ease.SineIn(at2);
                    Vector2 to = Vector2.Lerp(start, target, percent);
                    ScrapeParticlesCheck(to);
                    if (Scene.OnInterval(0.1f))
                    {
                        pathRenderer.CreateSparks();
                    }
                    MoveTo(to);
                }
                sfx.Play("event:/game/xaphan/zip_mover_end");
                StartShaking(0.2f);
                Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
                SceneAs<Level>().Shake();
                StopPlayerRunIntoAnimation = true;
                yield return 0.5f;
                StopPlayerRunIntoAnimation = false;
                streetlight.SetAnimationFrame(2);
                float at = 0f;
                while (at < 1f)
                {
                    yield return null;
                    at = Calc.Approach(at, 1f, 0.5f * Engine.DeltaTime);
                    percent = 1f - Ease.SineIn(at);
                    Vector2 to2 = Vector2.Lerp(target, start, Ease.SineIn(at));
                    MoveTo(to2);
                }
                StopPlayerRunIntoAnimation = true;
                StartShaking(0.2f);
                streetlight.SetAnimationFrame(1);
                yield return 0.5f;
            }
        }

        private float mod(float x, float m)
        {
            return (x % m + m) % m;
        }
    }
}
