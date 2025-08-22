using System;
using System.Reflection;
using Verse;

namespace AdeptusMechanicus
{
    static class UtilJurassicRimworld
    {
        public static bool JurassicRimworld = false;
        public static ModContentPack modContent = null;
        static UtilJurassicRimworld()
        {
            foreach (ModContentPack p in LoadedModManager.RunningMods)
            {
                if (p.Name.Contains("Jurassic Rimworld"))
                {
                    modContent = p;
                    JurassicRimworld = true;
                }
            }
        }
    }
}