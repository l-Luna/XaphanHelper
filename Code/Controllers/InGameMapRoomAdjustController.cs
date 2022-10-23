using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Controllers
{
    [CustomEntity("XaphanHelper/InGameMapRoomAdjustController")]
    class InGameMapRoomAdjustController : Entity
    {
        public InGameMapRoomAdjustController(EntityData data, Vector2 offset) : base(data.Position + offset)
        {

        }
    }
}
