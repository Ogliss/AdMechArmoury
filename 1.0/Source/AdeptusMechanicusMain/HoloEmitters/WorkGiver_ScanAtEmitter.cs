using System;
using System.Collections.Generic;
using System.Linq;
using HoloEmitters.Things;
using RimWorld;
using Verse;
using Verse.AI;

namespace HoloEmitters
{
    // Token: 0x02000052 RID: 82
    public class WorkGiver_ScanAtEmitter : WorkGiver_Scanner
    {
        // Token: 0x17000017 RID: 23
        // (get) Token: 0x06000122 RID: 290 RVA: 0x0000BD98 File Offset: 0x0000A198
        public override PathEndMode PathEndMode
        {
            get
            {
                return PathEndMode.ClosestTouch;
            }
        }

        // Token: 0x17000018 RID: 24
        // (get) Token: 0x06000123 RID: 291 RVA: 0x0000BDB0 File Offset: 0x0000A1B0
        public override ThingRequest PotentialWorkThingRequest
        {
            get
            {
                return ThingRequest.ForGroup(ThingRequestGroup.Corpse);
            }
        }

        // Token: 0x06000124 RID: 292 RVA: 0x0000BDCC File Offset: 0x0000A1CC
        private HoloEmitter FindEmitter(Pawn p, Corpse corpse)
        {
            IEnumerable<ThingDef> enumerable = from def in DefDatabase<ThingDef>.AllDefs
                                               where typeof(HoloEmitter).IsAssignableFrom(def.thingClass)
                                               select def;
            foreach (ThingDef thingDef in enumerable)
            {
                Predicate<Thing> predicate = (Thing x) => ((HoloEmitter)x).GetComp<CompHoloEmitter>().SimPawn == null && ReservationUtility.CanReserve(p, x, 1, -1, null, false);
                HoloEmitter holoEmitter = (HoloEmitter)GenClosest.ClosestThingReachable(p.Position, p.Map, ThingRequest.ForDef(thingDef), (PathEndMode)4, TraverseParms.For(p, (Danger)3, 0, false), 9999f, predicate, null, 0, -1, false, (RegionType)6, false);
                if (holoEmitter != null)
                {
                    return holoEmitter;
                }
            }
            return null;
        }

        // Token: 0x06000125 RID: 293 RVA: 0x0000BEC0 File Offset: 0x0000A2C0
        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Corpse corpse = t as Corpse;
            Job result;
            Log.Message("start");
            if (corpse != null)
            {
                Log.Message("a");
                if (corpse.InnerPawn.Faction != Faction.OfPlayer || !corpse.InnerPawn.RaceProps.Humanlike)
                {
                    Log.Message("fail a");
                    result = null;
                }
                else if (!HaulAIUtility.PawnCanAutomaticallyHaul(pawn, t, forced))
                {
                    Log.Message("fail b");
                    result = null;
                }
                else
                {
                    HoloEmitter holoEmitter = this.FindEmitter(pawn, corpse);
                    if (holoEmitter == null)
                    {
                        Log.Message("fail c");
                        result = null;
                    }
                    else if (holoEmitter.GetComp<CompHoloEmitter>().SimPawn != null)
                    {
                        Log.Message("fail f");
                        result = null;
                    }
                    else
                    {
                        Log.Message("win ");
                        result = new Job(JobDefOfHoloEmitters.ScanAtEmitter, t, holoEmitter)
                        {
                            count = corpse.stackCount
                        };
                    }
                }
            }
            else
            {
                Log.Message("fail ");
                result = null;
            }
            return result;
        }
        /*
        // Token: 0x06000126 RID: 294 RVA: 0x0000BF84 File Offset: 0x0000A384
        public override bool ShouldSkip(Pawn pawn)
        {
            return pawn.Map.listerThings.ThingsInGroup((ThingRequestGroup)8).Count == 0;
        }
        */
    }
}
