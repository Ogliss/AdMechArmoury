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
    [HarmonyPatch(typeof(IncidentWorker), "TryExecute")]
    public static class AM_IncidentWorker_TryExecute_Test_Patch
    {
        [HarmonyPrefix]
        public static void Pre_TryExecuteWorker(IncidentWorker __instance, IncidentParms parms)
        {
        //    log.message(string.Format("{0}: {1}",__instance, parms));
        }

        /*
            [HarmonyPostfix]
            public static void Post_TryExecuteWorker(Pawn p, ThingDef apparel, ref bool __result)
            {

            }
        */
    }
}
