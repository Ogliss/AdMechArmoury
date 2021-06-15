using RimWorld;
using System;
using Verse;
using Verse.AI;

namespace AdeptusMechanicus
{
	// Token: 0x020000A3 RID: 163
	public class JobGiver_MaintainHiveLikes : JobGiver_AIFightEnemies
	{
		// Token: 0x0600041A RID: 1050 RVA: 0x0002C918 File Offset: 0x0002AD18
		public override ThinkNode DeepCopy(bool resolve = true)
		{
			JobGiver_MaintainHiveLikes jobGiver_MaintainHivelikes = (JobGiver_MaintainHiveLikes)base.DeepCopy(resolve);
			jobGiver_MaintainHivelikes.onlyIfDamagingState = this.onlyIfDamagingState;
			return jobGiver_MaintainHivelikes;
		}

		// Token: 0x0600041B RID: 1051 RVA: 0x0002C940 File Offset: 0x0002AD40
		protected override Job TryGiveJob(Pawn pawn)
		{
			Room room = pawn.GetRoom(RegionType.Set_Passable);
			int num = 0;
			while ((float)num < JobGiver_MaintainHiveLikes.CellsInScanRadius)
			{
				IntVec3 intVec = pawn.Position + GenRadial.RadialPattern[num];
				if (intVec.InBounds(pawn.Map))
				{
					if (intVec.GetRoom(pawn.Map, RegionType.Set_Passable) == room)
					{
						HiveLike hivelike = (HiveLike)pawn.Map.thingGrid.ThingsListAt(intVec).Find(x=> x.def.thingClass == typeof(AdeptusMechanicus.HiveLike));
						if (hivelike != null && pawn.CanReserve(hivelike, 1, -1, null, false))
						{
							CompMaintainable compMaintainable = hivelike.TryGetComp<CompMaintainable>();
							if (compMaintainable.CurStage != MaintainableStage.Healthy)
							{
								if (!this.onlyIfDamagingState || compMaintainable.CurStage == MaintainableStage.Damaging)
								{
									return new Job(JobDefOf.Maintain, hivelike);
								}
							}
						}
					}
				}
				num++;
			}
			return null;
		}

		// Token: 0x04000273 RID: 627
		private bool onlyIfDamagingState;

		// Token: 0x04000274 RID: 628
		private static readonly float CellsInScanRadius = (float)GenRadial.NumCellsInRadius(7.9f);
	}
}
