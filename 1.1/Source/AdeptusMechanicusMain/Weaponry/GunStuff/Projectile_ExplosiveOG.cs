using AdeptusMechanicus;
using RimWorld;
using System;
using UnityEngine;

namespace Verse
{
    public class ThingDef_BulletExplosiveOG : ThingDef
    {
        public float explosionradius = 0.05f;
    }

    // Token: 0x02000E4A RID: 3658
    public class Projectile_ExplosiveOG : Projectile
    {
        private int TicksforAppearence = 3;
        #region Properties
        public ThingDef_BulletExplosiveOG Def
        {
            get
            {
                return this.def as ThingDef_BulletExplosiveOG;
            }
        }
        #endregion Properties
        // Token: 0x06005295 RID: 21141 RVA: 0x00261693 File Offset: 0x0025FA93
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.ticksToDetonation, "ticksToDetonation", 0, false);
        }

        // Token: 0x06005296 RID: 21142 RVA: 0x002616AD File Offset: 0x0025FAAD
        public override void Tick()
        {
            base.Tick();
            checked
            {
                this.TicksforAppearence--;
                bool flag = this.TicksforAppearence == 0 && base.Map != null;
                if (flag)
                {
                    // TrailThrower.ThrowSmokeTrail(base.ExactPosition, 0.7f, base.Map, "OG_Mote_BoltTrailPuff");

                    if (this.def.projectile.damageDef.defName.Contains("OG") && this.def.projectile.damageDef.defName.Contains("Bolt"))
                    {
                        ThrowBoltSmoke(this.DrawPos, base.Map, 0.75f);
                    }
                    this.TicksforAppearence = 2;
                }
            }
            if (this.ticksToDetonation > 0)
            {
                this.ticksToDetonation--;
                if (this.ticksToDetonation <= 0)
                {
                    this.Explode();
                }
            }
        }

        // Token: 0x06005297 RID: 21143 RVA: 0x002616E4 File Offset: 0x0025FAE4
        protected override void Impact(Thing hitThing)
        {
            if (this.def.projectile.explosionDelay == 0)
            {
                this.Explode();
                return;
            }
            this.landed = true;
            this.ticksToDetonation = this.def.projectile.explosionDelay;
            GenExplosion.NotifyNearbyPawnsOfDangerousExplosive(this, this.def.projectile.damageDef, this.launcher.Faction);
        }

        // Token: 0x06005298 RID: 21144 RVA: 0x0026174C File Offset: 0x0025FB4C
        protected virtual void Explode()
        {
            Map map = base.Map;
            this.Destroy(DestroyMode.Vanish);
            if (this.def.projectile.explosionEffect != null)
            {
                Effecter effecter = this.def.projectile.explosionEffect.Spawn();
                effecter.Trigger(new TargetInfo(base.Position, map, false), new TargetInfo(base.Position, map, false));
                effecter.Cleanup();
            }
            IntVec3 position = base.Position;
            Map map2 = map;
            DamageDef damageDef = this.def.projectile.damageDef;
            Thing launcher = this.launcher;
            int damageAmount = base.DamageAmount;
            float armorPenetration = base.ArmorPenetration;
            SoundDef soundExplode = this.def.projectile.soundExplode;
            ThingDef equipmentDef = this.equipmentDef;
            ThingDef def = this.def;
            Thing thing = this.intendedTarget.Thing;
            ThingDef postExplosionSpawnThingDef = this.def.projectile.postExplosionSpawnThingDef;
            float postExplosionSpawnChance = this.def.projectile.postExplosionSpawnChance;
            int postExplosionSpawnThingCount = this.def.projectile.postExplosionSpawnThingCount;
            ThingDef preExplosionSpawnThingDef = this.def.projectile.preExplosionSpawnThingDef;
            GenExplosion.DoExplosion(position, map2, Def.explosionradius, damageDef, launcher, damageAmount, armorPenetration, soundExplode, equipmentDef, def, thing, postExplosionSpawnThingDef, postExplosionSpawnChance, postExplosionSpawnThingCount, this.def.projectile.applyDamageToExplosionCellsNeighbors, preExplosionSpawnThingDef, this.def.projectile.preExplosionSpawnChance, this.def.projectile.preExplosionSpawnThingCount, this.def.projectile.explosionChanceToStartFire, this.def.projectile.explosionDamageFalloff);
        }

        // Token: 0x060026BE RID: 9918 RVA: 0x00126340 File Offset: 0x00124740
        public static void ThrowBoltSmoke(Vector3 loc, Map map, float size)
        {
            if (!loc.ShouldSpawnMotesAt(map) || map.moteCounter.SaturatedLowPriority)
            {
                return;
            }
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(ThingDef.Named("OG_Mote_BoltTrailPuff"), null);
            moteThrown.Scale = Rand.Range(1.5f, 2.5f) * size;
            moteThrown.rotationRate = Rand.Range(-30f, 30f);
            moteThrown.exactPosition = loc;
            moteThrown.SetVelocity((float)Rand.Range(30, 40), Rand.Range(0.5f, 0.7f));
            GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map, WipeMode.Vanish);
        }
        // Token: 0x040036E3 RID: 14051
        private int ticksToDetonation;
    }
}
