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

namespace AdeptusMechanicus.HarmonyInstance
{
    // Projectile.Impact
    [HarmonyPatch(typeof(Projectile), "CanHit")]
    public static class AM_Projectile_CanHit_PhaseShifter_Patch
    {
        [HarmonyPostfix]
        public static void Impact_CanHit_Postfix(ref Projectile __instance, Thing thing, LocalTargetInfo ___intendedTarget, ref bool __result)
        {
            if (thing != null)
            {
                Pawn hitPawn = thing as Pawn;
                if (hitPawn != null)
                {
                    if (hitPawn.health.hediffSet.hediffs.Any(x => x.TryGetComp<HediffComp_PhaseShifter>() != null))
                    {
                        List<Hediff> list = hitPawn.health.hediffSet.hediffs.FindAll(x => x.TryGetComp<HediffComp_PhaseShifter>() != null);
                        foreach (Hediff item in list)
                        {
                            HediffComp_PhaseShifter _Shifter = item.TryGetComp<HediffComp_PhaseShifter>();
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
                                        if (_Shifter.phasedNotifcationTick==0)
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
