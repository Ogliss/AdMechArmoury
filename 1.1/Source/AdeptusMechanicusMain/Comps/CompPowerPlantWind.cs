using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
	// AdeptusMechanicus.CompPowerPlantWind
	[StaticConstructorOnStartup]
    public class CompPowerPlantWind : CompPowerPlant
	{
		// Token: 0x17000B9A RID: 2970
		// (get) Token: 0x060041C3 RID: 16835 RVA: 0x0015DB08 File Offset: 0x0015BD08
		protected override float DesiredPowerOutput
		{
			get
			{
				return this.cachedPowerOutput;
			}
		}

		// Token: 0x17000B9B RID: 2971
		// (get) Token: 0x060041C4 RID: 16836 RVA: 0x0015DB10 File Offset: 0x0015BD10
		protected float PowerPercent
		{
			get
			{
				return base.PowerOutput / (-base.Props.basePowerConsumption * 1.5f);
			}
		}

		// Token: 0x060041C5 RID: 16837 RVA: 0x0015DB2C File Offset: 0x0015BD2C
		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			CompPowerPlantWind.BarSize = new Vector2((float)this.parent.def.size.z - 0.95f, 0.14f);
			this.RecalculateBlockages();
			this.spinPosition = Rand.Range(0f, 15f);
		}

		// Token: 0x060041C6 RID: 16838 RVA: 0x0015DB86 File Offset: 0x0015BD86
		public override void PostDeSpawn(Map map)
		{
			base.PostDeSpawn(map);
			if (this.sustainer != null && !this.sustainer.Ended)
			{
				this.sustainer.End();
			}
		}

		// Token: 0x060041C7 RID: 16839 RVA: 0x0015DBAF File Offset: 0x0015BDAF
		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look<int>(ref this.ticksSinceWeatherUpdate, "updateCounter", 0, false);
			Scribe_Values.Look<float>(ref this.cachedPowerOutput, "cachedPowerOutput", 0f, false);
		}

		// Token: 0x060041C8 RID: 16840 RVA: 0x0015DBE0 File Offset: 0x0015BDE0
		public override void CompTick()
		{
			base.CompTick();
			if (!base.PowerOn)
			{
				this.cachedPowerOutput = 0f;
				return;
			}
			this.ticksSinceWeatherUpdate++;
			if (this.ticksSinceWeatherUpdate >= this.updateWeatherEveryXTicks)
			{
				float num = Mathf.Min(this.parent.Map.windManager.WindSpeed, 1.5f);
				this.ticksSinceWeatherUpdate = 0;
				this.cachedPowerOutput = -(base.Props.basePowerConsumption * num);
				this.RecalculateBlockages();
				if (this.windPathBlockedCells.Count > 0)
				{
					float num2 = 0f;
					for (int i = 0; i < this.windPathBlockedCells.Count; i++)
					{
						num2 += this.cachedPowerOutput * 0.2f;
					}
					this.cachedPowerOutput -= num2;
					if (this.cachedPowerOutput < 0f)
					{
						this.cachedPowerOutput = 0f;
					}
				}
			}
			if (this.cachedPowerOutput > 0.01f)
			{
				this.spinPosition += this.PowerPercent * CompPowerPlantWind.SpinRateFactor;
			}
			if (this.sustainer == null || this.sustainer.Ended)
			{
				this.sustainer = SoundDefOf.WindTurbine_Ambience.TrySpawnSustainer(SoundInfo.InMap(this.parent, MaintenanceType.None));
			}
			this.sustainer.Maintain();
			this.sustainer.externalParams["PowerOutput"] = this.PowerPercent;
		}

		// Token: 0x060041C9 RID: 16841 RVA: 0x0015DD48 File Offset: 0x0015BF48
		public override void PostDraw()
		{
			base.PostDraw();
			GenDraw.FillableBarRequest r = new GenDraw.FillableBarRequest
			{
				center = this.parent.DrawPos + Vector3.up * 0.1f,
				size = CompPowerPlantWind.BarSize,
				fillPercent = this.PowerPercent,
				filledMat = CompPowerPlantWind.WindTurbineBarFilledMat,
				unfilledMat = CompPowerPlantWind.WindTurbineBarUnfilledMat,
				margin = 0.15f
			};
			Rot4 rotation = this.parent.Rotation;
			rotation.Rotate(RotationDirection.Clockwise);
			r.rotation = rotation;
			GenDraw.DrawFillableBar(r);
			Vector3 vector = this.parent.TrueCenter();
			vector += this.parent.Rotation.FacingCell.ToVector3() * CompPowerPlantWind.VerticalBladeOffset;
			vector += this.parent.Rotation.RighthandCell.ToVector3() * CompPowerPlantWind.HorizontalBladeOffset;
			vector.y += 0.042857144f;
			float num = CompPowerPlantWind.BladeWidth * Mathf.Sin(this.spinPosition);
			if (num < 0f)
			{
				num *= -1f;
			}
			bool flag = this.spinPosition % 3.1415927f * 2f < 3.1415927f;
			Vector2 vector2 = new Vector2(num, 1f);
			Vector3 s = new Vector3(vector2.x, 1f, vector2.y);
			Matrix4x4 matrix = default(Matrix4x4);
			matrix.SetTRS(vector, this.parent.Rotation.AsQuat, s);
			Graphics.DrawMesh(flag ? MeshPool.plane10 : MeshPool.plane10Flip, matrix, CompPowerPlantWind.WindTurbineBladesMat, 0);
			vector.y -= 0.08571429f;
			matrix.SetTRS(vector, this.parent.Rotation.AsQuat, s);
			Graphics.DrawMesh(flag ? MeshPool.plane10Flip : MeshPool.plane10, matrix, CompPowerPlantWind.WindTurbineBladesMat, 0);
		}

		// Token: 0x060041CA RID: 16842 RVA: 0x0015DF50 File Offset: 0x0015C150
		public override string CompInspectStringExtra()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(base.CompInspectStringExtra());
			if (this.windPathBlockedCells.Count > 0)
			{
				stringBuilder.AppendLine();
				Thing thing = null;
				if (this.windPathBlockedByThings != null)
				{
					thing = this.windPathBlockedByThings[0];
				}
				if (thing != null)
				{
					stringBuilder.Append("WindTurbine_WindPathIsBlockedBy".Translate() + " " + thing.Label);
				}
				else
				{
					stringBuilder.Append("WindTurbine_WindPathIsBlockedByRoof".Translate());
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060041CB RID: 16843 RVA: 0x0015DFE8 File Offset: 0x0015C1E8
		protected void RecalculateBlockages()
		{
			if (this.windPathCells.Count == 0)
			{
				IEnumerable<IntVec3> collection = WindTurbineUtility.CalculateWindCells(this.parent.Position, this.parent.Rotation, this.parent.def.size);
				this.windPathCells.AddRange(collection);
			}
			this.windPathBlockedCells.Clear();
			this.windPathBlockedByThings.Clear();
			for (int i = 0; i < this.windPathCells.Count; i++)
			{
				IntVec3 intVec = this.windPathCells[i];
				if (this.parent.Map.roofGrid.Roofed(intVec))
				{
					this.windPathBlockedByThings.Add(null);
					this.windPathBlockedCells.Add(intVec);
				}
				else
				{
					List<Thing> list = this.parent.Map.thingGrid.ThingsListAt(intVec);
					for (int j = 0; j < list.Count; j++)
					{
						Thing thing = list[j];
						if (thing.def.blockWind)
						{
							this.windPathBlockedByThings.Add(thing);
							this.windPathBlockedCells.Add(intVec);
							break;
						}
					}
				}
			}
		}

		// Token: 0x040026C0 RID: 9920
		public int updateWeatherEveryXTicks = 250;

		// Token: 0x040026C1 RID: 9921
		protected int ticksSinceWeatherUpdate;

		// Token: 0x040026C2 RID: 9922
		protected float cachedPowerOutput;

		// Token: 0x040026C3 RID: 9923
		protected List<IntVec3> windPathCells = new List<IntVec3>();

		// Token: 0x040026C4 RID: 9924
		protected List<Thing> windPathBlockedByThings = new List<Thing>();

		// Token: 0x040026C5 RID: 9925
		protected List<IntVec3> windPathBlockedCells = new List<IntVec3>();

		// Token: 0x040026C6 RID: 9926
		protected float spinPosition;

		// Token: 0x040026C7 RID: 9927
		protected Sustainer sustainer;

		// Token: 0x040026C8 RID: 9928
		protected const float MaxUsableWindIntensity = 1.5f;

		// Token: 0x040026C9 RID: 9929
		[TweakValue("Graphics", 0f, 0.1f)]
		protected static float SpinRateFactor = 0.035f;

		// Token: 0x040026CA RID: 9930
		[TweakValue("Graphics", -1f, 1f)]
		protected static float HorizontalBladeOffset = -0.02f;

		// Token: 0x040026CB RID: 9931
		[TweakValue("Graphics", 0f, 1f)]
		protected static float VerticalBladeOffset = 0.7f;

		// Token: 0x040026CC RID: 9932
		[TweakValue("Graphics", 4f, 8f)]
		protected static float BladeWidth = 6.6f;

		// Token: 0x040026CD RID: 9933
		protected const float PowerReductionPercentPerObstacle = 0.2f;

		// Token: 0x040026CE RID: 9934
		protected const string TranslateWindPathIsBlockedBy = "WindTurbine_WindPathIsBlockedBy";

		// Token: 0x040026CF RID: 9935
		protected const string TranslateWindPathIsBlockedByRoof = "WindTurbine_WindPathIsBlockedByRoof";

		// Token: 0x040026D0 RID: 9936
		protected static Vector2 BarSize;

		// Token: 0x040026D1 RID: 9937
		protected static readonly Material WindTurbineBarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.5f, 0.475f, 0.1f), false);

		// Token: 0x040026D2 RID: 9938
		protected static readonly Material WindTurbineBarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.15f, 0.15f, 0.15f), false);

		// Token: 0x040026D3 RID: 9939
		protected static readonly Material WindTurbineBladesMat = MaterialPool.MatFrom("Things/Building/Eldar/Power/WindTurbine/WindTurbineBlades",true);
	}
}
