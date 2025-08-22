using RimWorld;
using System;
using Verse;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class AdeptusHediffGiverSetDefOf
    {
        static AdeptusHediffGiverSetDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AdeptusHediffGiverSetDefOf));
        }

        [MayRequireAstartes]
        public static HediffGiverSetDef OG_Astartes_HediffGiverSet;
    }
}
