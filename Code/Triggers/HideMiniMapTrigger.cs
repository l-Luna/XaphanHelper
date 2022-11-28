using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Triggers
{
    [Tracked(true)]
    [CustomEntity("XaphanHelper/HideMiniMapTrigger")]
    class HideMiniMapTrigger : Trigger
    {
        public bool playerInside;

        public HideMiniMapTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {

        }

        public override void OnStay(Player player)
        {
            base.OnStay(player);
            playerInside = true;
        }

        public override void OnLeave(Player player)
        {
            playerInside = false;
        }
    }
}
