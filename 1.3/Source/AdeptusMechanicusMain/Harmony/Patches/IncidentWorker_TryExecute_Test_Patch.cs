using System;
using System.Linq;
using System.Text;
using RimWorld;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using Verse.Sound;
using AdeptusMechanicus;
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{

    [HarmonyPatch(typeof(IncidentWorker), "TryExecute")]
    public static class IncidentWorker_TryExecute_Test_Patch
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

    [HarmonyPatch(typeof(IncidentWorker_OrbitalTraderArrival), "TryExecuteWorker")]
    public static class IncidentWorker_OrbitalTraderArrival_TryExecuteWorker_Test_Patch
    {
        [HarmonyPrefix]
        public static void Pre_TryExecuteWorker(IncidentWorker __instance, IncidentParms parms)
        {
        //    Log.Message($"{__instance}: {parms.traderKind}({parms.faction})");
        }

        /*
            [HarmonyPostfix]
            public static void Post_TryExecuteWorker(Pawn p, ThingDef apparel, ref bool __result)
            {

            }
        */
    }
}
