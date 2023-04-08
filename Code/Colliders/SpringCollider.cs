using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Colliders
{
    [Tracked(false)]
    public class SpringCollider : Component
    {
        public Action<Spring> OnCollide;

        public Collider Collider;

        public SpringCollider(Action<Spring> onCollide, Collider collider = null) : base(active: false, visible: false)
        {
            OnCollide = onCollide;
            Collider = collider;
        }

        public bool Check(Spring spring)
        {
            Collider collider = Collider;
            if (collider == null)
            {
                if (spring.CollideCheck(Entity))
                {
                    OnCollide(spring);
                    return true;
                }
                return false;
            }
            Collider collider2 = Entity.Collider;
            Entity.Collider = collider;
            bool flag = spring.CollideCheck(Entity);
            Entity.Collider = collider2;
            if (flag)
            {
                OnCollide(spring);
                return true;
            }
            return false;
        }

        public override void DebugRender(Camera camera)
        {
            if (Collider != null)
            {
                Collider collider = Entity.Collider;
                Entity.Collider = Collider;
                Collider.Render(camera, Color.HotPink);
                Entity.Collider = collider;
            }
        }
    }
}
