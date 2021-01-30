using RimWorld;
using System;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
	// AdeptusMechanicus.CompOrbitalBeam
	[StaticConstructorOnStartup]
    public class CompOrbitalBeam2 : ThingComp
	{
		// Token: 0x17000F15 RID: 3861
		// (get) Token: 0x060055FB RID: 22011 RVA: 0x001CCAEA File Offset: 0x001CACEA
		public CompProperties_OrbitalBeam Props
		{
			get
			{
				return (CompProperties_OrbitalBeam)this.props;
			}
		}

		// Token: 0x17000F16 RID: 3862
		// (get) Token: 0x060055FC RID: 22012 RVA: 0x001CCAF7 File Offset: 0x001CACF7
		private int TicksPassed
		{
			get
			{
				return Find.TickManager.TicksGame - this.startTick;
			}
		}

		// Token: 0x17000F17 RID: 3863
		// (get) Token: 0x060055FD RID: 22013 RVA: 0x001CCB0A File Offset: 0x001CAD0A
		private int TicksLeft
		{
			get
			{
				return this.totalDuration - this.TicksPassed;
			}
		}

		// Token: 0x17000F18 RID: 3864
		// (get) Token: 0x060055FE RID: 22014 RVA: 0x001CCB19 File Offset: 0x001CAD19
		private float BeamEndHeight
		{
			get
			{
				return this.Props.width * 0.5f;
			}
		}

		// Token: 0x060055FF RID: 22015 RVA: 0x001CCB2C File Offset: 0x001CAD2C
		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look<int>(ref this.startTick, "startTick", 0, false);
			Scribe_Values.Look<int>(ref this.totalDuration, "totalDuration", 0, false);
			Scribe_Values.Look<int>(ref this.fadeOutDuration, "fadeOutDuration", 0, false);
			Scribe_Values.Look<float>(ref this.angle, "angle", 0f, false);
		}

		// Token: 0x06005600 RID: 22016 RVA: 0x001CCB8B File Offset: 0x001CAD8B
		public void StartAnimation(int totalDuration, int fadeOutDuration, float angle)
		{
			this.startTick = Find.TickManager.TicksGame;
			this.totalDuration = totalDuration;
			this.fadeOutDuration = fadeOutDuration;
			this.angle = angle;
			this.CheckSpawnSustainer();
		}

		// Token: 0x06005601 RID: 22017 RVA: 0x001CCBB8 File Offset: 0x001CADB8
		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			this.CheckSpawnSustainer();
		}

		// Token: 0x06005602 RID: 22018 RVA: 0x001CCBC7 File Offset: 0x001CADC7
		public override void CompTick()
		{
			base.CompTick();
			if (this.sustainer != null)
			{
				this.sustainer.Maintain();
				if (this.TicksLeft < this.fadeOutDuration)
				{
					this.sustainer.End();
					this.sustainer = null;
				}
			}
		}

		// Token: 0x06005603 RID: 22019 RVA: 0x001CCC04 File Offset: 0x001CAE04
		public override void PostDraw()
		{
			base.PostDraw();
			if (this.TicksLeft <= 0)
			{
				return;
			}
			Vector3 drawPos = this.parent.DrawPos;
			float num = ((float)this.parent.Map.Size.z - drawPos.z) * 1.41421354f;
			Vector3 a = Vector3Utility.FromAngleFlat(this.angle - 90f);
			Vector3 a2 = drawPos + a * num * 0.5f;
			a2.y = AltitudeLayer.MetaOverlays.AltitudeFor();
			float num2 = Mathf.Min((float)this.TicksPassed / 10f, 1f);
			Vector3 b = a * ((1f - num2) * num);
			float num3 = 0.975f + Mathf.Sin((float)this.TicksPassed * 0.3f) * 0.025f;
			if (this.TicksLeft < this.fadeOutDuration)
			{
				num3 *= (float)this.TicksLeft / (float)this.fadeOutDuration;
			}
			Color color = this.Props.color;
			color.a *= num3;
			CompOrbitalBeam2.MatPropertyBlock.SetColor(ShaderPropertyIDs.Color, color);
			Matrix4x4 matrix = default(Matrix4x4);
			matrix.SetTRS(a2 + a * this.BeamEndHeight * 0.5f + b, Quaternion.Euler(0f, this.angle, 0f), new Vector3(this.Props.width, 1f, num));
			Graphics.DrawMesh(MeshPool.plane10, matrix, CompOrbitalBeam2.BeamMat, 0, null, 0, CompOrbitalBeam2.MatPropertyBlock);
			Vector3 pos = drawPos + b;
			pos.y = AltitudeLayer.MetaOverlays.AltitudeFor();
			Matrix4x4 matrix2 = default(Matrix4x4);
			matrix2.SetTRS(pos, Quaternion.Euler(0f, this.angle, 0f), new Vector3(this.Props.width, 1f, this.BeamEndHeight));
			Graphics.DrawMesh(MeshPool.plane10, matrix2, CompOrbitalBeam2.BeamEndMat, 0, null, 0, CompOrbitalBeam2.MatPropertyBlock);
		}

		// Token: 0x06005604 RID: 22020 RVA: 0x001CCE09 File Offset: 0x001CB009
		private void CheckSpawnSustainer()
		{
			if (this.TicksLeft >= this.fadeOutDuration && this.Props.sound != null)
			{
				LongEventHandler.ExecuteWhenFinished(delegate
				{
					this.sustainer = this.Props.sound.TrySpawnSustainer(SoundInfo.InMap(this.parent, MaintenanceType.PerTick));
				});
			}
		}

		// Token: 0x04002FAF RID: 12207
		private int startTick;

		// Token: 0x04002FB0 RID: 12208
		private int totalDuration;

		// Token: 0x04002FB1 RID: 12209
		private int fadeOutDuration;

		// Token: 0x04002FB2 RID: 12210
		private float angle;

		// Token: 0x04002FB3 RID: 12211
		private Sustainer sustainer;

		// Token: 0x04002FB4 RID: 12212
		private const float AlphaAnimationSpeed = 0.3f;

		// Token: 0x04002FB5 RID: 12213
		private const float AlphaAnimationStrength = 0.025f;

		// Token: 0x04002FB6 RID: 12214
		private const float BeamEndHeightRatio = 0.5f;

		// Token: 0x04002FB7 RID: 12215
		private static readonly Material BeamMat = MaterialPool.MatFrom("Other/OrbitalBeam", ShaderDatabase.MoteGlow, MapMaterialRenderQueues.OrbitalBeam);

		// Token: 0x04002FB8 RID: 12216
		private static readonly Material BeamEndMat = MaterialPool.MatFrom("Other/OrbitalBeamEnd", ShaderDatabase.MoteGlow, MapMaterialRenderQueues.OrbitalBeam);

		// Token: 0x04002FB9 RID: 12217
		private static readonly MaterialPropertyBlock MatPropertyBlock = new MaterialPropertyBlock();
	}

}
