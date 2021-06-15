using Verse;

namespace AdeptusMechanicus
{
    static class UtilDinosauria
    {
        public static bool Dinosauria = false;
        public static ModContentPack modContent = null;
        static UtilDinosauria()
        {
            foreach (ModContentPack p in LoadedModManager.RunningMods)
            {
                if (p.Name.Contains("Dinosauria"))
                {
                    modContent = p;
                    Dinosauria = true;
                }
            }
        }

    }
}