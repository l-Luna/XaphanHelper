using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.XaphanHelper.Triggers
{
    [CustomEntity("XaphanHelper/DisableMovementTrigger")]
    class DisableMovementTrigger : Trigger
    {
        public DisableMovementTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {

        }

        public override void OnStay(Player player)
        {
            base.OnStay(player);
            player.StateMachine.State = Player.StDummy;
            SceneAs<Level>().CanRetry = false;
        }
    }
}
