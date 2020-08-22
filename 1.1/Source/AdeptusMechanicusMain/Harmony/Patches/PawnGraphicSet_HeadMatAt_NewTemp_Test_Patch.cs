using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using Verse.Sound;
using AdeptusMechanicus;
using AdeptusMechanicus.ExtensionMethods;
using UnityEngine;

namespace AdeptusMechanicus.HarmonyInstance
{
    public static class PawnGraphicSet_HeadMatAt_NewTemp_Test_Patch
    {
        public static void Postfix(PawnGraphicSet __instance, ref Material __result)
        {
            Pawn pawn = __instance.pawn;
            if (pawn.apparel.AnyApparel)
            {
                if (pawn.apparel.WornApparel.Any(x => x.TryGetComp<CompApparelExtraDrawer>() !=null))
                {
                    foreach (var item in pawn.apparel.WornApparel)
                    {
                        CompApparelExtraDrawer extraDrawer = item.TryGetComp<CompApparelExtraDrawer>();
                        if (extraDrawer !=null && extraDrawer.hidesHead)
                        {
                            __result = AMConstants.Invisiblegraphics(pawn).headGraphic.MatSingle;
                            return;
                        }
                    }
                }
            }
        }
    }

}
