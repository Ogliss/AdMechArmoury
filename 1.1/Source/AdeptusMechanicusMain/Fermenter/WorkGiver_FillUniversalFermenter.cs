using System;
using RimWorld;
using Verse;
using Verse.AI;

namespace AdeptusMechanicus
{
	// Token: 0x02000009 RID: 9
	public class WorkGiver_FillUniversalFermenter : WorkGiver_Scanner
	{
		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000039 RID: 57 RVA: 0x00002EDE File Offset: 0x000010DE
		public override ThingRequest PotentialWorkThingRequest
		{
			get
			{
				return ThingRequest.ForGroup((ThingRequestGroup)9);
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600003A RID: 58 RVA: 0x00002EE7 File Offset: 0x000010E7
		public override PathEndMode PathEndMode
		{
			get
			{
				return (PathEndMode)2;
			}
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00002EEA File Offset: 0x000010EA
		public static void Reset()
		{
			WorkGiver_FillUniversalFermenter.TemperatureTrans = Translator.Translate("BadTemperature").ToLower();
			WorkGiver_FillUniversalFermenter.NoIngredientTrans = Translator.Translate("VG_NoIngredient");
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00002F10 File Offset: 0x00001110
		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			CompUniversalFermenter compUniversalFermenter = ThingCompUtility.TryGetComp<CompUniversalFermenter>(t);
			if (compUniversalFermenter == null || compUniversalFermenter.Fermented || compUniversalFermenter.SpaceLeftForIngredient <= 0)
			{
				return false;
			}
			float ambientTemperature = compUniversalFermenter.parent.AmbientTemperature;
			if (ambientTemperature < compUniversalFermenter.Product.temperatureSafe.min + 2f || ambientTemperature > compUniversalFermenter.Product.temperatureSafe.max - 2f)
			{
				JobFailReason.Is(WorkGiver_FillUniversalFermenter.TemperatureTrans, null);
				return false;
			}
			if (ForbidUtility.IsForbidden(t, pawn) || !ReservationUtility.CanReserveAndReach(pawn, t, (PathEndMode)2, DangerUtility.NormalMaxDanger(pawn), 1, -1, null, forced))
			{
				return false;
			}
			if (pawn.Map.designationManager.DesignationOn(t, DesignationDefOf.Deconstruct) != null)
			{
				return false;
			}
			if (this.FindIngredient(pawn, t) == null)
			{
				JobFailReason.Is(WorkGiver_FillUniversalFermenter.NoIngredientTrans, null);
				return false;
			}
			return !FireUtility.IsBurning(t);
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00002FE4 File Offset: 0x000011E4
		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Thing thing = this.FindIngredient(pawn, t);
			return new Job(DefDatabase<JobDef>.GetNamed("VG_FillUniversalFermenter", true), t, thing)
			{
				count = ThingCompUtility.TryGetComp<CompUniversalFermenter>(t).SpaceLeftForIngredient
			};
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00003028 File Offset: 0x00001228
		private Thing FindIngredient(Pawn pawn, Thing fermenter)
		{
			ThingFilter filter = ThingCompUtility.TryGetComp<CompUniversalFermenter>(fermenter).Product.ingredientFilter;
			Predicate<Thing> predicate = (Thing x) => !ForbidUtility.IsForbidden(x, pawn) && ReservationUtility.CanReserve(pawn, x, 1, -1, null, false) && filter.Allows(x);
			Predicate<Thing> predicate2 = predicate;
			return GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, filter.BestThingRequest, (PathEndMode)3, TraverseParms.For(pawn, (Danger)3, 0, false), 9999f, predicate2, null, 0, -1, false, (RegionType)6, false);
		}

		// Token: 0x0400001F RID: 31
		private static string TemperatureTrans;

		// Token: 0x04000020 RID: 32
		private static string NoIngredientTrans;
	}
}
