using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus.AirStrikes
{
	// Token: 0x02000045 RID: 69
	public class AirStrikeDef : Def
	{
		public GraphicData graphicData = new GraphicData();
		public GraphicData shadowData = new GraphicData();

		public Vector2 scale = new Vector2(1f,1f);
		// Token: 0x040000D3 RID: 211
		public const int maxWeapons = 3;

		// Token: 0x040000D4 RID: 212
		public int runsNumber = 1;

		// Token: 0x040000D5 RID: 213
		public int costInSilver = 500;

		// Token: 0x040000D6 RID: 214
		public float ammoResupplyDays = 1f;

		// Token: 0x040000D7 RID: 215
		public float cellsTravelledPerTick = 0.25f;

		// Token: 0x040000D8 RID: 216
		public int ticksBeforeOverflightInitialValue = 600;

		// Token: 0x040000D9 RID: 217
		public int ticksBeforeOverflightPlaySound = 240;

		// Token: 0x040000DA RID: 218
		public int ticksBeforeOverflightReducedSpeed = 240;

		// Token: 0x040000DB RID: 219
		public int ticksAfterOverflightReducedSpeed = 0;

		// Token: 0x040000DC RID: 220
		public int ticksAfterOverflightFinalValue = 600;

		// Token: 0x040000DD RID: 221
		public List<WeaponDef> weapons = new List<WeaponDef>();
	}
}
