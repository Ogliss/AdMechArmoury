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
    [HarmonyPatch(typeof(Verse.GenGrid), "Standable")]
    public static class GenGrid_Standable_DeployableBarricade_Patch
    {
        [HarmonyPostfix]
        public static void StandablePostfix(IntVec3 c, Map map, ref bool __result)
        {
            if (map != null)
            {
                if (c.InBounds(map))
                {
                    List<Thing> list = map.thingGrid.ThingsListAt(c);

                    list = list.FindAll(x => x is Building_DeployableBarricade);
                    for (int j = 0; j < list.Count; j++)
                    {
                        if (c.InBounds(map))
                        {
                            if (list[j] is Building_DeployableBarricade barricade)
                            {
                                if (barricade.Toggled && barricade.Deployed.deployedpassability != Traversability.Standable)
                                {
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
