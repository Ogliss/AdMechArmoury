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
            List<ApparelGraphicRecord> bodyGraphics = __instance.apparelGraphics.Where(x=> x.sourceApparel.def.apparel.LastLayer != ApparelLayerDefOf.Shell && x.sourceApparel.def.apparel.LastLayer != ApparelLayerDefOf.Overhead).ToList();
            List<ApparelGraphicRecord> shellGraphics = __instance.apparelGraphics.Where(x => x.sourceApparel.def.apparel.LastLayer == ApparelLayerDefOf.Shell).OrderBy(x => x.sourceApparel.def.apparel.layers[Math.Max(x.sourceApparel.def.apparel.layers.Count - 2, 0)].drawOrder).ToList();
            List<ApparelGraphicRecord> overheadGraphics = __instance.apparelGraphics.Where(x => x.sourceApparel.def.apparel.LastLayer == ApparelLayerDefOf.Overhead).OrderBy(x => x.sourceApparel.def.apparel.layers[Math.Max(x.sourceApparel.def.apparel.layers.Count - 2, 0)].drawOrder).ToList();

            graphics.AddRange(bodyGraphics);
            graphics.AddRange(shellGraphics);
            graphics.AddRange(overheadGraphics);
            __instance.apparelGraphics = graphics;
            /*
            graphics.OrderBy(x => x.sourceApparel.def.apparel.LastLayer.drawOrder);
            if (graphics.Count>1)
            {
                for (int i = 0; i < __instance.apparelGraphics.Count; i++)
                {
                    ApparelGraphicRecord item = __instance.apparelGraphics[i];
                    ApparelLayerDef layer = item.sourceApparel.def.apparel.LastLayer;
                    int layers = item.sourceApparel.def.apparel.layers.Count;
                    if (layer == ApparelLayerDefOf.Shell || layer == ApparelLayerDefOf.Overhead)
                    {
                        ApparelLayerDef layer2 = item.sourceApparel.def.apparel.layers[layers - 2];
                        if (layer2.defName.Contains(layer.defName))
                        {

                        }
                    }
                }
            }
            */
        }
    }

}
