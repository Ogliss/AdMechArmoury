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
    public static class Pawn_PathFollower_CostToMoveIntoCell_MoveThroughCover_Patch
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
    /*
    [HarmonyPatch(typeof(Pawn_PathFollower), "CostToMoveIntoCell", new Type[] { typeof(IntVec3) })]
    class SwimmerFollowerPatch
    {
        public static int CostToMoveIntoCell(Pawn pawn, IntVec3 c)
        {
            int num;
            if (c.x == pawn.Position.x || c.z == pawn.Position.z)
            {
                num = pawn.TerrainAwareTicksPerMoveCardinal(c);
            }
            else
            {
                num = pawn.TerrainAwareTicksPerMoveDiagonal(c);
            }
            int gridCost = pawn.Map.pathGrid.CalculatedCostAt(c, false, pawn.Position);
            TerrainDef terrain = c.GetTerrain(pawn.Map);
            StatDef swimDef = DefDatabase<StatDef>.GetNamed("SwimSpeed", true);
            StatDef swimPathCostDef = DefDatabase<StatDef>.GetNamed("pathCostSwimming", false);
            int swimPathCost = (swimPathCostDef != null) ? (int)terrain.GetStatValueAbstract(swimPathCostDef) : 0;
            float swimSpeed = pawn.GetStatValue(swimDef, true);
            bool water = terrain.HasTag("Water");
            bool swimming = water && swimSpeed > 0;
            if (swimming)
            {
                if (swimPathCost > 0)
                {
                    // Replace grid cost with swimming cost
                    gridCost += swimPathCost - terrain.pathCost;
                }
                else
                {
                    // Reduce the path penalty for swimming by 10x
                    gridCost -= (terrain.pathCost * 9) / 10;
                }
            }
            num += gridCost;
            Building edifice = c.GetEdifice(pawn.Map);
            if (edifice != null)
            {
                num += (int)edifice.PathWalkCostFor(pawn);
            }
            if (num > 450)
            {
                num = 450;
            }
            if (pawn.CurJob != null)
            {
                Pawn locomotionUrgencySameAs = pawn.jobs.curDriver.locomotionUrgencySameAs;
                if (locomotionUrgencySameAs != null && locomotionUrgencySameAs != pawn && locomotionUrgencySameAs.Spawned)
                {
                    int num2 = SwimmerFollowerPatch.CostToMoveIntoCell(locomotionUrgencySameAs, c);
                    if (num < num2)
                    {
                        num = num2;
                    }
                }
                else
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
                            break;
                        case LocomotionUrgency.Sprint:
                            num = Mathf.RoundToInt((float)num * 0.75f);
                            break;
                    }
                }
            }
            return Mathf.Max(num, 1);
        }

        static bool Prefix(ref int __result, Pawn ___pawn, IntVec3 c)
        {
            __result = CostToMoveIntoCell(___pawn, c);
            return false;
        }
    }

    static class PawnExtensions
    {
        public static int TerrainAwareTicksPerMoveCardinal(this Pawn pawn, IntVec3 loc)
        {
            return pawn.TerrainAwareTicksPerMove(loc, false);
        }

        public static int TerrainAwareTicksPerMoveDiagonal(this Pawn pawn, IntVec3 loc)
        {
            return pawn.TerrainAwareTicksPerMove(loc, true);
        }

        public static int TerrainAwareTicksPerMove(this Pawn pawn, IntVec3 loc, bool diagonal)
        {
            float num;
            TerrainDef terrain = pawn.Map.terrainGrid.TerrainAt(loc);
            StatDef swimDef = DefDatabase<StatDef>.GetNamed("SwimSpeed", false);
            float swimSpeed = (swimDef != null) ? pawn.GetStatValue(swimDef, true) : 0;
            if (terrain.HasTag("Water") && swimSpeed > 0.000001)
            {
                num = swimSpeed;
            }
            else
            {
                num = pawn.GetStatValue(StatDefOf.MoveSpeed, true);
            }
            if (RestraintsUtility.InRestraints(pawn))
            {
                num *= 0.35f;
            }
            if (pawn.carryTracker != null && pawn.carryTracker.CarriedThing != null && pawn.carryTracker.CarriedThing.def.category == ThingCategory.Pawn)
            {
                num *= 0.6f;
            }
            float num2 = num / 60f;
            float num3;
            if (num2 == 0f)
            {
                num3 = 450f;
            }
            else
            {
                num3 = 1f / num2;
                if (pawn.Spawned && !pawn.Map.roofGrid.Roofed(pawn.Position))
                {
                    num3 /= pawn.Map.weatherManager.CurMoveSpeedMultiplier;
                }
                if (diagonal)
                {
                    num3 *= 1.41421f;
                }
            }
            int value = Mathf.RoundToInt(num3);
            return Mathf.Clamp(value, 1, 450);
        }
    }
    */
}
