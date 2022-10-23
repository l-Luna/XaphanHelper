namespace Celeste.Mod.XaphanHelper.Upgrades
{
    class LongBeam : Upgrade
    {
        public override int GetDefaultValue()
        {
            return 0;
        }

        public override int GetValue()
        {
            return Settings.LongBeam ? 1 : 0;
        }

        public override void SetValue(int value)
        {
            Settings.LongBeam = (value != 0);
        }

        public override void Load()
        {
        }

        public override void Unload()
        {
        }

        public static bool Active(Level level)
        {
            return XaphanModule.Settings.LongBeam && !(XaphanModule.Instance._SaveData as XaphanModuleSaveData).LongBeamInactive.Contains(level.Session.Area.GetLevelSet());
        }
    }
}
