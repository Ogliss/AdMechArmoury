using System.Linq;
using Verse;
using UnityEngine;
using HarmonyLib;
using System.Collections.Generic;
using RimWorld;
using System;

namespace AdeptusMechanicus.HarmonyInstance
{
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
            //    pawn.apparel.wornApparel.innerList = pawn.apparel.wornApparel.innerList.OrderBy(x => x.def.apparel.layers[Math.Max(x.def.apparel.layers.Count - 1, 0)]).ToList();
            //    __instance.apparelGraphics.OrderBy(x => x.sourceApparel.def.apparel.LastLayer.drawOrder);
            /*
            List<ApparelGraphicRecord> graphics = new List<ApparelGraphicRecord>();

            List<ApparelGraphicRecord> bodyGraphics = new List<ApparelGraphicRecord>();
            List<ApparelGraphicRecord> shellGraphics = new List<ApparelGraphicRecord>();
            List<ApparelGraphicRecord> overheadGraphics = new List<ApparelGraphicRecord>();


        //    List<ApparelGraphicRecord> bodyGraphics = __instance.apparelGraphics.FindAll(x => x.sourceApparel.def.apparel.LastLayer != ApparelLayerDefOf.Shell && x.sourceApparel.def.apparel.LastLayer != ApparelLayerDefOf.Overhead).ToList();
        //    List<ApparelGraphicRecord> shellGraphics = __instance.apparelGraphics.FindAll(x => x.sourceApparel.def.apparel.LastLayer == ApparelLayerDefOf.Shell).OrderBy(x => x.sourceApparel.def.apparel.layers[Math.Max(x.sourceApparel.def.apparel.layers.Count - 2, 0)].drawOrder).ToList();
        //    List<ApparelGraphicRecord> overheadGraphics = __instance.apparelGraphics.FindAll(x => x.sourceApparel.def.apparel.LastLayer == ApparelLayerDefOf.Overhead).OrderBy(x => x.sourceApparel.def.apparel.layers[Math.Max(x.sourceApparel.def.apparel.layers.Count - 2, 0)].drawOrder).ToList();

            for (int i = 0; i < __instance.apparelGraphics.Count; i++)
            {
                ApparelGraphicRecord record = __instance.apparelGraphics[i];
                if (record.sourceApparel.def.apparel.LastLayer == ApparelLayerDefOf.Overhead)
                {
                    overheadGraphics.Add(record);
                    continue;
                }
                if (record.sourceApparel.def.apparel.LastLayer == ApparelLayerDefOf.Shell)
                {
                    shellGraphics.Add(record);
                    continue;
                }
                bodyGraphics.Add(record);
            }
            overheadGraphics.OrderBy(x=> x.sourceApparel.def.apparel.layers[Math.Max(x.sourceApparel.def.apparel.layers.Count - 2, 0)].drawOrder);
            shellGraphics.OrderBy(x=> x.sourceApparel.def.apparel.layers[Math.Max(x.sourceApparel.def.apparel.layers.Count - 2, 0)].drawOrder);
            bodyGraphics.OrderBy(x=> x.sourceApparel.def.apparel.layers[Math.Max(x.sourceApparel.def.apparel.layers.Count - 2, 0)].drawOrder);
            graphics.AddRange(bodyGraphics);
            graphics.AddRange(shellGraphics);
            graphics.AddRange(overheadGraphics);
            __instance.apparelGraphics = graphics;
            */
        }
        public static List<ApparelGraphicRecord> apparelGraphicRecordsOrdered(List<ApparelGraphicRecord> list)
        {
            //    Log.Message($"returning {list.Count} appareal, in draw order of last layer");
            return list.OrderBy(x => x.sourceApparel.def.apparel.layers.Last().drawOrder).ToList();
        }
    }

}
