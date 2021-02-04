using System;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus.OrbitalStrikes
{
	// AdeptusMechanicus.OrbitalStrikes.OrbitalBombardment
	[StaticConstructorOnStartup]
    public class OrbitalBombardment : OrbitalStrike
	{
		public override void StartStrike()
		{
			this.duration = this.bombIntervalTicks * this.explosionCount;
			base.StartStrike();
		}

		public override void Tick()
		{
			if (base.Destroyed)
			{
				return;
			}
			if (this.warmupTicks > 0)
			{
				this.warmupTicks--;
				if (this.warmupTicks == 0)
				{
					this.StartStrike();
				}
			}
			else
			{
				if (this.TicksPassed >= this.duration && this.projectiles.EnumerableNullOrEmpty())
				{
					this.Destroy(DestroyMode.Vanish);
				}
				if (Find.TickManager.TicksGame % 20 == 0 && base.TicksLeft > 0 )
				{
				//	this.StartRandomFire();
				}
			}
			this.EffectTick();
		}

		private void EffectTick()
		{
			if (!this.nextExplosionCell.IsValid)
			{
				this.ticksToNextEffect = this.warmupTicks - this.bombIntervalTicks;
				this.GetNextExplosionCell();
			}
			this.ticksToNextEffect--;
			if (this.ticksToNextEffect <= 0 && base.TicksLeft >= this.bombIntervalTicks)
			{
				SoundDefOf.Bombardment_PreImpact.PlayOneShot(new TargetInfo(this.nextExplosionCell, base.Map, false));
				this.projectiles.Add(new OrbitalBombardment.BombardmentProjectile(base.Map, 60, this.nextExplosionCell, strikeDef.ordnance, this.angle));
				this.ticksToNextEffect = this.bombIntervalTicks;
				this.GetNextExplosionCell();
			}

			for (int i = this.projectiles.Count - 1; i >= 0; i--)
			{
				this.projectiles[i].Tick();
				if (this.projectiles[i].LifeTime == 5 && this.projectiles[i].targetCell.Roofed(base.Map))
				{
					if (base.HitRoof(this.projectiles[i].targetCell))
					{
						this.projectiles.RemoveAt(i);
						continue;
					}
				}
				if (this.projectiles[i].LifeTime <= 0)
				{
					IntVec3 targetCell = this.projectiles[i].targetCell;
					Map map = base.Map;
					float randomInRange = this.explosionRadiusRange.RandomInRange;
					DamageDef bomb = this.projectiles[i].ordnance.projectile.damageDef;
					Thing instigator = this.instigator;
					int damAmount = this.projectiles[i].ordnance.projectile.GetDamageAmount_NewTmp(this.def, null);
					float armorPenetration = this.projectiles[i].ordnance.projectile.GetArmorPenetration(this);
					SoundDef explosionSound = null;
					ThingDef def = this.def;

					Vector3 vector = targetCell.ToVector3ShiftedWithAltitude(AltitudeLayer.Skyfaller);
					if (this.projectiles[i].ordnance.HasModExtension<EffectProjectileExtension>())
					{
						EffectProjectileExtension effects = this.projectiles[i].ordnance.GetModExtension<EffectProjectileExtension>();
						effects.ThrowMote(vector, map, this.projectiles[i].ordnance.projectile.damageDef.explosionCellMote, randomInRange, this.projectiles[i].ordnance.projectile.damageDef.explosionColorCenter, this.projectiles[i].ordnance.projectile.damageDef.soundExplosion, ThingDef.Named(effects.ImpactMoteDef) ?? null, randomInRange, ThingDef.Named(effects.ImpactGlowMoteDef) ?? null, randomInRange);
					}
					GenExplosion.DoExplosion(targetCell, map, randomInRange, bomb, instigator, damAmount, armorPenetration, explosionSound, this.weaponDef, def, null, null, 0f, 1, false, null, 0f, 1, 0f, false, null, null);
					this.projectiles.RemoveAt(i);
				}
			}
		}

		public static void WarpRift(Projectile __instance, Thing ___launcher, Pawn hitPawn)
		{
			Map map = hitPawn.Map;
			if (__instance.def.projectile.explosionEffect != null)
			{
				Effecter effecter = __instance.def.projectile.explosionEffect.Spawn();
				effecter.Trigger(new TargetInfo(hitPawn.Position, map, false), new TargetInfo(hitPawn.Position, map, false));
				effecter.Cleanup();
			}
			IntVec3 position = hitPawn.Position;
			Map map2 = map;
			float explosionRadius = __instance.def.projectile.explosionRadius;
			DamageDef damageDef = __instance.def.projectile.damageDef;
			int DamageAmount = __instance.def.projectile.GetDamageAmount(___launcher, null);
			DamageArmorCategoryDef armorCategory = damageDef.armorCategory;
			StatDef armorcatdef = armorCategory.armorRatingStat;
			float ArmorPenetration = hitPawn.GetStatValue(armorcatdef, true);
			SoundDef soundExplode = __instance.def.projectile.soundExplode;
			ThingDef postExplosionSpawnThingDef = __instance.def.projectile.postExplosionSpawnThingDef;
			float postExplosionSpawnChance = __instance.def.projectile.postExplosionSpawnChance;
			int postExplosionSpawnThingCount = __instance.def.projectile.postExplosionSpawnThingCount;
			float y = __instance.ExactRotation.eulerAngles.y;
			ThingDef preExplosionSpawnThingDef = __instance.def.projectile.preExplosionSpawnThingDef;
			damageDef = OGDamageDefOf.OG_E_Distortion_Damage_Blast;
			GenExplosion.DoExplosion(position, map2, explosionRadius, damageDef, ___launcher, DamageAmount, ArmorPenetration, soundExplode);//, equipmentDef, def, thing, postExplosionSpawnThingDef, postExplosionSpawnChance, postExplosionSpawnThingCount, EquipmentSource.def.projectile.applyDamageToExplosionCellsNeighbors, preExplosionSpawnThingDef, EquipmentSource.def.projectile.preExplosionSpawnChance, EquipmentSource.def.projectile.preExplosionSpawnThingCount, EquipmentSource.def.projectile.explosionChanceToStartFire, EquipmentSource.def.projectile.explosionDamageFalloff);

			DamageInfo dinfo = new DamageInfo(damageDef, DamageAmount, ArmorPenetration, y, ___launcher, null, ___launcher.def, DamageInfo.SourceCategory.ThingOrUnknown, hitPawn);
			hitPawn.TakeDamage(dinfo);
			string msg = string.Format("{0} was lost to the warp", hitPawn.LabelCap);
			if (!hitPawn.Dead)
			{
				hitPawn.Kill(dinfo);
			}
			if (hitPawn.Faction == Faction.OfPlayer) { Messages.Message(msg, MessageTypeDefOf.PawnDeath); }
			if (hitPawn.Dead)
			{
				hitPawn.Corpse.Destroy(DestroyMode.KillFinalize);

			}
		}

		public static void Arc(Projectile __instance, Thing ___launcher, Pawn hitPawn, float radius = 5f)
		{
			Map map = hitPawn.Map;
			if (hitPawn.Faction != null)
			{
				if (Find.CurrentMap.mapPawns.AllPawns.Any(x => x.Position.InBounds(map) && x.Position.InHorDistOf(hitPawn.Position, radius) && x != hitPawn))
				{
					IEnumerable<Pawn> pawns = Find.CurrentMap.mapPawns.AllPawns.Where(x => x.Position.InBounds(map) && x.Position.InHorDistOf(hitPawn.Position, radius) && x != hitPawn);
					int t = Math.Min(pawns.Count(), 3);
					List<Pawn> alreadyhit = new List<Pawn>();
					for (int i = 0; i < t; i++)
					{
						Pawn target = pawns.Where(x => !alreadyhit.Contains(x)).RandomElement();
						if (target != null)
						{
							Projectile projectile = (Projectile)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("OGN_Bullet_TeslaCarbine_Arc"), null);
							GenSpawn.Spawn(projectile, hitPawn.Position, hitPawn.Map, 0);
							//    Log.Message(string.Format("Launch projectile2 {0} at {1}", projectile, OriginalPawn));
							projectile.Launch(___launcher, hitPawn.Position.ToVector3ShiftedWithAltitude(AltitudeLayer.Projectile), target, target, ProjectileHitFlags.All);
							alreadyhit.Add(target);
						}
						else break;
					}
				}
			}
		}

		public override void Draw()
		{
			base.Draw();
			if (this.projectiles.NullOrEmpty<OrbitalBombardment.BombardmentProjectile>())
			{
				return;
			}
			for (int i = 0; i < this.projectiles.Count; i++)
			{
				this.projectiles[i].Draw(this.projectiles[i].ordnance?.DrawMatSingle ?? ProjectileMaterial);
			}
		}

		private void StartRandomFire()
		{
			FireUtility.TryStartFireIn((from x in GenRadial.RadialCellsAround(base.Position, (float)this.randomFireRadius, true)
										where x.InBounds(base.Map)
										select x).RandomElementByWeight((IntVec3 x) => OrbitalBombardment.DistanceChanceFactor.Evaluate(x.DistanceTo(base.Position))), base.Map, Rand.Range(0.1f, 0.925f));
		}

		private void GetNextExplosionCell()
		{
			this.nextExplosionCell = (from x in GenRadial.RadialCellsAround(base.Position, this.impactAreaRadius, true)
									  where x.InBounds(base.Map)
									  select x).RandomElementByWeight((IntVec3 x) => OrbitalBombardment.DistanceChanceFactor.Evaluate(x.DistanceTo(base.Position) / this.impactAreaRadius));
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<float>(ref this.impactAreaRadius, "impactAreaRadius", 15f, false);
			Scribe_Values.Look<FloatRange>(ref this.explosionRadiusRange, "explosionRadiusRange", new FloatRange(6f, 8f), false);
			Scribe_Values.Look<int>(ref this.randomFireRadius, "randomFireRadius", 25, false);
			Scribe_Values.Look<int>(ref this.bombIntervalTicks, "bombIntervalTicks", 18, false);
			Scribe_Values.Look<int>(ref this.warmupTicks, "warmupTicks", 0, false);
			Scribe_Values.Look<int>(ref this.ticksToNextEffect, "ticksToNextEffect", 0, false);
			Scribe_Values.Look<IntVec3>(ref this.nextExplosionCell, "nextExplosionCell", default(IntVec3), false);
			Scribe_Collections.Look<OrbitalBombardment.BombardmentProjectile>(ref this.projectiles, "projectiles", LookMode.Deep, Array.Empty<object>());
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				if (!this.nextExplosionCell.IsValid)
				{
					this.GetNextExplosionCell();
				}
				if (this.projectiles == null)
				{
					this.projectiles = new List<OrbitalBombardment.BombardmentProjectile>();
				}
			}
		}
		/*
		public float impactAreaRadius = 15f;
		public FloatRange explosionRadiusRange = new FloatRange(6f, 8f);
		public int randomFireRadius = 25;
		public int bombIntervalTicks = 18;
		public int warmupTicks = 60;
		public int explosionCount = 30;
		*/

		private int fired;
		private int ticksToNextEffect;
		private IntVec3 nextExplosionCell = IntVec3.Invalid;
		private List<OrbitalBombardment.BombardmentProjectile> projectiles = new List<OrbitalBombardment.BombardmentProjectile>();
		public const int EffectiveAreaRadius = 23;
		private const int StartRandomFireEveryTicks = 20;
		private const int EffectDuration = 60;
		private static readonly Material ProjectileMaterial = MaterialPool.MatFrom("Things/Projectile/Bullet_Big", ShaderDatabase.Transparent, Color.white);
		public static readonly SimpleCurve DistanceChanceFactor = new SimpleCurve
		{
			{
				new CurvePoint(0f, 1f),
				true
			},
			{
				new CurvePoint(1f, 0.1f),
				true
			}
		};

		public class BombardmentProjectile : IExposable
		{
			public int LifeTime
			{
				get
				{
					return this.lifeTime;
				}
			}

			public BombardmentProjectile()
			{
			}

			public BombardmentProjectile(Map map, int lifeTime, IntVec3 targetCell, ThingDef ordnance = null, float angle = 180f, float startz = 60f)
			{
				this.lifeTime = lifeTime;
				this.maxLifeTime = lifeTime;
				this.targetCell = targetCell;
				this.map = map;

				if (ordnance != null)
				{
					this.ordnance = ordnance;
					this.Scale = ordnance.graphicData.drawSize.magnitude;
				}
				Angle = angle;
				StartZ = startz;
			}

			public void Tick()
			{
				this.lifeTime--;
				Vector3 pos = this.targetCell.ToVector3() + Velocity(-Angle) * Mathf.Lerp(StartZ, 0f, 1f - (float)this.lifeTime / (float)this.maxLifeTime);
				pos.z += 1.25f;
				pos.y = AltitudeLayer.MetaOverlays.AltitudeFor();
				AdeptusMoteMaker.ThrowLightningBolt(pos, map);
				AdeptusMoteMaker.ThrowEMPLightningGlow(pos, map, 1.25f);
			}
			/*
			protected virtual Vector3 NextExactPosition(float deltaTime)
			{
				return this.exactPosition + this.velocity * deltaTime;
			}
			*/
			// Token: 0x06001638 RID: 5688 RVA: 0x00081554 File Offset: 0x0007F754
			public Vector3 Velocity(float angle)
			{
				return Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward;
			}
			public void Draw(Material material)
			{
				if (this.lifeTime > 0)
				{
					Vector3 pos = this.targetCell.ToVector3() + Velocity(-Angle) * Mathf.Lerp(StartZ, 0f, 1f - (float)this.lifeTime / (float)this.maxLifeTime);
					pos.z += 1.25f;
					pos.y = AltitudeLayer.MetaOverlays.AltitudeFor();
					Matrix4x4 matrix = default(Matrix4x4);
					matrix.SetTRS(pos, Quaternion.LookRotation((this.targetCell.ToVector3() - pos).Yto0())/*Quaternion.Euler(0f, -Angle, 0f)*/, new Vector3(Scale, 1f, Scale));
					Graphics.DrawMesh(MeshPool.plane10, matrix, material, 0);
					if (ordnance.HasModExtension<GlowerProjectileExtension>())
					{
						GlowerProjectileExtension glower = ordnance.GetModExtension<GlowerProjectileExtension>();
						if (glower != null)
						{
							Mesh mesh2 = MeshPool.GridPlane(DefDatabase<ThingDef>.GetNamed(glower.GlowMoteDef).graphicData.drawSize * glower.GlowMoteSize);
							Graphics.DrawMesh(mesh2, pos, Quaternion.Euler(0f, Angle, 0f), DefDatabase<ThingDef>.GetNamed(glower.GlowMoteDef).graphic.MatSingle, 0);
						}
					}
				}
			}

			public void ExposeData()
			{
				Scribe_Values.Look<int>(ref this.lifeTime, "lifeTime", 0, false);
				Scribe_Values.Look<int>(ref this.maxLifeTime, "maxLifeTime", 0, false);
				Scribe_Values.Look<float>(ref this.StartZ, "StartZ", 60f, false);
				Scribe_Values.Look<float>(ref this.Angle, "Angle", 180f, false);
				Scribe_Values.Look<float>(ref this.Scale, "Scale", 2.5f, false);
				Scribe_Values.Look<IntVec3>(ref this.targetCell, "targetCell", default(IntVec3), false);
				Scribe_Values.Look<ThingDef>(ref this.ordnance, "ordnance", null, false);
			}

			private int lifeTime;

			private int maxLifeTime;

			public IntVec3 targetCell;
			public ThingDef ordnance;

			private Map map;

			private float StartZ;

			private float Scale;

			private float Angle;
		}
	}
}
