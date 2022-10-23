using Celeste.Mod.XaphanHelper.Events;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Triggers
{
    [CustomEntity("XaphanHelper/SoCMEventTrigger")]
    class SoCMEventTrigger : Trigger
    {
        private bool triggered;

        public string Event;

        public SoCMEventTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            Event = data.Attr("event");
        }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            if (triggered)
            {
                return;
            }
            triggered = true;
            Session session = SceneAs<Level>().Session;
            Level level = base.Scene as Level;
            switch (Event)
            {
                case "Ch2 - Boss":
                    Scene.Add(new E02_Boss(player, level));
                    break;
            }
        }
    }
}