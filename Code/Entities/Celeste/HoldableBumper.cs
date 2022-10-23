using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [CustomEntity("XaphanHelper/HoldableBumper")]
    class HoldableBumper : Entity
    {
        public HoldableBumper(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            Collider = new Hitbox(data.Width, 4f, -8, -3f);
            Add(new HoldableCollider(OnHoldable));
            
        }

        private void OnHoldable(Holdable h)
        {
            h.HitSpinner(this);
        }
    }
}
