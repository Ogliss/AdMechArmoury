using RimWorld;
using System;
using Verse;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class AdeptusScenPartDefOf
    {
        static AdeptusScenPartDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AdeptusScenPartDefOf));
        }
        [MayRequireXenobiologis]
        public static ScenPartDef OG_Rule_EnforceFactionRelations;

    }
}
