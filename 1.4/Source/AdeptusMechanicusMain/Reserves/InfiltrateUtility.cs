using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace AdeptusMechanicus
{
    public static class InfiltrateUtility 
    {
        public static void PlaceInfiltratorsNear(IntVec3 dropCenter, Map map, IEnumerable<Thing> things, int openDelay = 110, bool canInstaDropDuringInit = false, bool leaveSlag = false, bool canRoofPunch = true)
        {
            InfiltrateUtility.tempList.Clear();
            foreach (Thing item in things)
            {
                List<Thing> list = new List<Thing>
                {
                    item
                };
                InfiltrateUtility.tempList.Add(list);
            }
            InfiltrateUtility.PlaceInfiltratorGroupsNear(dropCenter, map, InfiltrateUtility.tempList, openDelay, canInstaDropDuringInit, leaveSlag, canRoofPunch);
            InfiltrateUtility.tempList.Clear();
        }

        public static void PlaceInfiltratorGroupsNear(IntVec3 dropCenter, Map map, List<List<Thing>> thingsGroups, int openDelay = 110, bool instaDrop = false, bool leaveSlag = false, bool canRoofPunch = true)
        {
            foreach (List<Thing> list in thingsGroups)
            {
                IntVec3 intVec;
                if (!DropCellFinder.TryFindDropSpotNear(dropCenter, map, out intVec, true, canRoofPunch))
                {
                    Log.Warning(string.Concat(new object[]
                    {
                        "DropThingsNear failed to find a place to drop ",
                        list.FirstOrDefault<Thing>(),
                        " near ",
                        dropCenter,
                        ". Dropping on random square instead."
                    }));
                    intVec = CellFinderLoose.RandomCellWith((IntVec3 c) => c.Walkable(map) && (c.Roofed(map) && c.GetRoof(map) != RoofDefOf.RoofRockThick), map, 1000);
                }
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].SetForbidden(true, false);
                }
                foreach (Thing thing in list)
                {
                    GenPlace.TryPlaceThing(thing, intVec, map, ThingPlaceMode.Near, null, null);
                }
            }
        }

        private static List<List<Thing>> tempList = new List<List<Thing>>();
    }
}
