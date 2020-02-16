using System;
using System.Collections.Generic;
using System.Linq;
using HoloEmitters.Things;
using RimWorld;
using Verse;
using Verse.AI;

namespace HoloEmitters
{
    // Token: 0x0200005B RID: 91
    public class WorkGiver_LoadIntoEmitter : WorkGiver_Scanner
    {
        // Token: 0x1700001D RID: 29
        // (get) Token: 0x0600014A RID: 330 RVA: 0x0000D3EC File Offset: 0x0000B7EC
        public override PathEndMode PathEndMode
        {
            get
            {
                return (Verse.AI.PathEndMode)3;
            }
        }

        // Token: 0x1700001E RID: 30
        // (get) Token: 0x0600014B RID: 331 RVA: 0x0000D404 File Offset: 0x0000B804
        public override ThingRequest PotentialWorkThingRequest
        {
            get
            {
                return ThingRequest.ForDef(ThingDef.Named("HoloDisk"));
            }
        }

        // Token: 0x0600014C RID: 332 RVA: 0x0000D428 File Offset: 0x0000B828
        private HoloEmitter FindEmitter(Pawn p, Thing corpse)
        {
            IEnumerable<ThingDef> enumerable = from def in DefDatabase<ThingDef>.AllDefs
                                               where typeof(HoloEmitter).IsAssignableFrom(def.thingClass)
                                               select def;
            foreach (ThingDef thingDef in enumerable)
            {
                Predicate<Thing> predicate = (Thing x) => ((HoloEmitter)x).GetComp<CompHoloEmitter>().SimPawn == null && ReservationUtility.CanReserve(p, x, 1, -1, null, false);
                HoloEmitter holoEmitter = (HoloEmitter)GenClosest.ClosestThingReachable(p.Position, p.Map, ThingRequest.ForDef(thingDef), (Verse.AI.PathEndMode)4, TraverseParms.For(p, (Verse.Danger)3, 0, false), 9999f, predicate, null, 0, -1, false, (Verse.RegionType)6, false);
                if (holoEmitter != null)
                {
                    return holoEmitter;
                }
            }
            return null;
        }

        // Token: 0x0600014D RID: 333 RVA: 0x0000D51C File Offset: 0x0000B91C
        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Job result;
            if (t.def.defName != "HoloDisk")
            {
                result = null;
            }
            else if (!ReservationUtility.CanReserveAndReach(pawn, t, (Verse.AI.PathEndMode)2, (Verse.Danger)3, 1, 1, null, false))
            {
                result = null;
            }
            else
            {
                HoloEmitter holoEmitter = this.FindEmitter(pawn, t);
                if (holoEmitter == null)
                {
                    result = null;
                }
                else if (holoEmitter.GetComp<CompHoloEmitter>().SimPawn != null)
                {
                    result = null;
                }
                else
                {
                    result = new Job(JobDefOfHoloEmitters.LoadIntoEmitter, t, holoEmitter)
                    {
                        count = 1
                    };
                }
            }
            return result;
        }
        /*
        // Token: 0x0600014E RID: 334 RVA: 0x0000D5C0 File Offset: 0x0000B9C0
        public override bool ShouldSkip(Pawn pawn)
        {
            return pawn.Map.listerThings.ThingsOfDef(ThingDef.Named("HoloDisk")).Count == 0;
        }
        */
    }
}
