using RimWorld;
using HarmonyLib;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Pawn_ApparelTracker), "Notify_ApparelRemoved")]
    public static class Pawn_ApparelTracker_Notify_ApparelRemoved_CompHediffApparel_Patch
    {
        [HarmonyPostfix, HarmonyPriority(Priority.First)]
        public static void Postfix(Pawn_ApparelTracker __instance, Apparel apparel)
        {
            if (apparel.Wearer == null)
            {
                apparel.BroadcastCompSignal(CompHediffApparel.RemoveHediffsFromPawnSignal);
            }
        }
    }
    
}
