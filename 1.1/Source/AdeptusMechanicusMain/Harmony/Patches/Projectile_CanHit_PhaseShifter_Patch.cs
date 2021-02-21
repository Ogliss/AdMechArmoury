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
using UnityEngine;
using AdeptusMechanicus.settings;
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{
    // Projectile.Impact
    [HarmonyPatch(typeof(Projectile), "CanHit")]
    public static class Projectile_CanHit_PhaseShifter_Patch
    {
        public static List<HediffDef> phaseHediffs;
        [HarmonyPostfix]
        public static void Impact_CanHit_Postfix(ref Projectile __instance, Thing thing, LocalTargetInfo ___intendedTarget, ref bool __result)
        {
            if (phaseHediffs == null)
            {
                if (AMAMod.Dev) Log.Message("Projectile_CanHit_PhaseShifter_Patch phaseHediffs is null, Populating");
                phaseHediffs = DefDatabase<HediffDef>.AllDefsListForReading.FindAll(x => x.HasComp(typeof(HediffComp_PhaseShifter)));
                if (AMAMod.Dev) Log.Message("Projectile_CanHit_PhaseShifter_Patch phaseHediffs Populated: " + phaseHediffs.Count);
            }
            if (thing != null)
            {
                Pawn hitPawn = thing as Pawn;
                if (hitPawn != null)
                {
                    for (int i = 0; i < phaseHediffs.Count; i++)
                    {
                        HediffWithComps item = hitPawn.health.hediffSet.GetFirstHediffOfDef(phaseHediffs[i]) as HediffWithComps;
                        if (item != null)
                        {
                            List<Hediff> list = hitPawn.health.hediffSet.hediffs.FindAll(x => phaseHediffs.Contains(x.def));
                            foreach (Hediff hediff in list)
                            {
                                HediffComp_PhaseShifter _Shifter = hediff.TryGetCompFast<HediffComp_PhaseShifter>();
                                if (_Shifter != null)
                                {
                                    if (_Shifter.phasedfor.Contains(__instance))
                                    {
                                        __result = false;
                                    }
                                    else
                                    {
                                        if (!_Shifter.isPhasedIn)
                                        {
                                            _Shifter.phasedfor.Add(__instance);
                                            if (_Shifter.phasedNotifcationTick == 0)
                                            {
                                                MoteMaker.ThrowText(hitPawn.Position.ToVector3(), hitPawn.Map, "AMA_Phased_Out".Translate(__instance.LabelCap, hitPawn.LabelShortCap), 3f);
                                                _Shifter.phasedNotifcationTick = _Shifter.Props.minPhasedNotifcation.SecondsToTicks();
                                            }
                                            __result = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

}
