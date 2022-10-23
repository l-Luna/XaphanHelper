using Celeste.Mod.Entities;
using Celeste.Mod.XaphanHelper.UI_Elements;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.XaphanHelper.Triggers
{

    [CustomEntity("XaphanHelper/StopCountdownTrigger")]
    class StopCountdownTrigger : Trigger
    {
        public StopCountdownTrigger(EntityData data, Vector2 offset, EntityID ID) : base(data, offset)
        {

        }

        public override void OnStay(Player player)
        {
            base.OnStay(player);
            CountdownDisplay timer = SceneAs<Level>().Tracker.GetEntity<CountdownDisplay>();
            if (timer != null)
            {
                SceneAs<Level>().Session.SetFlag(timer.activeFlag, false);
                (XaphanModule.Instance._SaveData as XaphanModuleSaveData).CountdownCurrentTime = -1;
                (XaphanModule.Instance._SaveData as XaphanModuleSaveData).CountdownShake = false;
                (XaphanModule.Instance._SaveData as XaphanModuleSaveData).CountdownExplode = false;
                (XaphanModule.Instance._SaveData as XaphanModuleSaveData).CountdownActiveFlag = "";
                (XaphanModule.Instance._SaveData as XaphanModuleSaveData).CountdownStartChapter = -1;
                (XaphanModule.Instance._SaveData as XaphanModuleSaveData).CountdownStartRoom = "";
                (XaphanModule.Instance._SaveData as XaphanModuleSaveData).CountdownSpawn = new Vector2();
                SceneAs<Level>().SaveQuitDisabled = false;
                timer.RemoveSelf();
                StartCountdownTrigger startTrigger = SceneAs<Level>().Tracker.GetEntity<StartCountdownTrigger>();
                if (startTrigger != null)
                {
                    startTrigger.CancelTimer();
                }
            }
        }
    }
}
