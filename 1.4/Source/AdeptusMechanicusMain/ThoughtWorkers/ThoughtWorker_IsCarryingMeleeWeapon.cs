using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;

namespace AdeptusMechanicus
{
	// Token: 0x02000866 RID: 2150 AdeptusMechanicus.ThoughtWorker_IsCarryingMeleeWeapon
	public class ThoughtWorker_IsCarryingMeleeWeapon : ThoughtWorker
	{
		// Token: 0x060035C1 RID: 13761 RVA: 0x00126595 File Offset: 0x00124795
		public override ThoughtState CurrentStateInternal(Pawn p)
		{
			return p.equipment.Primary != null && p.equipment.Primary.def.IsMeleeWeapon;
		}
	}
}
