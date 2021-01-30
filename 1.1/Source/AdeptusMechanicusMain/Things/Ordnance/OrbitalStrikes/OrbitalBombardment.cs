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
				if (Find.TickManager.TicksGame % 20 == 0 && base.TicksLeft > 0)
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
				this.projectiles.Add(new OrbitalBombardment.BombardmentProjectile(60, this.nextExplosionCell, strikeDef.ordnance));
				this.ticksToNextEffect = this.bombIntervalTicks;
				this.GetNextExplosionCell();
			}

			for (int i = this.projectiles.Count - 1; i >= 0; i--)
			{
				this.projectiles[i].Tick();
                try
				{
					if (this.projectiles[i].LifeTime == 5 && this.projectiles[i].targetCell.Roofed(base.Map))
					{
						if (base.HitRoof(this.projectiles[i].targetCell))
						{
							this.projectiles.RemoveAt(i);
							continue;
						}
					}
				}
                catch (Exception)
                {
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
					GenExplosion.DoExplosion(targetCell, map, randomInRange, bomb, instigator, damAmount, armorPenetration, explosionSound, this.weaponDef, def, null, null, 0f, 1, false, null, 0f, 1, 0f, false, null, null);
					this.projectiles.RemoveAt(i);
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

			public BombardmentProjectile(int lifeTime, IntVec3 targetCell, ThingDef ordnance = null, float angle = 180f)
			{
				this.lifeTime = lifeTime;
				this.maxLifeTime = lifeTime;
				this.targetCell = targetCell;
                if (ordnance != null)
				{
					this.ordnance = ordnance;
				}
				Angle = angle;
			}

			public void Tick()
			{
				this.lifeTime--;
			}

			public void Draw(Material material)
			{
				if (this.lifeTime > 0)
				{
					Vector3 pos = this.targetCell.ToVector3() + Vector3.forward * Mathf.Lerp(StartZ, 0f, 1f - (float)this.lifeTime / (float)this.maxLifeTime);
					pos.z += 1.25f;
					pos.y = AltitudeLayer.MoteOverhead.AltitudeFor();
					Matrix4x4 matrix = default(Matrix4x4);
					matrix.SetTRS(pos, Quaternion.Euler(0f, Angle, 0f), new Vector3(Scale, 1f, Scale));
					Graphics.DrawMesh(MeshPool.plane10, matrix, material, 0);
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

			private float StartZ;

			private float Scale;

			private float Angle;
		}
	}
}
