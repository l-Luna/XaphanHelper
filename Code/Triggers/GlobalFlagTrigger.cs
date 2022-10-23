using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.XaphanHelper.Triggers
{
    [CustomEntity("XaphanHelper/GlobalFlagTrigger")]
    class GlobalFlagTrigger : Trigger
    {
        public string flag;

        public bool state;

        public bool switchFlag;

        public string levelSet;

        public GlobalFlagTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            flag = data.Attr("flag");
            state = data.Bool("state", true);
            switchFlag = data.Bool("switchFlag");
            levelSet = data.Attr("levelSet");
        }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            string Prefix = SceneAs<Level>().Session.Area.GetLevelSet();
            if (string.IsNullOrEmpty(levelSet))
            {
                levelSet = Prefix;
            }
            int chapterIndex = SceneAs<Level>().Session.Area.ChapterIndex == -1 ? 0 : SceneAs<Level>().Session.Area.ChapterIndex;
            if ((!switchFlag && state) || (switchFlag && !(XaphanModule.Instance._SaveData as XaphanModuleSaveData).GlobalFlags.Contains(levelSet + "_Ch" + chapterIndex + "_" + flag)))
            {
                if (!(XaphanModule.Instance._SaveData as XaphanModuleSaveData).GlobalFlags.Contains(levelSet + "_Ch" + chapterIndex + "_" + flag))
                {
                    (XaphanModule.Instance._SaveData as XaphanModuleSaveData).GlobalFlags.Add(levelSet + "_Ch" + chapterIndex + "_" + flag);
                    if (levelSet == Prefix)
                    {
                        SceneAs<Level>().Session.SetFlag("Ch" + chapterIndex + "_" + flag, true);
                    }
                }
            }
            else
            {
                if ((XaphanModule.Instance._SaveData as XaphanModuleSaveData).GlobalFlags.Contains(levelSet + "_Ch" + chapterIndex + "_" + flag))
                {
                    (XaphanModule.Instance._SaveData as XaphanModuleSaveData).GlobalFlags.Remove(levelSet + "_Ch" + chapterIndex + "_" + flag);
                    if (levelSet == Prefix)
                    {
                        SceneAs<Level>().Session.SetFlag("Ch" + chapterIndex + "_" + flag, false);
                    }
                }
            }
        }
    }
}