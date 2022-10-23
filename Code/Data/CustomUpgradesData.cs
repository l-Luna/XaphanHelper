namespace Celeste.Mod.XaphanHelper.Data
{
    public class CustomUpgradesData
    {
        public string Upgrade;

        public string CustomName;

        public string CustomSpritePath;

        public CustomUpgradesData(string upgrade, string customName, string customSpritePath)
        {
            Upgrade = upgrade;
            CustomName = customName;
            CustomSpritePath = customSpritePath;
        }
    }
}
