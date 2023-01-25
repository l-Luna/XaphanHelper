namespace Celeste.Mod.XaphanHelper.Upgrades
{
    class SuperMissiles : Upgrade
    {
        public override int GetDefaultValue()
        {
            return 0;
        }

        public override int GetValue()
        {
            return Settings.SuperMissiles ? 1 : 0;
        }

        public override void SetValue(int value)
        {
            Settings.SuperMissiles = (value != 0);
        }

        public override void Load()
        {
        }

        public override void Unload()
        {
        }

        public static bool Active(Level level)
        {
            return XaphanModule.Settings.SuperMissiles && !XaphanModule.ModSaveData.SuperMissilesInactive.Contains(level.Session.Area.GetLevelSet());
        }
    }
}
