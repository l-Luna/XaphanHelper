using Celeste.Mod.XaphanHelper.Managers;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.XaphanHelper.Colliders
{
    [Tracked(false)]
    public class ScrewAttackCollider : Component
    {
        public Action<ScrewAttackManager> OnCollide;

        public Collider Collider;

        public ScrewAttackCollider(Action<ScrewAttackManager> onCollide, Collider collider = null) : base(active: false, visible: false)
        {
            OnCollide = onCollide;
            Collider = collider;
        }

        public bool Check(ScrewAttackManager manager)
        {
            Collider collider = Collider;
            if (collider == null)
            {
                if (manager.CollideCheck(Entity))
                {
                    OnCollide(manager);
                    return true;
                }
                return false;
            }
            Collider collider2 = Entity.Collider;
            Entity.Collider = collider;
            bool flag = manager.CollideCheck(Entity);
            Entity.Collider = collider2;
            if (flag)
            {
                OnCollide(manager);
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
