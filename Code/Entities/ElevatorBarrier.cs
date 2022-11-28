using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [CustomEntity("XaphanHelper/ElevatorBarrier")]
    class ElevatorBarrier : Solid
    {
        public ElevatorBarrier(EntityData data, Vector2 offset) : base(data.Position + offset, data.Width, data.Height, true)
        {
            SurfaceSoundIndex = 0;
        }

        public override void Update()
        {
            base.Update();
            if (SceneAs<Level>().Session.GetFlag("Using_Elevator"))
            {
                Collidable = false;
            }
            else
            {
                Collidable = true;
            }
        }
    }
}
