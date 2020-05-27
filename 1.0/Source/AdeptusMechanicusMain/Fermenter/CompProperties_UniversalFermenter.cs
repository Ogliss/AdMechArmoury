using System;
using System.Collections.Generic;
using Verse;

namespace AdeptusMechanicus
{
	// Token: 0x02000002 RID: 2
	public class CompProperties_UniversalFermenter : CompProperties
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public CompProperties_UniversalFermenter()
		{
			this.compClass = typeof(CompUniversalFermenter);
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002074 File Offset: 0x00000274
		public override void ResolveReferences(ThingDef parentDef)
		{
			base.ResolveReferences(parentDef);
			for (int i = 0; i < this.products.Count; i++)
			{
				this.products[i].ResolveReferences();
			}
		}

		// Token: 0x04000001 RID: 1
		public List<UniversalFermenterProduct> products = new List<UniversalFermenterProduct>();
	}
}
