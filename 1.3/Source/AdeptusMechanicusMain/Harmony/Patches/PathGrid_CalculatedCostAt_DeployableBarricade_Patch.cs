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

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Verse.AI.PathGrid), "CalculatedCostAt", new Type[] { typeof(IntVec3), typeof(bool), typeof(IntVec3) })]
    public static class PathGrid_CalculatedCostAt_DeployableBarricade_Patch
    {
        [HarmonyPostfix]
        public static void CalculatedCostAtPostfix(IntVec3 c, bool perceivedStatic, IntVec3 prevCell, ref int __result)
        {
            Map map = Find.CurrentMap;

            if (map != null && c.InBounds(map))
            {
                List<Thing> list = map.thingGrid.ThingsListAt(c);
                list = list.FindAll(x => x is Building_DeployableBarricade);
                for (int j = 0; j < list.Count; j++)
                {
                    Building_DeployableBarricade barricade = list[j] as Building_DeployableBarricade;
                    if (barricade != null)
                    {
                        //    Log.Message(string.Format("barricade CalculatedCostAt found perceivedStatic: {0}, c: {1}, prevCell: {2}, Toggled: {3}, __result: {4}", perceivedStatic, c, prevCell, barricade.Toggled, __result));
                        if (barricade.Toggled)
                        {
                            __result += barricade.Deployed.deployedpathCost;
                            //   Log.Message(string.Format("barricade CalculatedCostAt set to : {1}, @: {0}", c, __result));
                        }
                    }
                }
            }

        }
    }

}
