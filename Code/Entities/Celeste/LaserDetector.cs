using System;
using Celeste.Mod.Entities;
using Celeste.Mod.XaphanHelper.Managers;
using Microsoft.Xna.Framework;
using Monocle;

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

        public bool shouldActive;

        public bool active;

        public override void Update()
        {
            base.Update();
            if (!string.IsNullOrEmpty(flag))
            {
                LaserDetectorManager manager = SceneAs<Level>().Tracker.GetEntity<LaserDetectorManager>();
                if (manager != null)
                {
                    foreach (LaserBeam beam in SceneAs<Level>().Tracker.GetEntities<LaserBeam>())
                    {
                        if ((sides.Contains("Left") && beam.Top > Top + 2 && beam.Bottom < Bottom - 2 && beam.Right < Right && beam.Right > Left) || (sides.Contains("Right") && beam.Top > Top + 2 && beam.Bottom < Bottom - 2 && beam.Left > Left && beam.Left < Right) || (sides.Contains("Top") && beam.Left > Left + 2 && beam.Right < Right - 2 && beam.Bottom < Bottom && beam.Bottom > Top) || (sides.Contains("Bottom") && beam.Left > Left + 2 && beam.Right < Right - 2 && beam.Top > Top && beam.Top < Bottom))
                        {
                            if (!manager.activeDetectors.Contains(this))
                            {
                                manager.activeDetectors.Add(this);

                            }
                            if (manager.inactiveDetectors.Contains(this))
                            {
                                manager.inactiveDetectors.Remove(this);
                            }
                            manager.GetDetectorsFlags();
                            baseSprite.Play("baseActive");
                            return;
                        }
                    }
                    if (!manager.inactiveDetectors.Contains(this))
                    {
                        manager.inactiveDetectors.Add(this);

                    }
                    if (manager.activeDetectors.Contains(this))
                    {
                        manager.activeDetectors.Remove(this);
                    }
                    manager.GetDetectorsFlags();
                    baseSprite.Play("baseInactive");
                }
            }
        }
    }
}
