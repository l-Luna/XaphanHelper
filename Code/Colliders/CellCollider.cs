using System;
using Celeste.Mod.XaphanHelper.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Colliders
{
    [Tracked(false)]
    public class CellCollider : Component
    {
        public Action<Cell> OnCollide;

        public Collider Collider;

        public CellCollider(Action<Cell> onCollide, Collider collider = null) : base(active: false, visible: false)
        {
            OnCollide = onCollide;
            Collider = collider;
        }

        public bool Check(Cell cell)
        {
            Collider collider = Collider;
            if (collider == null)
            {
                if (cell.CollideCheck(Entity))
                {
                    OnCollide(cell);
                    return true;
                }
                return false;
            }
            Collider collider2 = Entity.Collider;
            Entity.Collider = collider;
            bool flag = cell.CollideCheck(Entity);
            Entity.Collider = collider2;
            if (flag)
            {
                OnCollide(cell);
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
