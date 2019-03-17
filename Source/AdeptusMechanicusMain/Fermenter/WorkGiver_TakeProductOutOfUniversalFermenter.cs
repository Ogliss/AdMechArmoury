using System;
using RimWorld;
using Verse;
using Verse.AI;

namespace AdeptusMechanicus
{
	// Token: 0x0200000A RID: 10
	public class WorkGiver_TakeProductOutOfUniversalFermenter : WorkGiver_Scanner
	{
		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000040 RID: 64 RVA: 0x00002EDE File Offset: 0x000010DE
		public override ThingRequest PotentialWorkThingRequest
		{
			get
			{
				return ThingRequest.ForGroup((ThingRequestGroup)9);
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000041 RID: 65 RVA: 0x00002EE7 File Offset: 0x000010E7
		public override PathEndMode PathEndMode
		{
			get
			{
				return (PathEndMode)2;
			}
		}

		// Token: 0x06000042 RID: 66 RVA: 0x000030B0 File Offset: 0x000012B0
		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			CompUniversalFermenter compUniversalFermenter = ThingCompUtility.TryGetComp<CompUniversalFermenter>(t);
			return compUniversalFermenter != null && compUniversalFermenter.Fermented && !FireUtility.IsBurning(t) && !ForbidUtility.IsForbidden(t, pawn) && ReservationUtility.CanReserveAndReach(pawn, t, (PathEndMode)2, DangerUtility.NormalMaxDanger(pawn), 1, -1, null, forced);
		}

		// Token: 0x06000043 RID: 67 RVA: 0x000030F9 File Offset: 0x000012F9
		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return new Job(DefDatabase<JobDef>.GetNamed("VG_TakeProductOutOfUniversalFermenter", true), t);
		}
	}
}
