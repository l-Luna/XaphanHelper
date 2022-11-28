using System;

namespace Celeste.Mod.XaphanHelper.Data
{
    public class CustomEndScreenControllerData : IComparable<CustomEndScreenControllerData>
    {
        public string Atlas;

        public string Images;

        public string Title;

        public bool ShowTitle;

        public string SubText1;

        public string SubText1Color;

        public string SubText2;

        public string SubText2Color;

        public string Music;

        public bool HideVanillaTimer;

        public long RequiredTime;

        public bool ShowTime;

        public int RequiredStrawberries;

        public bool ShowStrawberries;

        public string StrawberriesColor;

        public string StrawberriesMaxColor;

        public int RequiredItemPercent;

        public bool ShowItemPercent;

        public string ItemPercentColor;

        public string ItemPercentMaxColor;

        public int RequiredMapPercent;

        public bool ShowMapPercent;

        public string MapPercentColor;

        public string MapPercentMaxColor;

        public string RequiredFlags;

        public string RequirementsCheck;

        public int Priority;

        public CustomEndScreenControllerData(string atlas = "", string images = "", string title = "", bool showTitle = true, string subText1 = "", string subText1Color = "", string subText2 = "", string subText2Color = "", string music = "", bool hideVanillaTimer = false, long requiredTime = 0, bool showTime = false, int requiredStrawberries = 0, bool showStrawberries = false, string strawberriesColor = "", string strawberriesMaxColor = "", int requiredItemPercent = 0, bool showItemPercent = false, string itemPercentColor = "", string itemPercentMaxColor = "", int requiredMapPercent = 0, bool showMapPercent = false, string mapPercentColor = "", string mapPercentMaxColor = "", string requiredFlags = "", string requirementsCheck = "", int priority = 0)
        {
            Atlas = atlas;
            Images = images;
            Title = title;
            ShowTitle = showTitle;
            SubText1 = subText1;
            SubText1Color = subText1Color;
            SubText2 = subText2;
            SubText2Color = subText2Color;
            Music = music;
            HideVanillaTimer = hideVanillaTimer;
            RequiredTime = requiredTime;
            ShowTime = showTime;
            RequiredStrawberries = requiredStrawberries;
            ShowStrawberries = showStrawberries;
            StrawberriesColor = strawberriesColor;
            StrawberriesMaxColor = strawberriesMaxColor;
            RequiredItemPercent = requiredItemPercent;
            ShowItemPercent = showItemPercent;
            ItemPercentColor = itemPercentColor;
            ItemPercentMaxColor = itemPercentMaxColor;
            RequiredMapPercent = requiredMapPercent;
            ShowMapPercent = showMapPercent;
            MapPercentColor = mapPercentColor;
            MapPercentMaxColor = mapPercentMaxColor;
            RequiredFlags = requiredFlags;
            RequirementsCheck = requirementsCheck;
            Priority = priority;
        }

        public int CompareTo(CustomEndScreenControllerData compare)
        {
            if (compare == null)
            {
                return 1;
            }
            else
            {
                return Priority.CompareTo(compare.Priority);
            }
        }
    }
}
