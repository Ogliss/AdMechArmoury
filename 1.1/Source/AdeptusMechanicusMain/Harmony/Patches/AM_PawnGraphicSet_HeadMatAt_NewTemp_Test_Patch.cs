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

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(PawnGraphicSet), "HeadMatAt_NewTemp")]
    public static class AM_PawnGraphicSet_HeadMatAt_NewTemp_Test_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(PawnGraphicSet __instance, Rot4 facing, RotDrawMode bodyCondition, ref bool stump, bool portrait)
        {
            if (__instance.pawn.apparel.AnyApparel)
            {
                if (__instance.pawn.apparel.WornApparel.Any(x=> x.def.defName == "OGT_Apparel_XV25Battlesuit"))
                {
                    stump = true;
                }
            }
        }
    }

}
