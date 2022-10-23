namespace Celeste.Mod.XaphanHelper.Upgrades
{
    class DroneTeleport : Upgrade
    {
        public override int GetDefaultValue()
        {
            return 0;
        }

        public override int GetValue()
        {
            return Settings.DroneTeleport ? 1 : 0;
        }

        public override void SetValue(int value)
        {
            Settings.DroneTeleport = (value != 0);
        }

        public override void Load()
        {
        }

        public override void Unload()
        {
        }

        public static bool Active(Level level)
        {
            return XaphanModule.Settings.DroneTeleport && !(XaphanModule.Instance._SaveData as XaphanModuleSaveData).DroneTeleportInactive.Contains(level.Session.Area.GetLevelSet());
        }
    }
}
