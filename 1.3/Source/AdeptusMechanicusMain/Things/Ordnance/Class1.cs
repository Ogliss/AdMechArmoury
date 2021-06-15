using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Noise;
using Verse.Sound;

namespace AdeptusMechanicus.OrbitalStrikes
{
	// AdeptusMechanicus.OrbitalStrikes.GlassingBeam
	[StaticConstructorOnStartup]
	public class GlassingBeam : ThingWithComps
	{
		protected Mote powerBeamMote;
		private RimWorld.CompOrbitalBeam beam;
		public RimWorld.CompOrbitalBeam Beam
		{
			get
			{
				if (beam == null)
				{
					beam = base.GetComp<RimWorld.CompOrbitalBeam>();
				}
				return beam;
			}
		}
		private float FadeInOutFactor
		{
			get
			{
				float a = Mathf.Clamp01((float)(Find.TickManager.TicksGame - this.spawnTick) / 120f);
				float b = (this.leftFadeOutTicks < 0) ? 1f : Mathf.Min((float)this.leftFadeOutTicks / 120f, 1f);
				return Mathf.Min(a, b);
			}
		}

		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
			if (!respawningAfterLoad)
			{
				Vector3 vector = base.Position.ToVector3Shifted();
				this.realPosition = new Vector2(vector.x, vector.z);
				this.direction = Rand.Range(0f, 360f);
				this.spawnTick = Find.TickManager.TicksGame;
				this.leftFadeOutTicks = -1;
				this.ticksLeftToDisappear = GlassingBeam.DurationTicks.RandomInRange;
				IntVec3 effectLoc = new IntVec3((int)realPosition.x, 0, (int)realPosition.y);
				CompAffectsSky comp = base.GetComp<CompAffectsSky>();
				if (comp != null)
				{
					comp.StartFadeInHoldFadeOut(30, this.ticksLeftToDisappear - 30 - 15, 15, 1f);
				}
				CompOrbitalBeam comp2 = base.GetComp<CompOrbitalBeam>();
				if (comp2 != null)
				{
					if (this.beamAngle == 0)
					{
						this.beamAngle = GlassingBeam.AngleRange.RandomInRange;
					}
					comp2.StartAnimation(this.ticksLeftToDisappear, 10, this.beamAngle);
				}
			}
			this.CreateSustainer();
		}

		public override void Tick()
		{
			if (base.Spawned)
			{
				if (this.sustainer == null)
				{
					Log.Error("Tornado sustainer is null.", false);
					this.CreateSustainer();
				}
				this.sustainer.Maintain();
				this.UpdateSustainerVolume();

			//	base.GetComp<CompWindSource>().wind = 5f * this.FadeInOutFactor;
				if (this.leftFadeOutTicks > 0)
				{
					this.leftFadeOutTicks--;
					if (this.leftFadeOutTicks == 0)
					{
						this.Destroy(DestroyMode.Vanish);
						return;
					}
				}
				else
				{
					if (GlassingBeam.directionNoise == null)
					{
						GlassingBeam.directionNoise = new Perlin(0.0020000000949949026, 2.0, 0.5, 4, 1948573612, QualityMode.Medium);
					}
					this.direction += (float)GlassingBeam.directionNoise.GetValue((double)Find.TickManager.TicksAbs, (double)((float)(this.thingIDNumber % 500) * 1000f), 0.0) * 0.78f;
					this.realPosition = this.realPosition.Moved(this.direction, 0.0283333343f);
					IntVec3 intVec = new Vector3(this.realPosition.x, 0f, this.realPosition.y).ToIntVec3();
					if (intVec.InBounds(base.Map))
					{
						base.Position = intVec;
						if (powerBeamMote == null)
						{
							powerBeamMote = MakePowerBeamMote(base.Position, base.Map, beamWidth * 3f, this.ticksLeftToDisappear.TicksToSeconds());
						}
						powerBeamMote.exactPosition = DrawPos;
						powerBeamMote.spawnTick = Find.TickManager.TicksGame-60;
						for (int i = 0; i < 4; i++)
						{
							this.StartRandomFireAndDoFlameDamage();
						}
						if (this.IsHashIntervalTick(15))
						{
							this.DamageCloseThings();
						}
						if (Rand.MTBEventOccurs(15f, 1f, 1f))
						{
							this.DamageFarThings();
						}
						if (this.IsHashIntervalTick(20))
						{
							this.DestroyRoofs();
						}
						if (this.ticksLeftToDisappear > 0)
						{
							this.ticksLeftToDisappear--;
							if (this.ticksLeftToDisappear == 0)
							{
								this.leftFadeOutTicks = 120;
								Messages.Message("MessageTornadoDissipated".Translate(), new TargetInfo(base.Position, base.Map, false), MessageTypeDefOf.PositiveEvent, true);
							}
						}
						if (this.IsHashIntervalTick(4) && !this.CellImmuneToDamage(base.Position))
						{
							float num = Rand.Range(0.6f, 1f);
							FleckMaker.ThrowTornadoDustPuff(new Vector3(this.realPosition.x, 0f, this.realPosition.y)
							{
								y = AltitudeLayer.MoteOverhead.AltitudeFor()
							} + Vector3Utility.RandomHorizontalOffset(1.5f), base.Map, Rand.Range(1.5f, 3f), new Color(num, num, num));
							return;
						}
					}
					else
					{
						this.leftFadeOutTicks = 120;
						Messages.Message("MessageTornadoLeftMap".Translate(), new TargetInfo(base.Position, base.Map, false), MessageTypeDefOf.PositiveEvent, true);
					}
				}
			}
		}


        /*
		public bool HitRoof(IntVec3 c)
		{
			RoofDef roofDef = base.Map.roofGrid.RoofAt(c);
			if (roofDef != null)
			{
				if (roofDef.isThickRoof)
				{
					if (Rand.Chance(strikeDef.roofThickCollapseChance))
					{
						if (c.GetEdifice(base.Map) == null || c.GetEdifice(base.Map).def.Fillage != FillCategory.Full)
						{
							RoofCollapserImmediate.DropRoofInCells(c, base.Map, null);
						}
					}
					else
					{
						if (this.weaponDef?.projectile?.soundHitThickRoof != null)
						{
							this.weaponDef.projectile.soundHitThickRoof.PlayOneShot(new TargetInfo(c, base.Map, false));
						}
						return true;
					}
				}
				else
				{
					if (Rand.Chance(strikeDef.roofThinCollapseChance))
					{
						if (c.GetEdifice(base.Map) == null || c.GetEdifice(base.Map).def.Fillage != FillCategory.Full)
						{
							RoofCollapserImmediate.DropRoofInCells(c, base.Map, null);
						}
					}
					else
					{
						if (this.weaponDef?.projectile?.soundHitThickRoof != null)
						{
							this.weaponDef.projectile.soundHitThickRoof.PlayOneShot(new TargetInfo(c, base.Map, false));
						}
						return true;
					}
				}
			}
			return false;
		}
		*/
        public override Vector3 DrawPos => new Vector3(realPosition.x, 0, realPosition.y);
        public override void Draw()
		{
			/*
			Rand.PushState();
			Rand.Seed = this.thingIDNumber;
			for (int i = 0; i < 180; i++)
			{
				this.DrawTornadoPart(GlassingBeam.PartsDistanceFromCenter.RandomInRange, Rand.Range(0f, 360f), Rand.Range(0.9f, 1.1f), Rand.Range(0.52f, 0.88f));
			}
			Rand.PopState();
			*/
			base.Comps_PostDraw();
		}
		/*
		// distanceFromCenter controls the size
		private void DrawTornadoPart(float distanceFromCenter, float initialAngle, float speedMultiplier, float colorMultiplier)
		{
			int ticksGame = Find.TickManager.TicksGame;
			float num = 1f / distanceFromCenter;
			float num2 = 25f * speedMultiplier * num;
			float num3 = (initialAngle + (float)ticksGame * num2) % 360f;
			Vector2 vector = this.realPosition.Moved(num3, this.AdjustedDistanceFromCenter(distanceFromCenter));
			// 
			vector.y += distanceFromCenter * 4f;
			vector.y += GlassingBeam.ZOffsetBias;

			Vector3 a = new Vector3(vector.x, AltitudeLayer.Weather.AltitudeFor() + 0.042857144f * Rand.Range(0f, 1f), vector.y);
			float num4 = distanceFromCenter * 3f;
			float num5 = 1f;
			
			if (num3 > 270f)
			{
				num5 = GenMath.LerpDouble(270f, 360f, 0f, 1f, num3);
			}
			else if (num3 > 180f)
			{
				num5 = GenMath.LerpDouble(180f, 270f, 1f, 0f, num3);
			}
			
			float num6 = Mathf.Min(distanceFromCenter / (GlassingBeam.PartsDistanceFromCenter.max + 2f), 1f);
			float d = Mathf.InverseLerp(0.18f, 0.4f, num6);
			Vector3 a2 = new Vector3(Mathf.Sin((float)ticksGame / 1000f + (float)(this.thingIDNumber * 10)) * 2f, 0f, 0f);
			Vector3 pos = a + a2 * d;
			float a3 = Mathf.Max(1f - num6, 0f) * num5 * this.FadeInOutFactor;
			Color value = new Color(Beam.Props.color.r, Beam.Props.color.g, Beam.Props.color.b, a3);
			GlassingBeam.matPropertyBlock.SetColor(ShaderPropertyIDs.Color, value);
			Matrix4x4 matrix = Matrix4x4.TRS(pos, Quaternion.Euler(0f, num3, 0f), new Vector3(num4, 1f, num4));
			Graphics.DrawMesh(MeshPool.plane10, matrix, GlassingBeam.TornadoMaterial, 0, null, 0, GlassingBeam.matPropertyBlock);
		}
		*/
		private float AdjustedDistanceFromCenter(float distanceFromCenter)
		{
			float num = Mathf.Min(distanceFromCenter / 8f, 1f);
			num *= num;
			return distanceFromCenter * num;
		}
		public static Mote MakePowerBeamMote(IntVec3 cell, Map map, float scale, float duration)
		{
			Mote mote = (Mote)ThingMaker.MakeThing(ThingDefOf.Mote_PowerBeam, null);
			mote.exactPosition = cell.ToVector3Shifted();
			mote.Scale = scale;
			mote.rotationRate = 1.2f;
			mote.solidTimeOverride = duration;
			GenSpawn.Spawn(mote, cell, map, WipeMode.Vanish);
			return mote;
		}

		private void UpdateSustainerVolume()
		{
			this.sustainer.info.volumeFactor = this.FadeInOutFactor;
		}

		private void CreateSustainer()
		{
			LongEventHandler.ExecuteWhenFinished(delegate
			{
				SoundDef tornado = SoundDefOf.OrbitalBeam;
				this.sustainer = tornado.TrySpawnSustainer(SoundInfo.InMap(this, MaintenanceType.PerTick));
				this.UpdateSustainerVolume();
			});
		}

		private void DamageCloseThings()
		{
			int num = GenRadial.NumCellsInRadius(beamWidth / 2 + 2.5f);
			for (int i = 0; i < num; i++)
			{
				IntVec3 intVec = base.Position + GenRadial.RadialPattern[i];
				if (intVec.InBounds(base.Map) && !this.CellImmuneToDamage(intVec))
				{
					Pawn firstPawn = intVec.GetFirstPawn(base.Map);
					if (firstPawn == null || !firstPawn.Downed || !Rand.Bool)
					{
						float damageFactor = GenMath.LerpDouble(0f, 4.2f, 1f, 0.2f, intVec.DistanceTo(base.Position));
						this.DoDamage(intVec, damageFactor);
					}
				}
			}
		}

		private void DamageFarThings()
		{
			IntVec3 c = (from x in GenRadial.RadialCellsAround(base.Position, beamWidth/2 + 5f, true)
						 where x.InBounds(base.Map)
						 select x).RandomElement<IntVec3>();
			if (this.CellImmuneToDamage(c))
			{
				return;
			}
			this.DoDamage(c, 0.5f);
		}

		protected void StartRandomFireAndDoFlameDamage()
		{
			float effectRange = beamWidth / 2 + 7f;
			IntVec3 effectLoc = DrawPos.ToIntVec3();
			IntVec3 c = (from x in GenRadial.RadialCellsAround(effectLoc, effectRange, true)
						 where x.InBounds(base.Map)
						 select x).RandomElementByWeight((IntVec3 x) => 1f - Mathf.Min(x.DistanceTo(effectLoc) / effectRange, 1f) + 0.05f);
			FireUtility.TryStartFireIn(c, base.Map, Rand.Range(0.1f, 0.925f));
			GlassingBeam.tmpThings.Clear();
			GlassingBeam.tmpThings.AddRange(c.GetThingList(base.Map));
			for (int i = 0; i < GlassingBeam.tmpThings.Count; i++)
			{
				int num = (GlassingBeam.tmpThings[i] is Corpse) ? GlassingBeam.CorpseFlameDamageAmountRange.RandomInRange : GlassingBeam.FlameDamageAmountRange.RandomInRange;
				Pawn pawn = GlassingBeam.tmpThings[i] as Pawn;
				BattleLogEntry_DamageTaken battleLogEntry_DamageTaken = null;
				if (pawn != null)
				{
					battleLogEntry_DamageTaken = new BattleLogEntry_DamageTaken(pawn, RulePackDefOf.DamageEvent_PowerBeam, this.instigator as Pawn);
					Find.BattleLog.Add(battleLogEntry_DamageTaken);
				}
				GlassingBeam.tmpThings[i].TakeDamage(new DamageInfo(DamageDefOf.Flame, (float)num, 0, -1f, this.instigator, null, this.weaponDef, DamageInfo.SourceCategory.ThingOrUnknown, null)).AssociateWithLog(battleLogEntry_DamageTaken);
			}
			GlassingBeam.tmpThings.Clear();
		}

		private void DestroyRoofs()
		{
			this.removedRoofsTmp.Clear();
			foreach (IntVec3 intVec in from x in GenRadial.RadialCellsAround(base.Position, beamWidth * 0.75f, true)
									   where x.InBounds(base.Map)
									   select x)
			{
				if (!this.CellImmuneToDamage(intVec) && intVec.Roofed(base.Map))
				{
					RoofDef roof = intVec.GetRoof(base.Map);
					if (!roof.isThickRoof && !roof.isNatural)
					{
						RoofCollapserImmediate.DropRoofInCells(intVec, base.Map, null);
						this.removedRoofsTmp.Add(intVec);
					}
				}
			}
			if (this.removedRoofsTmp.Count > 0)
			{
				RoofCollapseCellsFinder.CheckCollapseFlyingRoofs(this.removedRoofsTmp, base.Map, true, false);
			}
		}

		private bool CellImmuneToDamage(IntVec3 c)
		{
			if (c.Roofed(base.Map) && c.GetRoof(base.Map).isThickRoof)
			{
				return true;
			}
			Building edifice = c.GetEdifice(base.Map);
			return edifice != null && edifice.def.category == ThingCategory.Building && (edifice.def.building.isNaturalRock || (edifice.def == ThingDefOf.Wall && edifice.Faction == null));
		}

		private void DoDamage(IntVec3 c, float damageFactor)
		{
			GlassingBeam.tmpThings.Clear();
			GlassingBeam.tmpThings.AddRange(c.GetThingList(base.Map));
			Vector3 vector = c.ToVector3Shifted();
			Vector2 b = new Vector2(vector.x, vector.z);
			float angle = -this.realPosition.AngleTo(b) + 180f;
			for (int i = 0; i < GlassingBeam.tmpThings.Count; i++)
			{
				BattleLogEntry_DamageTaken battleLogEntry_DamageTaken = null;
				switch (GlassingBeam.tmpThings[i].def.category)
				{
					case ThingCategory.Pawn:
						{
							Pawn pawn = (Pawn)GlassingBeam.tmpThings[i];
							battleLogEntry_DamageTaken = new BattleLogEntry_DamageTaken(pawn, RulePackDefOf.DamageEvent_Tornado, null);
							Find.BattleLog.Add(battleLogEntry_DamageTaken);
							if (pawn.RaceProps.baseHealthScale < 1f)
							{
								damageFactor *= pawn.RaceProps.baseHealthScale;
							}
							if (pawn.RaceProps.Animal)
							{
								damageFactor *= 0.75f;
							}
							if (pawn.Downed)
							{
								damageFactor *= 0.2f;
							}
							break;
						}
					case ThingCategory.Item:
						damageFactor *= 0.68f;
						break;
					case ThingCategory.Building:
						damageFactor *= 0.8f;
						break;
					case ThingCategory.Plant:
						damageFactor *= 1.7f;
						break;
				}
				int num = Mathf.Max(GenMath.RoundRandom(30f * damageFactor), 1);
				GlassingBeam.tmpThings[i].TakeDamage(new DamageInfo(DamageDefOf.TornadoScratch, (float)num, 0f, angle, this, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null)).AssociateWithLog(battleLogEntry_DamageTaken);
			}
			GlassingBeam.tmpThings.Clear();
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<Vector2>(ref this.realPosition, "realPosition", default(Vector2), false);
			Scribe_Values.Look<float>(ref this.direction, "direction", 0f, false);
			Scribe_Values.Look<int>(ref this.spawnTick, "spawnTick", 0, false);
			Scribe_Values.Look<int>(ref this.leftFadeOutTicks, "leftFadeOutTicks", 0, false);
			Scribe_Values.Look<int>(ref this.ticksLeftToDisappear, "ticksLeftToDisappear", 0, false);
			Scribe_Values.Look<float>(ref this.beamWidth, "beamWidth", 5f, false);
			Scribe_Values.Look<float>(ref this.beamAngle, "beamAngle", 180f, false);
			Scribe_Values.Look<ThingDef>(ref this.weaponDef, "weaponDef", null, false);
			Scribe_References.Look<Thing>(ref this.instigator, "instigator", false);
		}

		public float beamWidth = 5f;
		public float beamAngle = 0f;
		public ThingDef weaponDef;
		public Thing instigator;
		private Vector2 realPosition;
		private float direction;
		private int spawnTick;
		private int leftFadeOutTicks = -1;
		private int ticksLeftToDisappear = -1;
		private Sustainer sustainer;
		private static MaterialPropertyBlock matPropertyBlock = new MaterialPropertyBlock();
		private static ModuleBase directionNoise;
		private const float Wind = 5f;
		private const int CloseDamageIntervalTicks = 15;
		private const int RoofDestructionIntervalTicks = 20;
		private const float FarDamageMTBTicks = 15f;
		private const float CloseDamageRadius = 4.2f;
		private const float FarDamageRadius = 10f;
		private const float BaseDamage = 30f;
		private const int SpawnMoteEveryTicks = 4;
		private static readonly IntRange DurationTicks = new IntRange(2700, 10080);
		private const float DownedPawnDamageFactor = 0.2f;
		private const float AnimalPawnDamageFactor = 0.75f;
		private const float BuildingDamageFactor = 0.8f;
		private const float PlantDamageFactor = 1.7f;
		private const float ItemDamageFactor = 0.68f;
		private const float CellsPerSecond = 1.7f;
		private const float DirectionChangeSpeed = 0.78f;
		private const float DirectionNoiseFrequency = 0.002f;
		private const float TornadoAnimationSpeed = 25f;
		private const float ThreeDimensionalEffectStrength = 4f;
		private const int FadeInTicks = 120;
		private const int FadeOutTicks = 120;
		private const float MaxMidOffset = 2f;
		private IntVec3 nextTargetCell = IntVec3.Invalid;
		private static readonly IntRange FlameDamageAmountRange = new IntRange(65, 100);
		private static readonly IntRange CorpseFlameDamageAmountRange = new IntRange(5, 10);
		private static readonly Material TornadoMaterial = MaterialPool.MatFrom("Things/Ethereal/Tornado", ShaderDatabase.Transparent, MapMaterialRenderQueues.Tornado);
		private static readonly FloatRange PartsDistanceFromCenter = new FloatRange(1f, 10f);
		private static readonly FloatRange AngleRange = new FloatRange(-12f, 12f);
		private static readonly float ZOffsetBias = -4f * GlassingBeam.PartsDistanceFromCenter.min;
		private List<IntVec3> removedRoofsTmp = new List<IntVec3>();
		private static List<Thing> tmpThings = new List<Thing>();
	}
}
