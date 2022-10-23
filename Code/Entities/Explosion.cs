using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Entities
{
    class Explosion : Entity
    {
        Sprite explosionSprite;

        public Explosion(Vector2 position, bool big = false) : base(position)
        {
            Add(explosionSprite = new Sprite(GFX.Game, "upgrades/" + (big ? "SuperMissile" : "Missile") + "/"));
            explosionSprite.AddLoop("idle", big ? "superMissileExplode" : "missileExplode", 0.06f);
            explosionSprite.CenterOrigin();
            explosionSprite.Play("idle");
            explosionSprite.OnLastFrame = onLastFrame;
            Depth = -100000;
        }

        private void onLastFrame(string s)
        {
            Visible = false;
            RemoveSelf();
        }
    }
}
