using RimWorld;
using System;
using System.Reflection;
using Verse;

namespace AdeptusMechanicus
{
    [StaticConstructorOnStartup]
    static class UtilCE
    {
        private static bool logging = false;
        private static bool initialized = false;
        public static bool CombatExtended = false;
        static UtilCE()
        {
            foreach (ModContentPack p in LoadedModManager.RunningMods)
            {
                //Log.Message(string.Format("{0} Identifier is {1}", p.Name, p.Identifier));
                if (p.Identifier == "1631756268")
                {
                    CombatExtended = true;
                }
                /*
                */
            }
        }
        
    }
}