using RimWorld;
using System;
using Verse;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class OGVatJobDefOf
    {
        static OGVatJobDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(OGVatJobDefOf));
        }
        public static JobDef OG_ExtractFromGrowerJob; 
        public static JobDef OG_DepositIntoGrowerJob;
        public static JobDef OG_MaintainGrowerJob_Intellectual;
        public static JobDef OG_MaintainGrowerJob_Medicine;

    }
    [DefOf]
    public static class OGVatThingDefOf
    {
        static OGVatThingDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(OGVatThingDefOf));
        }

        public static ThingDef OG_NutrientSolution;
        public static ThingDef OG_ProteinMash;

    }

    public static class OGVatSoundDefOf
    {
        static OGVatSoundDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(OGVatSoundDefOf));
        }

        public static SoundDef Interact_Research;

    }

}
