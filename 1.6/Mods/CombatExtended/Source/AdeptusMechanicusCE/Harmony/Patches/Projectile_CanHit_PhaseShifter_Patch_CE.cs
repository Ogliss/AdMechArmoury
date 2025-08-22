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
using CombatExtended;
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{
    // ProjectileCE.TryCollideWith
    [HarmonyPatch(typeof(ProjectileCE), "TryCollideWith")]
    public static class Projectile_TryCollideWith_PhaseShifter_Patch_CE
    {
        public static bool Prefix(ref ProjectileCE __instance, Thing thing)
        {
            if (thing != null)
            {
                Pawn hitPawn = thing as Pawn;
                if (hitPawn != null)
                {
                    if (hitPawn.health.hediffSet.hediffs.Any(x => x.TryGetCompFast<HediffComp_PhaseShifter>() != null))
                    {
                        List<Hediff> list = hitPawn.health.hediffSet.hediffs.FindAll(x => x.TryGetCompFast<HediffComp_PhaseShifter>() != null);
                        foreach (Hediff item in list)
                        {
                            HediffComp_PhaseShifter _Shifter = item.TryGetCompFast<HediffComp_PhaseShifter>();
                            if (_Shifter != null)
                            {
                                if (_Shifter.phasedfor.Contains(__instance))
                                {
                                    return false;
                                }
                                else
                                {
                                    if (!_Shifter.isPhasedIn)
                                    {
                                        _Shifter.phasedfor.Add(__instance);
                                        if (_Shifter.phasedNotifcationTick==0)
                                        {
                                            MoteMaker.ThrowText(hitPawn.Position.ToVector3(), hitPawn.Map, "AdeptusMechanicus.Phased_Out".Translate(__instance.LabelCap, hitPawn.LabelShortCap), 3f);
                                            _Shifter.phasedNotifcationTick = _Shifter.Props.minPhasedNotifcation.SecondsToTicks();
                                        }
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }
    }

}
