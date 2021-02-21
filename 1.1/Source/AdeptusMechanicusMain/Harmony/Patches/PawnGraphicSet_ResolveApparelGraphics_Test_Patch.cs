using System.Linq;
using Verse;
using UnityEngine;
using HarmonyLib;
using System.Collections.Generic;
using RimWorld;
using System;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(PawnGraphicSet), "ResolveApparelGraphics")]
    public static class PawnGraphicSet_ResolveApparelGraphics_ApparelLayerDrawOrder_Patch
    {
        [HarmonyPostfix, HarmonyPriority(Priority.Last)]
        public static void Postfix(ref PawnGraphicSet __instance)
        {
            Pawn pawn = __instance.pawn;
            if (!pawn.RaceProps.Humanlike)
            {
                return;
            }
            if (__instance.apparelGraphics.NullOrEmpty())
            {
                return;
            }
            __instance.apparelGraphics.OrderBy(x => x.sourceApparel.def.apparel.LastLayer.drawOrder);

            List<ApparelGraphicRecord> graphics = new List<ApparelGraphicRecord>();

            List<ApparelGraphicRecord> bodyGraphics = new List<ApparelGraphicRecord>();
            List<ApparelGraphicRecord> shellGraphics = new List<ApparelGraphicRecord>();
            List<ApparelGraphicRecord> overheadGraphics = new List<ApparelGraphicRecord>();

            /*
            List<ApparelGraphicRecord> bodyGraphics = __instance.apparelGraphics.FindAll(x => x.sourceApparel.def.apparel.LastLayer != ApparelLayerDefOf.Shell && x.sourceApparel.def.apparel.LastLayer != ApparelLayerDefOf.Overhead).ToList();
            List<ApparelGraphicRecord> shellGraphics = __instance.apparelGraphics.FindAll(x => x.sourceApparel.def.apparel.LastLayer == ApparelLayerDefOf.Shell).OrderBy(x => x.sourceApparel.def.apparel.layers[Math.Max(x.sourceApparel.def.apparel.layers.Count - 2, 0)].drawOrder).ToList();
            List<ApparelGraphicRecord> overheadGraphics = __instance.apparelGraphics.FindAll(x => x.sourceApparel.def.apparel.LastLayer == ApparelLayerDefOf.Overhead).OrderBy(x => x.sourceApparel.def.apparel.layers[Math.Max(x.sourceApparel.def.apparel.layers.Count - 2, 0)].drawOrder).ToList();
            */
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
        }
    }

}
