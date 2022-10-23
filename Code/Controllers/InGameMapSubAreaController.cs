using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Controllers
{
    [CustomEntity("XaphanHelper/InGameMapSubAreaController")]
    class InGameMapSubAreaController : Entity
    {
        public InGameMapSubAreaController(EntityData data, Vector2 offset) : base(data.Position + offset)
        {

        }
    }
}
