using RimWorld;
using System;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus.ArtilleryStrikes
{
    // Token: 0x02000D3B RID: 3387
    public class ArtilleryIncoming : Skyfaller
	{
		public override Graphic Graphic
		{
			get
			{
				if (Payload == null)
				{
					return base.Graphic;
				}
				return Payload.graphic.GetShadowlessGraphic();
			}
		}
		public ThingDef Payload;
		Thing launcher;
		LocalTargetInfo target;

		public override void Tick()
		{
			if (Payload == null)
			{
				Log.Warning("Tried to spawn Artillery Strike with no Payload");
				this.Destroy();
				return;
			}
			if (this.ticksToDetonation > 0)
			{
				this.ticksToDetonation--;
				if (this.ticksToDetonation <= 0)
				{
					this.Explode();
				}
			}
			this.innerContainer.ThingOwnerTick(true);
			if (this.SpawnTimedMotes)
			{
				CellRect cellRect = this.OccupiedRect();
				for (int i = 0; i < cellRect.Area * this.def.skyfaller.motesPerCell; i++)
				{
					AdeptusFleckMaker.ThrowDustPuff(cellRect.RandomVector3, base.Map, 2f);
				}
			}
			if (this.def.skyfaller.reversed)
			{
				this.ticksToImpact++;
				if (!this.anticipationSoundPlayed && this.def.skyfaller.anticipationSound != null && this.ticksToImpact > this.def.skyfaller.anticipationSoundTicks)
				{
					this.anticipationSoundPlayed = true;
					this.def.skyfaller.anticipationSound.PlayOneShot(new TargetInfo(base.Position, base.Map, false));
				}
				if (this.ticksToImpact == 220)
				{
					this.LeaveMap();
					return;
				}
				if (this.ticksToImpact > 220)
				{
					Log.Error("ticksToImpact > LeaveMapAfterTicks. Was there an exception? Destroying skyfaller.", false);
					this.Destroy(DestroyMode.Vanish);
					return;
				}
			}
			else
			{
				this.ticksToImpact--;
				if (this.ticksToImpact == 15)
				{
					this.HitRoof();
				}
				if (!this.anticipationSoundPlayed && this.def.skyfaller.anticipationSound != null && this.ticksToImpact < this.def.skyfaller.anticipationSoundTicks)
				{
					this.anticipationSoundPlayed = true;
					this.def.skyfaller.anticipationSound.PlayOneShot(new TargetInfo(base.Position, base.Map, false));
				}
				if (this.ticksToImpact <= 0 && this.ticksToDetonation <= 0)
				{
					this.Impact();
					return;
				}
				if (this.ticksToImpact < 0 && Payload.projectile.explosionDelay == 0)
				{
					Log.Error("ticksToImpact < 0. Was there an exception? Destroying skyfaller.", false);
					this.Destroy(DestroyMode.Vanish);
				}
			}
		}

		public override void Impact()
		{
			if (Payload != null)
			{
				if (Payload.projectile.explosionDelay == 0)
				{
					this.Explode();
					return;
				}
				this.landed = true;
				this.ticksToDetonation = Payload.projectile.explosionDelay;
				GenExplosion.NotifyNearbyPawnsOfDangerousExplosive(this, Payload.projectile.damageDef, this.launcher.Faction);
				
			}
			else
			if (this.def.skyfaller.CausesExplosion)
			{
			//	Log.Message("CausesExplosion");
				GenExplosion.DoExplosion(base.Position, base.Map, this.def.skyfaller.explosionRadius, this.def.skyfaller.explosionDamage, null, GenMath.RoundRandom((float)this.def.skyfaller.explosionDamage.defaultDamage * this.def.skyfaller.explosionDamageFactor), -1f, null, null, null, null, null, 0f, 1, false, null, 0f, 1, 0f, false, null, (!this.def.skyfaller.damageSpawnedThings) ? this.innerContainer.ToList<Thing>() : null);
			}
		//	this.SpawnThings();
			this.innerContainer.ClearAndDestroyContents(DestroyMode.Vanish);
			CellRect cellRect = this.OccupiedRect();
			for (int i = 0; i < cellRect.Area * this.def.skyfaller.motesPerCell; i++)
			{
				AdeptusFleckMaker.ThrowDustPuff(cellRect.RandomVector3, base.Map, 2f);
			}
			if (this.def.skyfaller.MakesShrapnel)
			{
				SkyfallerShrapnelUtility.MakeShrapnel(base.Position, base.Map, this.shrapnelDirection, this.def.skyfaller.shrapnelDistanceFactor, this.def.skyfaller.metalShrapnelCountRange.RandomInRange, this.def.skyfaller.rubbleShrapnelCountRange.RandomInRange, true);
			}
			if (this.def.skyfaller.cameraShake > 0f && base.Map == Find.CurrentMap)
			{
				Find.CameraDriver.shaker.DoShake(this.def.skyfaller.cameraShake);
			}
			if (this.def.skyfaller.impactSound != null)
			{
				this.def.skyfaller.impactSound.PlayOneShot(SoundInfo.InMap(new TargetInfo(base.Position, base.Map, false), MaintenanceType.None));
			}
			this.Destroy(DestroyMode.Vanish);
		}

		// Token: 0x060052CD RID: 21197 RVA: 0x001BF0C4 File Offset: 0x001BD2C4
		public override void DrawAt(Vector3 drawLoc, bool flip = false)
		{
			float num = 0f;
			if (this.def.skyfaller.rotateGraphicTowardsDirection)
			{
				num = -this.angle;
			}
			if (this.def.skyfaller.angleCurve != null)
			{
				this.angle = this.def.skyfaller.angleCurve.Evaluate(this.TimeInAnimation);
			}
			if (this.def.skyfaller.rotationCurve != null)
			{
				num += this.def.skyfaller.rotationCurve.Evaluate(this.TimeInAnimation);
			}
			if (this.def.skyfaller.xPositionCurve != null)
			{
				drawLoc.x += this.def.skyfaller.xPositionCurve.Evaluate(this.TimeInAnimation);
			}
			if (this.def.skyfaller.zPositionCurve != null)
			{
				drawLoc.z += this.def.skyfaller.zPositionCurve.Evaluate(this.TimeInAnimation);
			}
		//	this.Graphic.Draw(drawLoc, flip ? this.Rotation.Opposite : this.Rotation, this, (base.Position.ToVector3Shifted() - drawLoc).ToAngleFlat());

			Graphics.DrawMesh(MeshPool.GridPlane(this.def.graphicData.drawSize), drawLoc, Quaternion.LookRotation((base.Position.ToVector3Shifted() - drawLoc).Yto0()), this.def.DrawMatSingle, 0);

		}
		protected virtual void Explode()
		{
			Map map = base.Map;
			this.Destroy(DestroyMode.Vanish);
			if (Payload.projectile.explosionEffect != null)
			{
				Effecter effecter = Payload.projectile.explosionEffect.Spawn();
				effecter.Trigger(new TargetInfo(base.Position, map, false), new TargetInfo(base.Position, map, false));
				effecter.Cleanup();
			}
			IntVec3 position = base.Position;
			Map map2 = map;
			float explosionRadius = Payload.projectile.explosionRadius;
			DamageDef damageDef = Payload.projectile.damageDef;
			Thing launcher = this.launcher;
			int damageAmount = Payload.projectile.GetDamageAmount(Payload, null);
			float armorPenetration = Payload.projectile.GetArmorPenetration(this);
			SoundDef soundExplode = Payload.projectile.soundExplode;
			ThingDef equipmentDef = this.def;
			ThingDef def = Payload;
			Thing thing = this.target.Thing;
			ThingDef postExplosionSpawnThingDef = Payload.projectile.postExplosionSpawnThingDef;
			float postExplosionSpawnChance = Payload.projectile.postExplosionSpawnChance;
			int postExplosionSpawnThingCount = Payload.projectile.postExplosionSpawnThingCount;
			ThingDef preExplosionSpawnThingDef = Payload.projectile.preExplosionSpawnThingDef;
			float preExplosionSpawnChance = Payload.projectile.preExplosionSpawnChance;
			int preExplosionSpawnThingCount = Payload.projectile.preExplosionSpawnThingCount;
			GenExplosion.DoExplosion(position, map2, explosionRadius, damageDef, launcher, damageAmount, armorPenetration, soundExplode, equipmentDef, def, thing, postExplosionSpawnThingDef, postExplosionSpawnChance, postExplosionSpawnThingCount, Payload.projectile.applyDamageToExplosionCellsNeighbors, preExplosionSpawnThingDef, preExplosionSpawnChance, preExplosionSpawnThingCount, Payload.projectile.explosionChanceToStartFire, Payload.projectile.explosionDamageFalloff, null, null);
		}

		private int ticksToDetonation = 0;
		private bool landed;
	}
}
