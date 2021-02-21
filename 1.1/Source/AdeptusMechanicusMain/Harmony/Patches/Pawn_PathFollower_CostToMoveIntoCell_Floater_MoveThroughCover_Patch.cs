using AdeptusMechanicus.ExtensionMethods;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace AdeptusMechanicus.HarmonyInstance
{
    /*This Harmony Postfix changes terrain calculation for floating creatures
   */
    [HarmonyPatch(typeof(Verse.AI.Pawn_PathFollower))]
    [HarmonyPatch("CostToMoveIntoCell")]
    [HarmonyPatch(new Type[] { typeof(Pawn), typeof(IntVec3) })]
    public static class Pawn_PathFollower_CostToMoveIntoCell_Floater_MoveThroughCover_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn pawn, IntVec3 c, ref int __result)
        {
            if ((pawn.Map != null) && (pawn.TryGetCompFast<CompFloating>() != null))
            {
                CompFloating floating = pawn.TryGetCompFast<CompFloating>();
                if (floating != null)
                {
                    //    Log.Message(pawn + " has CompFloating isFloater: "+ floating.Props.isFloater+ " canCrossWater: " + floating.Props.canCrossWater);
                    if (floating.Props.isFloater)
                    {
                        int num;
                        if (c.x == pawn.Position.x || c.z == pawn.Position.z)
                        {
                            num = pawn.TicksPerMoveCardinal;
                        }
                        else
                        {
                            num = pawn.TicksPerMoveDiagonal;
                        }
                        TerrainDef terrainDef = pawn.Map.terrainGrid.TerrainAt(c);
                        if (terrainDef == null)
                        {
                            num = 10000;
                        }
                        else if ((terrainDef.passability == Traversability.Impassable) && !terrainDef.IsWater)
                        {
                            num = 10000;
                        }
                        else if (terrainDef.IsWater && !floating.Props.canCrossWater)
                        {
                            num = 10000;
                        }
                        List<Thing> list = pawn.Map.thingGrid.ThingsListAt(c);
                        for (int i = 0; i < list.Count; i++)
                        {
                            Thing thing = list[i];
                            if (thing.def.passability == Traversability.Impassable)
                            {
                                num = 10000;
                            }

                            if (thing is Building_Door)
                            {
                                num += 45;
                            }
                        }

                        __result = num;

                        if (terrainDef.IsWater)
                        {
                            //    Log.Message("IsWater "+ __result);
                        }
                    }
                }

            }
            else
            {

                for (int i = 0; i < pawn.health.hediffSet.hediffs.Count; i++)
                {
                    Hediff hediff = pawn.health.hediffSet.hediffs[i];
                    HediffComp_MoveThroughCover throughCover = hediff.TryGetCompFast<HediffComp_MoveThroughCover>();
                    if (throughCover != null)
                    {
                        if (!throughCover.Active)
                        {
                            //    Log.Warning("throughCover !Active");
                            continue;
                        }
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
                        return;
                    }
                }
            }
        }
    }
}
