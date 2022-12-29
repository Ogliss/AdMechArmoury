using Verse;
using Verse.AI.Group;
using HarmonyLib;
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Lord), "Notify_PawnDamaged")]
    public static class Lord_Notify_PawnDamaged_Reserves_Patch
    {
        [HarmonyPostfix, HarmonyPriority(Priority.First)]
        public static void Postfix(Lord __instance, Pawn victim, DamageInfo dinfo)
        {
            if (victim != null)
            {
                if (__instance.Map.Reserves() is MapComponent_Reserves _Reserves)
                {
                    _Reserves.Notify_PawnDamaged(__instance, dinfo);
                }
            }
        }
    }
}
