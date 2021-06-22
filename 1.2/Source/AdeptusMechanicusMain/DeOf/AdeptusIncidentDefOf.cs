using RimWorld;
using System;
using Verse;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class AdeptusIncidentDefOf
    {
        static AdeptusIncidentDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AdeptusIncidentDefOf));
        }

        [MayRequireXenobiologis, MayRequireOrkz]
        public static IncidentDef OG_Ork_Rok_Crash;

        [MayRequireXenobiologis]
        public static IncidentDef OG_Chaos_Deamon_Warpstorm_Deamonic;
        [MayRequireXenobiologis]
        public static IncidentDef OG_Chaos_Deamon_Deamonic_Incursion;
        [MayRequireXenobiologis]
        public static IncidentDef OG_Chaos_Deamon_Daemonic_Infestation;
        [MayRequireXenobiologis, MayRequireNecrons]
        public static IncidentDef OGN_MonolithAppears;
    }
}
