using RimWorld;
using System;
using Verse;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class AdeptusMentalBreakDefOf
    {
        static AdeptusMentalBreakDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AdeptusMentalBreakDefOf));
        }
        [MayRequireOrkz]
        public static MentalBreakDef OGO_Orkoid_Starving;

    }
}
