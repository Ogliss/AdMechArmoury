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
    [HarmonyPatch(typeof(Verse.GenGrid), "Impassable")]
    public static class GenGrid_Impassable_DeployableBarricade_Patch
    {
        [HarmonyPostfix]
        public static void ImpassablePostfix(IntVec3 c, Map map, ref bool __result)
        {
            if (map != null)
            {
                List<Thing> list = map.thingGrid.ThingsListAt(c);
                list = list.FindAll(x => x is Building_DeployableBarricade);
                for (int j = 0; j < list.Count; j++)
                {
                    Building_DeployableBarricade barricade = null;
                    if (c.InBounds(map))
                    {
                        barricade = (list[j] as Building_DeployableBarricade);
                        if (barricade != null)
                        {
                            if (barricade.Toggled && barricade.Deployed.deployedpassability == Traversability.Impassable)
                            {
                                __result = true;
                            }
                        }
                    }
                }
            }

        }
    }
}
