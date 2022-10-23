using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using CelesteTags = Celeste.Tags;

namespace Celeste.Mod.XaphanHelper.Effects
{
    [CustomEntity("XaphanHelper/Glow")]
    public class Glow : Backdrop
    {
        [Tracked(true)]
        public class BgGlow : Entity
        {
            private float alpha = 0f;

            public int ID;

            Color color;

            public BgGlow(Color color, int ID)
            {
                Depth = 10100;
                Tag = (CelesteTags.Persistent | CelesteTags.TransitionUpdate);
                this.color = color;
                this.ID = ID;
            }

            public override void Update()
            {
                base.Update();
                alpha += Engine.DeltaTime * 4f;
            }

            public override void Render()
            {
                Vector2 position = (Scene as Level).Camera.Position;
                Draw.Rect(position.X - 10f, position.Y - 10f, 340f, 200f, color * (0.5f * (0.5f + ((float)Math.Sin(alpha) + 1f) * 0.25f)));
            }
        }

        public int ID = -1;

        public Color color;

        public BgGlow glow;

        public Glow(string color)
        {
            this.color = Calc.HexToColor(color);
        }

        public override void Update(Scene scene)
        {
            Level level = scene as Level;
            if (ID == -1)
            {
                HashSet<int> glows = new HashSet<int>();
                foreach (BgGlow glow in scene.Tracker.GetEntities<BgGlow>())
                {
                    glows.Add(glow.ID);
                }
                ID = AffectID(glows);
            }
            if (IsVisible(level))
            {
                bool alreadyAdded = false;
                foreach (BgGlow glow in scene.Tracker.GetEntities<BgGlow>())
                {
                    if (glow.ID == ID)
                    {
                        alreadyAdded = true;
                        break;
                    }
                }
                if (!alreadyAdded)
                {
                    scene.Add(glow = new BgGlow(color, ID));
                }
            }
            if (!IsVisible(level))
            {
                foreach (BgGlow glow in scene.Tracker.GetEntities<BgGlow>())
                {
                    if (glow.ID == ID)
                    {
                        scene.Remove(glow);
                        break;
                    }
                }
            }
        }

        public int AffectID(HashSet<int> glows)
        {
            int SetID = Calc.Random.Next(0, 51);
            if (glows.Contains(SetID))
            {
                AffectID(glows);
            }
            else
            {
                return SetID;
            }
            return -1;
        }
    }
}
