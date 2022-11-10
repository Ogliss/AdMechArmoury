using AdeptusMechanicus.settings;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.Bullet_Explosive
    public class Bullet_Explosive : Bullet
    {

        public override void Tick()
        {
            base.Tick();
            if (this.ticksToDetonation > 0 && this.landed)
            {
                this.ticksToDetonation--;
                if (this.ticksToDetonation <= 0)
                {
                    this.Explode();
                }
            }
            this.TrailTick();
        }

        public virtual void TrailTick()
        {
            if (AMAMod.settings.AllowProjectileTrail)
            {
                if (!Trailers.NullOrEmpty())
                {
                    foreach (TrailerProjectileExtension trailer in Trailers)
                    {
                        if (ticksToImpact % trailer.trailerMoteInterval == 0 && (trailer.trailWhenLanded || !this.landed))
                        {
                            for (int ii = 0; ii < trailer.motesThrown; ii++)
                            {
                                //    Trail1Thrower.ThrowSmokeTrail(__instance.Position.ToVector3Shifted(), trailer.trailMoteSize, __instance.Map, trailer.trailMoteDef);

                                //    TrailThrower.ThrowSmokeTrail(__instance.DrawPos, trailer.trailMoteSize * DistanceCoveredFraction(___origin, ___destination, ___ticksToImpact, __instance.def.projectile.SpeedTilesPerTick), __instance.Map, trailer.trailMoteDef, __instance);
                                Color? DC = null;
                                if (trailer.useGraphicColor)
                                {
                                    DC = this.DrawColor;
                                }
                                else
                                if (trailer.useGraphicColorTwo)
                                {
                                    DC = this.DrawColorTwo;
                                }
                                TrailThrower.ThrowSprayTrail(this.DrawPos, this.Map, origin, destination, trailer.TrailMoteDef, trailer.trailMoteSize, 240, this.def.projectile.SpeedTilesPerTick, DC);
                            }
                        }
                    }
                }
            }
        }

        private List<TrailerProjectileExtension> _trailers;
        public List<TrailerProjectileExtension> Trailers
        {
            get
            {
                if (_trailers == null)
                {
                    _trailers = new List<TrailerProjectileExtension>();
                    if (this.def.HasModExtension<TrailerProjectileExtension>())
                    {
                        for (int i = 0; i < def.modExtensions.Count; i++)
                        {
                            if (def.modExtensions[i] is TrailerProjectileExtension trailer)
                            {
                                _trailers.Add(trailer);
                            }
                        }
                    }
                }
                return _trailers;
            }
        }

        public virtual void drawGlow()
        {
            if (AMAMod.settings.AllowProjectileGlow)
            {
                if (!Glower.NullOrEmpty())
                {
                    foreach (GlowerProjectileExtension glower in Glower)
                    {
                        glower.Glow(this, this.ExactRotation);
                    }
                }
            }
        }

        private List<GlowerProjectileExtension> _glower;
        public List<GlowerProjectileExtension> Glower
        {
            get
            {
                if (_glower == null)
                {
                    _glower = new List<GlowerProjectileExtension>();
                    if (this.def.HasModExtension<GlowerProjectileExtension>())
                    {
                        for (int i = 0; i < def.modExtensions.Count; i++)
                        {
                            if (def.modExtensions[i] is GlowerProjectileExtension trailer)
                            {
                                _glower.Add(trailer);
                            }
                        }
                    }
                }
                return _glower;
            }
        }

        public override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            if (this.def.projectile.explosionRadius>0)
            {
                if (this.def.projectile.explosionDelay == 0)
                {
                    this.Explode(hitThing);
                    base.Impact(hitThing, blockedByShield);
                    return;
                }
                this.landed = true;
                this.ticksToDetonation = this.def.projectile.explosionDelay;
                GenExplosion.NotifyNearbyPawnsOfDangerousExplosive(this, this.def.projectile.damageDef, this.launcher.Faction);
            }
            else
            if (this.def.projectile.explosionEffect != null)
            {
                Effecter effecter = this.def.projectile.explosionEffect.Spawn();
                effecter.Trigger(new TargetInfo(base.Position, this.Map, false), new TargetInfo(base.Position, Map, false));
                effecter.Cleanup();
            }
            base.Impact(hitThing);
        }

        protected virtual void Explode(Thing hitThing = null)
        {
            Map map = base.Map;
            List<Thing> ignored = new List<Thing>();
            if (hitThing != null)
            {
                ignored.Add(hitThing);
            }
            if (this.def.projectile.explosionEffect != null)
            {
                Effecter effecter = this.def.projectile.explosionEffect.Spawn();
                effecter.Trigger(new TargetInfo(base.Position, map, false), new TargetInfo(base.Position, map, false));
                effecter.Cleanup();
            }
            IntVec3 position = base.Position;
            Map map2 = map;
            float explosionRadius = this.def.projectile.explosionRadius;
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

            GenExplosion.DoExplosion(position, map2, explosionRadius, damageDef, launcher, damageAmount, armorPenetration, soundExplode, equipmentDef, def, thing, postExplosionSpawnThingDef, postExplosionSpawnChance, postExplosionSpawnThingCount, null, this.def.projectile.applyDamageToExplosionCellsNeighbors, preExplosionSpawnThingDef, this.def.projectile.preExplosionSpawnChance, this.def.projectile.preExplosionSpawnThingCount, this.def.projectile.explosionChanceToStartFire, this.def.projectile.explosionDamageFalloff, ignoredThings: ignored);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.ticksToDetonation, "ticksToDetonation", 0, false);
        }

        private int ticksToDetonation;
    }
}
