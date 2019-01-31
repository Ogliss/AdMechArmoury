using System;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x02000007 RID: 7
    [StaticConstructorOnStartup]
	public class OgDef
	{

		// Token: 0x0400000A RID: 10
		public static ThingDef GrenadePack = ThingDef.Named("MedicalKit");

		// Token: 0x04000010 RID: 16
		public static JobDef BandageOthers = DefDatabase<JobDef>.GetNamed("BandageOthers", true);
	}
}
