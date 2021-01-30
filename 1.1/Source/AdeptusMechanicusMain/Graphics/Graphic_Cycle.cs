using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
	// Token: 0x020002E9 RID: 745
	public class Graphic_Cycle : Graphic_Collection
	{
		// Token: 0x17000448 RID: 1096
		// (get) Token: 0x0600151A RID: 5402 RVA: 0x0007BCD4 File Offset: 0x00079ED4
		public override Material MatSingle
		{
			get
			{
				Rand.PushState();
				int i = Rand.Range(0, this.subGraphics.Length);
				Rand.PopState();
				return this.subGraphics[i].MatSingle;
			}
		}
		public Graphic[] graphics => this.subGraphics;
		// Token: 0x0600151B RID: 5403 RVA: 0x0007C0A8 File Offset: 0x0007A2A8
		public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
		{
			if (thingDef == null)
			{
				Log.ErrorOnce("Fire DrawWorker with null thingDef: " + loc, 3427324, false);
				return;
			}
			if (this.subGraphics == null)
			{
				Log.ErrorOnce("Graphic_Flicker has no subgraphics " + thingDef, 358773632, false);
				return;
			}
			int num = Find.TickManager.TicksGame;
			if (thing != null)
			{
				num += Mathf.Abs(thing.thingIDNumber ^ 8453458);
			}
			int num2 = num / 15;
			int num3 = Mathf.Abs(num2 ^ ((thing != null) ? thing.thingIDNumber : 0) * 391) % this.subGraphics.Length;
			float num4 = 1f;
			CompProperties_FireOverlay compProperties_FireOverlay = null;
			Fire fire = thing as Fire;
			if (fire != null)
			{
				num4 = fire.fireSize;
			}
			else if (thingDef != null)
			{
				compProperties_FireOverlay = thingDef.GetCompProperties<CompProperties_FireOverlay>();
				if (compProperties_FireOverlay != null)
				{
					num4 = compProperties_FireOverlay.fireSize;
				}
			}
			if (num3 < 0 || num3 >= this.subGraphics.Length)
			{
				Log.ErrorOnce("Fire drawing out of range: " + num3, 7453435, false);
				num3 = 0;
			}
			Graphic graphic = this.subGraphics[num3];
			float num5 = Mathf.Min(num4 / 1.2f, 1.2f);
			Vector3 a = GenRadial.RadialPattern[num2 % GenRadial.RadialPattern.Length].ToVector3() / GenRadial.MaxRadialPatternRadius;
			a *= 0.05f;
			Vector3 vector = loc + a * num4;
			if (compProperties_FireOverlay != null)
			{
				vector += compProperties_FireOverlay.offset;
			}
			Vector3 s = new Vector3(num5, 1f, num5);
			Matrix4x4 matrix = default(Matrix4x4);
			matrix.SetTRS(vector, Quaternion.identity, s);
			Graphics.DrawMesh(MeshPool.plane10, matrix, graphic.MatSingle, 0);
		}

		// Token: 0x0600151C RID: 5404 RVA: 0x0007C254 File Offset: 0x0007A454
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"Flicker(subGraphic[0]=",
				this.subGraphics[0].ToString(),
				", count=",
				this.subGraphics.Length,
				")"
			});
		}

		// Token: 0x04000DF2 RID: 3570
		private const int BaseTicksPerFrameChange = 15;

		// Token: 0x04000DF3 RID: 3571
		private const int ExtraTicksPerFrameChange = 10;

		// Token: 0x04000DF4 RID: 3572
		private const float MaxOffset = 0.05f;
	}
}
