namespace Celeste.Mod.XaphanHelper.Upgrades
{
    class Missiles : Upgrade
    {
        public override int GetDefaultValue()
        {
            return 0;
        }

        public override int GetValue()
        {
            return Settings.Missiles ? 1 : 0;
        }

        public override void SetValue(int value)
        {
            Settings.Missiles = (value != 0);
        }

        public override void Load()
        {
        }

        public override void Unload()
        {
        }

        public static bool Active(Level level)
        {
            return XaphanModule.Settings.Missiles && !XaphanModule.ModSaveData.MissilesInactive.Contains(level.Session.Area.GetLevelSet());
        }
    }
}
