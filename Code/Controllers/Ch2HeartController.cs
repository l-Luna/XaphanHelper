using System.Collections;
using System.Collections.Generic;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Controllers
{
    [CustomEntity("XaphanHelper/Ch2HeartController")]
    class Ch2HeartController : Entity
    {
        private string flagA;

        private string flagB;

        private string flagC;

        private string flagD;

        private int startPositionX;

        private int startPositionY;

        private bool allActivated => (!string.IsNullOrEmpty(flagA) ? SceneAs<Level>().Session.GetFlag(flagA) : true) && (!string.IsNullOrEmpty(flagB) ? SceneAs<Level>().Session.GetFlag(flagB) : true) && (!string.IsNullOrEmpty(flagC) ? SceneAs<Level>().Session.GetFlag(flagC) : true) && (!string.IsNullOrEmpty(flagD) ? SceneAs<Level>().Session.GetFlag(flagD) : true);

        public Ch2HeartController(EntityData data, Vector2 position) : base(data.Position + position)
        {
            flagA = data.Attr("flagA");
            flagB = data.Attr("flagB");
            flagC = data.Attr("flagC");
            flagD = data.Attr("flagD");
            startPositionX = data.Int("startPositionX");
            startPositionY = data.Int("startPositionY");
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            if (!SceneAs<Level>().Session.HeartGem)
            {
                if (allActivated)
                {
                    Scene.Add(new HeartGem(Position));
                }
                else
                {
                    Add(new Coroutine(CheckIfLastTorch()));
                }
            }
        }

        public IEnumerator CheckIfLastTorch()
        {
            float timer1 = 3f;
            while (timer1 > 0f)
            {
                yield return null;
                timer1 -= Engine.DeltaTime;
            }
            if (allActivated)
            {
                yield return 0.533f;
                Audio.Play("event:/game/06_reflection/supersecret_heartappear");
                Entity dummy = new(Position)
                {
                    Depth = 1
                };
                Scene.Add(dummy);
                Image white = new(GFX.Game["collectables/heartgem/white00"]);
                white.CenterOrigin();
                white.Scale = Vector2.Zero;
                dummy.Add(white);
                BloomPoint glow = new(0f, 16f);
                dummy.Add(glow);
                List<Entity> absorbs = new();
                for (int i = 0; i < 20; i++)
                {
                    AbsorbOrb orb = new(Position + new Vector2(startPositionX, startPositionY), dummy);
                    Scene.Add(orb);
                    absorbs.Add(orb);
                    yield return null;
                }
                yield return 0.8f;
                float duration = 0.6f;
                for (float p = 0f; p < 1f; p += Engine.DeltaTime / duration)
                {
                    white.Scale = Vector2.One * p;
                    glow.Alpha = p;
                    (Scene as Level).Shake();
                    yield return null;
                }
                foreach (Entity orb2 in absorbs)
                {
                    orb2.RemoveSelf();
                }
                (Scene as Level).Flash(Color.White);
                Scene.Remove(dummy);
                Scene.Add(new HeartGem(Position));
                float timer2 = 2f;
                while (timer2 > 0f)
                {
                    yield return null;
                    timer2 -= Engine.DeltaTime;
                }
            }
            SceneAs<Level>().Session.SetFlag("Teleport_Back", true);
            yield return 0.1;
            SceneAs<Level>().Session.SetFlag("Teleport_Back", false);
            SceneAs<Level>().CanRetry = true;
        }
    }
}
