using System.Collections;
using Celeste.Mod.XaphanHelper.Controllers;
using Celeste.Mod.XaphanHelper.Enemies;
using Celeste.Mod.XaphanHelper.Upgrades;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [Tracked(true)]
    public class MorphBomb : Entity
    {
        private Sprite BombSprite;

        public int damage = 30;

        public MorphBomb(Vector2 position) : base(position)
        {
            Add(BombSprite = new Sprite(GFX.Game, "upgrades/morphBomb/"));
            BombSprite.AddLoop("idle", "bomb", 0.06f);
            BombSprite.AddLoop("explode", "explode", 0.06f);
            BombSprite.CenterOrigin();
            BombSprite.Justify = new Vector2(0.5f, 0.5f);
            BombSprite.Position.Y -= 1;
            BombSprite.Play("idle");
            Depth = -10002;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            Audio.Play("event:/game/xaphan/use_bomb");
            Add(new Coroutine(Explode()));
        }

        public IEnumerator Explode()
        {
            yield return 1f;
            Collider = new Circle(8f, 0f, 0f);
            Audio.Play("event:/game/xaphan/bomb_explode");
            BombSprite.Play("explode", false);
            BombSprite.OnLastFrame = onLastFrame;
            Player player = CollideFirst<Player>();
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
            foreach (Enemy enemy in Scene.Tracker.GetEntities<Enemy>())
            {
                if (CollideCheck(enemy))
                {
                    enemy.HitByMorphBomb(this);
                }
            }
            foreach (DestructibleBlock destructibleBlock in Scene.Tracker.GetEntities<DestructibleBlock>())
            {
                if (destructibleBlock.mode == "Shoot" || destructibleBlock.mode == "Bomb")
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
                if (bubbleDoor.color == "Blue" || bubbleDoor.color == "Grey" && bubbleDoor.isActive && !bubbleDoor.locked)
                {
                    if (CollideCheck(bubbleDoor))
                    {
                        bubbleDoor.keepOpen = true;
                        bubbleDoor.Open();
                    }
                }
            }
        }

        private void onLastFrame(string s)
        {
            Visible = false;
            RemoveSelf();
        }
    }
}
