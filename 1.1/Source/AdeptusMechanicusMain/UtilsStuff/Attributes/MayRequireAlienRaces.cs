using System;
using Verse;

namespace RimWorld
{
	// Token: 0x02000FE2 RID: 4066
	[AttributeUsage(AttributeTargets.All)]
	public class MayRequireAlienRaces : MayRequireAttribute
	{
		// Token: 0x060064E7 RID: 25831 RVA: 0x0023288A File Offset: 0x00230A8A
		public MayRequireAlienRaces() : base("erdelf.HumanoidAlienRaces")
		{
		}
	}
}
