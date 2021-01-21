using System;
using Verse;

namespace AdeptusMechanicus.AirStrikes
{
	// Token: 0x0200003D RID: 61
	public class WeaponDef : Def
	{
		// Token: 0x04000085 RID: 133
		public ThingDef ammoDef = null;

		// Token: 0x04000086 RID: 134
		public SoundDef soundCastDef = null;

		// Token: 0x04000087 RID: 135
		public int ammoQuantity = 0;

		// Token: 0x04000088 RID: 136
		public int ticksBetweenShots = 0;

		// Token: 0x04000089 RID: 137
		public float startShootingDistance = 0f;

		// Token: 0x0400008A RID: 138
		public float ammoTravelDistance = 0f;

		// Token: 0x0400008B RID: 139
		public float ammoDispersion = 0f;

		// Token: 0x0400008C RID: 140
		public float targetAcquireRange = 0f;

		// Token: 0x0400008D RID: 141
		public bool isTwinGun = true;

		// Token: 0x0400008E RID: 142
		public float horizontalPositionOffset = 0f;

		// Token: 0x0400008F RID: 143
		public float verticalPositionOffset = 0f;

		// Token: 0x04000090 RID: 144
		public int disableRainDurationInTicks = 0;
	}
}
