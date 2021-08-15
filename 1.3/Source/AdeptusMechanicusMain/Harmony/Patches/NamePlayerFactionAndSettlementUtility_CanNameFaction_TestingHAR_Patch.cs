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
    /*
    [HarmonyPatch(typeof(NamePlayerFactionAndSettlementUtility), "CanNameFaction")]
    public static class NamePlayerFactionAndSettlementUtility_CanNameFaction_TestingHAR_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(int ticksPassed, ref bool __result)
        {
            {
                Log.Message("Has Name: "+ !Faction.OfPlayer.HasName +" Days passed: "+ ((float)ticksPassed / 60000f >= 4.3f) + " CanNameAnythingNow: " + NamePlayerFactionAndSettlementUtility.CanNameAnythingNow());
            }
        }
    }
    */
}
