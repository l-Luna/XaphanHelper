namespace Celeste.Mod.XaphanHelper.Data
{
    public class InGameMapImageControllerData
    {
        public int ChapterIndex;

        public string Room;

        public string ImagePath;

        public string Color;

        public InGameMapImageControllerData(int chapterIndex, string room, string imagePath, string color)
        {
            ChapterIndex = chapterIndex;
            Room = room;
            ImagePath = imagePath;
            Color = color;
        }
    }
}
