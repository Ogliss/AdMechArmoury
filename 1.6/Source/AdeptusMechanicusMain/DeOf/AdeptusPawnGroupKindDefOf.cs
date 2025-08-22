using RimWorld;
using System;
using Verse;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class AdeptusPawnGroupKindDefOf
    {
        static AdeptusPawnGroupKindDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AdeptusPawnGroupKindDefOf));
        }

        // AdeptusPawnGroupKindDefOf.OG_Combat_DeepStrike
        public static PawnGroupKindDef OG_Combat_DeepStrike;
    }
}
