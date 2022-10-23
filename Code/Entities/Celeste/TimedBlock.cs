using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [Tracked(true)]
    [CustomEntity("XaphanHelper/TimedBlock")]
    class TimedBlock : Solid
    {
        public int Index;

        public string directory;

        private List<TimedBlock> group;

        private bool groupLeader;

        private Vector2 groupOrigin;

        private Color color;

        private List<Image> pressed = new List<Image>();

        private List<Image> solid = new List<Image>();

        private List<Image> all = new List<Image>();

        private LightOcclude occluder;

        private Wiggler wiggler;

        private Vector2 wigglerScaler;

        public TimedBlock(EntityData data, Vector2 position) : base(data.Position + position, data.Width, data.Height, safe: true)
        {
            Tag = Tags.TransitionUpdate;
            SurfaceSoundIndex = data.Int("soundIndex", 35);
            if (data.Bool("startPressed"))
            {
                Index = 1;
            }
            else
            {
                Index = 0;
            }
            directory = data.Attr("directory");
            if (string.IsNullOrEmpty(directory))
            {
                directory = "objects/XaphanHelper/TimedBlock";
            }
            Collidable = false;
            color = Calc.HexToColor(data.Attr("color"));
            Add(occluder = new LightOcclude());
        }

        public int currentIndex = 0;

        public void setCurrentIndex(int index)
        {
            currentIndex = index;
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            Color color = Calc.HexToColor("667da5");
            Color disabledColor = new Color(color.R / 255f * (this.color.R / 255f), color.G / 255f * (this.color.G / 255f), color.B / 255f * (this.color.B / 255f), 1f);
            foreach (StaticMover staticMover in staticMovers)
            {
                Spikes spikes = staticMover.Entity as Spikes;
                if (spikes != null)
                {
                    spikes.EnabledColor = this.color;
                    spikes.DisabledColor = disabledColor;
                    spikes.VisibleWhenDisabled = true;
                    spikes.SetSpikeColor(this.color);
                }
                Spring spring = staticMover.Entity as Spring;
                if (spring != null)
                {
                    spring.DisabledColor = disabledColor;
                    spring.VisibleWhenDisabled = true;
                }
            }
            if (group == null)
            {
                groupLeader = true;
                group = new List<TimedBlock>();
                group.Add(this);
                FindInGroup(this);
                float num = float.MaxValue;
                float num2 = float.MinValue;
                float num3 = float.MaxValue;
                float num4 = float.MinValue;
                foreach (TimedBlock item in group)
                {
                    if (item.Left < num)
                    {
                        num = item.Left;
                    }
                    if (item.Right > num2)
                    {
                        num2 = item.Right;
                    }
                    if (item.Bottom > num4)
                    {
                        num4 = item.Bottom;
                    }
                    if (item.Top < num3)
                    {
                        num3 = item.Top;
                    }
                }
                groupOrigin = new Vector2((int)(num + (num2 - num) / 2f), (int)num4);
                wigglerScaler = new Vector2(Calc.ClampedMap(num2 - num, 32f, 96f, 1f, 0.2f), Calc.ClampedMap(num4 - num3, 32f, 96f, 1f, 0.2f));
                Add(wiggler = Wiggler.Create(0.3f, 3f));
                foreach (TimedBlock item2 in group)
                {
                    item2.wiggler = wiggler;
                    item2.wigglerScaler = wigglerScaler;
                    item2.groupOrigin = groupOrigin;
                }
            }
            foreach (StaticMover staticMover2 in staticMovers)
            {
                (staticMover2.Entity as Spikes)?.SetOrigins(groupOrigin);
            }
            for (float num5 = Left; num5 < Right; num5 += 8f)
            {
                for (float num6 = Top; num6 < Bottom; num6 += 8f)
                {
                    bool flag = CheckForSame(num5 - 8f, num6);
                    bool flag2 = CheckForSame(num5 + 8f, num6);
                    bool flag3 = CheckForSame(num5, num6 - 8f);
                    bool flag4 = CheckForSame(num5, num6 + 8f);
                    if ((flag && flag2) & flag3 & flag4)
                    {
                        if (!CheckForSame(num5 + 8f, num6 - 8f))
                        {
                            SetImage(num5, num6, 3, 0);
                        }
                        else if (!CheckForSame(num5 - 8f, num6 - 8f))
                        {
                            SetImage(num5, num6, 3, 1);
                        }
                        else if (!CheckForSame(num5 + 8f, num6 + 8f))
                        {
                            SetImage(num5, num6, 3, 2);
                        }
                        else if (!CheckForSame(num5 - 8f, num6 + 8f))
                        {
                            SetImage(num5, num6, 3, 3);
                        }
                        else
                        {
                            SetImage(num5, num6, 1, 1);
                        }
                    }
                    else if ((flag && flag2 && !flag3) & flag4)
                    {
                        SetImage(num5, num6, 1, 0);
                    }
                    else if (((flag && flag2) & flag3) && !flag4)
                    {
                        SetImage(num5, num6, 1, 2);
                    }
                    else if ((flag && !flag2) & flag3 & flag4)
                    {
                        SetImage(num5, num6, 2, 1);
                    }
                    else if ((!flag && flag2) & flag3 & flag4)
                    {
                        SetImage(num5, num6, 0, 1);
                    }
                    else if ((flag && !flag2 && !flag3) & flag4)
                    {
                        SetImage(num5, num6, 2, 0);
                    }
                    else if ((!flag && flag2 && !flag3) & flag4)
                    {
                        SetImage(num5, num6, 0, 0);
                    }
                    else if (((flag && !flag2) & flag3) && !flag4)
                    {
                        SetImage(num5, num6, 2, 2);
                    }
                    else if (((!flag && flag2) & flag3) && !flag4)
                    {
                        SetImage(num5, num6, 0, 2);
                    }
                }
            }
            UpdateVisualState();
        }

        private void FindInGroup(TimedBlock block)
        {
            foreach (TimedBlock entity in base.Scene.Tracker.GetEntities<TimedBlock>())
            {
                if (entity != this && entity != block && entity.Index == Index && (entity.CollideRect(new Rectangle((int)block.X - 1, (int)block.Y, (int)block.Width + 2, (int)block.Height)) || entity.CollideRect(new Rectangle((int)block.X, (int)block.Y - 1, (int)block.Width, (int)block.Height + 2))) && !group.Contains(entity))
                {
                    group.Add(entity);
                    FindInGroup(entity);
                    entity.group = group;
                }
            }
        }

        private bool CheckForSame(float x, float y)
        {
            foreach (TimedBlock entity in Scene.Tracker.GetEntities<TimedBlock>())
            {
                if (entity.Index == Index && entity.Collider.Collide(new Rectangle((int)x, (int)y, 8, 8)))
                {
                    return true;
                }
            }
            return false;
        }

        private void SetImage(float x, float y, int tx, int ty)
        {
            List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures("objects/XaphanHelper/TimedBlock/pressed");
            pressed.Add(CreateImage(x, y, tx, ty, atlasSubtextures[Index % atlasSubtextures.Count]));
            solid.Add(CreateImage(x, y, tx, ty, GFX.Game["objects/XaphanHelper/TimedBlock/solid"]));
        }

        private Image CreateImage(float x, float y, int tx, int ty, MTexture tex)
        {
            Vector2 value = new Vector2(x - X, y - Y);
            Image image = new Image(tex.GetSubtexture(tx * 8, ty * 8, 8, 8));
            Vector2 vector = groupOrigin - Position;
            image.Origin = vector - value;
            image.Position = vector;
            image.Color = color;
            Add(image);
            all.Add(image);
            return image;
        }

        public override void Update()
        {
            base.Update();
            if (!SceneAs<Level>().Transitioning)
            {
                Tag = 0;
            }
            if (currentIndex != Index)
            {
                DisableStaticMovers();
            }
            if (groupLeader && currentIndex == Index && !Collidable)
            {
                bool flag = false;
                foreach (TimedBlock item in group)
                {
                    if (item.BlockedCheck())
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    foreach (TimedBlock item2 in group)
                    {
                        item2.Collidable = true;
                        item2.EnableStaticMovers();
                    }
                    if (!SceneAs<Level>().Transitioning)
                    {
                        wiggler.Start();
                    }
                }
            }
            else if (currentIndex != Index && Collidable)
            {
                Collidable = false;
                DisableStaticMovers();
            }
            UpdateVisualState();
        }

        public bool BlockedCheck()
        {
            TheoCrystal theoCrystal = CollideFirst<TheoCrystal>();
            if (theoCrystal != null && !TryActorWiggleUp(theoCrystal))
            {
                return true;
            }
            Player player = CollideFirst<Player>();
            if (player != null && !TryActorWiggleUp(player))
            {
                return true;
            }
            return false;
        }

        private void UpdateVisualState()
        {
            if (!Collidable)
            {
                Depth = 8990;
            }
            else
            {
                Player entity = Scene.Tracker.GetEntity<Player>();
                if (entity != null && entity.Top >= Bottom - 1f)
                {
                    Depth = 10;
                }
                else
                {
                    Depth = -10;
                }
            }
            foreach (StaticMover staticMover in staticMovers)
            {
                staticMover.Entity.Depth = Depth + 1;
            }
            occluder.Visible = Collidable;
            foreach (Image item in solid)
            {
                item.Visible = Collidable;
            }
            foreach (Image item2 in pressed)
            {
                item2.Visible = !Collidable;
            }
            if (groupLeader)
            {
                Vector2 scale = new Vector2(1f + wiggler.Value * 0.05f * wigglerScaler.X, 1f + wiggler.Value * 0.15f * wigglerScaler.Y);
                foreach (TimedBlock item3 in group)
                {
                    foreach (Image item4 in item3.all)
                    {
                        item4.Scale = scale;
                    }
                    foreach (StaticMover staticMover2 in item3.staticMovers)
                    {
                        Spikes spikes = staticMover2.Entity as Spikes;
                        if (spikes != null)
                        {
                            foreach (Component component in spikes.Components)
                            {
                                Image image = component as Image;
                                if (image != null)
                                {
                                    image.Scale = scale;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void WillToggle()
        {
            UpdateVisualState();
        }

        private bool TryActorWiggleUp(Entity actor)
        {
            foreach (TimedBlock item in group)
            {
                if (item != this && item.CollideCheck(actor, item.Position + Vector2.UnitY * 4f))
                {
                    return false;
                }
            }
            bool collidable = Collidable;
            Collidable = true;
            for (int i = 1; i <= 4; i++)
            {
                if (!actor.CollideCheck<Solid>(actor.Position - Vector2.UnitY * i))
                {
                    actor.Position -= Vector2.UnitY * i;
                    Collidable = collidable;
                    return true;
                }
            }
            Collidable = collidable;
            return false;
        }
    }
}
