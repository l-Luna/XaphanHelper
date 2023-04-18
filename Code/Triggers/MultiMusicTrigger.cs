using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.XaphanHelper.Triggers
{
    [CustomEntity("XaphanHelper/MultiMusicTrigger")]
    class MultiMusicTrigger : Trigger
    {
        public string flagA;

        public string flagB;

        public string trackNone;

        public string trackA;

        public string trackB;

        public string trackBoth;

        public bool removeWhenOutside;

        public MultiMusicTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            flagA = data.Attr("flagA", "");
            flagB = data.Attr("flagB", "");
            trackNone = data.Attr("trackNone", "");
            trackA = data.Attr("trackA", "");
            trackB = data.Attr("trackB", "");
            trackBoth = data.Attr("trackBoth", "");
            removeWhenOutside = data.Bool("removeWhenOutside");
        }

        public override void Update()
        {
            Player player = SceneAs<Level>().Tracker.GetEntity<Player>();
            if (player != null && !CollideCheck(player) && removeWhenOutside)
            {
                RemoveSelf();
            }
        }

        public override void OnStay(Player player)
        {
            if (flagA != "")
            {
                Session session = SceneAs<Level>().Session;
                if ((!SceneAs<Level>().Session.GetFlag(flagA) && flagB != "" && !SceneAs<Level>().Session.GetFlag(flagB)) || (!SceneAs<Level>().Session.GetFlag(flagA) && flagB == ""))
                {
                    if (trackNone != "")
                    {
                        session.Audio.Music.Event = SFX.EventnameByHandle(trackNone);
                    }
                }
                else if (SceneAs<Level>().Session.GetFlag(flagA) && (flagB == "" || !SceneAs<Level>().Session.GetFlag(flagB)))
                {
                    if (trackA != "")
                    {
                        session.Audio.Music.Event = SFX.EventnameByHandle(trackA);
                    }
                }
                else if (!SceneAs<Level>().Session.GetFlag(flagA) && flagB != "" && SceneAs<Level>().Session.GetFlag(flagB))
                {
                    if (trackB != "")
                    {
                        session.Audio.Music.Event = SFX.EventnameByHandle(trackB);
                    }
                }
                else if (SceneAs<Level>().Session.GetFlag(flagA) && flagB != "" && SceneAs<Level>().Session.GetFlag(flagB))
                {
                    if (trackBoth != "")
                    {
                        session.Audio.Music.Event = SFX.EventnameByHandle(trackBoth);
                    }
                }
                session.Audio.Apply(forceSixteenthNoteHack: false);
            }
        }
    }
}
