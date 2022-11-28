using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Controllers
{
    [Tracked(true)]
    [CustomEntity("XaphanHelper/InGameMapTilesController")]
    class InGameMapTilesController : Entity
    {
        public EntityData Data;

        public InGameMapTilesController(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            Data = data;
        }
    }
}
