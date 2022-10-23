using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [Tracked(true)]
    [CustomEntity("XaphanHelper/BreakBlockIndicator")]
    public class BreakBlockIndicator : Entity
    {
        public Sprite blockType;

        private bool broken;

        private bool breakBlockAlreadyBroken = true;

        private bool startRevealed;

        public int index;

        public string mode;

        private string color;

        private string directory;

        private EntityID eid;

        private bool autoAdded;

        public BreakBlockIndicator(EntityData data, Vector2 offset, EntityID id) : base(data.Position + offset)
        {
            eid = id;
            mode = data.Attr("mode");
            color = data.Attr("color");
            index = data.Int("index");
            startRevealed = data.Bool("startRevealed");
            directory = data.Attr("directory");
            if (string.IsNullOrEmpty(directory))
            {
                directory = "objects/XaphanHelper/BreakBlock";
            }
            Collider = new Hitbox(8f, 8f, 0f, 0f);
            Add(new PlayerCollider(OnPlayer, new Hitbox(10f, 12f, -1f, -3f)));
            Add(new PlayerCollider(OnPlayerBooster, new Hitbox(16f, 16f, -4f, -4f)));
            Add(blockType = new Sprite(GFX.Game, directory + "/"));
            blockType.AddLoop("bomb", "Bomb", 1f);
            blockType.AddLoop("megaBomb", "MegaBomb", 1f);
            blockType.AddLoop("lightningDash", "LightningDash", 1f);
            blockType.AddLoop("redBooster", "RedBooster", 1f);
            blockType.AddLoop("drone", "Drone", 1f);
            blockType.AddLoop("screwAttack", "ScrewAttack", 1f);
            Depth = -13001;
        }

        public BreakBlockIndicator(EntityID id, bool autoAdded, Vector2 position, string mode, string color, bool startRevealed, string directory)
        {
            eid = id;
            this.autoAdded = autoAdded;
            Position = position;
            this.mode = mode;
            this.color = color;
            this.startRevealed = startRevealed;
            this.directory = directory;
            if (string.IsNullOrEmpty(directory))
            {
                directory = "objects/XaphanHelper/BreakBlock";
            }
            Collider = new Hitbox(8f, 8f, 0f, 0f);
            Add(new PlayerCollider(OnPlayer, new Hitbox(10f, 12f, -1f, -3f)));
            Add(new PlayerCollider(OnPlayerBooster, new Hitbox(16f, 16f, -4f, -4f)));
            Add(blockType = new Sprite(GFX.Game, directory + "/"));
            blockType.AddLoop("bomb", "Bomb", 1f);
            blockType.AddLoop("megaBomb", "MegaBomb", 1f);
            blockType.AddLoop("lightningDash", "LightningDash", 1f);
            blockType.AddLoop("redBooster", "RedBooster", 1f);
            blockType.AddLoop("drone", "Drone", 1f);
            blockType.AddLoop("screwAttack", "ScrewAttack", 1f);
            Depth = -13001;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            if (CollideCheck<SolidTiles>())
            {
                RemoveSelf();
            }
            if (CollideCheck<Player>())
            {
                foreach (BreakBlockIndicator breakblock in Scene.Entities.FindAll<BreakBlockIndicator>())
                {
                    if ((!autoAdded && breakblock.index == index) || (breakblock.eid.ID == eid.ID && breakblock.eid.Level == eid.Level))
                    {
                        breakblock.RemoveSelf();
                    }
                }
            }
            else
            {
                Collidable = (Visible = true);
            }
            if (startRevealed)
            {
                RevealSequence();
            }
        }

        public override void Update()
        {
            base.Update();
            foreach (BreakBlock breakblock in Scene.Entities.FindAll<BreakBlock>())
            {
                if ((!autoAdded && breakblock.index == index) || (breakblock.eid.ID == eid.ID && breakblock.eid.Level == eid.Level))
                {
                    breakBlockAlreadyBroken = false;
                }
            }
            if (broken || breakBlockAlreadyBroken)
            {
                RemoveSelf();
            }
        }

        public override void Render()
        {
            blockType.Render();
        }

        public void OnPlayer(Player player)
        {
            Vector2 aim = Input.GetAimVector();
            if (player.StateMachine.State == 2 && ((player.Left >= Right && aim.X < 0) || (player.Right <= Left && aim.X > 0) || (player.Top >= Bottom && aim.Y < 0) || (player.Bottom <= Top && aim.Y > 0)))
            {
                RevealSequence();
            }
            if (mode == "LightningDash" && SceneAs<Level>().Session.GetFlag("Xaphan_Helper_Shinesparking"))
            {
                BreakSequence();
            }
        }

        public void OnPlayerBooster(Player player)
        {
            if (mode == "RedBooster" && player.StateMachine.State == 5)
            {
                BreakSequence();
            }
        }

        public void BreakSequence()
        {
            foreach (BreakBlockIndicator indicator in SceneAs<Level>().Entities.FindAll<BreakBlockIndicator>())
            {
                if ((!autoAdded && indicator.index == index) || (indicator.eid.ID == eid.ID && indicator.eid.Level == eid.Level))
                {
                    indicator.blockType.RemoveSelf();
                    indicator.broken = true;
                    indicator.Collidable = false;
                    if (!autoAdded)
                    {
                        SceneAs<Level>().Session.DoNotLoad.Add(eid);
                    }
                }
            }
            foreach (BreakBlock breakblock in SceneAs<Level>().Entities.FindAll<BreakBlock>())
            {
                if ((!autoAdded && breakblock.index == index) || (breakblock.eid.ID == eid.ID && breakblock.eid.Level == eid.Level))
                {
                    breakblock.Break();
                }
            }
        }

        public void RevealSequence()
        {
            if (mode == "LightningDash")
            {
                blockType.Play("lightningDash");
            }
            else if (mode == "Bomb")
            {
                blockType.Play("bomb");
            }
            else if (mode == "MegaBomb")
            {
                blockType.Play("megaBomb");
            }
            else if (mode == "RedBooster")
            {
                blockType.Play("redBooster");
            }
            else if (mode == "Drone")
            {
                blockType.Play("drone");
            }
            else if (mode == "ScrewAttack")
            {
                blockType.Play("screwAttack");
            }
        }
    }
}
