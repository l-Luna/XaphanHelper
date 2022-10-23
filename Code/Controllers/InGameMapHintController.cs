using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Controllers
{
    [CustomEntity("XaphanHelper/InGameMapHintController")]
    class InGameMapHintController : Entity
    {
        public InGameMapHintController(EntityData data, Vector2 offset) : base(data.Position + offset)
        {

        }
    }
}
