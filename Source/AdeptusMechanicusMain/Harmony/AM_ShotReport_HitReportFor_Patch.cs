using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Harmony;
using Verse.Sound;

namespace AdeptusMechanicus.Harmony
{
    [HarmonyPatch(typeof(Verse.ShotReport), "HitReportFor")]
    public static class AM_ShotReport_HitReportFor_Patch
    {
        [HarmonyPostfix]
        public static void ApparelGizmosFromComps(Thing caster, Verb verb, LocalTargetInfo target, ref ShotReport __result)
        {

        }
    }
}
