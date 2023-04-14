namespace Celeste.Mod.XaphanHelper.Data
{
    public class AchievementData
    {
        public string AchievementID;

        public int CategoryID;

        public string Name;

        public string Description;

        public string Icon;

        public string Flag;

        public int CurrentValue;

        public int MaxValue;

        public int Medals;

        public bool Hidden;

        public AchievementData(string achievementID, int categoryID, string name, string icon, string flag, int currentValue, int maxValue, int medals, bool hidden = false)
        {
            AchievementID = achievementID;
            CategoryID = categoryID;
            Name = "Achievement_" + name + "_Name";
            Description = "Achievement_" + name + "_Description";
            Icon = icon;
            Flag = flag;
            CurrentValue = currentValue;
            MaxValue = maxValue;
            Medals = medals;
            Hidden = hidden;
        }
    }
}
