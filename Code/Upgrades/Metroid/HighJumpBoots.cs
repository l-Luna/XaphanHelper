namespace Celeste.Mod.XaphanHelper.Upgrades
{
    class HighJumpBoots : Upgrade
    {
        public override int GetDefaultValue()
        {
            return 0;
        }

        public override int GetValue()
        {
            return Settings.HighJumpBoots ? 1 : 0;
        }

        public override void SetValue(int value)
        {
            Settings.HighJumpBoots = (value != 0);
        }

        public override void Load()
        {
        }

        public override void Unload()
        {
        }

        public static bool Active(Level level)
        {
            return XaphanModule.Settings.HighJumpBoots && !(XaphanModule.Instance._SaveData as XaphanModuleSaveData).HighJumpBootsInactive.Contains(level.Session.Area.GetLevelSet());
        }
    }
}
