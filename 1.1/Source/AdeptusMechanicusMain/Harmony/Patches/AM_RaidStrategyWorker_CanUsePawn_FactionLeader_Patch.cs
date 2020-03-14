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

namespace AdeptusMechanicus.AdeptusAstartes
{
    // FoodUtility.BestPawnToHuntForPredator(getter, forceScanWholeMap)  BestPawnToHuntForPredator(Pawn predator, bool forceScanWholeMap)
    // Xeno/Neomorph Hunting patch
    [HarmonyPatch(typeof(RaidStrategyWorker), "CanUsePawn")]
    public static class AM_RaidStrategyWorker_CanUsePawn_FactionLeader_Patch
    {
        [HarmonyPostfix]
        public static void CanUse_factionLeader_Postfix(RaidStrategyWorker __instance, Pawn p, List<Pawn> otherPawns, ref bool __result)
        {
            if (p.kindDef.factionLeader)
            {
                if (otherPawns.Any(x => x.kindDef.factionLeader))
                {
                    __result = false;
                    Log.Warning(string.Format("Excess {0} detected:  there are {1} already in the raid, Disallowing", p.LabelShortCap, otherPawns.Where(x => x.def == p.def).Count()));
                    return;
                }
            }
        }
    }

}
