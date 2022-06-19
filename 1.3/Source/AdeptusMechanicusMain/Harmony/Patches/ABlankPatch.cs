using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse.AI;
using Verse.AI.Group;
using AdeptusMechanicus;
using AdeptusMechanicus.ExtensionMethods;
using Verse.Sound;
using System;
using RimWorld;
using Verse;
using HarmonyLib;

namespace AdeptusMechanicus.HarmonyInstance
{
    
    [HarmonyPatch(typeof(ApparelProperties), "get_LastLayer")]
    public static class ApparelProperties_get_LastLayer_Tweaks_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref ApparelLayerDef __result)
        {
            if (__result.defName.StartsWith("OG_"))
            {
                if (__result.defName.Contains("Overhead"))
                {
                    __result = ApparelLayerDefOf.Overhead;
                }
                if (__result.defName.Contains("Shell"))
                {
                    __result = ApparelLayerDefOf.Shell;
                }
                if (__result.defName.Contains("Belt"))
                {
                    __result = ApparelLayerDefOf.Belt;
                }
                if (__result.defName.Contains("EyeCover"))
                {
                    __result = ApparelLayerDefOf.EyeCover;
                }
            }
        }
    }
    
    /*
    [HarmonyPatch(typeof(Class), "Method")]
    public static class Class_Method_Name_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(Pawn p, ThingDef apparel, ref bool __result)
        {

        }

        [HarmonyPostfix]
        public static void Postfix(Pawn p, ThingDef apparel, ref bool __result)
        {

        }
    }
    */
}
