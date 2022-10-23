using Celeste.Mod.XaphanHelper.Entities;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Managers
{
    [Tracked(true)]
    class LaserDetectorManager : Entity
    {
        private string Prefix;

        private int chapterIndex;

        public LaserDetectorManager()
        {

        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            Prefix = SceneAs<Level>().Session.Area.GetLevelSet();
            chapterIndex = SceneAs<Level>().Session.Area.ChapterIndex;
        }

        public override void Update()
        {
            base.Update();
            foreach (LaserDetector detector in SceneAs<Level>().Tracker.GetEntities<LaserDetector>())
            {
                if (!string.IsNullOrEmpty(detector.flag) && !(XaphanModule.Instance._SaveData as XaphanModuleSaveData).SavedFlags.Contains(Prefix + "_Ch" + chapterIndex + "_" + detector.flag))
                {
                    if (detector.isActive)
                    {
                        SceneAs<Level>().Session.SetFlag(detector.flag, true);
                    }
                    else
                    {
                        bool atLeastOneActive = false;
                        foreach (LaserDetector detector2 in SceneAs<Level>().Tracker.GetEntities<LaserDetector>())
                        {
                            if (detector2 != detector && detector2.flag == detector.flag)
                            {
                                if (detector2.isActive)
                                {
                                    SceneAs<Level>().Session.SetFlag(detector.flag, true);
                                    atLeastOneActive = true;
                                    break;
                                }
                            }
                        }
                        if (!atLeastOneActive)
                        {
                            SceneAs<Level>().Session.SetFlag(detector.flag, false);
                        }
                    }
                }
            }
        }
    }
}
