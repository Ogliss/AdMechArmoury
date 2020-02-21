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
    /*
    // ShotReport.AimOnTargetChance_StandardTarget ShotReport
    [HarmonyPatch(typeof(ShotReport), "get_AimOnTargetChance_StandardTarget")]
    public static class AM_ShotReport_GetAimOnTargetChance_StandardTarget_Debuff_Patch
    {
        [HarmonyPostfix]
        public static void Get_AimOnTargetChance_Necron_Wraith_Postfix(ref ShotReport __instance, ref LocalTargetInfo ___target, ref float ___distance, ref float __result)
        {
            if (___target != null)
            {
                if (___target.HasThing)
                {
                    if (___target.Thing.def.thingClass == typeof(Pawn))
                    {
                        Pawn pawn = (Pawn)___target.Thing;
                        if (pawn.kindDef == OGNecronDefOf.OG_Necron_Wraith)
                        {
                            if (___distance > 5f && __result > 0.1f)
                            {
                                __result = __result * 0.1f;
                            }
                        }
                    }
                }
            }
        }
    }
    */
}
