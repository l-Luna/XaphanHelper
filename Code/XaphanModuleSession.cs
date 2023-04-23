using System.Collections.Generic;

namespace Celeste.Mod.XaphanHelper
{
    public class XaphanModuleSession : EverestModuleSession
    {
        // Drone

        public int CurrentDroneMissile = 0;

        public int CurrentDroneSuperMissile = 0;

        public int CurrentAmmoSelected = 0;

        // Entities respawn

        public HashSet<EntityID> NoRespawnIds = new();
    }
}