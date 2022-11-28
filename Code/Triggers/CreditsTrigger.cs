using Celeste.Mod.Entities;
using Celeste.Mod.XaphanHelper.Cutscenes;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Triggers
{
    [CustomEntity("XaphanHelper/CreditsTrigger")]
    class CreditsTrigger : Trigger
    {
        private bool triggered;

        public CreditsTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {

        }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            if (triggered)
            {
                return;
            }
            triggered = true;
            Scene.Add(new CS_Credits(player));
        }
    }
}
