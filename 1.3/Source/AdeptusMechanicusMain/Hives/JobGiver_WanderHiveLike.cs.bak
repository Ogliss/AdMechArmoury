using System;
using Verse;
using Verse.AI;

namespace AdeptusMechanicus
{
	// Token: 0x020000A5 RID: 165
	public class JobGiver_WanderHiveLike : JobGiver_Wander
	{
		// Token: 0x0600041F RID: 1055 RVA: 0x0002CD08 File Offset: 0x0002B108
		public JobGiver_WanderHiveLike()
		{
			this.wanderRadius = 7.5f;
			this.ticksBetweenWandersRange = new IntRange(125, 200);
		}

		// Token: 0x06000420 RID: 1056 RVA: 0x0002CD30 File Offset: 0x0002B130
		protected override IntVec3 GetWanderRoot(Pawn pawn)
		{
			HiveLike hivelike = pawn.mindState.duty.focus.Thing as HiveLike;
			if (hivelike == null || !hivelike.Spawned)
			{
				return pawn.Position;
			}
			return hivelike.Position;
		}
	}
}
