using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [Tracked(true)]
    class EnemyBarrier : Solid
    {
        public EnemyBarrier(Vector2 position, int width, int height, int soundIndex) : base(position, width, height, true)
        {
            Collider = new Hitbox(width, height);
            SurfaceSoundIndex = soundIndex;
            Add(new LightOcclude(1f));
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            Collidable = false;
        }
    }
}
