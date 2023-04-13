namespace Celeste.Mod.XaphanHelper.Data
{
    public class AchievementData
    {
        public int CategoryID;

        public string Name;

        public string Description;

        public string Icon;

        public string Flag;

        public int CurrentValue;

        public int MaxValue;

        public int Medals;

        public AchievementData(int categoryID, string name, string icon, string flag, int currentValue, int maxValue, int medals)
        {
            CategoryID = categoryID;
            Name = "Achievement_" + name + "_Name";
            Description = "Achievement_" + name + "_Description";
            Icon = icon;
            Flag = flag;
            CurrentValue = currentValue;
            MaxValue = maxValue;
            Medals = medals;
        }
    }
}
