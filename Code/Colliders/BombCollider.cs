using System;
using Celeste.Mod.XaphanHelper.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Colliders
{
    [Tracked(false)]
    public class BombCollider : Component
    {
        public Action<Bomb> OnCollide;

        public Collider Collider;

        public BombCollider(Action<Bomb> onCollide, Collider collider = null) : base(active: false, visible: false)
        {
            OnCollide = onCollide;
            Collider = collider;
        }

        public bool Check(Bomb Bomb)
        {
            Collider collider = Collider;
            if (collider == null)
            {
                if (Bomb.CollideCheck(Entity))
                {
                    OnCollide(Bomb);
                    return true;
                }
                return false;
            }
            Collider collider2 = Entity.Collider;
            Entity.Collider = collider;
            bool flag = Bomb.CollideCheck(Entity);
            Entity.Collider = collider2;
            if (flag)
            {
                OnCollide(Bomb);
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
