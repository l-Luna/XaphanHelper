using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [Tracked(true)]
    [CustomEntity("XaphanHelper/CustomDashBlock")]
    class CustomDashBlock : Solid
    {
        private bool permanent;

        private EntityID id;

        private char tileType;

        private char flagTileType;

        private bool blendIn;

        private bool canDash;

        private string flag;

        public CustomDashBlock(EntityData data, Vector2 position, EntityID ID) : base(data.Position + position, data.Width, data.Height, safe: true)
        {
            Depth = -12999;
            id = ID;
            flag = data.Attr("flag");
            permanent = data.Bool("permanent");
            blendIn = data.Bool("blendIn");
            canDash = data.Bool("canDash");
            tileType = data.Char("tiletype", '3');
            flagTileType = data.Char("flagTiletype", '3');
            OnDashCollide = OnDashed;
            SurfaceSoundIndex = SurfaceIndex.TileToIndex[tileType];
        }
        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            TileGrid tileGrid;
            Level level = SceneAs<Level>();
            if (!blendIn)
            {
                tileGrid = GFX.FGAutotiler.GenerateBox((!string.IsNullOrEmpty(flag) && level.Session.GetFlag(flag)) ? flagTileType : tileType, (int)Width / 8, (int)Height / 8).TileGrid;
                Add(new LightOcclude());
            }
            else
            {
                Rectangle tileBounds = level.Session.MapData.TileBounds;
                VirtualMap<char> solidsData = level.SolidsData;
                int x = (int)(X / 8f) - tileBounds.Left;
                int y = (int)(Y / 8f) - tileBounds.Top;
                int tilesX = (int)Width / 8;
                int tilesY = (int)Height / 8;
                tileGrid = GFX.FGAutotiler.GenerateOverlay((!string.IsNullOrEmpty(flag) && level.Session.GetFlag(flag)) ? flagTileType : tileType, x, y, tilesX, tilesY, solidsData).TileGrid;
                Add(new EffectCutout());
                Depth = -10501;
            }
            Add(tileGrid);
            Add(new TileInterceptor(tileGrid, highPriority: true));
            if (CollideCheck<Player>())
            {
                RemoveSelf();
            }
        }

        public override void Removed(Scene scene)
        {
            base.Removed(scene);
            Celeste.Freeze(0.05f);
        }

        public void Break(Vector2 from, Vector2 direction, bool playSound = true, bool playDebrisSound = true)
        {
            Level level = SceneAs<Level>();
            if (playSound)
            {
                if (((!string.IsNullOrEmpty(flag) && level.Session.GetFlag(flag)) ? flagTileType : tileType) == '1')
                {
                    Audio.Play("event:/game/general/wall_break_dirt", Position);
                }
                else if (((!string.IsNullOrEmpty(flag) && level.Session.GetFlag(flag)) ? flagTileType : tileType) == '3')
                {
                    Audio.Play("event:/game/general/wall_break_ice", Position);
                }
                else if (((!string.IsNullOrEmpty(flag) && level.Session.GetFlag(flag)) ? flagTileType : tileType) == '9')
                {
                    Audio.Play("event:/game/general/wall_break_wood", Position);
                }
                else
                {
                    Audio.Play("event:/game/general/wall_break_stone", Position);
                }
            }
            for (int i = 0; i < Width / 8f; i++)
            {
                for (int j = 0; j < Height / 8f; j++)
                {
                    Scene.Add(Engine.Pooler.Create<Debris>().Init(Position + new Vector2(4 + i * 8, 4 + j * 8), (!string.IsNullOrEmpty(flag) && level.Session.GetFlag(flag)) ? flagTileType : tileType, playDebrisSound).BlastFrom(from));
                }
            }
            Collidable = false;
            if (permanent)
            {
                RemoveAndFlagAsGone();
            }
            else
            {
                RemoveSelf();
            }
        }

        public void RemoveAndFlagAsGone()
        {
            RemoveSelf();
            SceneAs<Level>().Session.DoNotLoad.Add(id);
        }

        private DashCollisionResults OnDashed(Player player, Vector2 direction)
        {
            if (!canDash && player.StateMachine.State != 5 && player.StateMachine.State != 10)
            {
                return DashCollisionResults.NormalCollision;
            }
            Break(player.Center, direction);
            return DashCollisionResults.Rebound;
        }
    }
}
