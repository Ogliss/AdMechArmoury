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
        public static HediffDef Regenerated_Part_OG;
        public static HediffDef Regenerating_Part_OG;
        public static HediffDef PlasmaBurn;
        public static HediffDef RadiationPoisioning;
        public static HediffDef FWPsychicShock;
        public static DamageDef OGForceStrike;

    }
}
