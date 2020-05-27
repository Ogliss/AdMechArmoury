using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace AdeptusMechanicus
{
	// Token: 0x02000005 RID: 5
	public class JobDriver_TakeProductOutOfUniversalFermenter : JobDriver
	{
		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600002D RID: 45 RVA: 0x00002C1C File Offset: 0x00000E1C
		protected Thing Fermenter
		{
			get
			{
				return this.job.GetTarget((TargetIndex)1).Thing;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600002E RID: 46 RVA: 0x00002C40 File Offset: 0x00000E40
		protected Thing Product
		{
			get
			{
				return this.job.GetTarget((TargetIndex)2).Thing;
			}
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00002C61 File Offset: 0x00000E61
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return ReservationUtility.Reserve(this.pawn, this.Fermenter, this.job, 1, -1, null, true);
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00002C83 File Offset: 0x00000E83
		protected override IEnumerable<Toil> MakeNewToils()
		{
			CompUniversalFermenter comp = ThingCompUtility.TryGetComp<CompUniversalFermenter>(this.Fermenter);
			ToilFailConditions.FailOn<JobDriver_TakeProductOutOfUniversalFermenter>(this, () => !comp.Fermented);
			ToilFailConditions.FailOnDestroyedNullOrForbidden<JobDriver_TakeProductOutOfUniversalFermenter>(this, (TargetIndex)1);
			yield return Toils_Reserve.Reserve((TargetIndex)1, 1, -1, null);
			yield return Toils_Goto.GotoThing((TargetIndex)1, (PathEndMode)3);
			yield return ToilEffects.WithProgressBarToilDelay(ToilFailConditions.FailOnDestroyedNullOrForbidden<Toil>(Toils_General.Wait(200, 0), (TargetIndex)1), (TargetIndex)1, false, -0.5f);
			yield return new Toil
			{
				initAction = delegate()
				{
					Thing thing = comp.TakeOutProduct();
					GenPlace.TryPlaceThing(thing, this.pawn.Position, this.Map, (ThingPlaceMode)1, null, null);
					StoragePriority storagePriority = StoreUtility.CurrentStoragePriorityOf(thing);
					IntVec3 intVec;
					if (StoreUtility.TryFindBestBetterStoreCellFor(thing, this.pawn, this.Map, storagePriority, this.pawn.Faction, out intVec, true))
					{
						this.job.SetTarget((TargetIndex)2, thing);
						this.job.count = thing.stackCount;
						this.job.SetTarget((TargetIndex)3, intVec);
						return;
					}
					this.EndJobWith((JobCondition)3);
				},
				defaultCompleteMode = (ToilCompleteMode)1
			};
			yield return Toils_Reserve.Reserve((TargetIndex)2, 1, -1, null);
			yield return Toils_Reserve.Reserve((TargetIndex)3, 1, -1, null);
			yield return Toils_Goto.GotoThing((TargetIndex)2, (PathEndMode)3);
			yield return Toils_Haul.StartCarryThing((TargetIndex)2, false, false, false);
			Toil carry = Toils_Haul.CarryHauledThingToCell((TargetIndex)3);
			yield return carry;
			yield return Toils_Haul.PlaceHauledThingInCell((TargetIndex)3, carry, true);
			yield break;
		}

		// Token: 0x0400000D RID: 13
		private const TargetIndex FermenterInd = (TargetIndex)1;

		// Token: 0x0400000E RID: 14
		private const TargetIndex ProductToHaulInd = (TargetIndex)2;

		// Token: 0x0400000F RID: 15
		private const TargetIndex StorageCellInd = (TargetIndex)3;

		// Token: 0x04000010 RID: 16
		private const int Duration = 200;
	}
}
