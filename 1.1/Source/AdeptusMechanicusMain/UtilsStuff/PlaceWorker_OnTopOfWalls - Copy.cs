using System;
using System.Linq;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x02000033 RID: 51
    public class PlaceWorker_OnTopOfLowWalls : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
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
    }
}
