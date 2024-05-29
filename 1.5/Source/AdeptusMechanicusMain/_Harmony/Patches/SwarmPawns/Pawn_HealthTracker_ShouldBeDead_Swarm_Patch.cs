using Verse;
using HarmonyLib;

namespace AdeptusMechanicus
{
    [HarmonyPatch(typeof(Pawn_HealthTracker), "ShouldBeDead", null)]
    public static class Pawn_HealthTracker_ShouldBeDead_Swarm_Patch
    {
        public static bool Prefix(Pawn_HealthTracker __instance, Pawn ___pawn, ref bool __result)
        {
            if (___pawn.RaceProps.body.corePart.def == AdeptusBodyPartDefOf.OG_SwarmCore)
            {
                __result = true;
                foreach (var item in ___pawn.health.hediffSet.GetNotMissingParts())
                {
                    if (item.def != AdeptusBodyPartDefOf.OG_SwarmCore)
                    {
                        __result = false;
                        break;
                    }
                }
                return false;
            }
            return true;
        }
    }
}
