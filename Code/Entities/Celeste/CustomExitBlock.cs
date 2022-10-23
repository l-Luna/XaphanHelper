using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [CustomEntity("XaphanHelper/CustomExitBlock")]
    class CustomExitBlock : Solid
    {
        private TileGrid tiles;

        private TransitionListener tl;

        private EffectCutout cutout;

        private float startAlpha;

        private char fillTile;

        private bool CloseSound;

        private int group;

        public CustomExitBlock(EntityData data, Vector2 offset) : base(data.Position + offset, data.Width, data.Height, safe: true)
        {
            Depth = -13000;
            fillTile = data.Char("tiletype", '3');
            CloseSound = data.Bool("closeSound");
            group = data.Int("group");
            tl = new TransitionListener();
            tl.OnOutBegin = OnTransitionOutBegin;
            tl.OnInBegin = OnTransitionInBegin;
            Add(tl);
            Add(cutout = new EffectCutout());
            SurfaceSoundIndex = SurfaceIndex.TileToIndex[fillTile];
            EnableAssistModeChecks = false;
        }

        private void OnTransitionOutBegin()
        {
            if (Collide.CheckRect(this, SceneAs<Level>().Bounds))
            {
                tl.OnOut = OnTransitionOut;
                startAlpha = tiles.Alpha;
            }
        }

        private void OnTransitionOut(float percent)
        {
            cutout.Alpha = (tiles.Alpha = MathHelper.Lerp(startAlpha, 0f, percent));
            cutout.Update();
        }

        private void OnTransitionInBegin()
        {
            if (Collide.CheckRect(this, SceneAs<Level>().PreviousBounds.Value) && !CollideCheck<Player>())
            {
                cutout.Alpha = 0f;
                tiles.Alpha = 0f;
                tl.OnIn = OnTransitionIn;
            }
        }

        private void OnTransitionIn(float percent)
        {
            cutout.Alpha = (tiles.Alpha = percent);
            cutout.Update();
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            int tilesX = (int)Width / 8;
            int tilesY = (int)Height / 8;
            tiles = GFX.FGAutotiler.GenerateBox(fillTile, tilesX, tilesY).TileGrid;
            Add(tiles);
            Add(new TileInterceptor(tiles, highPriority: false));
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            int count = 0;
            foreach (CustomExitBlock exitblock in Scene.Entities.FindAll<CustomExitBlock>())
            {
                if (exitblock.group == group)
                {
                    if (exitblock.CollideCheck<Player>())
                    {
                        count++;
                    }
                }
            }
            if (count > 0)
            {
                cutout.Alpha = (tiles.Alpha = 0f);
                Collidable = false;
            }
        }

        public override void Update()
        {
            base.Update();
            int count = 0;
            foreach (CustomExitBlock exitblock in Scene.Entities.FindAll<CustomExitBlock>())
            {
                if (exitblock.group == group)
                {
                    if (exitblock.CollideCheck<Player>())
                    {
                        SceneAs<Level>().Session.SetFlag("Inside_ExitBlock_" + group, true);
                        count++;
                    }
                }

            }
            if (count == 0)
            {
                SceneAs<Level>().Session.SetFlag("Inside_ExitBlock_" + group, false);
            }
            if (Collidable)
            {
                cutout.Alpha = (tiles.Alpha = Calc.Approach(tiles.Alpha, 1f, Engine.DeltaTime));
            }
            else if (!SceneAs<Level>().Session.GetFlag("Inside_ExitBlock_" + group))
            {
                Collidable = true;
                if (CloseSound)
                {
                    Audio.Play("event:/game/general/passage_closed_behind", Center);
                }
            }
        }
    }
}
