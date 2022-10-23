using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Controllers
{
    [CustomEntity("XaphanHelper/InGameMapImageController")]
    class InGameMapImageController : Entity
    {
        public InGameMapImageController(EntityData data, Vector2 offset) : base(data.Position + offset)
        {

        }
    }
}
