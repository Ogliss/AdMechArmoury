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

        [MayRequireOrkz]
        public static IncidentDef OG_Ork_Rok_Crash;

        [MayRequireTyranids]
        public static IncidentDef OG_Tyranid_Infestation;
        [MayRequireChaos]
        public static IncidentDef OG_Chaos_Deamon_Warpstorm_Deamonic;
        [MayRequireChaos]
        public static IncidentDef OG_Chaos_Deamon_Deamonic_Incursion;
        [MayRequireChaos]
        public static IncidentDef OG_Chaos_Deamon_Daemonic_Infestation;

        [MayRequireNecrons]
        public static IncidentDef OGN_MonolithAppears;
    }
}
