using Monocle;
using System.Collections;

namespace Celeste.Mod.XaphanHelper.Cutscenes
{
    class CS01_MainHall : CutsceneEntity
    {
        private readonly Player player;

        public CS01_MainHall(Player player)
        {
            this.player = player;
        }

        public override void OnBegin(Level level)
        {
            Add(new Coroutine(Cutscene(level)));
        }

        public override void OnEnd(Level level)
        {
            (XaphanModule.Instance._SaveData as XaphanModuleSaveData).WatchedCutscenes.Add("Xaphan/0_Ch1_Main_Hall");
            level.Session.SetFlag("CS_Ch1_Main_Hall");
            player.StateMachine.State = 0;
        }

        public IEnumerator Cutscene(Level level)
        {
            player.StateMachine.State = 11;
            yield return Textbox.Say("Xaphan_Ch1_A_Main_Hall");
            EndCutscene(Level);
        }
    }
}