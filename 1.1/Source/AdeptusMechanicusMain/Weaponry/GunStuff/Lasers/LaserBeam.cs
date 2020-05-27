using System;

using UnityEngine;
using RimWorld;
using Verse;
using System.Collections.Generic;
using Verse.Sound;
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus
{
    public class LaserBeam : Projectile
    {
        new LaserBeamDef def => base.def as LaserBeamDef;

        public override void Draw()
        {

        }
        
        void TriggerEffect(EffecterDef effect, Vector3 position)
        {
            TriggerEffect(effect, IntVec3.FromVector3(position));
        }

        void TriggerEffect(EffecterDef effect, IntVec3 dest)
        {
            if (effect == null) return;

            var targetInfo = new TargetInfo(dest, Map, false);

            Effecter effecter = effect.Spawn();
            effecter.Trigger(targetInfo, targetInfo);
            effecter.Cleanup();
        }

        void SpawnBeam(Vector3 a, Vector3 b)
        {
            LaserBeamGraphic graphic = ThingMaker.MakeThing(def.beamGraphic, null) as LaserBeamGraphic;
            if (graphic == null) return;
            graphic.ticksToDetonation = this.def.projectile.explosionDelay;
            graphic.def = def;
            graphic.Setup(launcher, a, b);
            GenSpawn.Spawn(graphic, origin.ToIntVec3(), Map, WipeMode.Vanish);
        }

        void SpawnBeamReflections(Vector3 a, Vector3 b, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Vector3 dir = (b - a).normalized;
                Vector3 c = b - dir.RotatedBy(Rand.Range(-22.5f,22.5f)) * Rand.Range(1f,4f);

                SpawnBeam(b, c);
            }
        }

        protected override void Impact(Thing hitThing)
        {
            bool shielded = hitThing.IsShielded() && def.IsWeakToShields;

            LaserGunDef defWeapon = equipmentDef as LaserGunDef;
            Vector3 dir = (destination - origin).normalized;
            dir.y = 0;

            Vector3 a = origin + dir * (defWeapon == null ? 0.9f : defWeapon.barrelLength);
            Vector3 b = shielded ? hitThing.TrueCenter() - dir.RotatedBy(Rand.Range(-22.5f,22.5f)) * 0.8f : destination;
            a.y = b.y = def.Altitude;

            SpawnBeam(a, b);
            /*
            bool createsExplosion = this.def.projectile.explosionRadius>0f;
            if (createsExplosion)
            {
                this.Explode(hitThing, false);
                GenExplosion.NotifyNearbyPawnsOfDangerousExplosive(this, this.def.projectile.damageDef, this.launcher.Faction);
            }
            */
            Pawn pawn = launcher as Pawn;
            IDrawnWeaponWithRotation weapon = null;
            if (pawn != null && pawn.equipment != null) weapon = pawn.equipment.Primary as IDrawnWeaponWithRotation;
            if (weapon == null) {
                Building_LaserGun turret = launcher as Building_LaserGun;
                if (turret != null) {
                    weapon = turret.gun as IDrawnWeaponWithRotation;
                }
            }
            if (weapon != null)
            {
                float angle = (b - a).AngleFlat() - (intendedTarget.CenterVector3 - a).AngleFlat();
                weapon.RotationOffset = (angle + 180) % 360 - 180;
            }

            if (hitThing == null)
            {
                TriggerEffect(def.explosionEffect, destination);
                Rand.PushState();
                bool flag2 = this.def.causefireChance > 0f && Rand.Chance(this.def.causefireChance);
                Rand.PopState();
                if (flag2)
                {
                    FireUtility.TryStartFireIn(destination.ToIntVec3(), pawn.Map, 0.01f);
                }
            }
            else
            {
                if (hitThing is Pawn)
                {
                    Pawn hitPawn = hitThing as Pawn;
                    if (shielded)
                    {
                        weaponDamageMultiplier *= def.shieldDamageMultiplier;

                        SpawnBeamReflections(a, b, 5);
                    }
                }
                Rand.PushState();
                bool flag2 = this.def.causefireChance > 0f && Rand.Chance(this.def.causefireChance);
                Rand.PopState();
                if (flag2)
                {
                    hitThing.TryAttachFire(0.01f);
                }
                TriggerEffect(def.explosionEffect, ExactPosition);
            }
            if (def.HediffToAdd!=null)
            {
                AddedEffect(hitThing);
            }
            Map map = base.Map;
            base.Impact(hitThing);
            BattleLogEntry_RangedImpact battleLogEntry_RangedImpact = new BattleLogEntry_RangedImpact(this.launcher, hitThing, this.intendedTarget.Thing, this.equipmentDef, this.def, this.targetCoverDef);
            Find.BattleLog.Add(battleLogEntry_RangedImpact);
            if (hitThing != null)
            {
                DamageDef damageDef = this.def.projectile.damageDef;
                float amount = DamageAmount;
                float armorPenetration = ArmorPenetration;
                float y = this.ExactRotation.eulerAngles.y;
                Thing launcher = this.launcher;
                ThingDef equipmentDef = this.equipmentDef;
                DamageInfo dinfo = new DamageInfo(damageDef, amount, armorPenetration, y, launcher, null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, this.intendedTarget.Thing);
                hitThing.TakeDamage(dinfo).AssociateWithLog(battleLogEntry_RangedImpact);
                Pawn hitPawn = hitThing as Pawn;
                if (hitPawn != null && hitPawn.stances != null && hitPawn.BodySize <= this.def.projectile.StoppingPower + 0.001f)
                {
                    hitPawn.stances.StaggerFor(95);
                }
            }
            else
            {
                SoundDefOf.BulletImpact_Ground.PlayOneShot(new TargetInfo(base.Position, map, false));
                MoteMaker.MakeStaticMote(this.ExactPosition, map, ThingDefOf.Mote_ShotHit_Dirt, 1f);
                if (base.Position.GetTerrain(map).takeSplashes)
                {
                    MoteMaker.MakeWaterSplash(this.ExactPosition, map, Mathf.Sqrt((float)base.DamageAmount) * 1f, 4f);
                }
            }
        }

        public new float DamageAmount
        {
            get
            {
                if (this.def.projectile.Conversion())
                {
                    //    Log.Message("Conversion Blast");
                    float distance = Vector3.Distance(origin, destination);
                    return base.DamageAmount + distance;
                }
                return base.DamageAmount;
            }
        }

        public new float ArmorPenetration
        {
            get
            {
                if (this.def.projectile.Melta())
                {
                    //    Log.Message("Melta Blast");
                    Pawn pawn = launcher as Pawn;
                    if (pawn!=null)
                    {
                        IDrawnWeaponWithRotation weapon = null;
                        if (pawn != null && pawn.equipment != null) weapon = pawn.equipment.Primary as IDrawnWeaponWithRotation;
                        if (weapon !=null)
                        {
                            if (origin !=null && destination!=null)
                            {
                                float distance = Vector3.Distance(origin, destination);
                                if (distance > ((ThingWithComps)weapon).def.Verbs.Find(x=> x.defaultProjectile == this.def).range / 2)
                                {
                                    return base.ArmorPenetration * 2;
                                }
                            }
                        }
                    }
                }
                else if (this.def.projectile.Volkite())
                {
                    //    Log.Message("Volkite Blast");
                    Pawn pawn = launcher as Pawn;
                    if (pawn != null)
                    {
                        IDrawnWeaponWithRotation weapon = null;
                        if (pawn != null && pawn.equipment != null) weapon = pawn.equipment.Primary as IDrawnWeaponWithRotation;
                        if (weapon != null)
                        {
                            if (origin != null && destination != null)
                            {
                                float distance = Vector3.Distance(origin, destination);
                                if (distance > ((ThingWithComps)weapon).def.Verbs[0].range / 2)
                                {

                                    return base.ArmorPenetration / distance;
                                }
                            }
                        }
                    }

                }
                return base.ArmorPenetration;
            }
        }

        // Token: 0x060000FB RID: 251 RVA: 0x00009248 File Offset: 0x00007448
        protected virtual void Explode(Thing hitThing, bool destroy = false)
        {
            Map map = base.Map;
            IntVec3 intVec = (hitThing != null) ? hitThing.PositionHeld : this.destination.ToIntVec3();
            if (destroy)
            {
                this.Destroy(DestroyMode.Vanish);
            }
            bool flag = this.def.projectile.explosionEffect != null;
            if (flag)
            {
                Effecter effecter = this.def.projectile.explosionEffect.Spawn();
                effecter.Trigger(new TargetInfo(intVec, map, false), new TargetInfo(intVec, map, false));
                effecter.Cleanup();
            }
            IntVec3 center = intVec;
            Map map2 = map;
            float explosionRadius = this.def.projectile.explosionRadius;
            DamageDef damageDef = this.def.projectile.damageDef;
            Thing launcher = this.launcher;
            int damageAmount = this.def.projectile.GetDamageAmount(1f, null);
            SoundDef soundExplode = this.def.projectile.soundExplode;
            ThingDef equipmentDef = this.equipmentDef;
            ThingDef def = this.def;
            ThingDef postExplosionSpawnThingDef = this.def.projectile.postExplosionSpawnThingDef;
            float postExplosionSpawnChance = this.def.projectile.postExplosionSpawnChance;
            int postExplosionSpawnThingCount = this.def.projectile.postExplosionSpawnThingCount;
            ThingDef preExplosionSpawnThingDef = this.def.projectile.preExplosionSpawnThingDef;
            GenExplosion.DoExplosion(center, map2, explosionRadius, damageDef, launcher, damageAmount, 0f, soundExplode, equipmentDef, def, null, postExplosionSpawnThingDef, postExplosionSpawnChance, postExplosionSpawnThingCount, this.def.projectile.applyDamageToExplosionCellsNeighbors, preExplosionSpawnThingDef, this.def.projectile.preExplosionSpawnChance, this.def.projectile.preExplosionSpawnThingCount, this.def.projectile.explosionChanceToStartFire, this.def.projectile.explosionDamageFalloff);
        }


        protected void AddedEffect(Thing hitThing)
        {
            if (def != null && hitThing != null && hitThing is Pawn hitPawn)
            {
                var rand = Rand.Value; // This is a random percentage between 0% and 100%

                StatDef ResistHediffStat = def.ResistHediffStat;
                float AddHediffChance = def.AddHediffChance;
                float ResistHediffChance = def.ResistHediffChance;
                if (def.CanResistHediff == true)
                {
                    /*
                    if (Def.ResistHediffChance!=0)
                    {
                        rand = rand + Def.ResistHediffChance;
                    }
                    else */
                    if (def.ResistHediffStat != null)
                    {
                        ResistHediffChance = hitPawn.GetStatValue(ResistHediffStat, true);
                    }
                    AddHediffChance = AddHediffChance * ResistHediffChance;
                }

                if (rand <= AddHediffChance) // If the percentage falls under the chance, success!
                {

                    var effectOnPawn = hitPawn?.health?.hediffSet?.GetFirstHediffOfDef(def.HediffToAdd);
                    var randomSeverity = Rand.Range(0.15f, 0.30f);
                    if (effectOnPawn != null)
                    {
                        //If they already have plague, add a random range to its severity.
                        //If severity reaches 1.0f, or 100%, plague kills the target.
                        effectOnPawn.Severity += randomSeverity;
                    }
                    else
                    {
                        //These three lines create a new health differential or Hediff,
                        //put them on the character, and increase its severity by a random amount.
                        Hediff hediff = HediffMaker.MakeHediff(def.HediffToAdd, hitPawn, null);
                        hediff.Severity = randomSeverity;
                        hitPawn.health.AddHediff(hediff, null, null);
                    }
                }
                else //failure!
                {

                    /*
                    MoteMaker.ThrowText(hitThing.PositionHeld.ToVector3(), hitThing.MapHeld, "TST_PlagueBullet_FailureMote".Translate(Def.AddHediffChance), 12f);
                    */
                }
            }
        }
    }
}
