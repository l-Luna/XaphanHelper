using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [Tracked(true)]
    class TimerExplosion : Entity
    {
        Sprite explosionSprite;

        public TimerExplosion(Vector2 position) : base(position)
        {
            Tag = Tags.TransitionUpdate;
            Depth = -100000;
            Add(explosionSprite = new Sprite(GFX.Game, "upgrades/Missile/"));
            explosionSprite.AddLoop("idle", "missileExplode", 0.08f);
            explosionSprite.CenterOrigin();
            explosionSprite.Play("idle");
            explosionSprite.OnLastFrame = onLastFrame;
            Collider = new Circle(50f);
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            if (CollideCheck<Player>() || CollideCheck<TimerExplosion>())
            {
                RemoveSelf();
            }
            else
            {
                Random rand = Calc.Random;
                if (rand.Next(101) <= 75)
                Audio.Play("event:/game/xaphan/explosion");
            }
        }


        private void onLastFrame(string s)
        {
            Visible = false;
            RemoveSelf();
        }
    }
}
