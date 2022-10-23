namespace Celeste.Mod.XaphanHelper.Upgrades
{
    class MorphingBall : Upgrade
    {
        public override int GetDefaultValue()
        {
            return 0;
        }

        public override int GetValue()
        {
            return Settings.MorphingBall ? 1 : 0;
        }

        public override void SetValue(int value)
        {
            Settings.MorphingBall = (value != 0);
        }

        public override void Load()
        {
        }

        public override void Unload()
        {
        }

        public static bool Active(Level level)
        {
            return XaphanModule.Settings.MorphingBall && !(XaphanModule.Instance._SaveData as XaphanModuleSaveData).MorphingBallInactive.Contains(level.Session.Area.GetLevelSet());
        }
    }
}
