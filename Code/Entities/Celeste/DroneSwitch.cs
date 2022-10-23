using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;
using System;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [Tracked(true)]
    [CustomEntity("XaphanHelper/DroneSwitch")]
    public class DroneSwitch : Entity
    {
        public string flag;

        public string side;

        public float cooldown;

        private Sprite buttonSprite;

        private StaticMover staticMover;

        public DroneSwitch(EntityData data, Vector2 position) : base(data.Position + position)
        {
            side = data.Attr("side");
            flag = data.Attr("flag");
            Add(buttonSprite = new Sprite(GFX.Game, "objects/XaphanHelper/DroneSwitch/"));
            buttonSprite.Add("idle", "idle", 0.2f);
            buttonSprite.Add("active", "active", 0.2f);
            buttonSprite.Origin = new Vector2(buttonSprite.Width / 2, buttonSprite.Height / 2);
            buttonSprite.Position = new Vector2(4f, 4f);
            staticMover = new StaticMover();
            staticMover.OnAttach = delegate (Platform p)
            {
                Depth = p.Depth + 1;
            };
            switch (side)
            {
                case "Left":
                    Collider = new Hitbox(3f, 4f, 0f, 2f);
                    staticMover.SolidChecker = ((Solid s) => CollideCheck(s, Position - Vector2.UnitX));
                    staticMover.JumpThruChecker = ((JumpThru jt) => CollideCheck(jt, Position - Vector2.UnitX));
                    Add(staticMover);
                    buttonSprite.Rotation = (float)Math.PI / 2f;
                    break;
                case "Right":
                    Collider = new Hitbox(3f, 4f, 5f, 2f);
                    staticMover.SolidChecker = ((Solid s) => CollideCheck(s, Position + Vector2.UnitX));
                    staticMover.JumpThruChecker = ((JumpThru jt) => CollideCheck(jt, Position + Vector2.UnitX));
                    Add(staticMover);
                    buttonSprite.Rotation = -(float)Math.PI / 2f;
                    break;
                case "Down":
                    Collider = new Hitbox(4f, 3f, 2f, 0f);
                    staticMover.SolidChecker = ((Solid s) => CollideCheck(s, Position - Vector2.UnitY));
                    staticMover.JumpThruChecker = ((JumpThru jt) => CollideCheck(jt, Position - Vector2.UnitY));
                    Add(staticMover);
                    buttonSprite.Rotation = (float)Math.PI;
                    break;
            }
            staticMover.OnEnable = OnEnable;
            staticMover.OnDisable = OnDisable;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            if (!SceneAs<Level>().Session.GetFlag(flag))
            {
                buttonSprite.Play("idle");
            }
            else
            {
                buttonSprite.Play("active");
            }
        }

        public override void Update()
        {
            base.Update();
            if (!SceneAs<Level>().Session.GetFlag(flag))
            {
                buttonSprite.Play("idle");
            }
            else
            {
                buttonSprite.Play("active");
            }
        }

        private void OnEnable()
        {
            Active = (Visible = (Collidable = true));
        }

        private void OnDisable()
        {
            Active = (Visible = (Collidable = false));
        }

        public void Triggered(string dir)
        {
            if (cooldown <= 0)
            {
                if (dir != null && dir == side)
                {
                    Add(new Coroutine(SwitchFlag()));
                }
            }
        }

        private IEnumerator SwitchFlag()
        {
            SceneAs<Level>().Session.SetFlag(flag, SceneAs<Level>().Session.GetFlag(flag) ? false : true);
            Audio.Play("event:/game/05_mirror_temple/button_activate", Position);
            cooldown = 0.5f;
            while (cooldown > 0)
            {
                cooldown -= Engine.DeltaTime;
                yield return null;
            }
        }

        public override void Render()
        {
            base.Render();
            buttonSprite.Render();
        }
    }
}
