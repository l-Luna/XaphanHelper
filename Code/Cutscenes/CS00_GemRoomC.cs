using Celeste.Mod.XaphanHelper.Controllers;
using Celeste.Mod.XaphanHelper.Entities;
using Monocle;
using System.Collections;

namespace Celeste.Mod.XaphanHelper.Cutscenes
{
    class CS00_GemRoomC : CutsceneEntity
    {
        private readonly Player player;

        private GemController gemController;

        public CS00_GemRoomC(Player player)
        {
            this.player = player;
        }

        public override void OnBegin(Level level)
        {
            gemController = Scene.Entities.FindFirst<GemController>();
            Add(new Coroutine(Cutscene(level)));
        }

        public override void OnEnd(Level level)
        {
            if (WasSkipped)
            {
                level.Session.SetFlag("CS_Ch0_Gem_Room_Activeate_Gems");
            }
            if ((XaphanModule.Instance._SaveData as XaphanModuleSaveData).SavedFlags.Contains("Xaphan/0_End_Area_Open"))
            {
                (XaphanModule.Instance._SaveData as XaphanModuleSaveData).WatchedCutscenes.Add("Xaphan/0_Ch0_Gem_Room_C");
                level.Session.SetFlag("CS_Ch0_Gem_Room_C");
            }
            player.StateMachine.State = 0;
        }

        public IEnumerator Cutscene(Level level)
        {
            player.StateMachine.State = 11;
            if (!(XaphanModule.Instance._SaveData as XaphanModuleSaveData).SavedFlags.Contains("Xaphan/0_Ch0_Gem_Room_First_Time"))
            {
                player.Facing = Facings.Right;
                (XaphanModule.Instance._SaveData as XaphanModuleSaveData).SavedFlags.Add("Xaphan/0_Ch0_Gem_Room_First_Time");
                yield return Textbox.Say("Xaphan_Ch0_A_Gem_Room_C");
            }
            yield return player.DummyWalkToExact((int)gemController.X);
            yield return gemController.ActivateGems();
            int missingGems = 0;
            for (int i = 1; i <= 4; i++)
            {
                if (!(XaphanModule.Instance._SaveData as XaphanModuleSaveData).SavedFlags.Contains("Xaphan/0_Ch" + i + "_Gem_Sloted"))
                {
                    missingGems++;
                }
            }
            if (missingGems > 0)
            {
                yield return Textbox.Say("Xaphan_Ch0_A_Gem_Room_C_b_" + missingGems);
            }
            if (missingGems == 3)
            {
                (XaphanModule.Instance._SaveData as XaphanModuleSaveData).GlobalFlags.Add("Xaphan/0_Ch0_Ch1_Gem_Sloted");
            }
            else if (missingGems == 2)
            {
                (XaphanModule.Instance._SaveData as XaphanModuleSaveData).GlobalFlags.Add("Xaphan/0_Ch0_Ch1_Gem_Sloted");
                (XaphanModule.Instance._SaveData as XaphanModuleSaveData).GlobalFlags.Add("Xaphan/0_Ch0_Ch2_Gem_Sloted");
            }
            else if (missingGems == 3)
            {
                (XaphanModule.Instance._SaveData as XaphanModuleSaveData).GlobalFlags.Add("Xaphan/0_Ch0_Ch1_Gem_Sloted");
                (XaphanModule.Instance._SaveData as XaphanModuleSaveData).GlobalFlags.Add("Xaphan/0_Ch0_Ch2_Gem_Sloted");
                (XaphanModule.Instance._SaveData as XaphanModuleSaveData).GlobalFlags.Add("Xaphan/0_Ch0_Ch3_Gem_Sloted");
            }
            EndCutscene(Level);
        }
    }
}
