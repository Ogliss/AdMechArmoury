using System.Collections.Generic;
using Verse;
using HarmonyLib;
using System.Runtime.InteropServices;
using RimWorld;
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Pawn_HealthTracker), "CheckForStateChange", null)]
    public static class Pawn_HealthTracker_CheckForStateChange_HediffComp_ForceIncap_Patch
    {
        public static void Prefix(Pawn_HealthTracker __instance, Pawn ___pawn, ref DamageInfo? dinfo, ref Hediff hediff)
        {
            if (__instance.ShouldBeDowned())
            {
                if (dinfo != null)
                {

                }
                HediffComp_ForceIncap forceIncap = hediff.TryGetCompFast<HediffComp_ForceIncap>();
                if (forceIncap != null)
                {
                    Rand.PushState();
                    __instance.forceIncap = Rand.Chance(forceIncap.Props.chance);
                    Rand.PopState();
                }
            }
        }
        private static bool ShouldBeDowned(this Pawn_HealthTracker __instance)
        {
            return __instance.InPainShock || !__instance.capacities.CanBeAwake || !__instance.capacities.CapableOf(PawnCapacityDefOf.Moving);
        }
    }
}
