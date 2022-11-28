using Celeste.Mod.Entities;
using Celeste.Mod.XaphanHelper.Upgrades;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [Tracked(true)]
    [CustomEntity("XaphanHelper/EtherealBlock")]
    class EtherealBlock : Entity
    {
        private TileGrid tiles;

        private static char fillTile;

        private PlayerCollider pc;

        public static bool PreventRefill;

        public static bool HadSpaceJump;

        protected static XaphanModuleSettings Settings => XaphanModule.Settings;

        public EtherealBlock(EntityData data, Vector2 position) : base(data.Position + position)
        {
            fillTile = data.Char("tiletype", '3');
            Collider = new Hitbox(data.Width, data.Height);
            Add(pc = new PlayerCollider(onCollide, Collider));
            Add(new LightOcclude());
            Depth = -13000;
        }

        private void onCollide(Player player)
        {
            PreventRefill = true;
            if (SpaceJump.Active(player.SceneAs<Level>()) && SpaceJump.GetJumpBuffer() != 1)
            {
                HadSpaceJump = true;
                Settings.SpaceJump = 1;
            }
        }

        public static void Load()
        {
            On.Celeste.DreamBlock.FootstepRipple += onDreamBlockFootstepRipple;
            On.Celeste.DreamBlock.Update += onDreamBlockUpdate;
            On.Celeste.Player.RefillDash += modRefillDash;
        }

        public static void Unload()
        {
            On.Celeste.DreamBlock.FootstepRipple -= onDreamBlockFootstepRipple;
            On.Celeste.DreamBlock.Update -= onDreamBlockUpdate;
            On.Celeste.Player.RefillDash -= modRefillDash;
        }

        private static void onDreamBlockFootstepRipple(On.Celeste.DreamBlock.orig_FootstepRipple orig, DreamBlock self, Vector2 position)
        {
            if (self.SceneAs<Level>().Tracker.GetEntities<EtherealBlock>().Count > 0)
            {
                foreach (EtherealBlock etherealBlock in self.SceneAs<Level>().Tracker.GetEntities<EtherealBlock>())
                {
                    if (etherealBlock.Position != self.Position || etherealBlock.Height != self.Height || etherealBlock.Width != self.Width)
                    {
                        orig(self, position);
                    }
                }
            }
            else
            {
                orig(self, position);
            }
        }

        private static void onDreamBlockUpdate(On.Celeste.DreamBlock.orig_Update orig, DreamBlock self)
        {
            orig(self);
            if (self.SceneAs<Level>().Tracker.GetEntities<EtherealBlock>().Count > 0)
            {
                foreach (EtherealBlock etherealBlock in self.SceneAs<Level>().Tracker.GetEntities<EtherealBlock>())
                {
                    if (etherealBlock.Position != self.Position || etherealBlock.Height != self.Height || etherealBlock.Width != self.Width)
                    {
                        orig(self);
                    }
                    else
                    {
                        self.SurfaceSoundIndex = SurfaceIndex.TileToIndex[fillTile];
                    }
                }
            }
        }

        private static bool modRefillDash(On.Celeste.Player.orig_RefillDash orig, Player self)
        {
            if (self.Scene != null)
            {
                if (PreventRefill && !self.OnGround())
                {
                    return false;
                }
                else
                {
                    if (self.OnGround())
                    {
                        PreventRefill = false;
                        if (HadSpaceJump)
                        {
                            Settings.SpaceJump = 2;
                        }
                    }
                }
            }
            return orig(self);
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            int tilesX = (int)Width / 8;
            int tilesY = (int)Height / 8;
            Level level = SceneAs<Level>();
            Rectangle tileBounds = level.Session.MapData.TileBounds;
            VirtualMap<char> solidsData = level.SolidsData;
            int x = (int)X / 8 - tileBounds.Left;
            int y = (int)Y / 8 - tileBounds.Top;
            tiles = GFX.FGAutotiler.GenerateOverlay(fillTile, x, y, tilesX, tilesY, solidsData).TileGrid;
            Add(tiles);
            Add(new TileInterceptor(tiles, highPriority: false));
            level.Add(new DreamBlock(Position, Width, Height, null, false, false)
            {
                Visible = false,
            });
        }

        public override void Render()
        {
            base.Render();
            Level level = Scene as Level;
            if (level.ShakeVector.X < 0f && level.Camera.X <= level.Bounds.Left && X <= level.Bounds.Left)
            {
                tiles.RenderAt(Position + new Vector2(-3f, 0f));
            }
            if (level.ShakeVector.X > 0f && level.Camera.X + 320f >= level.Bounds.Right && X + Width >= level.Bounds.Right)
            {
                tiles.RenderAt(Position + new Vector2(3f, 0f));
            }
            if (level.ShakeVector.Y < 0f && level.Camera.Y <= level.Bounds.Top && Y <= level.Bounds.Top)
            {
                tiles.RenderAt(Position + new Vector2(0f, -3f));
            }
            if (level.ShakeVector.Y > 0f && level.Camera.Y + 180f >= level.Bounds.Bottom && Y + Height >= level.Bounds.Bottom)
            {
                tiles.RenderAt(Position + new Vector2(0f, 3f));
            }
        }
    }
}
