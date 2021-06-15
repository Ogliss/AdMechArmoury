using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
	public class HediffCompProperties_DetonateOnDeath : HediffCompProperties
	{
		public HediffCompProperties_DetonateOnDeath()
		{
			this.compClass = typeof(HediffComp_DetonateOnDeath);
		}

		public FleckDef fleck;
		public float explosiveRadius = 1.9f;
		public DamageDef explosiveDamageType;
		public int damageAmountBase = -1;
		public float armorPenetrationBase = -1f;
		public ThingDef postExplosionSpawnThingDef;
		public float postExplosionSpawnChance;
		public int postExplosionSpawnThingCount = 1;
		public bool applyDamageToExplosionCellsNeighbors;
		public ThingDef preExplosionSpawnThingDef;
		public float preExplosionSpawnChance;
		public int preExplosionSpawnThingCount = 1;
		public float chanceToStartFire;
		public bool damageFalloff;
		public bool explodeOnKilled;
		public float explosiveExpandPerSeverity;
		public float explosiveExpandPerFuel;
		public EffecterDef explosionEffect;
		public SoundDef explosionSound;
		public List<DamageDef> startWickOnDamageTaken;
		public float startWickHitPointsPercent = 0.2f;
		public IntRange wickTicks = new IntRange(140, 150);
		public float wickScale = 1f;
		public float chanceNeverExplodeFromDamage;
		public float destroyThingOnExplosionSize;
		public DamageDef requiredDamageTypeToExplode;
		public IntRange? countdownTicks;
		public string extraInspectStringKey;
		public ThingDef mote;
		public int moteCount = 3;
		public FloatRange moteOffsetRange = new FloatRange(0.2f, 0.4f);
		public ThingDef filth;
		public int filthCount = 4;
		public HediffDef injuryCreatedOnDeath;
		public IntRange injuryCount;
		public SoundDef sound;
	}
	public class HediffComp_DetonateOnDeath : HediffComp
	{
		public HediffCompProperties_DetonateOnDeath Props
		{
			get
			{
				return (HediffCompProperties_DetonateOnDeath)this.props;
			}
		}

		public override void Notify_PawnDied()
		{
			base.Notify_PawnDied();
			if (this.Props.injuryCreatedOnDeath != null)
			{
				List<BodyPartRecord> list = new List<BodyPartRecord>(from part in base.Pawn.RaceProps.body.AllParts
																	 where part.coverageAbs > 0f && !base.Pawn.health.hediffSet.PartIsMissing(part)
																	 select part);
				int num = Mathf.Min(this.Props.injuryCount.RandomInRange, list.Count);
				for (int i = 0; i < num; i++)
				{
					int index = Rand.Range(0, list.Count);
					BodyPartRecord part2 = list[index];
					list.RemoveAt(index);
					base.Pawn.health.AddHediff(this.Props.injuryCreatedOnDeath, part2, null, null);
				}
			}
			if (!base.Pawn.Spawned)
			{
				return;
			}
			Detonate(base.Pawn.Map, false, false);
		}

		protected int StartWickThreshold
		{
			get
			{
				return Mathf.RoundToInt(this.Props.startWickHitPointsPercent * (float)this.parent.pawn.MaxHitPoints);
			}
		}

		private bool CanEverExplodeFromDamage
		{
			get
			{
				if (this.Props.chanceNeverExplodeFromDamage < 1E-05f)
				{
					return true;
				}
				Rand.PushState();
				Rand.Seed = this.parent.pawn.thingIDNumber.GetHashCode();
				bool result = Rand.Value < this.Props.chanceNeverExplodeFromDamage;
				Rand.PopState();
				return result;
			}
		}
		private bool CanExplodeFromDamageType(DamageDef damage)
		{
			return this.Props.requiredDamageTypeToExplode == null || this.Props.requiredDamageTypeToExplode == damage;
		}
		public override void Notify_PawnPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.Notify_PawnPostApplyDamage(dinfo, totalDamageDealt);
			if (!this.CanEverExplodeFromDamage)
			{
				return;
			}
			if (!this.CanExplodeFromDamageType(dinfo.Def))
			{
				return;
			}
			if (!this.parent.pawn.Destroyed)
			{
				if (this.wickStarted && dinfo.Def == DamageDefOf.Stun)
				{
					this.StopWick();
					return;
				}
				if (!this.wickStarted && this.parent.pawn.HitPoints <= this.StartWickThreshold && dinfo.Def.ExternalViolenceFor(this.parent.pawn))
				{
					this.StartWick(dinfo.Instigator);
				}
			}
		}

        public override void Notify_PawnKilled()
		{
			base.Pawn.equipment.DestroyAllEquipment(DestroyMode.Vanish);
			base.Pawn.apparel.DestroyAll(DestroyMode.Vanish);
			bool flag = !base.Pawn.Spawned;
			if (!flag)
			{
				bool flag2 = this.Props.mote != null || this.Props.fleck != null;
				if (flag2)
				{
					Vector3 pos = base.Pawn.DrawPos;
					for (int i = 0; i < this.Props.moteCount; i++)
					{
						Vector2 offset = Rand.InsideUnitCircle * this.Props.moteOffsetRange.RandomInRange * (float)Rand.Sign;
						Vector3 myPos = new Vector3(pos.x + offset.x, pos.y, pos.z + offset.y);
						bool flag3 = this.Props.mote != null;
						if (flag3)
						{
							MoteMaker.MakeStaticMote(myPos, base.Pawn.Map, this.Props.mote, 1f);
						}
						else
						{
							FleckMaker.Static(myPos, base.Pawn.Map, this.Props.fleck, 1f);
						}
					}
				}
				bool flag4 = this.Props.filth != null;
				if (flag4)
				{
					FilthMaker.TryMakeFilth(base.Pawn.Position, base.Pawn.Map, this.Props.filth, this.Props.filthCount, FilthSourceFlags.None);
				}
				bool flag5 = this.Props.sound != null;
				if (flag5)
				{
					this.Props.sound.PlayOneShot(SoundInfo.InMap(base.Pawn, MaintenanceType.None));
				}
			}
		}

		public float ExplosiveRadius()
		{
			HediffCompProperties_DetonateOnDeath props = this.Props;
			float num = props.explosiveRadius;
			if (props.explosiveExpandPerSeverity > 0f)
			{
				num += Mathf.Sqrt((float)1 * props.explosiveExpandPerSeverity);
			}
			return num;
		}

		protected void Detonate(Map map, bool ignoreUnspawned = false, bool kill = true)
		{
			if (!ignoreUnspawned && !this.parent.pawn.SpawnedOrAnyParentSpawned)
			{
				return;
			}
			HediffCompProperties_DetonateOnDeath props = this.Props;
			float num = this.ExplosiveRadius();

			if (props.destroyThingOnExplosionSize <= num && !this.parent.pawn.Destroyed)
			{
				this.destroyedThroughDetonation = true;
				if (!this.parent.pawn.Dead && kill) this.parent.pawn.Kill(null, null);
			}
				this.EndWickSustainer();
				this.wickStarted = false;
			if (map == null)
			{
				Log.Warning("Tried to detonate HediffComp_DetonateOnDeath in a null map.", false);
				return;
			};
			if (props.explosionEffect != null)
			{
				Effecter effecter = props.explosionEffect.Spawn();
				effecter.Trigger(new TargetInfo(this.parent.pawn.PositionHeld, map, false), new TargetInfo(this.parent.pawn.PositionHeld, map, false));
				effecter.Cleanup();
			}
			Thing parent;
			if (this.instigator != null && !this.instigator.HostileTo(this.parent.pawn.Faction))
			{
				parent = this.instigator;
			}
			else
			{
				parent = this.parent.pawn;
			}
			DamageDef dmg = props.explosiveDamageType ?? DamageDefOf.Bomb;
			GenExplosion.DoExplosion(this.parent.pawn.PositionHeld, map, num, dmg, parent, props.damageAmountBase > 0 ? props.damageAmountBase : dmg.defaultDamage, props.armorPenetrationBase, props.explosionSound ?? dmg.soundExplosion, null, null, null, props.postExplosionSpawnThingDef, props.postExplosionSpawnChance, props.postExplosionSpawnThingCount, props.applyDamageToExplosionCellsNeighbors, props.preExplosionSpawnThingDef, props.preExplosionSpawnChance, props.preExplosionSpawnThingCount, props.chanceToStartFire, props.damageFalloff, null, null);
		}
		public void StartWick(Thing instigator = null)
		{
			if (this.wickStarted)
			{
				return;
			}
			if (this.ExplosiveRadius() <= 0f)
			{
				return;
			}
			this.instigator = instigator;
			this.wickStarted = true;
			this.wickTicksLeft = this.Props.wickTicks.RandomInRange;
			this.StartWickSustainer();
			GenExplosion.NotifyNearbyPawnsOfDangerousExplosive(this.parent.pawn, this.Props.explosiveDamageType ?? DamageDefOf.Bomb, null);
		}

		private void StartWickSustainer()
		{
			SoundDefOf.MetalHitImportant.PlayOneShot(new TargetInfo(this.parent.pawn.Position, this.parent.pawn.Map, false));
			SoundInfo info = SoundInfo.InMap(this.parent.pawn, MaintenanceType.PerTick);
			this.wickSoundSustainer = SoundDefOf.HissSmall.TrySpawnSustainer(info);
		}

		private void EndWickSustainer()
		{
			if (this.wickSoundSustainer != null)
			{
				this.wickSoundSustainer.End();
				this.wickSoundSustainer = null;
			}
		}

		public void StopWick()
		{
			this.wickStarted = false;
			this.instigator = null;
		}


		public override void CompExposeData()
        {
            base.CompExposeData();
			Scribe_References.Look<Thing>(ref this.instigator, "instigator", false);
			Scribe_Collections.Look<Thing>(ref this.thingsIgnoredByExplosion, "thingsIgnoredByExplosion", LookMode.Reference, Array.Empty<object>());
			Scribe_Values.Look<bool>(ref this.wickStarted, "wickStarted", false, false);
			Scribe_Values.Look<int>(ref this.wickTicksLeft, "wickTicksLeft", 0, false);
			Scribe_Values.Look<bool>(ref this.destroyedThroughDetonation, "destroyedThroughDetonation", false, false);
			Scribe_Values.Look<int>(ref this.countdownTicksLeft, "countdownTicksLeft", 0, false);
		}

		public bool wickStarted;
		protected int wickTicksLeft;
		private Thing instigator;
		private int countdownTicksLeft = -1;
		public bool destroyedThroughDetonation;
		private List<Thing> thingsIgnoredByExplosion;
		protected Sustainer wickSoundSustainer;

	}
}
