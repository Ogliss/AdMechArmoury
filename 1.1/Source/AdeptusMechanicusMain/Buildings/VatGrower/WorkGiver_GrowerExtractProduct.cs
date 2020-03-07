using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using Verse.AI;

namespace AdeptusMechanicus
{
    /// <summary>
    /// WorkGiver for order processors.
    /// </summary>
    public class WorkGiver_GrowerExtractProduct : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(def.fixedBillGiverDefs.FirstOrDefault());

        //JobDriver_HaulToContainer is Job 'HaulToContainer'
        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Building_GrowerBase grower = t as Building_GrowerBase;
            if (grower == null)
            {
                return false;
            }

            if (!grower.GrowerProps.productRequireManualExtraction)
            {
                return false;
            }

            if (t.IsForbidden(pawn))
            {
                return false;
            }

            if (!pawn.CanReserve(t))
            {
                return false;
            }

            if (grower.status != CrafterStatus.Finished)
            {
                return false;
            }

            return true;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Building_GrowerBase grower = t as Building_GrowerBase;

            Job job = new Job(OGVatJobDefOf.OG_ExtractFromGrowerJob, t);
            return job;
        }
    }
}
