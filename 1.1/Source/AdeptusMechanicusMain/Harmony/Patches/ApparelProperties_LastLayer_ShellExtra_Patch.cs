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
    [HarmonyPatch(typeof(ApparelProperties), "get_LastLayer")]
    public static class ApparelProperties_LastLayer_OverheadExtra_Patch
    {
        [HarmonyPostfix]
        public static void Post_(ref ApparelLayerDef __result)
        {
            if (__result.defName.Contains("Overhead")) __result = ApparelLayerDefOf.Overhead;
        }
    }
}
