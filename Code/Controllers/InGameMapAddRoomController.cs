using Celeste.Mod.Entities;
using Celeste.Mod.XaphanHelper.UI_Elements;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Controllers
{
    [CustomEntity("XaphanHelper/InGameMapAddRoomController")]
    class InGameMapAddRoomController : Entity
    {
        public InGameMapAddRoomController(EntityData data, Vector2 offset) : base(data.Position + offset)
        {

        }

        public override void Update()
        {
            base.Update();
            MapDisplay mapDisplay = SceneAs<Level>().Tracker.GetEntity<MapDisplay>();
            if (mapDisplay != null && mapDisplay.ExtraUnexploredRooms.Count > mapDisplay.ExtraUnexploredRoomsCount)
            {
                AreaKey area = SceneAs<Level>().Session.Area;
                int chapterIndex = area.ChapterIndex == -1 ? 0 : area.ChapterIndex;
                mapDisplay.UpdateExtraRooms();
            }
        }
    }
}
