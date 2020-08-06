using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace AdeptusMechanicus.HarmonyInstance
{
    /*This Harmony Postfix changes terrain calculation for floating creatures
   */
    [HarmonyPatch(typeof(Verse.AI.Pawn_PathFollower))]
    [HarmonyPatch("CostToMoveIntoCell")]
    [HarmonyPatch(new Type[] { typeof(Pawn), typeof(IntVec3) })]
    public static class Pawn_PathFollower_CostToMoveIntoCell_Patch
    {
        [HarmonyPostfix]
        public static void MakeFloatingCreaturesGreatAgaian(Pawn pawn, IntVec3 c, ref int __result)

        {
            if ((pawn.Map != null) && (pawn.TryGetComp<CompFloating>() != null))
            {
                CompFloating floating = pawn.TryGetComp<CompFloating>();
                Log.Message(pawn + " has CompFloating isFloater: "+ floating.Props.isFloater+ " canCrossWater: " + floating.Props.canCrossWater);
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
                        Log.Message("IsWater "+ __result);
                    }
                }

            }
        }
    }
}
