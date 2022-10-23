using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

namespace Celeste.Mod.XaphanHelper.Cutscenes
{
    class CS02_TempleDoor : CutsceneEntity
    {
        private readonly Player player;

        public CS02_TempleDoor(Player player)
        {
            this.player = player;
        }

        public override void OnBegin(Level level)
        {
            Add(new Coroutine(Cutscene(level)));
        }

        public override void OnEnd(Level level)
        {
            (XaphanModule.Instance._SaveData as XaphanModuleSaveData).WatchedCutscenes.Add("Xaphan/0_Ch2_TempleDoor");
            level.Session.SetFlag("CS_Ch2_TempleDoor");
        }

        public IEnumerator Cutscene(Level level)
        {
            level.Add(new MiniTextbox("Xaphan_Ch2_A_TempleDoor"));
            yield return null;
            EndCutscene(Level);
        }
    }
}