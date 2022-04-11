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
    [HarmonyPatch(typeof(VerbProperties), "AdjustedAccuracy")]
    public static class VerbProperties_AdjustedAccuracy_Patch
    { 
        [HarmonyPostfix]
        public static void Postfix(VerbProperties __instance, Byte cat, Thing equipment)
        {

            /*
        __state = __result;
        if (ownerVerb.RapidFire(__result, out bool InRange, out float modified))
        {
            if (InRange)
            {
                __result = modified;
            }
        }
            */
        //    Log.Message("Postfix AdjustedAccuracy " + __instance.defaultProjectile.label + " fired by " + equipment.LabelCap + "\nwarmup " + __instance.warmupTime + /*" Cooldown " + __instance.AdjustedCooldown(__instance, __instance.CasterPawn) +*/ " Ticks between shots " + __instance.ticksBetweenBurstShots);

        }
    }
}