namespace Celeste.Mod.XaphanHelper.Upgrades
{
    class Spazer : Upgrade
    {
        public override int GetDefaultValue()
        {
            return 0;
        }

        public override int GetValue()
        {
            return Settings.Spazer ? 1 : 0;
        }

        public override void SetValue(int value)
        {
            Settings.Spazer = (value != 0);
        }

        public override void Load()
        {
        }

        public override void Unload()
        {
        }

        public static bool Active(Level level)
        {
            return XaphanModule.Settings.Spazer && !(XaphanModule.Instance._SaveData as XaphanModuleSaveData).SpazerInactive.Contains(level.Session.Area.GetLevelSet());
        }
    }
}
