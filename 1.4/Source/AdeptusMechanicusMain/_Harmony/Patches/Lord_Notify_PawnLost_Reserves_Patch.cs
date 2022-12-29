using Verse;
using Verse.AI.Group;
using HarmonyLib;
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Lord), "Notify_PawnLost")]
    public static class Lord_Notify_PawnLost_Reserves_Patch
    {
        [HarmonyPostfix, HarmonyPriority(Priority.First)]
        public static void Postfix(Lord __instance, Pawn pawn, PawnLostCondition cond, DamageInfo? dinfo = null)
        {
            if (pawn != null && (cond == PawnLostCondition.Killed || cond == PawnLostCondition.Incapped))
            {
                if (__instance.Map.Reserves() is MapComponent_Reserves _Reserves)
                {
                    _Reserves.Notify_PawnLostViolently(__instance, dinfo);
                }
            }
        }
    }
}
