using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Controllers
{
    [Tracked(true)]
    [CustomEntity("XaphanHelper/InGameMapRoomController")]
    public class InGameMapRoomController : Entity
    {
        public EntityData Data;

        public InGameMapRoomController(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            Data = data;
        }
    }
}
