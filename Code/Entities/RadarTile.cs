using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [Tracked(true)]
    class RadarTile : Entity
    {
        public float alpha = 1f;

        private bool AlphaDecreases = true;

        private bool Show = true;

        private Coroutine FadeRoutine = new Coroutine();

        public RadarTile(Vector2 position) : base(position)
        {
            Tag = (Tags.TransitionUpdate | Tags.Global);
            Collider = new Hitbox(8f, 8f);
            Visible = false;
            Depth = -15000;
        }

        public override void Update()
        {
            base.Update();
            if (!CollideCheck<DashBlock>() && !CollideCheck<FakeWall>() && !CollideCheck<CustomFakeWall>() && !CollideCheck<BreakBlock>() && !FadeRoutine.Active)
            {
                Disapear();
            }
            if (CollideCheck<SolidTiles>())
            {
                RemoveSelf();
            }
        }

        public void Disapear()
        {
            Show = false;
            Add(FadeRoutine = new Coroutine(DisapearRoutine()));
        }

        public IEnumerator DisapearRoutine()
        {
            while (alpha > 0)
            {
                alpha = Calc.AngleApproach(alpha, 0f, Engine.DeltaTime);
                yield return null;
            }
            RemoveSelf();
        }

        public override void Render()
        {
            base.Render();
            if (Show)
            {
                if (AlphaDecreases)
                {
                    alpha = Calc.AngleApproach(alpha, 0.5f, Engine.DeltaTime);
                    if (alpha <= 0.5f)
                    {
                        AlphaDecreases = false;
                    }
                }
                else
                {
                    alpha = Calc.AngleApproach(alpha, 1f, Engine.DeltaTime);
                    if (alpha >= 1)
                    {
                        AlphaDecreases = true;
                    }
                }
            }
            Draw.Rect(Collider, Calc.HexToColor("473D7C") * (alpha - 0.4f));
            Color color = Calc.HexToColor("DCD5FC");
            if (!CollideCheck<RadarTile>(Position - Vector2.UnitX))
            {
                Draw.Line(Position + Vector2.UnitX, Position + new Vector2(1, 8), color * alpha);
            }
            if (!CollideCheck<RadarTile>(Position + Vector2.UnitX * 8))
            {
                Draw.Line(Position + Vector2.UnitX * 8, Position + new Vector2(8, 8), color * alpha);
            }
            if (!CollideCheck<RadarTile>(Position - Vector2.UnitY))
            {
                bool tileLeft = false;
                bool TileRight = false;
                if (CollideCheck<RadarTile>(Position - Vector2.UnitX))
                {
                    tileLeft = true;
                }
                if (CollideCheck<RadarTile>(Position + Vector2.UnitX * 8))
                {
                    TileRight = true;
                }
                Draw.Rect(Position + (!tileLeft ? Vector2.UnitX : Vector2.Zero), 8 - (!tileLeft ? 1 : 0) - (!TileRight ? 1 : 0), 1, color * alpha);
            }
            if (!CollideCheck<RadarTile>(Position + Vector2.UnitY * 8))
            {
                bool tileLeft = false;
                bool TileRight = false;
                if (CollideCheck<RadarTile>(Position - Vector2.UnitX))
                {
                    tileLeft = true;
                }
                if (CollideCheck<RadarTile>(Position + Vector2.UnitX * 8))
                {
                    TileRight = true;
                }
                Draw.Rect(Position + Vector2.UnitY * 7 + (!tileLeft ? Vector2.UnitX : Vector2.Zero), 8 - (!tileLeft ? 1 : 0) - (!TileRight ? 1 : 0), 1, color * alpha);
            }
        }
    }
}
