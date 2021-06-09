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
    [HarmonyPatch(typeof(RelationsUtility), "TryDevelopBondRelation")]
    public static class RelationsUtility_TryDevelopBondRelation_Restricted_Patch
    {
        /*
        [HarmonyPrefix]
        public static void Prefix(Pawn humanlike, Pawn animal, float baseChance, ref bool __result)
        {

        }
        */
        [HarmonyPostfix]
        public static void Postfix(Pawn humanlike, Pawn animal, float baseChance, ref bool __result)
        {

        }
    }
}
