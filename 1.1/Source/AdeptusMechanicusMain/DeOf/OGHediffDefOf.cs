using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class OGHediffDefOf
    {
        static OGHediffDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(OGHediffDefOf));
        }
        public static HediffDef OG_Hediff_Regenerated_Part;
        public static HediffDef OG_Hediff_Regenerating_Part;
        public static HediffDef OG_Hediff_PlasmaBurn;
        public static HediffDef OG_Hediff_RadiationPoisioning;
        public static HediffDef OG_Hediff_FWPsychicShock;

    }
}
