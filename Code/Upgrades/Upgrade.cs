using System.Collections.Generic;
using Celeste.Mod.XaphanHelper.UI_Elements;
using Monocle;

namespace Celeste.Mod.XaphanHelper.Upgrades
{
    public abstract class Upgrade
    {
        public abstract int GetDefaultValue();

        public abstract void Load();

        public abstract void Unload();

        public abstract int GetValue();

        public abstract void SetValue(int value);

        public static BagDisplay GetDisplay(Level level, string type)
        {
            List<Entity> displays = level.Tracker.GetEntities<BagDisplay>();
            foreach (Entity entity in displays)
            {
                BagDisplay display = entity as BagDisplay;
                if (display.type == type)
                {
                    return display;
                }
            }
            return null;
        }
    }
}