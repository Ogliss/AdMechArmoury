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
    [HarmonyPatch(typeof(JobGiver_OptimizeApparel), nameof(JobGiver_OptimizeApparel.ApparelScoreGain))]
    public static class JobGiver_OptimizeApparel_ApparelScoreGain_Restrictions_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn pawn, Apparel ap, ref float __result)
        {
            if (!(__result >= 0f)) return;
            string r = string.Empty;
            if (!ApparelRestrictionUtility.CanWear(ap, pawn, ref r, true))
                __result = -50f;
        }
    }
}
