using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

namespace Celeste.Mod.XaphanHelper.Cutscenes
{
    class CS00_GemRoomA : CutsceneEntity
    {
        private readonly Player player;

        public CS00_GemRoomA(Player player)
        {
            this.player = player;
        }

        public override void OnBegin(Level level)
        {
            Add(new Coroutine(Cutscene(level)));
        }

        public override void OnEnd(Level level)
        {
            (XaphanModule.Instance._SaveData as XaphanModuleSaveData).WatchedCutscenes.Add("Xaphan/0_Ch0_Gem_Room_A");
            level.Session.SetFlag("CS_Ch0_Gem_Room_A");
            player.StateMachine.State = 0;
        }

        public IEnumerator Cutscene(Level level)
        {
            player.StateMachine.State = 11;
            yield return Textbox.Say("Xaphan_Ch0_A_Gem_Room_A");
            EndCutscene(Level);
        }
    }
}
