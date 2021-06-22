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
using System.Reflection;
using AdeptusMechanicus.ExtensionMethods;
using UnityEngine;
using AdeptusMechanicus.settings;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(VerbProperties), "AdjustedCooldown", new[] { typeof(Verb), typeof(Pawn) })]
    public static class VerbProperties_AdjustedCooldown_RapidFire_Patch
    { 
        [HarmonyPostfix]
        public static void Postfix(Verb ownerVerb, Pawn attacker, ref float __result, ref float __state)
        {
            __state = __result;
            if (ownerVerb.RapidFire(__result, out bool InRange, out float modified))
            {
                if (InRange)
                {
                    __result = modified;
                }
            }
            //    Log.Message("Prefix original warmup " + __state + " " + __instance.verbProps.label + " fired by " + __instance.CasterPawn.LabelShortCap + "\nwarmup " + __instance.verbProps.warmupTime + " Cooldown " + __instance.verbProps.AdjustedCooldown(__instance, __instance.CasterPawn) + " Ticks between shots " + __instance.verbProps.ticksBetweenBurstShots);

        }
    }
}