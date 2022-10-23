namespace Celeste.Mod.XaphanHelper.Upgrades
{
    class SpringBall : Upgrade
    {
        public override int GetDefaultValue()
        {
            return 0;
        }

        public override int GetValue()
        {
            return Settings.SpringBall ? 1 : 0;
        }

        public override void SetValue(int value)
        {
            Settings.SpringBall = (value != 0);
        }

        public override void Load()
        {
        }

        public override void Unload()
        {
        }

        public static bool Active(Level level)
        {
            return XaphanModule.Settings.SpringBall && !(XaphanModule.Instance._SaveData as XaphanModuleSaveData).SpringBallInactive.Contains(level.Session.Area.GetLevelSet());
        }
    }
}
