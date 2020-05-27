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
    
    [HarmonyPatch(typeof(HediffGiver_RandomAgeCurved), "OnIntervalPassed")]
    public static class AM_HediffGiver_RandomAgeCurved_OnIntervalPassed_RejuveTreatment_Patch
    {
        [HarmonyPrefix]
        public static bool OnIntervalPassed_Prefix(HediffGiver_RandomAgeCurved __instance, Pawn pawn, Hediff cause)
        {
            if (pawn == null)
            {
                return true;
            }
            if (pawn.health.hediffSet.hediffs.Any(x=> x.TryGetComp<HediffComp_RejuvTreatment>()!=null))
            {
                float increase = 0f;
                List<Hediff> rejuvTreatments = pawn.health.hediffSet.hediffs.FindAll(x => x.TryGetComp<HediffComp_RejuvTreatment>() != null);
                foreach (Hediff hd in rejuvTreatments)
                {
                    HediffComp_RejuvTreatment rejuvTreatment = hd.TryGetComp<HediffComp_RejuvTreatment>();
                    increase += rejuvTreatment.LifeExpectancyIncrease;
                }
                if (increase!=0)
                {
                    float x = (float)pawn.ageTracker.AgeBiologicalYears / (pawn.RaceProps.lifeExpectancy + increase);
                    if (Rand.MTBEventOccurs(__instance.ageFractionMtbDaysCurve.Evaluate(x), 60000f, 60f))
                    {
                        return true;
                    }
                }
            }
            return true;
        }
    }
    
}