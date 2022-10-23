using Celeste.Mod.Entities;
using Celeste.Mod.XaphanHelper.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.XaphanHelper.Triggers
{
    [CustomEntity("XaphanHelper/ResetFlagsTrigger")]
    class ResetFlagsTrigger : Trigger
    {
        public string setTrueFlags;

        public string setFalseFlags;

        public bool transitionUpdate;

        public bool removeWhenOutside;

        public bool registerInSaveData;

        public bool entered;

        public ResetFlagsTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            setTrueFlags = data.Attr("setTrueFlags");
            setFalseFlags = data.Attr("setFalseFlags");
            transitionUpdate = data.Bool("transitionUpdate");
            removeWhenOutside = data.Bool("removeWhenOutside");
            registerInSaveData = data.Bool("registerInSaveData");
            if (transitionUpdate)
            {
                Tag = Tags.TransitionUpdate;
            }
        }

        public override void Update()
        {
            base.Update();
            Player player = SceneAs<Level>().Tracker.GetEntity<Player>();
            string Prefix = SceneAs<Level>().Session.Area.GetLevelSet();
            int chapterIndex = SceneAs<Level>().Session.Area.ChapterIndex;
            if (player != null && CollideCheck(player) && !entered)
            {
                entered = true;
                string[] trueFlags = setTrueFlags.Split(',');
                string[] falseFlags = setFalseFlags.Split(',');
                foreach (string flag in trueFlags)
                {
                    if (!SceneAs<Level>().Session.GetFlag(flag))
                    {
                        SceneAs<Level>().Session.SetFlag(flag, true);
                        if (registerInSaveData)
                        {
                            if (!(XaphanModule.Instance._SaveData as XaphanModuleSaveData).SavedFlags.Contains(Prefix + "_Ch" + chapterIndex + "_" + flag))
                            {
                                (XaphanModule.Instance._SaveData as XaphanModuleSaveData).SavedFlags.Add(Prefix + "_Ch" + chapterIndex + "_" + flag);
                            }
                        }
                        foreach (FlagDashSwitch dashSwitch in SceneAs<Level>().Tracker.GetEntities<FlagDashSwitch>())
                        {
                            if (dashSwitch.flag == flag && !dashSwitch.registerInSaveData)
                            {
                                dashSwitch.flagState = true;
                                if (dashSwitch.mode == "SetFalse" || dashSwitch.mode == "SetInverted")
                                {
                                    dashSwitch.ResetSwitch();
                                }
                            }
                        }
                    }
                }
                foreach (string flag in falseFlags)
                {
                    if (SceneAs<Level>().Session.GetFlag(flag))
                    {
                        if (!(XaphanModule.Instance._SaveData as XaphanModuleSaveData).SavedFlags.Contains(Prefix + "_Ch" + SceneAs<Level>().Session.Area.ChapterIndex + "_" + flag))
                        {
                            SceneAs<Level>().Session.SetFlag(flag, false);
                        }
                        if (registerInSaveData)
                        {
                            if ((XaphanModule.Instance._SaveData as XaphanModuleSaveData).SavedFlags.Contains(Prefix + "_Ch" + chapterIndex + "_" + flag))
                            {
                                (XaphanModule.Instance._SaveData as XaphanModuleSaveData).SavedFlags.Remove(Prefix + "_Ch" + chapterIndex + "_" + flag);
                            }
                        }
                        foreach (FlagDashSwitch dashSwitch in SceneAs<Level>().Tracker.GetEntities<FlagDashSwitch>())
                        {
                            if (dashSwitch.flag == flag && !dashSwitch.registerInSaveData)
                            {
                                dashSwitch.flagState = false;
                                if (dashSwitch.mode == "SetTrue" || dashSwitch.mode == "SetInverted")
                                {
                                    dashSwitch.ResetSwitch();
                                }
                            }
                        }
                    }
                }
            }
            else if (player != null && !CollideCheck(player) && removeWhenOutside)
            {
                RemoveSelf();
            }
        }

        public override void OnLeave(Player player)
        {
            base.OnLeave(player);
            entered = false;
        }
    }
}
