using System;
using System.Linq;
using Verse;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.PlaceWorker_OnTopOfLowWalls
    public class PlaceWorker_OnTopOfLowWalls : PlaceWorker
    {

        public override bool ForceAllowPlaceOver(BuildableDef other)
        {
            if (other is ThingDef def)
            {
                if (def.graphicData != null)
                {
                    LinkFlags linkFlags = def.graphicData.linkFlags;
                    if (linkFlags.HasFlag(LinkFlags.Barricades) || linkFlags.HasFlag(LinkFlags.Sandbags) || linkFlags.HasFlag(LinkFlags.Fences))
                    {
                        return true;
                    }
                }
            }
            return base.ForceAllowPlaceOver(other);
        }
    }
}
