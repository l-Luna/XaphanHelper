using Celeste.Mod.Entities;
using Celeste.Mod.XaphanHelper.Controllers;
using Celeste.Mod.XaphanHelper.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.XaphanHelper.Triggers
{
    [CustomEntity("XaphanHelper/StopEnvironmentalControllerTrigger")]
    class StopEnvironmentalControllerTrigger : Trigger
    {
        public StopEnvironmentalControllerTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {

        }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            foreach (EnvironmentalController controller in Scene.Entities.FindAll<EnvironmentalController>())
            {
                controller.RemoveSelf();
            }
        }
    }
}