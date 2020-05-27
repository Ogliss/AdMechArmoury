using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Harmony;
using Verse.Sound;
using System.Reflection;
using AdeptusMechanicus.ExtensionMethods;
using UnityEngine;
using AdeptusMechanicus.settings;

namespace AdeptusMechanicus.Harmony
{
    /*
    [HarmonyPatch(typeof(HediffGiverUtility), "TryApply")]
    public static class AM_HediffGiverUtility_TryApply_Patch
    {
        [HarmonyPostfix]
        public static void HediffGiverUtility_TryApply_Postfix(Pawn pawn, HediffDef hediff, List<BodyPartDef> partsToAffect, bool canAffectAnyLivePart, int countToAffect, List<Hediff> outAddedHediffs)
        {
            if (pawn.health.hediffSet.GetFirstHediffOfDef(hediff)!=null)
            {
                if (pawn.health.hediffSet.GetFirstHediffOfDef(hediff).Part == null)
                {
                //    Log.Warning(string.Format("Pawn: {0}, hediff: {1}", pawn, hediff));
                    foreach (BodyPartDef bpd in partsToAffect)
                    {
                    //    Log.Warning(string.Format("partsToAffect: {0}", bpd.defName));
                        BodyPartRecord record = pawn.RaceProps.body.AllParts.Where(x=> x.def == bpd).First();
                        if (record!=null)
                        {
                            if (pawn.health.hediffSet.PartIsMissing(record))
                            {
                                Log.Warning(string.Format("MISSING!: {0}", bpd.label));
                            }
                        }
                        else
                        {

                            Log.Warning(string.Format("NULLRECORD!: {0}", bpd.defName));
                        }
                    }
                }
            }
        }
    }
    */
}