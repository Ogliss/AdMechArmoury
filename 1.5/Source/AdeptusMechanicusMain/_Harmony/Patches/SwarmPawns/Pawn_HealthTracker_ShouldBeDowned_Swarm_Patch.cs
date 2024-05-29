using Verse;
using HarmonyLib;

namespace AdeptusMechanicus
{
    [HarmonyPatch(typeof(Pawn_HealthTracker), "ShouldBeDowned", null)]
    public static class Pawn_HealthTracker_ShouldBeDowned_Swarm_Patch
    {
        public static bool Prefix(Pawn_HealthTracker __instance, Pawn ___pawn, ref bool __result)
        {
            if (___pawn.RaceProps.body.corePart.def == AdeptusBodyPartDefOf.OG_SwarmCore)
            {
                __result = false;
                return __result;
            }
            return true;
        }
    }
}
