using System;

using UnityEngine;
using RimWorld;
using Verse;
using System.Collections.Generic;
using Verse.Sound;
using AdeptusMechanicus.ExtensionMethods;
using AdeptusMechanicus.settings;

namespace AdeptusMechanicus.Lasers
{
    public class LaserBeam : Bullet
    {
        public new LaserBeamDef def => base.def as LaserBeamDef;
        public override void Draw() { /*if (AMAMod.Dev) Log.Message(this.Label);*/ }
        public virtual new Vector3 destination
        {
            get => base.destination;
            set => base.destination = value;
        }
        
        public virtual new Vector3 origin
        {
            get => base.origin;
            set => base.origin = value;
        }
        Effecter effecter;
        Thing hitThing;

        public void SpawnBeam(Vector3 a, Vector3 b, Thing hitThing = null)
        {
            LaserBeamGraphic graphic = ThingMaker.MakeThing(def.beamGraphic, null) as LaserBeamGraphic;
            if (graphic == null) return;
            graphic.ticksToDetonation = this.def.projectile.explosionDelay;
            graphic.projDef = def;
            Pawn pawn = launcher as Pawn;
            Building_TurretGun turretGun = launcher as Building_TurretGun;

            Verb verb = pawn?.equipment?.PrimaryEq?.PrimaryVerb ?? turretGun?.AttackVerb;
            graphic.Setup(launcher, a, b, verb, hitThing, effecter, def.explosionEffect);
            GenSpawn.Spawn(graphic, origin.ToIntVec3(), Map, WipeMode.Vanish);
        }

        public void SpawnBeamReflections(Vector3 a, Vector3 b, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Vector3 dir = (b - a).normalized;
                Rand.PushState();
                Vector3 c = b - dir.RotatedBy(Rand.Range(-22.5f, 22.5f)) * Rand.Range(1f, 4f);
                Rand.PopState();
                SpawnBeam(b, c);
            }
        }

        public override void Impact(Thing hitThing)
        {
            this.hitThing = hitThing;
            bool shielded = hitThing.IsShielded() && def.IsWeakToShields;

            Pawn pawn = launcher as Pawn;
            LaserGunDef defWeapon = equipmentDef as LaserGunDef;
            Vector3 dir = (destination - origin).normalized;
            dir.y = 0;

            Vector3 a = origin;// += dir * (defWeapon == null ? 0.9f : defWeapon.barrelLength);
            Vector3 b;
            if (hitThing == null)
            {
                b = destination;
            }
            else if (shielded)
            {
                Rand.PushState();
                b = hitThing.TrueCenter() - dir.RotatedBy(Rand.Range(-22.5f, 22.5f)) * 0.8f;
                Rand.PopState();
            }
            else if ((destination - hitThing.TrueCenter()).magnitude < 1)
            {
                b = destination;
            }
            else
            {
                b = hitThing.TrueCenter();
                Rand.PushState();
                b.x += Rand.Range(-0.5f, 0.5f);
                b.z += Rand.Range(-0.5f, 0.5f);
                Rand.PopState();
            }
            destination = b;

            /*a.y =*/ b.y = def.Altitude;

        //    SpawnBeam(a, b);
            
            if (this.def.projectile.explosionRadius > 0f)
            {
                this.Explode(hitThing, false);
                GenExplosion.NotifyNearbyPawnsOfDangerousExplosive(this, this.def.projectile.damageDef, this.launcher.Faction);
            }
            /*
            IDrawnWeaponWithRotation weapon = null;
            if (pawn != null && pawn.equipment != null) weapon = pawn.equipment.Primary as IDrawnWeaponWithRotation;
            if (weapon == null)
            {
                Building_LaserGun turret = launcher as Building_LaserGun;
                if (turret != null)
                {
                    weapon = turret.gun as IDrawnWeaponWithRotation;
                }
            }
            if (weapon != null)
            {
                float angle = (b - a).AngleFlat() - (intendedTarget.CenterVector3 - a).AngleFlat();
                weapon.RotationOffset = (angle + 180) % 360 - 180;
            }
            */
            if (hitThing == null)
            {
                Rand.PushState();
                bool flag2 = this.def.causefireChance > 0f && Rand.Chance(this.def.causefireChance);
                Rand.PopState();
                if (flag2)
                {
                    FireUtility.TryStartFireIn(b.ToIntVec3(), pawn.Map, 0.01f);
                }
            }
            else
            {

                if (hitThing is Pawn && shielded)
                {
                    weaponDamageMultiplier *= def.shieldDamageMultiplier;

                    SpawnBeamReflections(a, b, 5);
                }

                Rand.PushState();
                bool flag2 = this.def.causefireChance > 0f && Rand.Chance(this.def.causefireChance);
                Rand.PopState();
                if (flag2)
                {
                    hitThing.TryAttachFire(0.01f);
                }
                AddeEffects(hitThing);
            }
            //    TriggerEffect(def.explosionEffect, b, hitThing);
            Map map = base.Map;
            IntVec3 position = base.Position;
            GenClamor.DoClamor(this, 2.1f, ClamorDefOf.Impact);
            this.Destroy(DestroyMode.Vanish);
            if (this.EquipmentDef == null)
            {
                this.equipmentDef = this.launcher.def;
            }
            BattleLogEntry_RangedImpact battleLogEntry_RangedImpact = new BattleLogEntry_RangedImpact(this.launcher, hitThing, this.intendedTarget.Thing, this.equipmentDef, this.def, this.targetCoverDef);
            Find.BattleLog.Add(battleLogEntry_RangedImpact);
            this.NotifyImpact(hitThing, map, position);
            if (hitThing != null)
            {
                DamageInfo dinfo = new DamageInfo(this.def.projectile.damageDef, (float)this.DamageAmount, this.ArmorPenetration, this.ExactRotation.eulerAngles.y, this.launcher, null, this.equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, this.intendedTarget.Thing);
                hitThing.TakeDamage(dinfo).AssociateWithLog(battleLogEntry_RangedImpact);
                if (pawn != null && pawn.stances != null && pawn.BodySize <= this.def.projectile.StoppingPower + 0.001f)
                {
                    pawn.stances.StaggerFor(95);
                }
                if (this.def.projectile.extraDamages == null)
                {
                    return;
                }
                using List<ExtraDamage>.Enumerator enumerator = this.def.projectile.extraDamages.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ExtraDamage extraDamage = enumerator.Current;
                    if (Rand.Chance(extraDamage.chance))
                    {
                        DamageInfo dinfo2 = new DamageInfo(extraDamage.def, extraDamage.amount, extraDamage.AdjustedArmorPenetration(), this.ExactRotation.eulerAngles.y, this.launcher, null, this.equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, this.intendedTarget.Thing);
                        hitThing.TakeDamage(dinfo2).AssociateWithLog(battleLogEntry_RangedImpact);
                    }
                }
                return;
            }
            SoundDefOf.BulletImpact_Ground.PlayOneShot(new TargetInfo(base.Position, map, false));
            if (base.Position.GetTerrain(map).takeSplashes)
            {
                FleckMaker.WaterSplash(this.ExactPosition, map, Mathf.Sqrt((float)base.DamageAmount) * 1f, 4f);
                return;
            }
            //    AdeptusFleckMaker.Static(this.ExactPosition, map, ThingDefOf.Mote_ShotHit_Dirt, 1f);
        }

        public virtual void Explode(Thing hitThing, bool destroy = false)
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


        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {

            Vector3 b = destination;
           /* a.y = */b.y = def.Altitude;
            SpawnBeam(origin, b, hitThing);
            IDrawnWeaponWithRotation weapon = null;
            Pawn pawn = launcher as Pawn;
            if (pawn != null && pawn.equipment != null) weapon = pawn.equipment.Primary as IDrawnWeaponWithRotation;
            if (weapon == null)
            {
                Building_LaserGun turret = launcher as Building_LaserGun;
                if (turret != null)
                {
                    weapon = turret.gun as IDrawnWeaponWithRotation;
                }
            }
            if (weapon != null)
            {
                float angle = (b - origin).AngleFlat() - (intendedTarget.CenterVector3 - origin).AngleFlat();
                weapon.RotationOffset = (angle + 180) % 360 - 180;
            }
            /*
            if (this.def.impactReflection > 0)
            {
                Vector3 dir = (this.ExactRotation.eulerAngles - origin).normalized;
                Rand.PushState();
                for (int i = 0; i < this.def.impactReflection; i++)
                {
                    Vector3 c = ExactPosition - dir.RotatedBy(Rand.Range(-35.5f, 35.5f)) * Rand.Range(-1.5f, 1.5f);// 0.8f;
                    SpawnBeam(b, c);
                }
                Rand.PopState();
            }
            */

            base.DeSpawn(mode);
        }
        #region Methods
        public new virtual float DamageAmount
        {
            get
            {
                if (this.def.projectile.Melta())
                {
                    //    Log.Message("Melta Blast");
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
                                if (distance > ((ThingWithComps)weapon).def.Verbs.Find(x => x.defaultProjectile == this.def).range / 2)
                                {
                                    return base.DamageAmount * 2;
                                }
                            }
                        }
                    }
                }
                else
                if (this.def.projectile.Conversion())
                {
                    //    Log.Message("Conversion Blast");
                    float distance = Vector3.Distance(origin, destination);
                    return base.DamageAmount + distance;
                }
                return base.DamageAmount;
            }
        }

        public new virtual float ArmorPenetration
        {
            get
            {
                
                if (this.def.projectile.Melta())
                {
                    //    Log.Message("Melta Blast");
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
                                if (distance > ((ThingWithComps)weapon).def.Verbs.Find(x => x.defaultProjectile == this.def).range / 2)
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
                                float halfrange = ((ThingWithComps)weapon).def.Verbs[0].range / 2;
                                if (distance > halfrange)
                                {
                                    return Mathf.Lerp(base.ArmorPenetration, base.ArmorPenetration / 2, (halfrange/(distance - halfrange)));
                                }
                            }
                        }
                    }

                }
                
                return base.ArmorPenetration;
            }
        }

        public new void NotifyImpact(Thing hitThing, Map map, IntVec3 position)
        {
            BulletImpactData impactData = new BulletImpactData
            {
                bullet = this,
                hitThing = hitThing,
                impactPosition = position
            };
            if (hitThing != null)
            {
                hitThing.Notify_BulletImpactNearby(impactData);
            }
            int num = 9;
            for (int i = 0; i < num; i++)
            {
                IntVec3 c = position + GenRadial.RadialPattern[i];
                if (c.InBounds(map))
                {
                    List<Thing> thingList = c.GetThingList(map);
                    for (int j = 0; j < thingList.Count; j++)
                    {
                        if (thingList[j] != hitThing)
                        {
                            thingList[j].Notify_BulletImpactNearby(impactData);
                        }
                    }
                }
            }
        }

        protected virtual void AddeEffects(Thing hitThing)
        {
            if (def != null && def.HediffToAdd != null && hitThing != null && hitThing is Pawn hitPawn)
            {
                Rand.PushState();
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
                Rand.PopState();
            }
        }
        #endregion Methods
    }
}
