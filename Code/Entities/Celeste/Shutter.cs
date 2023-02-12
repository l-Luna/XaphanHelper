using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;
using System.Collections;
using static Celeste.Mod.XaphanHelper.Entities.DroneGate;
using static Celeste.GaussianBlur;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [CustomEntity("XaphanHelper/Shutter")]
    class Shutter : Solid
    {
        public class ShutterLightWall : Solid
        {
            private Sprite top;

            private BloomPoint bloom;

            private Shutter shutter;
            public ShutterLightWall(Shutter shutter, Vector2 offset) : base(shutter.Position + offset, 8, 8, safe: true)
            {
                this.shutter = shutter;
                Collider = new Hitbox(8f, 8f);
                Add(bloom = new BloomPoint(0.5f, 8f));
                bloom.Position = new Vector2(4);
                Add(top = new Sprite(GFX.Game, "objects/XaphanHelper/Shutter/"));
                top.Add("off", "top", 0, 0);
                top.Add("on", "top", 0, 1);
                top.CenterOrigin();
                top.Position = new Vector2(4);
                top.Play("off");
                Add(new LightOcclude(1f));
            }

            public override void Update()
            {
                base.Update();
                if (shutter.openOffset > 0 && shutter.openOffset != (shutter.MaxHeight - 8f))
                {
                    top.Play("on");
                }
                else
                {
                    top.Play("off");
                }
            }

            public override void Render()
            {
                base.Render();
                top.Render();
            }
        }

        private Sprite gate;

        private Coroutine GateRoutine = new();

        private float openOffset;

        private float MaxHeight;

        private Vector2 startPosition;

        private string direction;

        private int length;

        public Shutter(EntityData data, Vector2 offset) : base(data.Position + offset, data.Width, data.Height, safe: false)
        {
            direction = data.Attr("direction", "Bottom");
            length = data.Int("length", 8);
            Collider = new Hitbox(Width, length + 1);
            Collider.Position += Vector2.UnitY * 7;
            MaxHeight = length + 1;
            startPosition = Position;
            Add(gate = new Sprite(GFX.Game, "objects/XaphanHelper/Shutter/"));
            gate.Add("gate", "gate", 0);
            gate.Play("gate");
            Add(new LightOcclude());
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            SceneAs<Level>().Add(new ShutterLightWall(this, Vector2.Zero));
            if (SceneAs<Level>().Session.GetFlag("testFlagShutter"))
            {
                openOffset = MaxHeight;
            }
        }

        public override void Update()
        {
            base.Update();
            Player player = SceneAs<Level>().Tracker.GetEntity<Player>();
            if (!GateRoutine.Active)
            {
                if (SceneAs<Level>().Session.GetFlag("testFlagShutter"))
                {
                    Add(GateRoutine = new Coroutine(Open()));
                }
                else
                {
                    Add(GateRoutine = new Coroutine(Close(player)));
                }
            }
        }

        private IEnumerator Open()
        {
            while (Collider.Height > 0f && SceneAs<Level>().Session.GetFlag("testFlagShutter"))
            {
                Collider.Position.Y += Engine.DeltaTime * 20;
                Collider.Height -= Engine.DeltaTime * 20;
                openOffset += Engine.DeltaTime * 20;
                MoveV(Engine.DeltaTime * -20);
                yield return null;
            }
            if (openOffset > MaxHeight)
            {
                openOffset = MaxHeight;
            }
        }

        private IEnumerator Close(Player player)
        {
            while (Collider.Height < MaxHeight && !SceneAs<Level>().Session.GetFlag("testFlagShutter"))
            {
                Collider.Position.Y -= Engine.DeltaTime * 20;
                Collider.Height += Engine.DeltaTime * 20;
                openOffset -= Engine.DeltaTime * 20;
                MoveV(Engine.DeltaTime * 20);
                yield return null;
            }
            if (openOffset < 0)
            {
                openOffset = 0;
            }
        }

        public override void Render()
        {
            for (int i = 0; i <= MaxHeight / 8; i++)
            {
                gate.RenderPosition = startPosition + new Vector2(0, i * 8 - openOffset);
                if (gate.RenderPosition.Y > startPosition.Y)
                {
                    gate.Render();
                }
            }
        }
    }
}
