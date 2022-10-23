namespace Celeste.Mod.XaphanHelper.Upgrades
{
    class SpeedBooster : Upgrade
    {
        public override int GetDefaultValue()
        {
            return 0;
        }

        public override int GetValue()
        {
            return Settings.SpeedBooster ? 1 : 0;
        }

        public override void SetValue(int value)
        {
            Settings.SpeedBooster = (value != 0);
        }

        public override void Load()
        {
        }

        public override void Unload()
        {
        }

        public static bool Active(Level level)
        {
            return XaphanModule.Settings.SpeedBooster && !(XaphanModule.Instance._SaveData as XaphanModuleSaveData).SpeedBoosterInactive.Contains(level.Session.Area.GetLevelSet());
        }
    }
}
