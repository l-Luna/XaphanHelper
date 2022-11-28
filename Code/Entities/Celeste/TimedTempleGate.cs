using System;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;


namespace Celeste.Mod.XaphanHelper.Entities
{
    [Tracked(true)]
    [CustomEntity("XaphanHelper/TimedTempleGate")]
    class TimedTempleGate : Solid
    {
        private const int OpenHeight = 0;

        private const float HoldingWaitTime = 0.2f;

        private const float HoldingOpenDistSq = 4096f;

        private const float HoldingCloseDistSq = 6400f;

        private const int MinDrawHeight = 4;

        private int closedHeight;

        private Sprite sprite;

        private Shaker shaker;

        private float drawHeight;

        private float drawHeightMoveSpeed;

        private Vector2 holdingCheckFrom;

        private string spriteName;

        public bool startOpen;

        public TimedTempleGate(EntityData data, Vector2 offset) : base(data.Position + offset, data.Width, data.Height, true)
        {
            spriteName = data.Attr("spriteName", "default");
            startOpen = data.Bool("startOpen", false);
            closedHeight = data.Height;
            Add(sprite = GFX.SpriteBank.Create("templegate_" + spriteName));
            sprite.X = base.Collider.Width / 2f;
            sprite.Play("idle");
            Add(shaker = new Shaker(on: false));
            Depth = -9000;
            holdingCheckFrom = Position + new Vector2(Width / 2f, data.Height / 2);
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            drawHeight = Math.Max(4f, Height);
            if (startOpen)
            {
                StartOpen();
            }
        }

        public void Open()
        {
            Audio.Play("event:/game/05_mirror_temple/gate_main_open", Position);
            drawHeightMoveSpeed = 200f;
            drawHeight = Height;
            shaker.ShakeFor(0.2f, removeOnFinish: false);
            SetHeight(0);
            sprite.Play("open");
        }

        public void Close()
        {
            Audio.Play("event:/game/05_mirror_temple/gate_main_close", Position);
            drawHeightMoveSpeed = 300f;
            drawHeight = Math.Max(4f, base.Height);
            shaker.ShakeFor(0.2f, removeOnFinish: false);
            SetHeight(closedHeight);
            sprite.Play("hit");
        }

        public void StartOpen()
        {
            SetHeight(0);
            drawHeight = 4f;
        }

        private void SetHeight(int height)
        {
            if (height < Collider.Height)
            {
                Collider.Height = height;
                return;
            }
            float y = Y;
            int num = (int)Collider.Height;
            if (Collider.Height < 64f)
            {
                Y -= 64f - Collider.Height;
                Collider.Height = 64f;
            }
            MoveVExact(height - num);
            Y = y;
            Collider.Height = height;
        }

        public override void Update()
        {
            base.Update();
            float num = Math.Max(4f, Height);
            if (drawHeight != num)
            {
                drawHeight = Calc.Approach(drawHeight, num, drawHeightMoveSpeed * Engine.DeltaTime);
            }
        }

        public override void Render()
        {
            Vector2 value = new(Math.Sign(shaker.Value.X), 0f);
            Draw.Rect(X - 2f, Y - 8f, 14f, 10f, Color.Black);
            sprite.DrawSubrect(Vector2.Zero + value, new Rectangle(0, (int)(sprite.Height - drawHeight), (int)sprite.Width, (int)drawHeight));
        }
    }
}
