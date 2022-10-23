using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using System;
using Monocle;
using Celeste.Mod.XaphanHelper.Managers;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [Tracked(true)]
    [CustomEntity("XaphanHelper/LaserDetector")]
    public class LaserDetector : Solid
    {
        private Sprite baseSprite;

        private Sprite leftSensorSprite;

        private Sprite rightSensorSprite;

        private Sprite topSensorSprite;

        private Sprite bottomSensorSprite;

        private string sides;

        public string flag;

        public bool isActive;

        public LaserDetector(EntityData data, Vector2 offset) : base(data.Position + offset, 8, 8, safe: true)
        {
            Tag = Tags.TransitionUpdate;
            sides = data.Attr("sides");
            flag = data.Attr("flag");
            Add(baseSprite = new Sprite(GFX.Game, data.Attr("directory") + "/"));
            baseSprite.Add("baseInactive", "baseInactive", 0.2f);
            baseSprite.Add("baseActive", "baseActive", 0.2f);
            baseSprite.Play("baseInactive");
            Add(leftSensorSprite = new Sprite(GFX.Game, data.Attr("directory") + "/"));
            leftSensorSprite.Add("sensor", "sensor", 0.2f);
            Add(rightSensorSprite = new Sprite(GFX.Game, data.Attr("directory") + "/"));
            rightSensorSprite.Add("sensor", "sensor", 0.2f);
            Add(topSensorSprite = new Sprite(GFX.Game, data.Attr("directory") + "/"));
            topSensorSprite.Add("sensor", "sensor", 0.2f);
            Add(bottomSensorSprite = new Sprite(GFX.Game, data.Attr("directory") + "/"));
            bottomSensorSprite.Add("sensor", "sensor", 0.2f);
            leftSensorSprite.Rotation = -(float)Math.PI / 2f;
            leftSensorSprite.Position = new Vector2(-8f, 8f);
            rightSensorSprite.Rotation = (float)Math.PI / 2f;
            rightSensorSprite.Position = new Vector2(16f, 0f);
            topSensorSprite.Position = new Vector2(0f, -8f);
            bottomSensorSprite.Rotation = (float)Math.PI;
            bottomSensorSprite.Position = new Vector2(8f, 16f);
            if (sides.Contains("Left"))
            {
                leftSensorSprite.Play("sensor");
            }
            if (sides.Contains("Right"))
            {
                rightSensorSprite.Play("sensor");
            }
            if (sides.Contains("Top"))
            {
                topSensorSprite.Play("sensor");
            }
            if (sides.Contains("Bottom"))
            {
                bottomSensorSprite.Play("sensor");
            }
            Add(new LightOcclude(0.5f));
            Depth = -9998;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            if (SceneAs<Level>().Tracker.GetEntities<LaserDetectorManager>().Count == 0)
            {
                SceneAs<Level>().Add(new LaserDetectorManager());
            }
        }

        public override void Update()
        {
            base.Update();
            if (!string.IsNullOrEmpty(flag))
            {
                foreach (LaserBeam beam in SceneAs<Level>().Tracker.GetEntities<LaserBeam>())
                {
                    if ((sides.Contains("Left") && beam.Top > Top + 2 && beam.Bottom < Bottom - 2 && beam.Right < Right && beam.Right > Left) || (sides.Contains("Right") && beam.Top > Top + 2 && beam.Bottom < Bottom - 2 && beam.Left > Left && beam.Left < Right) || (sides.Contains("Top") && beam.Left > Left + 2 && beam.Right < Right - 2 && beam.Bottom < Bottom && beam.Bottom > Top) || (sides.Contains("Bottom") && beam.Left > Left + 2 && beam.Right < Right - 2 && beam.Top > Top && beam.Top < Bottom))
                    {
                        //SceneAs<Level>().Session.SetFlag(flag, true);
                        isActive = true;
                        baseSprite.Play("baseActive");
                        return;
                    }
                }
                //SceneAs<Level>().Session.SetFlag(flag, false);
                isActive = false;
                baseSprite.Play("baseInactive");
            }
        }
    }
}
