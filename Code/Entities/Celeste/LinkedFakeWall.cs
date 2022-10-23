using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [Tracked(true)]
    [CustomEntity("XaphanHelper/LinkedFakeWall")]
    class LinkedFakeWall : Entity
    {
        public enum Modes
        {
            Wall,
            Block
        }

        private Modes mode;

        private char fillTile;

        public TileGrid tiles;

        private bool fade;

        private EffectCutout cutout;

        private float transitionStartAlpha;

        private bool transitionFade;

        private EntityID eid;

        private bool playRevealWhenTransitionedInto;

        public LinkedFakeWall(EntityData data, Vector2 position, EntityID eid) : base(data.Position + position)
        {
            mode = data.Enum<Modes>("mode");
            this.eid = eid;
            fillTile = data.Char("tiletype", '3');
            playRevealWhenTransitionedInto = data.Bool("playTransitionReveal");
            Collider = new Hitbox(data.Width, data.Height);
            Depth = -13000;
            Add(cutout = new EffectCutout());
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            int tilesX = (int)Width / 8;
            int tilesY = (int)Height / 8;
            if (mode == Modes.Wall)
            {
                Level level = SceneAs<Level>();
                Rectangle tileBounds = level.Session.MapData.TileBounds;
                VirtualMap<char> solidsData = level.SolidsData;
                int x = (int)X / 8 - tileBounds.Left;
                int y = (int)Y / 8 - tileBounds.Top;
                tiles = GFX.FGAutotiler.GenerateOverlay(fillTile, x, y, tilesX, tilesY, solidsData).TileGrid;
            }
            else if (mode == Modes.Block)
            {
                tiles = GFX.FGAutotiler.GenerateBox(fillTile, tilesX, tilesY).TileGrid;
            }
            Add(tiles);
            Add(new TileInterceptor(tiles, highPriority: false));
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            if (CollideCheck<Player>())
            {
                if (playRevealWhenTransitionedInto)
                {
                    Audio.Play("event:/game/general/secret_revealed", Center);
                }
                foreach (LinkedFakeWall fakewall in Scene.Entities.FindAll<LinkedFakeWall>())
                {
                    fakewall.RevealWhenTransition();
                }
            }
            else
            {
                TransitionListener transitionListener = new TransitionListener();
                transitionListener.OnOut = OnTransitionOut;
                transitionListener.OnOutBegin = OnTransitionOutBegin;
                transitionListener.OnIn = OnTransitionIn;
                transitionListener.OnInBegin = OnTransitionInBegin;
                Add(transitionListener);
            }
        }

        public void RevealWhenTransition()
        {
            tiles.Alpha = 0f;
            fade = true;
            cutout.Visible = false;
            SceneAs<Level>().Session.DoNotLoad.Add(eid);
        }

        private void OnTransitionOutBegin()
        {
            if (Collide.CheckRect(this, SceneAs<Level>().Bounds))
            {
                transitionFade = true;
                transitionStartAlpha = tiles.Alpha;
            }
            else
            {
                transitionFade = false;
            }
        }

        private void OnTransitionOut(float percent)
        {
            if (transitionFade)
            {
                tiles.Alpha = transitionStartAlpha * (1f - percent);
            }
        }

        private void OnTransitionInBegin()
        {
            Level level = SceneAs<Level>();
            if (level.PreviousBounds.HasValue && Collide.CheckRect(this, level.PreviousBounds.Value))
            {
                transitionFade = true;
                tiles.Alpha = 0f;
            }
            else
            {
                transitionFade = false;
            }
        }

        private void OnTransitionIn(float percent)
        {
            if (transitionFade)
            {
                tiles.Alpha = percent;
            }
        }

        public override void Update()
        {
            base.Update();
            if (fade)
            {
                tiles.Alpha = Calc.Approach(tiles.Alpha, 0f, 2f * Engine.DeltaTime);
                cutout.Alpha = tiles.Alpha;
                if (tiles.Alpha <= 0f)
                {
                    RemoveSelf();
                }
                return;
            }
            Player player = CollideFirst<Player>();
            if (player != null && player.StateMachine.State != 9)
            {
                foreach (LinkedFakeWall fakewall in Scene.Entities.FindAll<LinkedFakeWall>())
                {
                    fakewall.Reveal();
                }
                Audio.Play("event:/game/general/secret_revealed", Center);
            }
        }

        public void Reveal()
        {
            SceneAs<Level>().Session.DoNotLoad.Add(eid);
            fade = true;
        }


        public override void Render()
        {
            if (mode == Modes.Wall)
            {
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
            base.Render();
        }
    }
}
