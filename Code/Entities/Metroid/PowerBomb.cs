using System.Collections;
using Celeste.Mod.XaphanHelper.Controllers;
using Celeste.Mod.XaphanHelper.Enemies;
using Celeste.Mod.XaphanHelper.Upgrades;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [Tracked(true)]
    public class PowerBomb : Entity
    {
        private Sprite BombSprite;

        public int damage = 200;

        protected XaphanModuleSettings Settings => XaphanModule.Settings;

        public PowerBomb(Vector2 position) : base(position)
        {
            Add(BombSprite = new Sprite(GFX.Game, "upgrades/powerBomb/"));
            BombSprite.AddLoop("idle", "PowerBomb", 0.06f);
            BombSprite.AddLoop("explode", "PowerBombExplode", 0.06f);
            BombSprite.CenterOrigin();
            BombSprite.Justify = new Vector2(0.5f, 0.5f);
            BombSprite.Position.Y -= 1;
            BombSprite.Play("idle");
            Depth = -10002;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            Audio.Play("event:/game/xaphan/use_power_bomb");
            Add(new Coroutine(Explode()));
        }

        public IEnumerator Explode()
        {
            yield return 1.2f;
            SceneAs<Level>().Flash(Color.White);
            Collider = new Circle(10f, 0f, 0f);
            Audio.Play("event:/game/xaphan/power_bomb_explode");
            Player player = CollideFirst<Player>();
            Add(new Coroutine(Explosion()));
            if (player != null && player.StateMachine.State != 11 && !Scene.CollideCheck<Solid>(Position + new Vector2(0, -10f), player.Center) && MetroidGameplayController.MorphMode)
            {
                float dirX = 0;
                float dirY = -1;
                if (player.Position.X < Position.X)
                {
                    dirX = -1;
                }
                else if (player.Position.X > Position.X)
                {
                    dirX = 1;
                }
                player.Speed.Y = 180f * dirY * GravityJacket.determineJumpHeightFactor();
                if (dirX != 0)
                {
                    player.Speed.X = 180f * dirX * GravityJacket.determineJumpHeightFactor();
                }
            }
        }

        public IEnumerator Explosion()
        {
            Player player = CollideFirst<Player>();
            BombSprite.Color = Color.White * 0.9f;
            BombSprite.Play("explode", false);
            for (int i = 0; i <= 50; i++)
            {
                Collider = new Circle(10f + 5f * i, 0f, 0f);
                BombSprite.Scale = new Vector2(0.02f * i, 0.02f * i);
                foreach (Enemy enemy in Scene.Tracker.GetEntities<Enemy>())
                {
                    if (CollideCheck(enemy))
                    {
                        enemy.HitByPowerBomb(this);
                    }
                }
                foreach (DestructibleBlock destructibleBlock in Scene.Tracker.GetEntities<DestructibleBlock>())
                {
                    if (destructibleBlock.mode == "Shoot" || destructibleBlock.mode == "Bomb" || destructibleBlock.mode == "PowerBomb")
                    {
                        if (CollideCheck(destructibleBlock))
                        {
                            destructibleBlock.Break();
                        }
                    }
                    else
                    {
                        if (CollideCheck(destructibleBlock))
                        {
                            destructibleBlock.Reveal();
                        }
                    }
                }
                foreach (BubbleDoor bubbleDoor in Scene.Tracker.GetEntities<BubbleDoor>())
                {
                    if (bubbleDoor.color == "Blue" || bubbleDoor.color == "Yellow" || bubbleDoor.color == "Grey" && bubbleDoor.isActive && !bubbleDoor.locked)
                    {
                        if (CollideCheck(bubbleDoor))
                        {
                            bubbleDoor.keepOpen = true;
                            bubbleDoor.Open();
                        }
                    }
                }
                yield return null;
            }
            for (int j = 1; j <= 50; j++)
            {
                BombSprite.Color = Color.White * (0.9f - 0.01f * j);
                BombSprite.Scale = new Vector2(1 + 0.015f * j, 1 + 0.015f * j);
                yield return null;
            }
            for (int k = 1; k <= 65; k++)
            {
                BombSprite.Color = Color.White * (0.4f - 0.01f * k);
                BombSprite.Scale = new Vector2(1.75f + 0.01f * k, 1.75f + 0.01f * k);
                yield return null;
            }
            Visible = false;
            RemoveSelf();
        }
    }
}