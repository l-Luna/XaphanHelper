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

        public string ReqID;

        public AchievementData(string achievementID, int categoryID, string icon, string flag, int currentValue, int maxValue, int medals, bool hidden = false, string reqID = null)
        {
            AchievementID = achievementID;
            CategoryID = categoryID;

            string subStr = "";
            if (AchievementID.Length > 1)
            {
                subStr = AchievementID.Substring(1);
            }
            string convertedID = char.ToUpper(AchievementID[0]) + subStr;

            Name = "Achievement_" + convertedID + "_Name";
            Description = "Achievement_" + convertedID + "_Description";
            Icon = icon;
            Flag = flag;
            CurrentValue = currentValue;
            MaxValue = maxValue;
            Medals = medals;
            Hidden = hidden;
            ReqID = reqID;
        }
    }
}
