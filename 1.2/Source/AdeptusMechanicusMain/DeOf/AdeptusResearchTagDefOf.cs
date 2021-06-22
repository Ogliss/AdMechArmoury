using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class AdeptusResearchTagDefOf
    {
        static AdeptusResearchTagDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AdeptusResearchTagDefOf));
        }
        public static ResearchProjectTagDef OG_Common_Tech;
        public static ResearchProjectTagDef OG_Imperial_Tech;
        public static ResearchProjectTagDef OG_Aeldari_Tech;
        public static ResearchProjectTagDef OG_Tau_Tech;
        public static ResearchProjectTagDef OG_Greenskin_Tech;

    }
}
