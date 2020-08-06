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
 //   [HarmonyPatch(typeof(PawnGraphicSet), "HeadMatAt")]
    public static class AM_PawnGraphicSet_HeadMatAt_Test_Patch
    {
        public static void Postfix(PawnGraphicSet __instance, ref bool stump, ref Material __result)
        {
            Pawn pawn = __instance.pawn;
            if (pawn.apparel.AnyApparel)
            {
                if (pawn.apparel.WornApparel.Any(x => x.def.defName == "OGT_Apparel_XV25Battlesuit"))
                {
                    __result.SetTexture(AMConstants.Invisiblegraphics(pawn).headGraphic.MatSingle.name, AMConstants.Invisiblegraphics(pawn).headGraphic.MatSingle.mainTexture);
                    __result.shader = ShaderDatabase.Cutout;
                }
            }
        }
    }
    
    public static class AM_PawnGraphicSet_HeadMatAt_NewTemp_Test_Patch
    {
        public static void Postfix(PawnGraphicSet __instance, ref bool stump, ref Material __result)
        {
            Pawn pawn = __instance.pawn;
            if (pawn.apparel.AnyApparel)
            {
                if (pawn.apparel.WornApparel.Any(x => x.def.defName == "OGT_Apparel_XV25Battlesuit"))
                {
                    __result.SetTexture(AMConstants.Invisiblegraphics(pawn).headGraphic.MatSingle.name, AMConstants.Invisiblegraphics(pawn).headGraphic.MatSingle.mainTexture);
                    __result.shader = ShaderDatabase.Cutout;
                }
            }
        }
    }
    
//    [HarmonyPatch(typeof(PawnGraphicSet), "HairMatAt_NewTemp")]
    public static class AM_PawnGraphicSet_HairMatAt_Test_Patch
    {
        public static void Postfix(PawnGraphicSet __instance, ref Material __result)
        {
            Pawn pawn = __instance.pawn;
            if (pawn.apparel.AnyApparel)
            {
                if (pawn.apparel.WornApparel.Any(x=> x.def.defName == "OGT_Apparel_XV25Battlesuit"))
                {
                    __result.SetTexture(AMConstants.Invisiblegraphics(pawn).headGraphic.MatSingle.name, AMConstants.Invisiblegraphics(pawn).headGraphic.MatSingle.mainTexture);
                    __result.shader = ShaderDatabase.Cutout;
                }
            }
        }
    }
    public static class AM_PawnGraphicSet_HairMatAt_NewTemp_Test_Patch
    {
        public static void Postfix(PawnGraphicSet __instance, ref Material __result)
        {
            Pawn pawn = __instance.pawn;
            if (pawn.apparel.AnyApparel)
            {
                if (pawn.apparel.WornApparel.Any(x=> x.def.defName == "OGT_Apparel_XV25Battlesuit"))
                {
                    __result.SetTexture(AMConstants.Invisiblegraphics(pawn).headGraphic.MatSingle.name, AMConstants.Invisiblegraphics(pawn).headGraphic.MatSingle.mainTexture);
                    __result.shader = ShaderDatabase.Cutout;
                }
            }
        }
    }

}
