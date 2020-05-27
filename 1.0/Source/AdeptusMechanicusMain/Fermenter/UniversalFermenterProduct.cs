using System;
using Verse;

namespace AdeptusMechanicus
{
	// Token: 0x02000007 RID: 7
	public class UniversalFermenterProduct
	{
		// Token: 0x06000033 RID: 51 RVA: 0x00002D06 File Offset: 0x00000F06
		public void ResolveReferences()
		{
			this.ingredientFilter.ResolveReferences();
		}

		// Token: 0x04000015 RID: 21
		public ThingDef thingDef;

		// Token: 0x04000016 RID: 22
		public ThingFilter ingredientFilter = new ThingFilter();

		// Token: 0x04000017 RID: 23
		public FloatRange temperatureSafe = new FloatRange(-1f, 32f);

		// Token: 0x04000018 RID: 24
		public FloatRange temperatureIdeal = new FloatRange(7f, 32f);

		// Token: 0x04000019 RID: 25
		public float progressPerDegreePerTick = 1E-05f;

		// Token: 0x0400001A RID: 26
		public int baseFermentationDuration = 600000;

		// Token: 0x0400001B RID: 27
		public int maxCapacity = 25;

		// Token: 0x0400001C RID: 28
		public float speedLessThanSafe = 0.1f;

		// Token: 0x0400001D RID: 29
		public float speedMoreThanSafe = 1f;

		// Token: 0x0400001E RID: 30
		public float efficiency = 1f;
	}
}
