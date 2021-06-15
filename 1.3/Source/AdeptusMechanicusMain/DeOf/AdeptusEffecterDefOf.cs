using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class AdeptusEffecterDefOf
    {
        static AdeptusEffecterDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AdeptusEffecterDefOf));
        }
        public static EffecterDef OG_Effecter_EMP;

    }
}
