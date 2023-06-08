using System.Collections;
using System.Collections.Generic;
using Celeste.Mod.Entities;
using Celeste.Mod.XaphanHelper.Controllers;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [Tracked(true)]
    [CustomEntity("XaphanHelper/JumpBlock")]
    class JumpBlock : Solid
    {
        public int Index;

        public string directory;

        private List<JumpBlock> group;

        private bool groupLeader;

        private Vector2 groupOrigin;

        private Color color;

        private Color disabledColor;

        private List<Image> black = new();

        private List<Image> pressed = new();

        private List<Image> solid = new();

        private List<Image> all = new();

        private LightOcclude occluder;

        private Wiggler wiggler;

        private Vector2 wigglerScaler;

        public float alpha;

        private string switchType;

        private bool improvedTileset;

        private bool onlyOneTileset;

        private Coroutine AlphaRoutine = new();

        public JumpBlock(EntityData data, Vector2 position) : base(data.Position + position, data.Width, data.Height, safe: true)
        {
            Tag = Tags.TransitionUpdate;
            SurfaceSoundIndex = data.Int("soundIndex", 35);
            Index = data.Int("index");
            directory = data.Attr("directory");
            if (string.IsNullOrEmpty(directory))
            {
                directory = "objects/XaphanHelper/JumpBlock";
            }
            color = Calc.HexToColor(data.Attr("color"));
            Collidable = false;
            switchType = data.Attr("switchType", "Wiggle");
            improvedTileset = data.Bool("improvedTileset", false);
            onlyOneTileset = data.Bool("onlyOneTileset", false);
            Add(occluder = new LightOcclude());
        }

        public static void Load()
        {
            On.Celeste.Player.Jump += onPlayerJump;
            On.Celeste.Player.SuperJump += onPlayerSuperJump;
            On.Celeste.Player.WallJump += onPlayerWallJump;
            On.Celeste.Player.SuperWallJump += onPlayerSuperWallJump;
        }

        public static void Unload()
        {
            On.Celeste.Player.Jump -= onPlayerJump;
            On.Celeste.Player.SuperJump -= onPlayerSuperJump;
            On.Celeste.Player.WallJump -= onPlayerWallJump;
            On.Celeste.Player.SuperWallJump -= onPlayerSuperWallJump;
        }

        public int currentIndex = -1;

        public void setCurrentIndex(int index)
        {
            currentIndex = index;
        }

        public void setAlpha(bool inverted)
        {
            Add(AlphaRoutine = new Coroutine(ChangeAlphaRoutine(inverted)));
        }

        private IEnumerator ChangeAlphaRoutine(bool inverted)
        {
            float timer = 0.15f;
            if (!inverted)
            {
                while (timer > 0)
                {
                    alpha -= 5 * Engine.DeltaTime;
                    timer -= Engine.DeltaTime;
                    yield return null;
                }
                alpha = 0.25f;
            }
            else
            {
                while (timer > 0)
                {
                    alpha += 5 * Engine.DeltaTime;
                    timer -= Engine.DeltaTime;
                    yield return null;
                }
                alpha = 1f;
            }
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            List<int> indexes = new();
            if (SceneAs<Level>().Tracker.GetEntities<JumpBlock>().Count != 0)
            {
                AreaKey area = SceneAs<Level>().Session.Area;
                MapData MapData = AreaData.Areas[area.ID].Mode[(int)area.Mode].MapData;
                foreach (LevelData levelData in MapData.Levels)
                {
                    if (levelData.Name == SceneAs<Level>().Session.Level)
                    {
                        foreach (EntityData entity in levelData.Entities)
                        {
                            if (entity.Name == "XaphanHelper/JumpBlock")
                            {
                                if (!indexes.Contains(entity.Int("index")))
                                {
                                    indexes.Add(entity.Int("index"));
                                }
                            }
                        }
                        break;
                    }
                }
                if (indexes.Count != 0)
                {
                    indexes.Sort();
                    foreach (JumpBlock jumpblock in SceneAs<Level>().Tracker.GetEntities<JumpBlock>())
                    {
                        setCurrentIndex(indexes[0]);
                    }
                    if (indexes.Count > 1)
                    {
                        alpha = Index == currentIndex ? 1f : 0.25f;
                    }
                    else
                    {
                        alpha = 1f;
                    }
                }
            }
            Color colorMask = Calc.HexToColor("667da5");
            disabledColor = new(colorMask.R / 255f * (color.R / 255f), colorMask.G / 255f * (color.G / 255f), colorMask.B / 255f * (color.B / 255f), alpha);
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            foreach (StaticMover staticMover in staticMovers)
            {
                Spikes spikes = staticMover.Entity as Spikes;
                if (spikes != null)
                {
                    spikes.EnabledColor = color;
                    spikes.DisabledColor = disabledColor;
                    spikes.VisibleWhenDisabled = true;
                    spikes.SetSpikeColor(color);
                }
                Spring spring = staticMover.Entity as Spring;
                if (spring != null)
                {
                    spring.DisabledColor = disabledColor;
                    spring.VisibleWhenDisabled = true;
                }
                FlagDashSwitch flagSwitch = staticMover.Entity as FlagDashSwitch;
                if (flagSwitch != null)
                {
                    flagSwitch.DisabledColor = disabledColor;
                    flagSwitch.VisibleWhenDisabled = true;
                }
            }
            if (group == null)
            {
                groupLeader = true;
                group = new List<JumpBlock>();
                group.Add(this);
                FindInGroup(this);
                float num = float.MaxValue;
                float num2 = float.MinValue;
                float num3 = float.MaxValue;
                float num4 = float.MinValue;
                foreach (JumpBlock jumpBlock in group)
                {
                    if (jumpBlock.Left < num)
                    {
                        num = jumpBlock.Left;
                    }
                    if (jumpBlock.Right > num2)
                    {
                        num2 = jumpBlock.Right;
                    }
                    if (jumpBlock.Bottom > num4)
                    {
                        num4 = jumpBlock.Bottom;
                    }
                    if (jumpBlock.Top < num3)
                    {
                        num3 = jumpBlock.Top;
                    }
                }
                groupOrigin = new Vector2((int)(num + (num2 - num) / 2f), (int)(num3 + (num4 - num3) / 2f));
                wigglerScaler = new Vector2(Calc.ClampedMap(num2 - num, 32f, 96f, 1f, 0.2f), Calc.ClampedMap(num4 - num3, 32f, 96f, 1f, 0.2f));
                Add(wiggler = Wiggler.Create(0.3f, 3f));
                foreach (JumpBlock jumpBlock in group)
                {
                    jumpBlock.wiggler = wiggler;
                    jumpBlock.wigglerScaler = wigglerScaler;
                    jumpBlock.groupOrigin = groupOrigin;
                }
            }
            foreach (StaticMover staticMover in staticMovers)
            {
                (staticMover.Entity as Spikes)?.SetOrigins(groupOrigin);
            }
            for (float num5 = Left; num5 < Right; num5 += 8f)
            {
                for (float num6 = Top; num6 < Bottom; num6 += 8f)
                {
                    bool flag = CheckForSame(num5 - 8f, num6);
                    bool flag2 = CheckForSame(num5 + 8f, num6);
                    bool flag3 = CheckForSame(num5, num6 - 8f);
                    bool flag4 = CheckForSame(num5, num6 + 8f);
                    if (improvedTileset)
                    {
                        if ((flag && flag2) & flag3 & flag4)
                        {
                            if (!CheckForSame(num5 + 8f, num6 - 8f))
                            {
                                SetImage(num5, num6, 0, 8);
                            }
                            else if (!CheckForSame(num5 - 8f, num6 - 8f))
                            {
                                SetImage(num5, num6, 1, 8);
                            }
                            else if (!CheckForSame(num5 + 8f, num6 + 8f))
                            {
                                SetImage(num5, num6, 2, 8);
                            }
                            else if (!CheckForSame(num5 - 8f, num6 + 8f))
                            {
                                SetImage(num5, num6, 3, 8);
                            }
                            else
                            {
                                SetImage(num5, num6, 0 + Calc.Random.Next(4), 9);
                            }
                        }
                        else if ((flag && flag2 && !flag3) & flag4)
                        {
                            SetImage(num5, num6, 0 + Calc.Random.Next(4), 0);
                        }
                        else if (((flag && flag2) & flag3) && !flag4)
                        {
                            SetImage(num5, num6, 0 + Calc.Random.Next(4), 1);
                        }
                        else if ((flag && !flag2) & flag3 & flag4)
                        {
                            SetImage(num5, num6, 0 + Calc.Random.Next(4), 3);
                        }
                        else if ((!flag && flag2) & flag3 & flag4)
                        {
                            SetImage(num5, num6, 0 + Calc.Random.Next(4), 2);
                        }
                        else if ((flag && !flag2 && !flag3) & flag4)
                        {
                            SetImage(num5, num6, 0 + Calc.Random.Next(4), 5);
                        }
                        else if ((!flag && flag2 && !flag3) & flag4)
                        {
                            SetImage(num5, num6, 0 + Calc.Random.Next(4), 4);
                        }
                        else if (((flag && !flag2) & flag3) && !flag4)
                        {
                            SetImage(num5, num6, 0 + Calc.Random.Next(4), 7);
                        }
                        else if (((!flag && flag2) & flag3) && !flag4)
                        {
                            SetImage(num5, num6, 0 + Calc.Random.Next(4), 6);
                        }
                    }
                    else
                    {
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
            }
            UpdateVisualState();
        }

        private void FindInGroup(JumpBlock block)
        {
            foreach (JumpBlock entity in Scene.Tracker.GetEntities<JumpBlock>())
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
            foreach (JumpBlock entity in Scene.Tracker.GetEntities<JumpBlock>())
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
            if (onlyOneTileset)
            {
                black.Add(CreateImage(x, y, tx, ty, GFX.Game[directory + "/solid" + (improvedTileset ? "-impr" : "")]));
                solid.Add(CreateImage(x, y, tx, ty, GFX.Game[directory + "/solid" + (improvedTileset ? "-impr" : "")]));
                pressed.Add(CreateImage(x, y, tx, ty, GFX.Game[directory + "/solid" + (improvedTileset ? "-impr" : "")]));
            }
            else
            {
                List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures(directory + "/pressed" + (improvedTileset ? "-impr" : ""));
                black.Add(CreateImage(x, y, tx, ty, atlasSubtextures[Index % 2 != 0 ? 0 : 1]));
                pressed.Add(CreateImage(x, y, tx, ty, atlasSubtextures[Index % 2 != 0 ? 0 : 1]));
                solid.Add(CreateImage(x, y, tx, ty, GFX.Game[directory + "/solid" + (improvedTileset ? "-impr" : "")]));
            }
        }

        private Image CreateImage(float x, float y, int tx, int ty, MTexture tex)
        {
            Vector2 value = new(x - X, y - Y);
            Image image = new(tex.GetSubtexture(tx * 8, ty * 8, 8, 8));
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
                foreach (JumpBlock block in group)
                {
                    if (block.BlockedCheck())
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    foreach (JumpBlock block in group)
                    {
                        block.Collidable = true;
                        block.EnableStaticMovers();
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
                if (Depth != 8000)
                {
                    Depth = 8000;
                }
                if (switchType == "Fade" && alpha > 0.25f && !AlphaRoutine.Active)
                {
                    setAlpha(false);
                }
            }
            else
            {
                if (Depth != -8000)
                {
                    Depth = -8000;
                }
                if (switchType == "Fade" && alpha != 1f && !AlphaRoutine.Active)
                {
                    setAlpha(true);
                }
            }
            foreach (StaticMover staticMover in staticMovers)
            {
                staticMover.Entity.Depth = Depth + 1;
            }
            occluder.Visible = Collidable;
            foreach (Image image in solid)
            {
                image.Visible = Collidable;
                if (switchType == "Fade")
                {
                    image.Color = color * alpha;
                }
            }
            foreach (Image image in pressed)
            {
                image.Visible = !Collidable;
                if (switchType == "Fade")
                {
                    image.Color = color * alpha;
                }
            }
            if (switchType == "Fade")
            {
                foreach (Image image in black)
                {
                    image.Visible = (alpha != 1f);
                    image.Color = Color.Black;
                }
            }
            if (groupLeader)
            {
                Vector2 scale = new();
                if (switchType == "Wiggle")
                {
                    scale = new(1f + wiggler.Value * 0.05f * wigglerScaler.X, 1f + wiggler.Value * 0.15f * wigglerScaler.Y);
                }
                else if (switchType == "Fade")
                {
                    scale = new(1f);
                }
                foreach (JumpBlock jumpBlock in group)
                {
                    jumpBlock.alpha = alpha;
                    foreach (Image image in jumpBlock.all)
                    {
                        image.Scale = scale;
                    }
                    foreach (StaticMover staticMover in jumpBlock.staticMovers)
                    {
                        Spikes spikes = staticMover.Entity as Spikes;
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
                        FlagDashSwitch flagSwitch = staticMover.Entity as FlagDashSwitch;
                        if (flagSwitch != null)
                        {
                            foreach (Component component in flagSwitch.Components)
                            {
                                Sprite sprite = component as Sprite;
                                if (sprite != null)
                                {
                                    sprite.Scale = scale;
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
            foreach (JumpBlock item in group)
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

        private static void jumpCheck(Player player)
        {
            string jumpOnSound = "event:/game/general/cassette_block_switch_1";
            string jumpOffSound = "event:/game/general/cassette_block_switch_2";
            if (player.SceneAs<Level>().Tracker.GetEntities<JumpBlocksFlipSoundController>().Count > 0)
            {
                JumpBlocksFlipSoundController controller = player.SceneAs<Level>().Tracker.GetEntity<JumpBlocksFlipSoundController>();
                jumpOnSound = controller.onSound;
                jumpOffSound = controller.offSound;
            }
            if (player.SceneAs<Level>().Tracker.GetEntities<JumpBlock>().Count > 0)
            {
                List<int> indexes = new();
                int currentIndex = -1;
                foreach (JumpBlock jumpblock in player.SceneAs<Level>().Tracker.GetEntities<JumpBlock>())
                {
                    if (!indexes.Contains(jumpblock.Index))
                    {
                        indexes.Add(jumpblock.Index);
                    }
                    currentIndex = jumpblock.currentIndex;
                }
                indexes.Sort();
                if (indexes.Count == 1)
                {
                    int soundID = 1;
                    foreach (JumpBlock jumpblock in player.SceneAs<Level>().Tracker.GetEntities<JumpBlock>())
                    {
                        if (jumpblock.currentIndex == -1)
                        {
                            jumpblock.setCurrentIndex(jumpblock.Index);
                            soundID = 2;
                        }
                        else
                        {
                            jumpblock.setCurrentIndex(-1);
                            soundID = 1;
                        }
                    }
                    _ = soundID == 1 ? Audio.Play(jumpOnSound) : Audio.Play(jumpOffSound);
                }
                else
                {
                    int listindex = indexes.IndexOf(currentIndex);
                    if (listindex + 1 > indexes.Count - 1)
                    {
                        listindex = -1;
                    }
                    _ = (listindex % 2 != 0) ? Audio.Play(jumpOnSound) : Audio.Play(jumpOffSound);
                    foreach (JumpBlock jumpblock in player.SceneAs<Level>().Tracker.GetEntities<JumpBlock>())
                    {
                        jumpblock.setCurrentIndex(indexes[listindex + 1]);
                    }
                }
            }
        }

        private static void onPlayerJump(On.Celeste.Player.orig_Jump orig, Player self, bool particles, bool playSfx)
        {
            jumpCheck(self);
            orig(self, particles, playSfx);
        }

        private static void onPlayerSuperJump(On.Celeste.Player.orig_SuperJump orig, Player self)
        {
            jumpCheck(self);
            orig(self);
        }

        private static void onPlayerWallJump(On.Celeste.Player.orig_WallJump orig, Player self, int dir)
        {
            jumpCheck(self);
            orig(self, dir);
        }

        private static void onPlayerSuperWallJump(On.Celeste.Player.orig_SuperWallJump orig, Player self, int dir)
        {
            jumpCheck(self);
            orig(self, dir);
        }
    }
}
