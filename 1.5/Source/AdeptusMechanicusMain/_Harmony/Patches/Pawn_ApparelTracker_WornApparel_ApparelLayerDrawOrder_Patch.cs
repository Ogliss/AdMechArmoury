using System.Linq;
using Verse;
using UnityEngine;
using HarmonyLib;
using System.Collections.Generic;
using RimWorld;
using System;

namespace AdeptusMechanicus.HarmonyInstance
{

    [HarmonyPatch(typeof(Pawn_ApparelTracker), "get_WornApparel"), HarmonyPriority(Priority.Last)]
    public static class Pawn_ApparelTracker_WornApparel_ApparelLayerDrawOrder_Patch
    {
        [HarmonyPostfix]
        public static List<Apparel> Postfix(List<Apparel> __result)
        {
            return __result.OrderBy(x => x.def.apparel.layers.Last().drawOrder).ToList();
        }
    }
    // 1.5 rewrite needed
    /*
    [HarmonyPatch(typeof(PawnGraphicSet), "ResolveAllGraphics"), HarmonyPriority(Priority.Last)]
    public static class PawnGraphicSet_ResolveAllGraphics_SwarmPawn_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref PawnGraphicSet __instance)
        {
            Pawn pawn = __instance.pawn;
            if (pawn.RaceProps.Humanlike)
            {
                return;
            }
            if (pawn.ageTracker.CurKindLifeStage is SwarmKindLifeStage swarm)
            {
                Log.Message($"{pawn}'s current PawnKindLifeStage is a SwarmKindLifeStage");
                if (!swarm.subStagesHealth.NullOrEmpty())
                {
                    int ind = (int)Mathf.Lerp(0, swarm.subStagesHealth.Count, pawn.health.summaryHealth.SummaryHealthPercent);
                    Log.Message($"{pawn}'s SwarmKindLifeStage using HealthSubStage @ Ind: {ind}");
                    if (ind > 0) __instance.nakedGraphic = swarm.subStagesHealth[ind - 1].bodyGraphicData.Graphic;
                }
            }
        }
    }
    
    [HarmonyPatch(typeof(PawnGraphicSet), "ResolveApparelGraphics"), HarmonyPriority(Priority.Last)]
    public static class PawnGraphicSet_ResolveApparelGraphics_ApparelLayerDrawOrder_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref PawnGraphicSet __instance)
        {
            Pawn pawn = __instance.pawn;
            if (!pawn.RaceProps.Humanlike)
            {
                return;
            }
            if (__instance.apparelGraphics.NullOrEmpty() || __instance.apparelGraphics.Count == 1)
            {
                return;
            }
                __instance.apparelGraphics = apparelGraphicRecordsOrdered(__instance.apparelGraphics);
        }
        public static List<ApparelGraphicRecord> apparelGraphicRecordsOrdered(List<ApparelGraphicRecord> list)
        {
            return list.OrderBy(x => x.sourceApparel.def.apparel.layers.Last().drawOrder).ToList();
        }
    }
    */
}
