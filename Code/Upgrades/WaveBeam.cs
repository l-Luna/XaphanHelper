namespace Celeste.Mod.XaphanHelper.Upgrades
{
    class WaveBeam : Upgrade
    {
        public override int GetDefaultValue()
        {
            return 0;
        }

        public override int GetValue()
        {
            return Settings.WaveBeam ? 1 : 0;
        }

        public override void SetValue(int value)
        {
            Settings.WaveBeam = (value != 0);
        }

        public override void Load()
        {
        }

        public override void Unload()
        {
        }

        public static bool Active(Level level)
        {
            return XaphanModule.Settings.WaveBeam && !(XaphanModule.Instance._SaveData as XaphanModuleSaveData).WaveBeamInactive.Contains(level.Session.Area.GetLevelSet());
        }
    }
}
