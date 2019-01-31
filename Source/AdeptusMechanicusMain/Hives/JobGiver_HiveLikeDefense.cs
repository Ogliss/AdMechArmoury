using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x020000A2 RID: 162
	public class JobGiver_HiveLikeDefense : JobGiver_AIFightEnemies
	{
		// Token: 0x06000416 RID: 1046 RVA: 0x0002C898 File Offset: 0x0002AC98
		protected override IntVec3 GetFlagPosition(Pawn pawn)
		{
			HiveLike hivelike = pawn.mindState.duty.focus.Thing as HiveLike;
			if (hivelike != null && hivelike.Spawned)
			{
				return hivelike.Position;
			}
			return pawn.Position;
		}

		// Token: 0x06000417 RID: 1047 RVA: 0x0002C8DE File Offset: 0x0002ACDE
		protected override float GetFlagRadius(Pawn pawn)
		{
			return pawn.mindState.duty.radius;
		}

		// Token: 0x06000418 RID: 1048 RVA: 0x0002C8F0 File Offset: 0x0002ACF0
		protected override Job MeleeAttackJob(Thing enemyTarget)
		{
			Job job = base.MeleeAttackJob(enemyTarget);
			job.attackDoorIfTargetLost = true;
			return job;
		}
	}
}
