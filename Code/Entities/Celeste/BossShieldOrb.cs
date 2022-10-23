using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Entities
{
    public class BossShieldOrb : Entity
    {
        private Sprite sprite;

        private float cantKillTimer;

        public BossShieldOrb() : base(Vector2.Zero)
        {
            Add(sprite = GFX.SpriteBank.Create("badeline_projectile"));
            Collider = new Hitbox(4f, 4f, -2f, -2f);
            Add(new PlayerCollider(OnPlayer));
            Depth = -1000000;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            cantKillTimer = 0.15f;
        }

        public override void Update()
        {
            base.Update();
            if (cantKillTimer > 0f)
            {
                cantKillTimer -= Engine.DeltaTime;
            }
        }

        private void OnPlayer(Player player)
        {
            if (cantKillTimer <= 0f)
            {
                player.Die((player.Center - Position).SafeNormalize());
            }
        }
    }
}
