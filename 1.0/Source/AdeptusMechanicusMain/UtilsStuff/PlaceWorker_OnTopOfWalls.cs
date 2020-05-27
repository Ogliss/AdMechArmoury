using System;
using System.Linq;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x02000033 RID: 51
    public class PlaceWorker_OnTopOfWalls : PlaceWorker
    {
        // Token: 0x0600013B RID: 315 RVA: 0x0000B4A4 File Offset: 0x000096A4
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null)
        {
            bool flag = GridsUtility.GetThingList(loc, map).FirstOrDefault((Thing x) => x.def.defName.Contains("Wall") || x.def.defName.Contains("Smoothed")) != null;
            AcceptanceReport result;
            if (flag)
            {
                result = true;
            }
            else
            {
                result = new AcceptanceReport(Translator.Translate("AM_PlaceWorker_OnTopOfWalls"));
            }
            return result;
        }
    }
}
