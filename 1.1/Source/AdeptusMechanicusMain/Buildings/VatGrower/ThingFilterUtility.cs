using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace AdeptusMechanicus
{
    /// <summary>
    /// Utility functions for ThingFilters.
    /// </summary>
    public static class ThingFilterUtility
    {
        public static int TotalStackCountForOrderRequestInContainer(this ThingOrderRequest request, ThingOwner thingOwner)
        {
            if (request == null)
            {
                return 0;
            }
            if (thingOwner.Count <= 0)
            {
                return 0;
            }

            int result = 0;
            if (request.HasThing)
            {
                result += thingOwner.Where(thing => thing == request.thing)?.Select(thing => thing.stackCount)?.Sum() ?? 0;
            }
            if (request.HasThingFilter)
            {
                foreach (ThingDef def in request.thingFilter.AllowedThingDefs)
                {
                    result += thingOwner.TotalStackCountOfDef(def);
                }
            }

            return result;
        }

        public static int TotalStackCountForFilterInContainer(this ThingFilter filter, ThingOwner thingOwner)
        {
            if (filter == null)
            {
                return 0;
            }
            if (thingOwner.Count <= 0)
            {
                return 0;
            }

            int result = 0;
            foreach (ThingDef def in filter.AllowedThingDefs)
            {
                result += thingOwner.TotalStackCountOfDef(def);
            }

            return result;
        }

        public static int TotalStackCountForFilterInList(this ThingFilter filter, IList<Thing> thingList)
        {
            if (filter == null)
            {
                return 0;
            }
            if (thingList.Count <= 0)
            {
                return 0;
            }

            int result = 0;
            foreach (ThingDef def in filter.AllowedThingDefs)
            {
                result += thingList.TotalStackCountOfDefInList(def);
            }

            return result;
        }

        public static int TotalStackCountOfDefInList(this IList<Thing> thingList, ThingDef def)
        {
            if (thingList.Count <= 0)
            {
                return 0;
            }

            int result = 0;
            foreach (Thing thing in thingList)
            {
                if (thing.def == def)
                {
                    result += thing.stackCount;
                }
            }

            return result;
        }
    }
}
