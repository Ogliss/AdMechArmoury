﻿using System;
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
    /*
    [HarmonyPatch(typeof(Verse.AI.PathGrid), "PerceivedPathCostAt", new Type[] { typeof(IntVec3) })]
    public static class PathGrid_PerceivedPathCostAt_DeployableBarricade_Patch
    {
        [HarmonyPostfix]
        public static void PerceivedPathCostAtPostfix(IntVec3 loc, ref int __result)
        {
            Map map = Find.CurrentMap;

            if (map != null && loc.InBounds(map))
            {
                List<Thing> list = map.thingGrid.ThingsListAt(loc);
                list = list.FindAll(x => x is Building_DeployableBarricade);
                for (int j = 0; j < list.Count; j++)
                {
                    Building_DeployableBarricade barricade = list[j] as Building_DeployableBarricade;
                    if (barricade != null)
                    {
                        //    Log.Message(string.Format("barricade PerceivedPathCostAt found loc: {0}, Status: {1}, __result: {2}", loc, barricade.Toggled, __result));
                        if (barricade.Toggled)
                        {
                            __result += barricade.Deployed.deployedpathCost;
                            //    Log.Message(string.Format("barricade PerceivedPathCostAt set to : {1}, @: {0}", loc, __result));
                        }
                    }
                }
            }

        }
    }
    */
}
