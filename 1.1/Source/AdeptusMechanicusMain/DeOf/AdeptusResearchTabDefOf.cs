using RimWorld;
using System;
using Verse;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class AdeptusResearchTabDefOf
    {
        static AdeptusResearchTabDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AdeptusResearchTabDefOf));
        }
        public static readonly ResearchTabDef OGAMA_RTab;
        public static readonly ResearchTabDef OGAMA_RSubTab_Imperial;
        public static readonly ResearchTabDef OGAMA_RSubTab_Aeldari;
        public static readonly ResearchTabDef OGAMA_RSubTab_Tau;
        public static readonly ResearchTabDef OGAMA_RSubTab_Greenskin;

    }
}
