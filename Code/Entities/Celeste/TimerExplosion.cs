using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.XaphanHelper.Entities
{
    class TimerExplosion : Entity
    {
        Sprite explosionSprite;

        public TimerExplosion(Vector2 position) : base(position)
        {
            Depth = -100000;
            Add(explosionSprite = new Sprite(GFX.Game, "upgrades/Missile/"));
            explosionSprite.AddLoop("idle", "missileExplode", 0.08f);
            explosionSprite.CenterOrigin();
            explosionSprite.Play("idle");
            explosionSprite.OnLastFrame = onLastFrame;
            Collider = new Hitbox(2f, 2f, -1f, -1f);
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            var randChance = Calc.Random;
            if (randChance.Next(0, 101) <= 50)
            {
                RemoveSelf();
            }
            if (!CollideCheck<SolidTiles>())
            {
                RemoveSelf();
            }
        }

        private void onLastFrame(string s)
        {
            Visible = false;
            RemoveSelf();
        }
    }
}
