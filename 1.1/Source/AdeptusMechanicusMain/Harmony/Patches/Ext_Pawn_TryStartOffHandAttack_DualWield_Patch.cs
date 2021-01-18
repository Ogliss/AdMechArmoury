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
    public static class Ext_Pawn_TryStartOffHandAttack_DualWield_Patch
    {
        [HarmonyArgument("__result", "result")]
        [HarmonyArgument("__instance", "pawn")]
        public static bool Prefix(Pawn pawn, LocalTargetInfo targ, ref bool result)
        {
        //    Log.Message("Ext_Pawn_TryStartOffHandAttack_DualWield_Patch Running om "+ pawn.jobs.curDriver.GetType().Name);
            if (pawn.jobs.curDriver.GetType().Name.Contains("CastVerbOnceStatic"))//Compatbility for 1.2's apparel verbs.
            {
            //    Log.Message("Ext_Pawn_TryStartOffHandAttack_DualWield_Patch CastVerbOnceStatic");
                result = false;
                return result;
            }
            return true;
        }

    }
}
