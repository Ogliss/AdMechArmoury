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

namespace AdeptusMechanicus.Harmony
{
    [HarmonyPatch(typeof(Verse.AI.PathGrid), "PerceivedPathCostAt", new Type[] { typeof(IntVec3) })]
    public static class AM_PathGrid_PerceivedPathCostAt_DeployableBarricade_Patch
    {
        [HarmonyPostfix]
        public static void PerceivedPathCostAtPostfix(IntVec3 loc, ref int __result)
        {
            Map map = Find.CurrentMap;

            if (map != null)
            {
                List<Thing> list = map.thingGrid.ThingsListAt(loc);
                list = list.FindAll(x => x is Building_DeployableBarricade);
                for (int j = 0; j < list.Count; j++)
                {
                    Building_DeployableBarricade barricade = null;
                    if (loc.InBounds(map))// && perceivedStatic)
                    {
                        barricade = (list[j] as Building_DeployableBarricade);
                        if (barricade != null)
                        {
                            //    Log.Message(string.Format("barricade PerceivedPathCostAt found loc: {0}, Status: {1}, __result: {2}", loc, barricade.Toggled, __result));
                            if (barricade.Toggled)
                            {
                                __result += barricade.deployed.deployedpathCost;
                                //    Log.Message(string.Format("barricade PerceivedPathCostAt set to : {1}, @: {0}", loc, __result));
                            }
                        }
                    }
                }
            }

        }
    }
}
