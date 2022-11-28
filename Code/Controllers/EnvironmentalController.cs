using System;
using System.Collections;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Controllers
{
    [Tracked(true)]
    [CustomEntity("XaphanHelper/EnvironmentalController")]
    class EnvironmentalController : Entity
    {
        private class BgFlash : Entity
        {
            private float alpha;

            public BgFlash(float Intensity)
            {
                Depth = 10100;
                Tag = Tags.Persistent;
                alpha = Intensity;
            }

            public override void Update()
            {
                base.Update();
                alpha = Calc.Approach(alpha, 0f, Engine.DeltaTime * 1.5f);
                if (alpha <= 0f)
                {
                    RemoveSelf();
                }
            }

            public override void Render()
            {
                Vector2 position = (Scene as Level).Camera.Position;
                Draw.Rect(position.X - 10f, position.Y - 10f, 340f, 200f, Color.White * alpha);
            }
        }

        public EnvironmentalController(EntityData data, Vector2 position) : base(data.Position + position)
        {
            Tag = Tags.Global + Tags.PauseUpdate + Tags.TransitionUpdate;
        }

        public bool active;

        public override void Added(Scene scene)
        {
            base.Added(scene);
            Add(new Coroutine(lightningStrikeRoutine()));
        }

        public override void Update()
        {
            base.Update();
            foreach (EnvironmentalController controller in Scene.Entities.FindAll<EnvironmentalController>())
            {
                if (controller.active == true)
                {
                    foreach (EnvironmentalController controller2 in Scene.Entities.FindAll<EnvironmentalController>())
                    {
                        if (controller2.active == false)
                        {
                            controller2.RemoveSelf();
                        }
                    }
                    break;
                }
            }
        }

        public IEnumerator lightningStrikeRoutine()
        {
            var rand = new Random();
            active = true;
            yield return 7.0f;
            Scene.Add(new BgFlash(0.4f));
            yield return 0.2f;
            Scene.Add(new BgFlash(0.4f));
            yield return 0.3f;
            Scene.Add(new BgFlash(1f));
            Scene.Add(new LightningStrike(new Vector2(SceneAs<Level>().Camera.Left + rand.Next(100, 220), SceneAs<Level>().Bounds.Top), rand.Next(50, 100), 240f));
            yield return 12.5f;
            Scene.Add(new BgFlash(0.7f));
            yield return 0.2f;
            Scene.Add(new BgFlash(0.4f));
            yield return 0.3f;
            Scene.Add(new BgFlash(0.4f));
            yield return 4.8f;
            Scene.Add(new BgFlash(0.4f));
            yield return 0.2f;
            Scene.Add(new BgFlash(1f));
            Scene.Add(new LightningStrike(new Vector2(SceneAs<Level>().Camera.Left + rand.Next(100, 220), SceneAs<Level>().Bounds.Top), rand.Next(50, 100), 240f));
            yield return 5f;
            Scene.Add(new BgFlash(0.4f));
            yield return 3.7f;
            Scene.Add(new BgFlash(0.4f));
            yield return 0.3f;
            Scene.Add(new BgFlash(0.7f));
            yield return 3f;
            Scene.Add(new BgFlash(0.4f));
            yield return 8.5f;
            Scene.Add(new BgFlash(0.7f));
            yield return 12.5f;
            Scene.Add(new BgFlash(0.4f));
            yield return 0.2f;
            Scene.Add(new BgFlash(0.4f));
            yield return 0.3f;
            Scene.Add(new BgFlash(1f));
            Scene.Add(new LightningStrike(new Vector2(SceneAs<Level>().Camera.Left + rand.Next(100, 220), SceneAs<Level>().Bounds.Top), rand.Next(50, 100), 240f));
            yield return 8.3f;
            Scene.Add(new BgFlash(0.4f));
            yield return 0.2f;
            Scene.Add(new BgFlash(0.7f));
            yield return 0.5f;
            Scene.Add(new BgFlash(0.7f));
            yield return 10.5f;
            Scene.Add(new BgFlash(0.4f));
            yield return 11f;
            Scene.Add(new BgFlash(0.7f));
            yield return 0.5f;
            Add(new Coroutine(lightningStrikeLoop(rand)));
        }

        public IEnumerator lightningStrikeLoop(Random rand)
        {
            yield return 2.3f;
            Scene.Add(new BgFlash(0.4f));
            yield return 0.2f;
            Scene.Add(new BgFlash(1f));
            Scene.Add(new LightningStrike(new Vector2(SceneAs<Level>().Camera.Left + rand.Next(100, 220), SceneAs<Level>().Bounds.Top), rand.Next(50, 100), 240f));
            yield return 5.5f;
            Scene.Add(new BgFlash(0.4f));
            yield return 3.3f;
            Scene.Add(new BgFlash(0.7f));
            yield return 0.2f;
            Scene.Add(new BgFlash(0.4f));
            yield return 5f;
            Scene.Add(new BgFlash(0.4f));
            yield return 5.2f;
            Scene.Add(new BgFlash(0.4f));
            yield return 0.3f;
            Scene.Add(new BgFlash(0.7f));
            yield return 12.7f;
            Scene.Add(new BgFlash(0.4f));
            yield return 0.3f;
            Scene.Add(new BgFlash(0.7f));
            yield return 5f;
            Scene.Add(new BgFlash(0.4f));
            yield return 3.7f;
            Scene.Add(new BgFlash(0.4f));
            yield return 0.3f;
            Scene.Add(new BgFlash(0.7f));
            Scene.Add(new LightningStrike(new Vector2(SceneAs<Level>().Camera.Left + rand.Next(100, 220), SceneAs<Level>().Bounds.Top), rand.Next(50, 100), 240f));
            yield return 11.5f;
            Scene.Add(new BgFlash(0.4f));
            yield return 0.2f;
            Scene.Add(new BgFlash(0.4f));
            yield return 0.3f;
            Scene.Add(new BgFlash(0.7f));
            yield return 6f;
            Scene.Add(new BgFlash(0.4f));
            yield return 4f;
            Add(new Coroutine(lightningStrikeLoop(rand)));
        }
    }
}
