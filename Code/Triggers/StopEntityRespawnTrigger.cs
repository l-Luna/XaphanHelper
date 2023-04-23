using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.XaphanHelper.Triggers
{
    [CustomEntity("XaphanHelper/StopEntityRespawnTrigger")]
    class StopEntityRespawnTrigger : Trigger
    {
        private string Room;

        public StopEntityRespawnTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            Tag = Tags.TransitionUpdate;
            Room = data.Attr("room");
        }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            if (XaphanModule.ModSession.NoRespawnIds.Count != 0)
            {
                foreach (EntityID entity in XaphanModule.ModSession.NoRespawnIds)
                {
                    if (entity.Level == Room)
                    {
                        SceneAs<Level>().Session.DoNotLoad.Add(entity);
                    }
                }
                XaphanModule.ModSession.NoRespawnIds.Clear();
            }
        }
    }
}
