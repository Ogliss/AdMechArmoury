using System;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x0200018F RID: 399
	public class LordToil_DefendAndExpandHiveLike : LordToil_HiveLikeRelated
	{
		// Token: 0x06000860 RID: 2144 RVA: 0x00047694 File Offset: 0x00045A94
		public override void UpdateAllDuties()
		{
			base.FilterOutUnspawnedHiveLikes();
			for (int i = 0; i < this.lord.ownedPawns.Count; i++)
			{
				HiveLike hiveFor = (HiveLike)base.GetHiveLikeFor(this.lord.ownedPawns[i]);
				PawnDuty duty = new PawnDuty(DutyDefOf.DefendAndExpandHive, hiveFor, this.distToHiveToAttack);
				this.lord.ownedPawns[i].mindState.duty = duty;
			}
		}

		// Token: 0x04000395 RID: 917
		public float distToHiveToAttack = 10f;
	}
}
