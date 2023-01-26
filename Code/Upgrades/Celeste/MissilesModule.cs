namespace Celeste.Mod.XaphanHelper.Upgrades
{
    class MissilesModule : Upgrade
    {
        public override int GetDefaultValue()
        {
            return 0;
        }

        public override int GetValue()
        {
            return Settings.MissilesModule ? 1 : 0;
        }

        public override void SetValue(int value)
        {
            Settings.MissilesModule = (value != 0);
        }

        public override void Load()
        {
        }

        public override void Unload()
        {
        }

        public static bool Active(Level level)
        {
            return XaphanModule.Settings.MissilesModule && !XaphanModule.ModSaveData.MissilesModuleInactive.Contains(level.Session.Area.GetLevelSet());
        }
    }
}
