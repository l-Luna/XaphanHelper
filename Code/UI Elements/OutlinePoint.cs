using System;
using System.Collections.Generic;

namespace Celeste.Mod.XaphanHelper.UI_Elements
{
    public class OutlinePoint
    {
        public float x;

        public float y;

        public bool visible;

        public OutlinePoint(float x, float y, bool visible)
        {
            this.x = x;
            this.y = y;
            this.visible = visible;
        }

        public static List<OutlinePoint> GenerateSolidOutline(Solid solid)
        {
            List<OutlinePoint> outline = new List<OutlinePoint>();
            for (int i = (int)solid.Width / 2; i < solid.Width; i++)
            {
                Math.DivRem(i, 4, out int pos);
                if (pos == 1 || pos == 2)
                {
                    outline.Add(new OutlinePoint(i, 0f, true));
                }
                else
                {
                    outline.Add(new OutlinePoint(i, 0f, false));
                }
            }
            for (int j = 0; j < solid.Height; j++)
            {
                Math.DivRem(j, 4, out int pos);
                if (pos == 1 || pos == 2)
                {
                    outline.Add(new OutlinePoint(solid.Width - 1, j, true));
                }
                else
                {
                    outline.Add(new OutlinePoint(solid.Width - 1, j, false));
                }
            }
            for (int i = (int)solid.Width; i >= 0; i--)
            {
                Math.DivRem(i, 4, out int pos);
                if (pos == 1 || pos == 2)
                {
                    outline.Add(new OutlinePoint(i, solid.Height - 1, true));
                }
                else
                {
                    outline.Add(new OutlinePoint(i, solid.Height - 1, false));
                }
            }
            for (int j = (int)solid.Height; j >= 0; j--)
            {
                Math.DivRem(j, 4, out int pos);
                if (pos == 1 || pos == 2)
                {
                    outline.Add(new OutlinePoint(0, j, true));
                }
                else
                {
                    outline.Add(new OutlinePoint(0, j, false));
                }
            }
            for (int i = 0; i < solid.Width / 2; i++)
            {
                Math.DivRem(i, 4, out int pos);
                if (pos == 1 || pos == 2)
                {
                    outline.Add(new OutlinePoint(i, 0f, true));
                }
                else
                {
                    outline.Add(new OutlinePoint(i, 0f, false));
                }
            }
            outline.Reverse();
            return outline;
        }
    }
}
