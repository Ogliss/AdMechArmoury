using System.Linq;
using Verse;
using UnityEngine;
using HarmonyLib;
using System.Collections.Generic;
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{
 //   [HarmonyPatch(typeof(PawnGraphicSet), "MatsBodyBaseAt")]
    public static class PawnGraphicSet_MatsBodyBaseAt_Test_Patch
    {
    //    [HarmonyPostfix, HarmonyPriority(Priority.Last)]
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
            try
            {
                Log.Message("4");
                if (pawn.apparel.AnyApparel && !pawn.apparel.WornApparel.NullOrEmpty())
                {
                    Log.Message("4a");
                    if (pawn.apparel.WornApparel.Any(x => x.def.HasComp(typeof(CompApparelExtraPartDrawer))))
                    {
                        Log.Message("4a1");
                        foreach (var item in pawn.apparel.WornApparel)
                        {
                            Log.Message("4a1a");
                            if (item.def.HasComp(typeof(CompApparelExtraPartDrawer)))
                            {
                                Log.Message("4a1a1");
                                CompApparelExtraPartDrawer extraDrawer = item.TryGetCompFast<CompApparelExtraPartDrawer>();
                                Log.Message("4a1a2");
                                if (extraDrawer != null && extraDrawer.hidesBody)
                                {
                                    Log.Message("4a1a2a");
                                    for (int i = 0; i < __result.Count; i++)
                                    {
                                        Log.Message("4a1a2a1");
                                        __result[i] = AMConstants.InvisibleGraphics(pawn).nakedGraphic.MatSingle;
                                        Log.Message("4a1a2a2");
                                    }
                                    Log.Message("4a1a2b");
                                    return;
                                }
                                Log.Message("4a1a3");
                            }
                            Log.Message("4a1b");
                        }
                        Log.Message("4a2");
                    }
                    Log.Message("4b");
                }
                Log.Message("5");
            }
            catch (System.Exception)
            {

                Log.Message("errored");
            }
        }
    }

}
