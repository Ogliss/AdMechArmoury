using System.Linq;
using Verse;
using UnityEngine;
using HarmonyLib;
using System.Collections.Generic;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(PawnGraphicSet), "MatsBodyBaseAt")]
    public static class PawnGraphicSet_MatsBodyBaseAt_Test_Patch
    {
        [HarmonyPostfix, HarmonyPriority(Priority.Last)]
        public static void Postfix(PawnGraphicSet __instance, ref List<Material> __result)
        {
            Pawn pawn = __instance.pawn;
            if (!pawn.RaceProps.Humanlike)
            {
                return;
            }
            if (pawn.apparel.AnyApparel)
            {
                if (pawn.apparel.WornApparel.Any(x => x.TryGetComp<CompApparelExtraDrawer>() != null))
                {
                    foreach (var item in pawn.apparel.WornApparel)
                    {
                        CompApparelExtraDrawer extraDrawer = item.TryGetComp<CompApparelExtraDrawer>();
                        if (extraDrawer != null && extraDrawer.hidesBody)
                        {
                            for (int i = 0; i < __result.Count; i++)
                            {
                                __result[i] = AMConstants.InvisibleGraphics(pawn).nakedGraphic.MatSingle;
                            }
                            return;
                        }
                    }
                }
            }
        }
    }

}
