using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace AdeptusMechanicus
{
	// Token: 0x02000004 RID: 4
	public class JobDriver_FillUniversalFermenter : JobDriver
	{
		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000028 RID: 40 RVA: 0x00002B9C File Offset: 0x00000D9C
		protected Thing Fermenter
		{
			get
			{
				return this.job.GetTarget((TargetIndex)1).Thing;
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000029 RID: 41 RVA: 0x00002BC0 File Offset: 0x00000DC0
		protected Thing Ingredient
		{
			get
			{
				return this.job.GetTarget((TargetIndex)2).Thing;
			}
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002BE1 File Offset: 0x00000DE1
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return ReservationUtility.Reserve(this.pawn, this.Fermenter, this.job, 1, -1, null, true);
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002C03 File Offset: 0x00000E03
		protected override IEnumerable<Toil> MakeNewToils()
		{
			CompUniversalFermenter comp = ThingCompUtility.TryGetComp<CompUniversalFermenter>(this.Fermenter);
			ToilFailConditions.FailOn<JobDriver_FillUniversalFermenter>(this, () => comp.SpaceLeftForIngredient <= 0);
			ToilFailConditions.FailOnDestroyedNullOrForbidden<JobDriver_FillUniversalFermenter>(this, (TargetIndex)1);
			ToilFailConditions.FailOnDestroyedNullOrForbidden<JobDriver_FillUniversalFermenter>(this, (TargetIndex)2);
			Toil ingrToil = Toils_Reserve.Reserve((TargetIndex)2, 1, -1, null);
			yield return ingrToil;
			yield return Toils_Reserve.Reserve((TargetIndex)1, 1, -1, null);
			yield return ToilFailConditions.FailOnDestroyedNullOrForbidden<Toil>(ToilFailConditions.FailOnSomeonePhysicallyInteracting<Toil>(Toils_Goto.GotoThing((TargetIndex)2, (PathEndMode)3), (TargetIndex)2), (TargetIndex)2);
			yield return ToilFailConditions.FailOnDestroyedNullOrForbidden<Toil>(Toils_Haul.StartCarryThing((TargetIndex)2, false, true, false), (TargetIndex)2);
			yield return Toils_Haul.CheckForGetOpportunityDuplicate(ingrToil, (TargetIndex)2, 0, true, null);
			yield return Toils_Haul.CarryHauledThingToCell((TargetIndex)1);
			yield return ToilEffects.WithProgressBarToilDelay(ToilFailConditions.FailOnDestroyedNullOrForbidden<Toil>(Toils_General.Wait(200, 0), (TargetIndex)1), (TargetIndex)1, false, -0.5f);
			yield return new Toil
			{
				initAction = delegate()
				{
					if (!comp.AddIngredient(this.Ingredient))
					{
						this.EndJobWith((JobCondition)3);
						Log.Message("JobCondition.Incompletable", false);
					}
				},
				defaultCompleteMode = (ToilCompleteMode)1
			};
			yield break;
		}

		// Token: 0x0400000A RID: 10
		private const TargetIndex FermenterInd = (TargetIndex)1;

		// Token: 0x0400000B RID: 11
		private const TargetIndex IngredientInd = (TargetIndex)2;

		// Token: 0x0400000C RID: 12
		private const int Duration = 200;
	}
}
