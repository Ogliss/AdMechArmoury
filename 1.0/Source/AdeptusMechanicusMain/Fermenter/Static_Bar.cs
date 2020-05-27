using System;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
	// Token: 0x02000006 RID: 6
	[StaticConstructorOnStartup]
	public static class Static_Bar
	{
		// Token: 0x04000011 RID: 17
		public static readonly Vector2 Size = new Vector2(0.55f, 0.1f);

		// Token: 0x04000012 RID: 18
		public static readonly Color ZeroProgressColor = new Color(0.4f, 0.27f, 0.22f);

		// Token: 0x04000013 RID: 19
		public static readonly Color FermentedColor = new Color(0.9f, 0.85f, 0.2f);

		// Token: 0x04000014 RID: 20
		public static readonly Material UnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.3f, 0.3f, 0.3f), false);
	}
}
