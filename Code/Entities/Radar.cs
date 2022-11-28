using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [Tracked(true)]
    class Radar : Entity
    {
        private Sprite ScanSprite;

        public Radar(Vector2 position) : base(position)
        {
            Tag = (Tags.TransitionUpdate | Tags.Global);
            Add(ScanSprite = new Sprite(GFX.Game, "upgrades/pulseRadar/"));
            ScanSprite.AddLoop("scan", "pulseRadarScan", 0.06f);
            ScanSprite.CenterOrigin();
            ScanSprite.Justify = new Vector2(0.5f, 0.5f);
            ScanSprite.Position.Y -= 1;
            Depth = -1000;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            Player player = SceneAs<Level>().Tracker.GetEntity<Player>();
            Add(new Coroutine(DetectSecrets(player, SceneAs<Level>())));
            Add(new Coroutine(Cooldown()));
            Add(new Coroutine(RadarSound(player)));
        }

        private IEnumerator DetectSecrets(Player player, Level level)
        {
            Audio.Play("event:/game/xaphan/radar_detect", player.Position);
            foreach (DashBlock wall in Scene.Tracker.GetEntities<DashBlock>())
            {
                for (int i = 0; i < wall.Height / 8; i++)
                {
                    for (int j = 0; j < wall.Width / 8; j++)
                    {
                        SceneAs<Level>().Add(new RadarTile(wall.Position + new Vector2(j * 8, i * 8)));
                    }
                }
            }
            foreach (FakeWall wall in Scene.Tracker.GetEntities<FakeWall>())
            {
                for (int i = 0; i < wall.Height / 8; i++)
                {
                    for (int j = 0; j < wall.Width / 8; j++)
                    {
                        SceneAs<Level>().Add(new RadarTile(wall.Position + new Vector2(j * 8, i * 8)));
                    }
                }
            }
            foreach (CustomFakeWall wall in Scene.Tracker.GetEntities<CustomFakeWall>())
            {
                for (int i = 0; i < wall.Height / 8; i++)
                {
                    for (int j = 0; j < wall.Width / 8; j++)
                    {
                        SceneAs<Level>().Add(new RadarTile(wall.Position + new Vector2(j * 8, i * 8)));
                    }
                }
            }
            foreach (BreakBlock wall in Scene.Tracker.GetEntities<BreakBlock>())
            {
                for (int i = 0; i < wall.Height / 8; i++)
                {
                    for (int j = 0; j < wall.Width / 8; j++)
                    {
                        SceneAs<Level>().Add(new RadarTile(wall.Position + new Vector2(j * 8, i * 8)));
                    }
                }
            }
            foreach (EtherealBlock wall in Scene.Tracker.GetEntities<EtherealBlock>())
            {
                for (int i = 0; i < wall.Height / 8; i++)
                {
                    for (int j = 0; j < wall.Width / 8; j++)
                    {
                        SceneAs<Level>().Add(new RadarTile(wall.Position + new Vector2(j * 8, i * 8)));
                    }
                }
            }
            ScanSprite.Color = Calc.HexToColor("DCD5FC") * 0.9f;
            ScanSprite.Play("scan", false);
            for (int i = 0; i <= 50; i++)
            {
                Collider = new Circle(10f + 6f * i, 0f, 0f);
                ScanSprite.Scale = new Vector2(0.02f * i, 0.02f * i);
                foreach (RadarTile radarTile in Scene.Tracker.GetEntities<RadarTile>())
                {
                    if (CollideCheck(radarTile))
                    {
                        radarTile.Visible = true;
                    }
                }
                yield return null;
            }
            for (int j = 1; j <= 50; j++)
            {
                ScanSprite.Color = Calc.HexToColor("DCD5FC") * (0.9f - 0.01f * j);
                ScanSprite.Scale = new Vector2(1 + 0.015f * j, 1 + 0.015f * j);
                yield return null;
            }
            for (int k = 1; k <= 65; k++)
            {
                ScanSprite.Color = Calc.HexToColor("DCD5FC") * (0.4f - 0.01f * k);
                ScanSprite.Scale = new Vector2(1.75f + 0.01f * k, 1.75f + 0.01f * k);
                yield return null;
            }
            yield return 4.92f;
            foreach (RadarTile radarTile in level.Tracker.GetEntities<RadarTile>())
            {
                radarTile.Disapear();
            }
            yield return 1f;
            Visible = false;
            RemoveSelf();
        }

        private IEnumerator Cooldown()
        {
            float timer = 3f;
            while (timer > 0f)
            {
                timer -= Engine.DeltaTime;
                yield return null;
            }
            if (SceneAs<Level>().Tracker.GetEntities<RadarTile>().Count == 0)
            {
                RemoveSelf();
            }
        }

        private IEnumerator RadarSound(Player player)
        {
            while (true)
            {
                RadarTile tile = SceneAs<Level>().Tracker.GetNearestEntity<RadarTile>(Position);
                if (tile != null && tile.Visible && tile.alpha == 1)
                {
                    Audio.Play("event:/game/xaphan/radar", player.Position);
                }
                yield return null;
            }
        }
    }
}
