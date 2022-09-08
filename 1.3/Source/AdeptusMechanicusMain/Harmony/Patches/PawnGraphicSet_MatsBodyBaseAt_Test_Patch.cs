using System.Linq;
using Verse;
using UnityEngine;
using HarmonyLib;
using System.Collections.Generic;
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(PawnGraphicSet), "MatsBodyBaseAt")]
    public static class PawnGraphicSet_MatsBodyBaseAt_Test_Patch
    {
        [HarmonyPostfix, HarmonyPriority(Priority.Last)]
        public static void Postfix(PawnGraphicSet __instance, ref List<Material> __result)
        {
            Pawn pawn = __instance.pawn;
            if (pawn == null)
            {
                return;
            }
            if (!pawn.RaceProps.Humanlike)
            {
                /*
                if (pawn is SwarmPawn swarm)
                {

                }
                */
                return;
            }
            if (pawn.apparel.AnyApparel)
            {
                if (pawn.apparel.WornApparel.Any(x => x.def.HasComp(typeof(CompApparelExtraPartDrawer))))
                {
                    foreach (var item in pawn.apparel.WornApparel)
                    {
                        if (item.def.HasComp(typeof(CompApparelExtraPartDrawer)))
                        {
                            CompApparelExtraPartDrawer extraDrawer = item.TryGetCompFast<CompApparelExtraPartDrawer>();
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

}
