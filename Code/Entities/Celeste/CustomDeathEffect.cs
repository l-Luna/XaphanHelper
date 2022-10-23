using Celeste;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.XaphanHelper.Entities
{
    public class CustomDeathEffect : Entity
    {
        public Color Color;

        public float Percent;

        public float Duration = 0.834f;

        public Action<float> OnUpdate;

        public Action OnEnd;

        public CustomDeathEffect(Color color, Vector2 position) : base(position)
        {
            Color = color;
            Position = position;
            Percent = 0f;
            Depth = -20000;
        }

        public override void Update()
        {
            base.Update();
            if (Percent > 1f)
            {
                RemoveSelf();
                OnEnd?.Invoke();
            }
            Percent = Calc.Approach(Percent, 1f, Engine.DeltaTime / Duration);
            OnUpdate?.Invoke(Percent);
        }

        public override void Render()
        {
            Draw(Position, Color, Percent);
        }

        public static void Draw(Vector2 position, Color color, float ease)
        {
            Color color2 = (Math.Floor(ease * 10f) % 2.0 == 0.0) ? color : Color.White;
            MTexture mTexture = GFX.Game["characters/player/hair00"];
            float num = (ease < 0.5f) ? (0.5f + ease) : Ease.CubeOut(1f - (ease - 0.5f) * 2f);
            for (int i = 0; i < 8; i++)
            {
                Vector2 value = Calc.AngleToVector(((float)i / 8f + ease * 0.25f) * ((float)Math.PI * 2f), Ease.CubeOut(ease) * 24f);
                mTexture.DrawCentered(position + value + new Vector2(-1f, 0f), Color.Black, new Vector2(num, num));
                mTexture.DrawCentered(position + value + new Vector2(1f, 0f), Color.Black, new Vector2(num, num));
                mTexture.DrawCentered(position + value + new Vector2(0f, -1f), Color.Black, new Vector2(num, num));
                mTexture.DrawCentered(position + value + new Vector2(0f, 1f), Color.Black, new Vector2(num, num));
            }
            for (int j = 0; j < 8; j++)
            {
                Vector2 value2 = Calc.AngleToVector(((float)j / 8f + ease * 0.25f) * ((float)Math.PI * 2f), Ease.CubeOut(ease) * 24f);
                mTexture.DrawCentered(position + value2, color2, new Vector2(num, num));
            }
        }
    }
}
