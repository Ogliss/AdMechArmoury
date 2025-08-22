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
        public static ScenPartDef DisableIncident;
        public static ScenPartDef StartingThing_Defined;
        [MayRequireXenobiologis]
        public static ScenPartDef OG_Rule_EnforceFactionRelations;

    }
}
