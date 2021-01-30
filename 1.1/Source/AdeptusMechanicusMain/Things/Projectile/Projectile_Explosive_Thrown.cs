using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
	// AdeptusMechanicus.Projectile_Explosive_Thrown
	[StaticConstructorOnStartup]
	class Projectile_Explosive_Thrown : Projectile_Explosive
    {
		private int rotinc = 0;
		private int rotrate = Rand.Range(-3, 3);
		private int heightinc = Rand.Range(-1, 3);
		// Token: 0x06001651 RID: 5713 RVA: 0x00081EDC File Offset: 0x000800DC
		public override void Draw()
		{
			float num = this.ArcHeightFactor * GenMath.InverseParabola(this.DistanceCoveredFraction);
			Vector3 drawPos = this.DrawPos;

			Vector3 position = drawPos + new Vector3(0f, 0f, 1f) * num;
			if (this.ticksToImpact > 0 && !Find.TickManager.Paused)
			{
				this.rotinc += rotrate;
			}
			if (this.def.projectile.shadowSize > 0f)
			{
				this.DrawShadow(drawPos, num);
			}
			Graphics.DrawMesh(MeshPool.GridPlane(this.def.graphicData.drawSize), position, this.ExactRotation, this.Graphic.MatSingleFor(this), 0);
			base.Comps_PostDraw();
		}
		public override Quaternion ExactRotation
		{
			get
			{
				// Time.deltaTime

				return Quaternion.LookRotation((this.destination - this.origin).Yto0().RotatedBy(rotinc));
			}
		}
		private float ArcHeightFactor
		{
			get
			{
				float num = this.def.projectile.arcHeightFactor + heightinc;
				float num2 = (this.destination - this.origin).MagnitudeHorizontalSquared();
				if (num * num > num2 * 0.2f * 0.2f)
				{
					num = Mathf.Sqrt(num2) * 0.2f;
				}
				return num;
			}
		}
		private void DrawShadow(Vector3 drawLoc, float height)
		{
			if (Projectile_Explosive_Thrown.shadowMaterial == null)
			{
				return;
			}
			float num = this.def.projectile.shadowSize * Mathf.Lerp(1f, 0.6f, height);
			Vector3 s = new Vector3(num, 1f, num);
			Vector3 b = new Vector3(0f, -0.01f, 0f);
			Matrix4x4 matrix = default(Matrix4x4);
			matrix.SetTRS(drawLoc + b, Quaternion.identity, s);
			Graphics.DrawMesh(MeshPool.plane10, matrix, Projectile_Explosive_Thrown.shadowMaterial, 0);
		}
		private static readonly Material shadowMaterial = MaterialPool.MatFrom("Things/Skyfaller/SkyfallerShadowCircle", ShaderDatabase.Transparent);
	}
}
