using System;
using AdeptusMechanicus.ExtensionMethods;
using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
	// AdeptusMechanicus.HediffCompProperties_Irradiator
	public class HediffCompProperties_Irradiator : HediffCompProperties
	{
		// Token: 0x060010C6 RID: 4294 RVA: 0x0005FBCE File Offset: 0x0005DDCE
		public HediffCompProperties_Irradiator()
		{
			this.compClass = typeof(HediffComp_Irradiator);
		}

		// Token: 0x04000C4F RID: 3151
		public float infectionChance = 0.5f;
	}

	public class HediffComp_Irradiator : HediffComp
	{
		// Token: 0x17000352 RID: 850
		// (get) Token: 0x060010C7 RID: 4295 RVA: 0x0005FBF1 File Offset: 0x0005DDF1
		public HediffCompProperties_Irradiator Props
		{
			get
			{
				return (HediffCompProperties_Irradiator)this.props;
			}
		}

		// Token: 0x060010C8 RID: 4296 RVA: 0x0005FC00 File Offset: 0x0005DE00
		public override void CompPostPostAdd(DamageInfo? dinfo)
		{
			/*
			if (this.parent.IsPermanent())
			{
				this.ticksUntilIrradiate = -2;
				return;
			}
			if (this.parent.Part.def.IsSolid(this.parent.Part, base.Pawn.health.hediffSet.hediffs))
			{
				this.ticksUntilIrradiate = -2;
				return;
			}
			if (base.Pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(this.parent.Part))
			{
				this.ticksUntilIrradiate = -2;
				return;
			}
			*/
			float num = this.Props.infectionChance;
			/*
			if (base.Pawn.RaceProps.Animal)
			{
				num *= 0.1f;
			}
			*/
			/*
			if (Rand.Value <= num)
			{
				this.ticksUntilInfect = HealthTuning.InfectionDelayRange.RandomInRange;
				return;
			}
			*/
			this.ticksUntilIrradiate = HealthTuning.InfectionDelayRange.RandomInRange;
			return;
		//	this.ticksUntilIrradiate = -2;
		}

		// Token: 0x060010C9 RID: 4297 RVA: 0x0005FCD8 File Offset: 0x0005DED8
		public override void CompExposeData()
		{
			Scribe_Values.Look<float>(ref this.irradiationChanceFactorFromTendRoom, "infectionChanceFactor", 0f, false);
			Scribe_Values.Look<int>(ref this.ticksUntilIrradiate, "ticksUntilInfect", -2, false);
		}

		// Token: 0x060010CA RID: 4298 RVA: 0x0005FD03 File Offset: 0x0005DF03
		public override void CompPostTick(ref float severityAdjustment)
		{
			if (this.ticksUntilIrradiate > 0)
			{
				this.ticksUntilIrradiate--;
				if (this.ticksUntilIrradiate == 0)
				{
					this.CheckMakeInfection();
					this.ticksUntilIrradiate = HealthTuning.InfectionDelayRange.RandomInRange;
				}
			}
		}

		
		// Token: 0x060010CB RID: 4299 RVA: 0x0005FD2C File Offset: 0x0005DF2C
		public override void CompTended_NewTemp(float quality, float maxQuality, int batchPosition = 0)
		{
			base.CompTended_NewTemp(quality, maxQuality, batchPosition);

			if (base.Pawn.Spawned)
			{
				Room room = base.Pawn.GetRoom(RegionType.Set_Passable);
				if (room != null)
				{
					this.irradiationChanceFactorFromTendRoom = room.GetStat(RoomStatDefOf.InfectionChanceFactor);
				}
			}

		}
		

		// Token: 0x060010CC RID: 4300 RVA: 0x0005FD70 File Offset: 0x0005DF70
		private void CheckMakeInfection()
		{
			/*
			if (base.Pawn.health.immunity.DiseaseContractChanceFactor(HediffDefOf.WoundInfection, this.parent.Part) <= 0.001f)
			{
				this.ticksUntilInfect = -3;
				return;
			}*/
			float num = 1f;
			HediffComp_TendDuration hediffComp_TendDuration = this.parent.TryGetCompFast<HediffComp_TendDuration>();
			if (hediffComp_TendDuration != null && hediffComp_TendDuration.IsTended)
			{
			//	num *= this.infectionChanceFactorFromTendRoom;
				num *= HediffComp_Irradiator.IrradiationChanceFactorFromTendQualityCurve.Evaluate(hediffComp_TendDuration.tendQuality);
			}
			num *= HediffComp_Irradiator.IrradiationChanceFactorFromSeverityCurve.Evaluate(this.parent.Severity);
			
			if (base.Pawn.Faction == Faction.OfPlayer)
			{
				num *= Find.Storyteller.difficultyValues.playerPawnInfectionChanceFactor;
			}
			
			if (Rand.Value < num)
			{
				this.ticksUntilIrradiate = -4;
				HediffWithComps hediff = base.Pawn.health.hediffSet.GetFirstHediffOfDef(OGHediffDefOf.OG_Hediff_RadiationPoisioning) as HediffWithComps;
				if (hediff == null)
				{
					hediff = HediffMaker.MakeHediff(OGHediffDefOf.OG_Hediff_RadiationPoisioning, base.Pawn) as HediffWithComps;
					base.Pawn.health.AddHediff(hediff);
				}
				hediff.Severity += this.parent.Severity / 100;
				return;
			}
			this.ticksUntilIrradiate = -3;
		}

		// Token: 0x060010CD RID: 4301 RVA: 0x0005FE6C File Offset: 0x0005E06C
		public override string CompDebugString()
		{
			if (this.ticksUntilIrradiate > 0)
			{
				return string.Concat(new object[]
				{
					"irradiation may appear in: ",
					this.ticksUntilIrradiate
					/*
					,
					" ticks\ninfectChnceFactorFromTendRoom: ",
					this.infectionChanceFactorFromTendRoom.ToStringPercent()
					*/
				});
			}
			if (this.ticksUntilIrradiate == -4)
			{
				return "already created irratiation";
			}
			if (this.ticksUntilIrradiate == -3)
			{
				return "failed to make irratiation";
			}
			if (this.ticksUntilIrradiate == -2)
			{
				return "will not make irratiation";
			}
			if (this.ticksUntilIrradiate == -1)
			{
				return "uninitialized data!";
			}
			return "unexpected ticksUntilInfect = " + this.ticksUntilIrradiate;
		}

		public static readonly IntRange InfectionDelayRange = new IntRange(2500, 25000);

		// Token: 0x04000C50 RID: 3152
		private int ticksUntilIrradiate = -1;

		// Token: 0x04000C51 RID: 3153
		private float irradiationChanceFactorFromTendRoom = 1f;


		// Token: 0x04000C56 RID: 3158
		private static readonly SimpleCurve IrradiationChanceFactorFromTendQualityCurve = new SimpleCurve
		{
			{
				new CurvePoint(0f, 0.7f),
				true
			},
			{
				new CurvePoint(1f, 0.4f),
				true
			}
		};

		// Token: 0x04000C57 RID: 3159
		private static readonly SimpleCurve IrradiationChanceFactorFromSeverityCurve = new SimpleCurve
		{
			{
				new CurvePoint(1f, 0.1f),
				true
			},
			{
				new CurvePoint(12f, 1f),
				true
			}
		};
	}
}
