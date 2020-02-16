using System;
using System.Collections.Generic;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
	// Token: 0x02000003 RID: 3
	public class CompUniversalFermenter : ThingComp
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000003 RID: 3 RVA: 0x000020AF File Offset: 0x000002AF
		public CompProperties_UniversalFermenter Props
		{
			get
			{
				return (CompProperties_UniversalFermenter)this.props;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000004 RID: 4 RVA: 0x000020BC File Offset: 0x000002BC
		private int ResourceListSize
		{
			get
			{
				return this.Props.products.Count;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000005 RID: 5 RVA: 0x000020CE File Offset: 0x000002CE
		public UniversalFermenterProduct Product
		{
			get
			{
				return this.Props.products[this.currentResourceInd];
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000006 RID: 6 RVA: 0x000020E6 File Offset: 0x000002E6
		public UniversalFermenterProduct NextProduct
		{
			get
			{
				return this.Props.products[this.nextResourceInd];
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000007 RID: 7 RVA: 0x000020FE File Offset: 0x000002FE
		public bool Ruined
		{
			get
			{
				return this.ruinedPercent >= 1f;
			}
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002110 File Offset: 0x00000310
		public override void ReceiveCompSignal(string signal)
		{
			if (signal == "RuinedByTemperature")
			{
				this.Reset();
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000009 RID: 9 RVA: 0x00002128 File Offset: 0x00000328
		public string SummaryAddedIngredients
		{
			get
			{
				int num = 60;
				string text = "";
				for (int i = 0; i < this.ingredientLabels.Count; i++)
				{
					if (i == 0)
					{
						text += this.ingredientLabels[i];
					}
					else
					{
						text = text + ", " + this.ingredientLabels[i];
					}
				}
				int length = string.Concat(new string[]
				{
					"Contains ",
					this.Product.maxCapacity.ToString(),
					"/",
					this.Product.maxCapacity.ToString(),
					" "
				}).Length;
				int limit = num - length;
				return Utils.VowelTrim(text, limit);
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600000A RID: 10 RVA: 0x000021E5 File Offset: 0x000003E5
		public string SummaryNextIngredientFilter
		{
			get
			{
				return Utils.IngredientFilterSummary(this.NextProduct.ingredientFilter);
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600000B RID: 11 RVA: 0x000021F7 File Offset: 0x000003F7
		// (set) Token: 0x0600000C RID: 12 RVA: 0x000021FF File Offset: 0x000003FF
		public float Progress
		{
			get
			{
				return this.progressInt;
			}
			set
			{
				if (value == this.progressInt)
				{
					return;
				}
				this.progressInt = value;
				this.barFilledCachedMat = null;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600000D RID: 13 RVA: 0x00002219 File Offset: 0x00000419
		private Material BarFilledMat
		{
			get
			{
				if (this.barFilledCachedMat == null)
				{
					this.barFilledCachedMat = SolidColorMaterials.SimpleSolidColorMaterial(Color.Lerp(Static_Bar.ZeroProgressColor, Static_Bar.FermentedColor, this.Progress), false);
				}
				return this.barFilledCachedMat;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600000E RID: 14 RVA: 0x00002250 File Offset: 0x00000450
		private bool Empty
		{
			get
			{
				return this.ingredientCount <= 0;
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600000F RID: 15 RVA: 0x0000225E File Offset: 0x0000045E
		public bool Fermented
		{
			get
			{
				return !this.Empty && this.Progress >= 1f;
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000010 RID: 16 RVA: 0x0000227A File Offset: 0x0000047A
		public int SpaceLeftForIngredient
		{
			get
			{
				if (this.Fermented)
				{
					return 0;
				}
				return this.Product.maxCapacity - this.ingredientCount;
			}
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002298 File Offset: 0x00000498
		private void NextResource()
		{
			this.nextResourceInd++;
			if (this.nextResourceInd >= this.ResourceListSize)
			{
				this.nextResourceInd = 0;
			}
			if (this.Empty)
			{
				this.currentResourceInd = this.nextResourceInd;
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000012 RID: 18 RVA: 0x000022D4 File Offset: 0x000004D4
		private float CurrentTempProgressSpeedFactor
		{
			get
			{
				float ambientTemperature = this.parent.AmbientTemperature;
				if (ambientTemperature < this.Product.temperatureSafe.min)
				{
					return this.Product.speedLessThanSafe;
				}
				if (ambientTemperature > this.Product.temperatureSafe.max)
				{
					return this.Product.speedMoreThanSafe;
				}
				if (ambientTemperature < this.Product.temperatureIdeal.min)
				{
					return GenMath.LerpDouble(this.Product.temperatureSafe.min, this.Product.temperatureIdeal.min, this.Product.speedLessThanSafe, 1f, ambientTemperature);
				}
				if (ambientTemperature > this.Product.temperatureIdeal.max)
				{
					return GenMath.LerpDouble(this.Product.temperatureIdeal.max, this.Product.temperatureSafe.max, 1f, this.Product.speedMoreThanSafe, ambientTemperature);
				}
				return 1f;
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000013 RID: 19 RVA: 0x000023C4 File Offset: 0x000005C4
		private float ProgressPerTickAtCurrentTemp
		{
			get
			{
				return 1f / (float)this.Product.baseFermentationDuration * this.CurrentTempProgressSpeedFactor;
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000014 RID: 20 RVA: 0x000023DF File Offset: 0x000005DF
		private int EstimatedTicksLeft
		{
			get
			{
				return Mathf.Max(Mathf.RoundToInt((1f - this.Progress) / this.ProgressPerTickAtCurrentTemp), 0);
			}
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002400 File Offset: 0x00000600
		public override void PostExposeData()
		{
			Scribe_Values.Look<float>(ref this.ruinedPercent, "ruinedPercent", 0f, false);
			Scribe_Values.Look<int>(ref this.ingredientCount, "VG_UniversalFermenter_IngredientCount", 0, false);
			Scribe_Values.Look<float>(ref this.progressInt, "VG_UniversalFermenter_Progress", 0f, false);
			Scribe_Values.Look<int>(ref this.nextResourceInd, "VG_nextResourceInd", 0, false);
			Scribe_Values.Look<int>(ref this.currentResourceInd, "VG_currentResourceInd", 0, false);
			Scribe_Collections.Look<string>(ref this.ingredientLabels, "VG_ingredientLabels", 0, new object[0]);
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002488 File Offset: 0x00000688
		public override void PostDraw()
		{
			base.PostDraw();
			if (!this.Empty)
			{
				Vector3 drawPos = this.parent.DrawPos;
				drawPos.y += 0.0483870953f;
				drawPos.z += 0.25f;
				GenDraw.FillableBarRequest fillableBarRequest = default(GenDraw.FillableBarRequest);
				fillableBarRequest.center = drawPos;
				fillableBarRequest.size = Static_Bar.Size;
				fillableBarRequest.fillPercent = (float)(this.ingredientCount / this.Product.maxCapacity);
				fillableBarRequest.filledMat = this.BarFilledMat;
				fillableBarRequest.unfilledMat = Static_Bar.UnfilledMat;
				fillableBarRequest.margin = 0.1f;
				fillableBarRequest.rotation = Rot4.North;
				GenDraw.DrawFillableBar(fillableBarRequest);
			}
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002540 File Offset: 0x00000740
		public bool AddIngredient(Thing ingredient)
		{
			if (!this.Product.ingredientFilter.Allows(ingredient))
			{
				return false;
			}
			if (!this.ingredientLabels.Contains(ingredient.def.label))
			{
				this.ingredientLabels.Add(ingredient.def.label);
			}
			this.AddIngredient(ingredient.stackCount);
			ingredient.Destroy(0);
			return true;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000025A4 File Offset: 0x000007A4
		public void AddIngredient(int count)
		{
			this.ruinedPercent = 0f;
			if (this.Fermented)
			{
				Log.Warning("Fermenter:: Tried to add ingredient to a fermenter full of product. Colonists should take the product first.", false);
				return;
			}
			int num = Mathf.Min(count, this.Product.maxCapacity - this.ingredientCount);
			if (num <= 0)
			{
				return;
			}
			this.Progress = GenMath.WeightedAverage(0f, (float)num, this.Progress, (float)this.ingredientCount);
			this.ingredientCount += num;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x0000261C File Offset: 0x0000081C
		public Thing TakeOutProduct()
		{
			if (!this.Fermented)
			{
				Log.Warning("Fermenter:: Tried to get product but it's not yet fermented.", false);
				return null;
			}
			Thing thing = ThingMaker.MakeThing(this.Product.thingDef, null);
			thing.stackCount = Mathf.RoundToInt((float)this.ingredientCount * this.Product.efficiency);
			this.Reset();
			return thing;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002675 File Offset: 0x00000875
		public void Reset()
		{
			this.ingredientCount = 0;
			this.Progress = 0f;
			this.currentResourceInd = this.nextResourceInd;
			this.ingredientLabels.Clear();
		}

		// Token: 0x0600001B RID: 27 RVA: 0x000026A0 File Offset: 0x000008A0
		public override void CompTick()
		{
			this.DoTicks(1);
		}

		// Token: 0x0600001C RID: 28 RVA: 0x000026A9 File Offset: 0x000008A9
		public override void CompTickRare()
		{
			if (!this.Empty)
			{
				this.Progress = Mathf.Min(this.Progress + 250f * this.ProgressPerTickAtCurrentTemp, 1f);
			}
			this.DoTicks(250);
		}

		// Token: 0x0600001D RID: 29 RVA: 0x000026E4 File Offset: 0x000008E4
		private void DoTicks(int ticks)
		{
			if (!this.Ruined)
			{
				float ambientTemperature = this.parent.AmbientTemperature;
				if (ambientTemperature > this.Product.temperatureSafe.max)
				{
					this.ruinedPercent += (ambientTemperature - this.Product.temperatureSafe.max) * this.Product.progressPerDegreePerTick * (float)ticks;
				}
				else if (ambientTemperature < this.Product.temperatureSafe.min)
				{
					this.ruinedPercent -= (ambientTemperature - this.Product.temperatureSafe.min) * this.Product.progressPerDegreePerTick * (float)ticks;
				}
				if (this.ruinedPercent >= 1f)
				{
					this.ruinedPercent = 1f;
					this.Reset();
					return;
				}
				if (this.ruinedPercent < 0f)
				{
					this.ruinedPercent = 0f;
				}
			}
		}

		// Token: 0x0600001E RID: 30 RVA: 0x000027C4 File Offset: 0x000009C4
		public override void PreAbsorbStack(Thing otherStack, int count)
		{
			float t = (float)count / (float)(this.parent.stackCount + count);
			CompUniversalFermenter comp = ((ThingWithComps)otherStack).GetComp<CompUniversalFermenter>();
			this.ruinedPercent = Mathf.Lerp(this.ruinedPercent, comp.ruinedPercent, t);
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002808 File Offset: 0x00000A08
		public override bool AllowStackWith(Thing other)
		{
			CompUniversalFermenter comp = ((ThingWithComps)other).GetComp<CompUniversalFermenter>();
			return this.Ruined == comp.Ruined;
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002830 File Offset: 0x00000A30
		public override void PostSplitOff(Thing piece)
		{
			CompUniversalFermenter comp = ((ThingWithComps)piece).GetComp<CompUniversalFermenter>();
			comp.ruinedPercent = this.ruinedPercent;
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002855 File Offset: 0x00000A55
		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (Prefs.DevMode && !this.Empty)
			{
				Command_Action command_Action = new Command_Action
				{
					defaultLabel = "DEBUG: Finish",
					activateSound = SoundDef.Named("Click"),
					action = delegate()
					{
						this.Progress = 1f;
					}
				};
				yield return command_Action;
			}
			foreach (Gizmo gizmo in base.CompGetGizmosExtra())
			{
				yield return gizmo;
			}
			IEnumerator<Gizmo> enumerator = null;
			if (this.ResourceListSize > 1)
			{
				Command_Action command_Action2 = new Command_Action
				{
					defaultLabel = this.NextProduct.thingDef.label,
					defaultDesc = string.Concat(new string[]
					{
						"Produce ",
						this.NextProduct.thingDef.label,
						" from ",
						this.SummaryNextIngredientFilter,
						"."
					}),
					activateSound = SoundDef.Named("Click"),
					icon = Utils.GetIcon(this.NextProduct.thingDef),
					action = delegate()
					{
						this.NextResource();
					}
				};
				yield return command_Action2;
			}
			yield break;
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002868 File Offset: 0x00000A68
		public override string CompInspectStringExtra()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(this.StatusInfo());
			if (!this.Empty && !this.Ruined)
			{
				if (this.Fermented)
				{
					stringBuilder.AppendLine(TranslatorFormattedStringExtensions.Translate("VG_ContainsProduct", this.ingredientCount, this.Product.maxCapacity, this.Product.thingDef.label));
				}
				else
				{
					stringBuilder.AppendLine(TranslatorFormattedStringExtensions.Translate("VG_ContainsIngredient", this.ingredientCount, this.Product.maxCapacity, this.SummaryAddedIngredients));
				}
			}
			if (!this.Empty)
			{
				if (this.Fermented)
				{
					stringBuilder.AppendLine(Translator.Translate("VG_Finished"));
				}
				else
				{
					stringBuilder.AppendLine(TranslatorFormattedStringExtensions.Translate("FermentationProgress", GenText.ToStringPercent(this.Progress), GenDate.ToStringTicksToPeriod(this.EstimatedTicksLeft)));
					if (this.CurrentTempProgressSpeedFactor != 1f)
					{
						stringBuilder.AppendLine(TranslatorFormattedStringExtensions.Translate("FermentationBarrelOutOfIdealTemperature", GenText.ToStringPercent(this.CurrentTempProgressSpeedFactor)));
					}
				}
			}
			stringBuilder.AppendLine(string.Concat(new string[]
			{
				Translator.Translate("VG_IdealSafeFermentingTemperature"),
				": ",
				GenText.ToStringTemperature(this.Product.temperatureIdeal.min, "F0"),
				"~",
				GenText.ToStringTemperature(this.Product.temperatureIdeal.max, "F0"),
				" (",
				GenText.ToStringTemperature(this.Product.temperatureSafe.min, "F0"),
				"~",
				GenText.ToStringTemperature(this.Product.temperatureSafe.max, "F0"),
				")"
			}));
			return GenText.TrimEndNewlines(stringBuilder.ToString());
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002A6C File Offset: 0x00000C6C
		public string StatusInfo()
		{
			if (this.Ruined)
			{
				return Translator.Translate("RuinedByTemperature");
			}
			float ambientTemperature = this.parent.AmbientTemperature;
			string text = null;
			string text2 = Translator.Translate("Temperature") + ": " + GenText.ToStringTemperature(this.parent.AmbientTemperature, "F0");
			if (this.Product.temperatureSafe.Includes(ambientTemperature))
			{
				if (this.Product.temperatureIdeal.Includes(ambientTemperature))
				{
					text = Translator.Translate("VG_Ideal");
				}
				else
				{
					text = Translator.Translate("VG_Safe");
				}
			}
			else if (this.ruinedPercent > 0f)
			{
				if (ambientTemperature < this.Product.temperatureSafe.min)
				{
					text = Translator.Translate("Freezing");
				}
				else
				{
					text = Translator.Translate("Overheating");
				}
				text = text + " " + GenText.ToStringPercent(this.ruinedPercent);
			}
			if (text == null)
			{
				return text2;
			}
			return text2 + " (" + text + ")";
		}

		// Token: 0x04000002 RID: 2
		private int ingredientCount;

		// Token: 0x04000003 RID: 3
		private float progressInt;

		// Token: 0x04000004 RID: 4
		private Material barFilledCachedMat;

		// Token: 0x04000005 RID: 5
		private int nextResourceInd;

		// Token: 0x04000006 RID: 6
		private int currentResourceInd;

		// Token: 0x04000007 RID: 7
		private List<string> ingredientLabels = new List<string>();

		// Token: 0x04000008 RID: 8
		protected float ruinedPercent;

		// Token: 0x04000009 RID: 9
		public const string RuinedSignal = "RuinedByTemperature";
	}
}
