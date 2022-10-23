using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Controllers
{
    [CustomEntity("XaphanHelper/MergeChaptersController")]
    class MergeChaptersController : Entity
    {
        public MergeChaptersController(EntityData data, Vector2 offset) : base(data.Position + offset)
        {

        }
    }
}
