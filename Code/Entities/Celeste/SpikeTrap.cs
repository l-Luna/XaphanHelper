using System;
using System.Collections;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [CustomEntity("XaphanHelper/SpikeTrap")]
    public class SpikeTrap : Entity
    {
        public enum Directions
        {
            Up,
            Down,
            Left,
            Right
        }

        private LedgeBlocker blocker;

        public Directions Direction;

        private bool activated;

        private float triggerTime;

        private bool triggered;

        private bool retract;

        private string sprite;

        private Sprite trapSprite;

        public Color EnabledColor = Color.White;

        public Color DisabledColor = Color.White;

        public bool VisibleWhenDisabled;

        private Vector2 imageOffset;

        public SpikeTrap(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            triggerTime = data.Float("triggerTime", 1.5f);
            Direction = (Directions)data.Int("direction", 0);
            sprite = data.Attr("sprite");
            retract = data.Bool("retract", false);
            if (string.IsNullOrEmpty(sprite))
            {
                sprite = "danger/XaphanHelper/SpikeTrap";
            }
            Add(trapSprite = new Sprite(GFX.Game, sprite + "/"));
            trapSprite.AddLoop("idle", "trap", 0f, 0);
            trapSprite.Add("triggered", "trap", 0.04f, 1, 2, 3, 4);
            trapSprite.Add("retract", "trap", 0.04f, 3, 2, 1, 0);
            switch (Direction)
            {
                case Directions.Up:
                    Collider = new Hitbox(data.Width, 3f, 0f, 5f);
                    break;
                case Directions.Down:
                    Collider = new Hitbox(data.Width, 3f);
                    trapSprite.FlipY = true;
                    break;
                case Directions.Left:
                    Collider = new Hitbox(3f, data.Height, 5f);
                    trapSprite.Rotation = -(float)Math.PI / 2f;
                    trapSprite.FlipX = true;
                    break;
                case Directions.Right:
                    Collider = new Hitbox(3f, data.Height);
                    trapSprite.Rotation = (float)Math.PI / 2f;
                    break;
            }
            Add(new PlayerCollider(OnPlayer, Collider));
            Add(new StaticMover
            {
                OnShake = OnShake,
                SolidChecker = IsRiding,
                JumpThruChecker = IsRiding,
                OnEnable = OnEnable,
                OnDisable = OnDisable
            });
            Depth = -10001;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            trapSprite.Play("idle");
        }

        private void OnPlayer(Player player)
        {
            if (!activated)
            {
                activated = true;
                Audio.Play("event:/game/03_resort/door_metal_open", Position);
                Add(new Coroutine(TrapRoutine()));
            }
            if (triggered)
            {
                switch (Direction)
                {
                    case Directions.Up:
                        if (player.Speed.Y >= 0f && player.Bottom <= Bottom)
                        {
                            player.Die(new Vector2(0f, -1f));
                        }
                        break;
                    case Directions.Down:
                        if (player.Speed.Y <= 0f)
                        {
                            player.Die(new Vector2(0f, 1f));
                        }
                        break;
                    case Directions.Left:
                        if (player.Speed.X >= 0f)
                        {
                            player.Die(new Vector2(-1f, 0f));
                        }
                        break;
                    case Directions.Right:
                        if (player.Speed.X <= 0f)
                        {
                            player.Die(new Vector2(1f, 0f));
                        }
                        break;
                }
            }
        }

        public void SetSpikeColor(Color color)
        {
            foreach (Component component in Components)
            {
                if (component is Sprite sprite)
                {
                    sprite.Color = color;
                }
            }
        }

        private void OnShake(Vector2 amount)
        {
            imageOffset += amount;
        }

        private bool IsRiding(Solid solid)
        {
            return Direction switch
            {
                Directions.Up => CollideCheckOutside(solid, Position + Vector2.UnitY),
                Directions.Down => CollideCheckOutside(solid, Position - Vector2.UnitY),
                Directions.Left => CollideCheckOutside(solid, Position + Vector2.UnitX),
                Directions.Right => CollideCheckOutside(solid, Position - Vector2.UnitX),
                _ => false,
            };
        }

        private bool IsRiding(JumpThru jumpThru)
        {
            if (Direction != 0)
            {
                return false;
            }
            return CollideCheck(jumpThru, Position + Vector2.UnitY);
        }

        private void OnEnable()
        {
            Visible = (Collidable = true);
            SetSpikeColor(EnabledColor);
        }

        private void OnDisable()
        {
            Collidable = false;
            if (VisibleWhenDisabled)
            {
                SetSpikeColor(DisabledColor);
                return;
            }
            Visible = false;
        }

        private IEnumerator TrapRoutine()
        {
            float nextRriggerTime = triggerTime;
            while (triggerTime > 0)
            {
                yield return null;
                triggerTime -= Engine.DeltaTime;
            }
            trapSprite.Play("triggered");
            Audio.Play("event:/game/05_mirror_temple/bladespinner_spin", Position);
            triggered = true;
            Add(blocker = new LedgeBlocker());
            if (retract)
            {
                float timer = 1.5f;
                while (timer > 0)
                {
                    yield return null;
                    timer -= Engine.DeltaTime;
                }
                Audio.Play("event:/game/05_mirror_temple/bladespinner_spin", Position);
                triggered = false;
                trapSprite.Play("retract");
                activated = false;
                triggerTime = nextRriggerTime;
                blocker.RemoveSelf();
            }
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Render()
        {
            base.Render();
            switch (Direction)
            {
                case Directions.Up:
                    for (int i = 0; i < Width / 8; i++)
                    {
                        trapSprite.RenderPosition = Position + imageOffset + new Vector2(i * 8, -6);
                        trapSprite.Render();
                    }
                    break;
                case Directions.Down:
                    for (int i = 0; i < Width / 8; i++)
                    {
                        trapSprite.RenderPosition = Position + imageOffset + new Vector2(i * 8, -2);
                        trapSprite.Render();
                    }
                    break;
                case Directions.Left:
                    for (int i = 0; i < Height / 8; i++)
                    {
                        trapSprite.RenderPosition = Position + imageOffset + new Vector2(-6, 8 + i * 8);
                        trapSprite.Render();
                    }
                    break;
                case Directions.Right:
                    for (int i = 0; i < Height / 8; i++)
                    {
                        trapSprite.RenderPosition = Position + imageOffset + new Vector2(14, i * 8);
                        trapSprite.Render();
                    }
                    break;
            }
        }
    }
}
