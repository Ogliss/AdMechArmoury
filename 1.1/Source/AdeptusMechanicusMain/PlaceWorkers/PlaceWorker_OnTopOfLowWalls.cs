using System;
using System.Linq;
using Verse;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.PlaceWorker_OnTopOfLowWalls
    public class PlaceWorker_OnTopOfLowWalls : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            foreach (IntVec3 c in GenAdj.CellsOccupiedBy(loc, rot, checkingDef.Size))
            {
                if (GridsUtility.GetThingList(c, map).FirstOrDefault((Thing x) => x.def.defName.Contains("Sandbag") || x.def.defName.Contains("Barricade")) == null)
                {
                    return new AcceptanceReport(Translator.Translate("AdeptusMechanicus.PlaceWorker_OnTopOfWalls"));
                }
            }
            return true;
            bool flag = GridsUtility.GetThingList(loc, map).FirstOrDefault((Thing x) => x.def.defName.Contains("Sandbag") || x.def.defName.Contains("Barricade")) != null;
            AcceptanceReport result;
            if (flag)
            {
                result = true;
            }
            else
            {
                return base.AllowsPlacing(checkingDef, loc, rot, map, thingToIgnore, thing);
            }
            return result;
        }
        public override bool ForceAllowPlaceOver(BuildableDef other)
        {
            if (other.defName.Contains("Sandbag") || other.defName.Contains("Barricade") || other.graphic.data.linkFlags.HasFlag(LinkFlags.Barricades) || other.graphic.data.linkFlags.HasFlag(LinkFlags.Sandbags))
            {
                return true;
            }
            return base.ForceAllowPlaceOver(other);
        }
    }
}
