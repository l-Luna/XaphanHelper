using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.XaphanHelper.Cutscenes;
using System.Reflection;

namespace Celeste.Mod.XaphanHelper.Entities
{
    [Tracked(true)]
    [RegisterStrawberry(true, false)]
    [CustomEntity("XaphanHelper/TeleportStrawberry")]
    class TeleportStrawberry : Strawberry
    {
        private bool tryToTeleport;

        private string DestinationRoom;

        private string WipeType;

        private float WipeDuration;

        private Vector2 SpawnPoint;

        private int RequiredStarwberriesToTeleport;

        private FieldInfo Strawberry_sprite = typeof(Strawberry).GetField("sprite", BindingFlags.Instance | BindingFlags.NonPublic);

        public TeleportStrawberry(EntityData data, Vector2 offset, EntityID gid) : base(data, offset, gid)
        {
            DestinationRoom = data.Attr("destinationRoom");
            WipeType = data.Attr("wipeType", "Fade");
            WipeDuration = data.Float("wipeDuration", 0.75f);
            RequiredStarwberriesToTeleport = data.Int("requiredStarwberriesToTeleport", 1);
            SpawnPoint = new Vector2(data.Int("spawnRoomX", 0), data.Int("spawnRoomY", 0));
        }

        public override void Update()
        {
            base.Update();
            if (this is TeleportStrawberry)
            {
                Sprite sprite = (Sprite)Strawberry_sprite.GetValue(this);
                if (sprite.CurrentAnimationID == "collect" && !tryToTeleport)
                {
                    tryToTeleport = true;
                    Session session = SceneAs<Level>().Session;
                    StatsFlags.GetStats(session);
                    int chapterIndex = session.Area.ChapterIndex == -1 ? 0 : session.Area.ChapterIndex;
                    if (StatsFlags.CurrentStrawberries[chapterIndex] >= RequiredStarwberriesToTeleport)
                    {
                        if (string.IsNullOrEmpty(DestinationRoom))
                        {
                            SceneAs<Level>().Add(new MiniTextbox("XaphanHelper_room_name_empty"));
                        }
                        else if (session.MapData.Get(DestinationRoom) == null)
                        {
                            SceneAs<Level>().Add(new MiniTextbox("XaphanHelper_room_not_exist"));
                        }
                        else
                        {
                            Player player = SceneAs<Level>().Tracker.GetEntity<Player>();
                            player.StateMachine.State = 11;
                            SceneAs<Level>().Add(new TeleportCutscene(player, DestinationRoom, SpawnPoint, 0, 0, true, 0.75f, string.IsNullOrEmpty(WipeType) ? "Fade" : WipeType, WipeDuration == 0 ? 0.75f : WipeDuration));
                        }
                    }
                    StatsFlags.ResetStats();
                }
            }            
        }
    }
}
