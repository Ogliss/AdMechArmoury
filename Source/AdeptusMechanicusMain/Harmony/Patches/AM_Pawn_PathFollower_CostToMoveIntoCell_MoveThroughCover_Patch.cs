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
    [HarmonyPatch(typeof(Pawn_PathFollower), "CostToMoveIntoCell", new[] { typeof(Pawn), typeof(IntVec3) })]
    public static class AM_Pawn_PathFollower_CostToMoveIntoCell_MoveThroughCover_Patch
    {
        [HarmonyPostfix]
        public static void CostToMoveIntoCell_MoveThroughCover_Postfix(Pawn_PathFollower __instance, Pawn pawn, IntVec3 c, ref int __result)
        {
            if (pawn.health.hediffSet.hediffs.Any(x=>x.TryGetComp<HediffComp_MoveThroughCover>()!=null))
            {
                HediffComp_MoveThroughCover throughCover = pawn.health.hediffSet.hediffs.First(x => x.TryGetComp<HediffComp_MoveThroughCover>()!=null).TryGetComp<HediffComp_MoveThroughCover>();
                if (throughCover==null)
                {
                //    Log.Warning("throughCover NULL");
                    return;
                }
                if (!throughCover.Active)
                {
                //    Log.Warning("throughCover !Active");
                    return;
                }
            //    Log.Warning("throughCover Active");
                int num;
                if (c.x == pawn.Position.x || c.z == pawn.Position.z)
                {
                    num = pawn.TicksPerMoveCardinal;
                }
                else
                {
                    num = pawn.TicksPerMoveDiagonal;
                }

                Building edifice = c.GetEdifice(pawn.Map);
                if (edifice != null)
                {
                    num += (int)edifice.PathWalkCostFor(pawn);
                }

                if (num > 450)
                {
                    num = 450;
                }

                if (pawn.jobs.curJob != null)
                {
                    switch (pawn.jobs.curJob.locomotionUrgency)
                    {
                        case LocomotionUrgency.Amble:
                            num *= 3;
                            if (num < 60)
                            {
                                num = 60;
                            }

                            break;
                        case LocomotionUrgency.Walk:
                            num *= 2;
                            if (num < 50)
                            {
                                num = 50;
                            }

                            break;
                        case LocomotionUrgency.Jog:
                            num *= 1;
                            break;
                        case LocomotionUrgency.Sprint:
                            num = Mathf.RoundToInt((float)num * 0.75f);
                            break;
                    }
                }

                __result = Mathf.Max(num, 1);
            }
        }
    }
    
}
