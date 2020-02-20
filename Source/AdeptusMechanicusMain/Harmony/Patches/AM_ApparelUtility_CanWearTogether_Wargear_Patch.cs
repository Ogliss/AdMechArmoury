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

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(ApparelUtility), "CanWearTogether")]
    public static class AM_ApparelUtility_CanWearTogether_Wargear_Patch
    {
        [HarmonyPostfix]
        public static void CanWearTogether_Postfix(ThingDef A, ThingDef B, BodyDef body, ref bool __result)
        {
            if (A.apparel.bodyPartGroups.NullOrEmpty() || A.defName.Contains("OG") && A.defName.Contains("_Wargear_"))
            {
                bool flag1 = (!A.apparel.tags.Contains("OGEnergyShield") || !B.apparel.tags.Contains("OGEnergyShield"));
                __result = A != B && flag1;
            }
        }
    }
}
