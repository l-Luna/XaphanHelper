using Monocle;
using Microsoft.Xna.Framework;
using System;

namespace Celeste.Mod.XaphanHelper.UI_Elements
{
    class CustomText : Entity
    {
        public string message;

        private SoundSource textSfx;

        private int firstLineLength;

        private float widestCharacter = 0f;

        public float timer = 0f;

        private float alpha = 0f;

        private float index;

        private int textPositionX;

        private int textPositionY;

        public bool Show;

        private bool textSfxPlaying;

        public CustomText(string text, string textPositionX, int textPositionY)
        {
            Tag = Tags.HUD | Tags.Global | Tags.PauseUpdate | Tags.TransitionUpdate;
            Add(textSfx = new SoundSource());
            message = "-" + Dialog.Clean(text) + "-";
            firstLineLength = CountToNewline(0);
            for (int i = 0; i < message.Length; i++)
            {
                float x = ActiveFont.Measure(message[i]).X;
                if (x > widestCharacter)
                {
                    widestCharacter = x;
                }
            }
            widestCharacter *= 0.9f;
            if (textPositionX == "Left")
            {
                this.textPositionX = message.Length * (int)widestCharacter / 2 + 20;
            }
            else if (textPositionX == "Middle")
            {
                this.textPositionX = 960;
            }
            else if (textPositionX == "Right")
            {
                this.textPositionX = 1920 - message.Length * (int)widestCharacter / 2 - 20;
            }
            this.textPositionY = textPositionY;
        }

        public override void Update()
        {
            base.Update();
            if ((Scene as Level).Paused)
            {
                textSfx.Pause();
                return;
            }
            timer += Engine.DeltaTime;
            if (!Show)
            {
                alpha = Calc.Approach(alpha, 0f, Engine.DeltaTime);
                if (alpha <= 0f)
                {
                    index = firstLineLength;
                }
            }
            else
            {
                alpha = Calc.Approach(alpha, 1f, Engine.DeltaTime * 2f);
                if (alpha >= 1f)
                {
                    index = Calc.Approach(index, message.Length, 32f * Engine.DeltaTime);
                }
            }
            if (Show && alpha >= 1f && index < message.Length)
            {
                if (!textSfxPlaying)
                {
                    textSfxPlaying = true;
                    textSfx.Play("event:/ui/game/memorial_text_loop");
                    textSfx.Param("end", 0f);
                }
            }
            else if (textSfxPlaying)
            {
                textSfxPlaying = false;
                textSfx.Stop();
                textSfx.Param("end", 1f);
            }
            textSfx.Resume();
        }

        private int CountToNewline(int start)
        {
            int i;
            for (i = start; i < message.Length && message[i] != '\n'; i++)
            {
            }
            return i - start;
        }

        public override void Render()
        {
            if ((Scene as Level).FrozenOrPaused || (Scene as Level).Completed || !(index > 0f) || !(alpha > 0f))
            {
                return;
            }
            Vector2 value = new Vector2(textPositionX, textPositionY);
            if (SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode)
            {
                value.X = 1920f - value.X;
            }
            float num = Ease.CubeInOut(alpha);
            int num2 = (int)Math.Min(message.Length, index);
            int num3 = 0;
            float num4 = 64f * (1f - num);
            int num5 = CountToNewline(0);
            for (int i = 0; i < num2; i++)
            {
                char c = message[i];
                if (c == '\n')
                {
                    num3 = 0;
                    num5 = CountToNewline(i + 1);
                    num4 += ActiveFont.LineHeight * 1.1f;
                    continue;
                }
                float x = 1f;
                float x2 = -num5 * widestCharacter / 2f + (num3 + 0.5f) * widestCharacter;
                float num6 = 0f;
                ActiveFont.Draw(c, value + new Vector2(x2, num4 + num6), new Vector2(0.5f, 1f), new Vector2(x, 1f), Color.White * num);
                num3++;
            }
        }
    }
}
