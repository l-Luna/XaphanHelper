

namespace Celeste.Mod.XaphanHelper.Events
{
    class E05_EscapeEnd : CutsceneEntity
    {
        private Player player;

        public E05_EscapeEnd(Player player, Level level)
        {
            this.player = player;
        }

        public override void OnBegin(Level level)
        {
            level.InCutscene = false;
            level.CancelCutscene();
            if (level.Session.Area.ChapterIndex == 5 && level.Session.GetFlag("Lab_Escape"))
            {
                player.StateMachine.State = Player.StTempleFall;
            }
            else if (level.Session.Area.ChapterIndex == 4)
            {
                player.StateMachine.State = XaphanModule.StFastFall;
            }
        }

        public override void OnEnd(Level level)
        {

        }
    }
}
