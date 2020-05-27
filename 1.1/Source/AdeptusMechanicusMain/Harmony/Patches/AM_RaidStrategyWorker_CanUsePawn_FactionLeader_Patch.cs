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
    // FoodUtility.BestPawnToHuntForPredator(getter, forceScanWholeMap)  BestPawnToHuntForPredator(Pawn predator, bool forceScanWholeMap)
    // Xeno/Neomorph Hunting patch CanUsePawnGenOption(PawnGenOption opt, List<PawnGenOption> chosenOpts)
    [HarmonyPatch(typeof(RaidStrategyWorker), "CanUsePawnGenOption")]
    public static class AM_RaidStrategyWorker_CanUsePawnGenOption_FactionLeader_Patch
    {
        [HarmonyPostfix]
        public static void CanUse_factionLeader_Postfix(RaidStrategyWorker __instance, PawnGenOption g, List<PawnGenOption> chosenGroups, ref bool __result)
        {
            if (g.kind.factionLeader)
            {
                if (chosenGroups.Any(x => x.kind.factionLeader && x.kind == g.kind))
                {
                    __result = false;
                //    Log.Warning(string.Format("Excess {0} detected:  there are {1} already in the raid, Disallowing", g.kind.LabelCap, chosenGroups.Where(x => x.kind == g.kind).Count()));
                    return;
                }
            }
        }
    }

}
