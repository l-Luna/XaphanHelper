using Celeste.Mod.Entities;
using Celeste.Mod.XaphanHelper.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Triggers
{
    [CustomEntity("XaphanHelper/DroneDestroyTutorialTrigger")]
    class DroneDestroyTutorialTrigger : Trigger
    {
        string flag;

        public DroneDestroyTutorialTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            flag = data.Attr("flag");
        }

        public override void OnStay(Player player)
        {
            if (player.GetType() == typeof(Player) && Scene != null)
            {
                base.OnStay(player);
                if (!string.IsNullOrEmpty(flag) ? SceneAs<Level>().Session.GetFlag(flag) : true)
                {
                    foreach (Drone drone in Scene.Tracker.GetEntities<Drone>())
                    {
                        if (drone != null)
                        {
                            drone.ShowTutorial(true);
                        }
                    }
                }
                else
                {
                    foreach (Drone drone in Scene.Tracker.GetEntities<Drone>())
                    {
                        if (drone != null)
                        {
                            drone.ShowTutorial(false);
                        }
                    }
                }
            }
        }

        public override void OnLeave(Player player)
        {
            if (player.GetType() == typeof(Player) && Scene != null)
            {
                if (!string.IsNullOrEmpty(flag) ? SceneAs<Level>().Session.GetFlag(flag) : true)
                {
                    base.OnLeave(player);
                    foreach (Drone drone in Scene.Tracker.GetEntities<Drone>())
                    {
                        if (drone != null)
                        {
                            drone.ShowTutorial(false);
                        }
                    }
                }
            }
        }
    }
}
