using System;

using UnityEngine;
using RimWorld;
using Verse;
using System.Collections.Generic;
using Verse.Sound;
using AdeptusMechanicus.ExtensionMethods;
using AdeptusMechanicus.Lasers;
using CombatExtended;
using HarmonyLib;
using System.Reflection;
using System.Linq;

namespace AdeptusMechanicus.Lasers
{
    public class LaserBeamCE : BulletCE
    {
        public new LaserBeamDefCE def => base.def as LaserBeamDefCE;
        public override void Draw() { /*if (AMAMod.Dev) Log.Message(this.Label);*/ }
        public virtual Vector3 destination
        {
            get => new Vector3(Mathf.Clamp(base.Destination.x, 0, Map.Size.x), 0, Mathf.Clamp(base.Destination.y, 0, Map.Size.z));
            set => base.destinationInt = value;
        }

        public virtual Vector3 Origin
        {
            get => new Vector3(base.origin.x, 0, base.origin.y);
            set => base.origin = value;
        }

        Effecter effecter;
        Thing hitThing;
        void SpawnBeam(Vector3 a, Vector3 b)
        {
            LaserBeamGraphicCE graphic = ThingMaker.MakeThing(def.beamGraphic, null) as LaserBeamGraphicCE;
            if (graphic == null) return;
            graphic.ticksToDetonation = this.def.projectile.explosionDelay;
            graphic.projDef = def;
            graphic.Setup(launcher, a, b);
            GenSpawn.Spawn(graphic, Origin.ToIntVec3(), Map, WipeMode.Vanish);
        }

        public void SpawnBeam(Vector3 a, Vector3 b, Thing hitThing = null)
        {
            LaserBeamGraphicCE graphic = ThingMaker.MakeThing(def.beamGraphic, null) as LaserBeamGraphicCE;
            if (graphic == null) return;
            graphic.ticksToDetonation = this.def.projectile.explosionDelay;
            graphic.projDef = def;
            Pawn pawn = launcher as Pawn;
            Building_TurretGunCE turretGun = launcher as Building_TurretGunCE;

            Verb verb = pawn?.equipment?.PrimaryEq?.PrimaryVerb ?? turretGun?.AttackVerb;
            graphic.Setup(launcher, a, b, verb, hitThing, effecter, def.explosionEffect);
            GenSpawn.Spawn(graphic, Origin.ToIntVec3(), Map, WipeMode.Vanish);
        }

        void SpawnBeamReflections(Vector3 a, Vector3 b, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Vector3 dir = (b - a).normalized;
                Rand.PushState();
                Vector3 c = b - dir.RotatedBy(Rand.Range(-22.5f,22.5f)) * Rand.Range(1f,4f);
                Rand.PopState();

                SpawnBeam(b, c);
            }
        }

        protected override void Impact(Thing hitThing)
        {
            this.hitThing = hitThing;
            bool shielded = hitThing.IsShielded() && def.IsWeakToShields;

            Pawn pawn = launcher as Pawn;
            LaserGunDef defWeapon = equipmentDef as LaserGunDef;
            Vector3 dir = (destination - Origin).normalized;
            dir.y = 0;

            Vector3 a = Origin;// += dir * (defWeapon == null ? 0.9f : defWeapon.barrelLength);
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
            /*a.y =*/
            b.y = def.Altitude;
            //     SpawnBeam(a, b, hitThing);
            if (this.def.projectile.explosionRadius>0f)
            {
                this.Explode(hitThing, false);
                GenExplosion.NotifyNearbyPawnsOfDangerousExplosive(this, this.def.projectile.damageDef, this.launcher.Faction);
            }
            /*
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
                float angle = (b - a).AngleFlat() - (destination - a).AngleFlat();
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
                //    weaponDamageMultiplier *= def.shieldDamageMultiplier;
                    SpawnBeamReflections(a, b, 5);
                }

                Rand.PushState();
                bool flag2 = this.def.causefireChance > 0f && Rand.Chance(this.def.causefireChance);
                Rand.PopState();
                if (flag2)
                {
                    hitThing.TryAttachFire(0.01f);
                }
                if (def.HediffToAdd != null)
                {
                    AddeEffects(hitThing);
                }
            }
            Map map = base.Map;
            this.BulletImpact(hitThing);
        }

        protected void BulletImpact(Thing hitThing)
        {
            bool flag = this.launcher is AmmoThing;
            Map map = base.Map;
            LogEntry_DamageResult logEntry_DamageResult = null;
            if (!flag && (this.logMisses || hitThing is Pawn || hitThing is Building_Turret))
            {
                this.LogImpact(hitThing, out logEntry_DamageResult);
            }
            if (hitThing != null)
            {
                DamageDefExtensionCE damageDefExtensionCE = this.def.projectile.damageDef.GetModExtension<DamageDefExtensionCE>() ?? new DamageDefExtensionCE();
                ProjectilePropertiesCE projectilePropertiesCE = (ProjectilePropertiesCE)this.def.projectile;
                DamageInfo damageInfo = new DamageInfo(this.def.projectile.damageDef, this.DamageAmount, this.ArmorPenetration, this.ExactRotation.eulerAngles.y, this.launcher, null, this.def, DamageInfo.SourceCategory.ThingOrUnknown, null);
                BodyPartDepth depth = damageDefExtensionCE.harmOnlyOutsideLayers ? BodyPartDepth.Outside : BodyPartDepth.Undefined;
                BodyPartHeight collisionBodyHeight = new CollisionVertical(hitThing).GetCollisionBodyHeight(this.ExactPosition.y);
                damageInfo.SetBodyRegion(collisionBodyHeight, depth);
                bool harmOnlyOutsideLayers = damageDefExtensionCE.harmOnlyOutsideLayers;
                if (harmOnlyOutsideLayers)
                {
                    damageInfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
                }
                Pawn pawn = (hitThing as Pawn);
                if (pawn != null)
                {
                    logEntry_DamageResult = new BattleLogEntry_DamageTaken(pawn, LaserBeamCE.CookOff, null);
                    Find.BattleLog.Add(logEntry_DamageResult);
                }
                try
                {
                    hitThing.TakeDamage(damageInfo).AssociateWithLog(logEntry_DamageResult);
                    if (!(hitThing is Pawn) && projectilePropertiesCE != null && !projectilePropertiesCE.secondaryDamage.NullOrEmpty<SecondaryDamage>())
                    {
                        foreach (SecondaryDamage secondaryDamage in projectilePropertiesCE.secondaryDamage)
                        {
                            bool destroyed = hitThing.Destroyed;
                            if (destroyed)
                            {
                                break;
                            }
                            DamageInfo dinfo = secondaryDamage.GetDinfo(damageInfo);
                            hitThing.TakeDamage(dinfo).AssociateWithLog(logEntry_DamageResult);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("CombatExtended :: BulletCE impacting thing " + hitThing.LabelCap + " of def " + hitThing.def.LabelCap + " added by mod " + hitThing.def.modContentPack.Name + ". See following stacktrace for information.", false);
                    throw ex;
                }
                finally
                {
                    this.ProjectileImpact(hitThing);
                }
            }
            else
            {
                SoundDefOf.BulletImpact_Ground.PlayOneShot(new TargetInfo(base.Position, map, false));
                bool castShadow = this.castShadow;
                if (castShadow)
                {
                    MoteMaker.MakeStaticMote(this.ExactPosition, map, ThingDefOf.Mote_ShotHit_Dirt, 1f);
                    bool takeSplashes = base.Position.GetTerrain(map).takeSplashes;
                    if (takeSplashes)
                    {
                        MoteMaker.MakeWaterSplash(this.ExactPosition, map, Mathf.Sqrt((float)this.def.projectile.GetDamageAmount(this.launcher, null)) * 1f, 4f);
                    }
                }
                this.ProjectileImpact(null);
            }
            this.NotifyImpact(hitThing, map, base.Position);
        }
        protected void ProjectileImpact(Thing hitThing)
        {
            List<Thing> list = new List<Thing>();
            bool flag = base.Position.IsValid && this.def.projectile.preExplosionSpawnChance > 0f && this.def.projectile.preExplosionSpawnThingDef != null && (Controller.settings.EnableAmmoSystem || !(this.def.projectile.preExplosionSpawnThingDef is AmmoDef)) && Rand.Value < this.def.projectile.preExplosionSpawnChance;
            if (flag)
            {
                ThingDef preExplosionSpawnThingDef = this.def.projectile.preExplosionSpawnThingDef;
                bool flag2 = preExplosionSpawnThingDef.IsFilth && base.Position.Walkable(base.Map);
                if (flag2)
                {
                    FilthMaker.TryMakeFilth(base.Position, base.Map, preExplosionSpawnThingDef, 1, FilthSourceFlags.None);
                }
                else
                {
                    bool reuseNeolithicProjectiles = Controller.settings.ReuseNeolithicProjectiles;
                    if (reuseNeolithicProjectiles)
                    {
                        Thing thing = ThingMaker.MakeThing(preExplosionSpawnThingDef, null);
                        thing.stackCount = 1;
                        thing.SetForbidden(true, false);
                        GenPlace.TryPlaceThing(thing, base.Position, base.Map, ThingPlaceMode.Near, null, null, default(Rot4));
                        LessonAutoActivator.TeachOpportunity(CE_ConceptDefOf.CE_ReusableNeolithicProjectiles, thing, OpportunityType.GoodToKnow);
                        list.Add(thing);
                    }
                }
            }
            Vector3 vector = (hitThing != null) ? hitThing.DrawPos : this.ExactPosition;
            bool flag3 = !vector.ToIntVec3().IsValid;
            if (flag3)
            {
                this.Destroy(DestroyMode.Vanish);
            }
            else
            {
                CompExplosiveCE compExplosiveCE = this.TryGetComp<CompExplosiveCE>();
                if (compExplosiveCE == null)
                {
                    CompFragments compFragments = this.TryGetComp<CompFragments>();
                    if (compFragments != null)
                    {
                        compFragments.Throw(vector, base.Map, this.launcher, 1f);
                    }
                }
                if (compExplosiveCE != null || this.def.projectile.explosionRadius > 0f)
                {
                    Pawn pawn = hitThing as Pawn;
                    if (pawn != null && pawn.Dead)
                    {
                        list.Add(pawn.Corpse);
                    }
                    List<Pawn> list2 = new List<Pawn>();
                    float? direction = new float?(this.origin.AngleTo(this.Vec2Position(-1f)));
                    if (this.def.projectile.explosionRadius > 0f)
                    {
                        GenExplosionCE.DoExplosion(vector.ToIntVec3(), base.Map, this.def.projectile.explosionRadius, this.def.projectile.damageDef, this.launcher, (int)this.DamageAmount, GenExplosionCE.GetExplosionAP(this.def.projectile), this.def.projectile.soundExplode, this.equipmentDef, this.def, null, this.def.projectile.postExplosionSpawnThingDef, this.def.projectile.postExplosionSpawnChance, this.def.projectile.postExplosionSpawnThingCount, this.def.projectile.applyDamageToExplosionCellsNeighbors, this.def.projectile.preExplosionSpawnThingDef, this.def.projectile.preExplosionSpawnChance, this.def.projectile.preExplosionSpawnThingCount, this.def.projectile.explosionChanceToStartFire, this.def.projectile.explosionDamageFalloff, direction, list, vector.y, 1f, false, null);
                        if (vector.y < 3f)
                        {
                            list2.AddRange(GenRadial.RadialDistinctThingsAround(vector.ToIntVec3(), base.Map, 3f + this.def.projectile.explosionRadius, true).OfType<Pawn>());
                        }
                    }
                    if (compExplosiveCE != null)
                    {
                        compExplosiveCE.Explode(this, vector, base.Map, 1f, direction, list);
                        if (vector.y < 3f)
                        {
                            list2.AddRange(GenRadial.RadialDistinctThingsAround(vector.ToIntVec3(), base.Map, 3f + (compExplosiveCE.props as CompProperties_ExplosiveCE).explosiveRadius, true).OfType<Pawn>());
                        }
                    }
                    foreach (Pawn pawn2 in list2)
                    {
                        this.ApplySuppression(pawn2);
                    }
                }
                this.Destroy(DestroyMode.Vanish);
            }
        }

        private void ApplySuppression(Pawn pawn)
        {
            ShieldBelt shieldBelt = null;
            bool humanlike = pawn.RaceProps.Humanlike;
            if (humanlike)
            {
                List<Apparel> wornApparel = pawn.apparel.WornApparel;
                for (int i = 0; i < wornApparel.Count; i++)
                {
                    ShieldBelt shieldBelt2 = wornApparel[i] as ShieldBelt;
                    bool flag = shieldBelt2 != null;
                    if (flag)
                    {
                        shieldBelt = shieldBelt2;
                        break;
                    }
                }
            }
            CompSuppressable compSuppressable = pawn.TryGetComp<CompSuppressable>();
            bool flag2;
            if (compSuppressable != null)
            {
                Faction faction = pawn.Faction;
                Thing thing = this.launcher;
                if (faction != ((thing != null) ? thing.Faction : null))
                {
                    flag2 = (shieldBelt == null || shieldBelt.ShieldState == ShieldState.Resetting);
                    goto IL_93;
                }
            }
            flag2 = false;
            IL_93:
            bool flag3 = flag2;
            if (flag3)
            {
                this.suppressionAmount = (float)this.def.projectile.GetDamageAmount(1f, null);
                ProjectilePropertiesCE projectilePropertiesCE = this.def.projectile as ProjectilePropertiesCE;
                float num = (projectilePropertiesCE != null) ? projectilePropertiesCE.armorPenetrationSharp : 0f;
                float num2 = (num <= 0f) ? 0f : (1f - Mathf.Clamp(pawn.GetStatValue(CE_StatDefOf.AverageSharpArmor, true) * 0.5f / num, 0f, 1f));
                this.suppressionAmount *= num2;
                compSuppressable.AddSuppression(this.suppressionAmount, this.OriginIV3);
            }
        }
        private Vector2 Vec2Position(float ticks = -1f)
        {
            bool flag = ticks < 0f;
            if (flag)
            {
                ticks = this.fTicks;
            }
            return Vector2.Lerp(this.origin, this.Destination, ticks / this.StartingTicksToImpact);
        }
        private void LogImpact(Thing hitThing, out LogEntry_DamageResult logEntry)
        {
            ThingDef weaponDef = this.equipmentDef ?? ThingDef.Named("Gun_Autopistol");
            logEntry = new BattleLogEntry_RangedImpact(this.launcher, hitThing, this.intendedTarget, weaponDef, this.def, null);
            bool flag = this.launcher is AmmoThing;
            if (flag)
            {
                Find.BattleLog.Add(logEntry);
            }
        }
        public static RulePackDef CookOff
        {
            get
            {
                RulePackDef result;
                if ((result = LaserBeamCE.cookOffDamageEvent) == null)
                {
                    result = (LaserBeamCE.cookOffDamageEvent = DefDatabase<RulePackDef>.GetNamed("DamageEvent_CookOff", true));
                }
                return result;
            }
        }
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

        /*
        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            LaserGunDef defWeapon = equipmentDef as LaserGunDef;
            Vector3 dir = (destination - Origin).normalized;
            dir.y = 0;
            Vector3 a = Origin + dir * (defWeapon == null ? 0.9f : defWeapon.barrelLength);
            Vector3 b = destination;
            a.y = b.y = def.Altitude;
            SpawnBeam(a, b);
            base.DeSpawn(mode);
        }
        */
        /*
        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {

            LaserGunDef defWeapon = equipmentDef as LaserGunDef;
            Vector3 dir = (destination - Origin).normalized;
            dir.y = 0;
            Vector3 a = Origin + dir * (defWeapon == null ? 0.9f : defWeapon.barrelLength);
            Vector3 b = destination;
            // a.y =
            b.y = def.Altitude;
            SpawnBeam(a, b, hitThing);
            IDrawnWeaponWithRotation weapon = null;
            Pawn pawn = launcher as Pawn;
            if (pawn != null && pawn.equipment != null) weapon = pawn.equipment.Primary as IDrawnWeaponWithRotation;
            if (weapon == null)
            {
                Building_LaserGunCE turret = launcher as Building_LaserGunCE;
                if (turret != null)
                {
                    weapon = turret.Gun as IDrawnWeaponWithRotation;
                }
            }
            if (weapon != null)
            {
                float angle = (b - a).AngleFlat() - (destination - a).AngleFlat();
                weapon.RotationOffset = (angle + 180) % 360 - 180;
            }
            
            //if (this.def.impactReflection > 0)
            //{
            //    Vector3 dir = (this.ExactRotation.eulerAngles - origin).normalized;
            //    Rand.PushState();
            //    for (int i = 0; i < this.def.impactReflection; i++)
            //    {
            //        Vector3 c = ExactPosition - dir.RotatedBy(Rand.Range(-35.5f, 35.5f)) * Rand.Range(-1.5f, 1.5f);// 0.8f;
            //        SpawnBeam(b, c);
            //    }
            //    Rand.PopState();
            //}

            base.DeSpawn(mode);
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {

            Vector3 b = destination;
            // a.y =
            b.y = def.Altitude;
            SpawnBeam(Origin, b, hitThing);
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
                float angle = (b - Origin).AngleFlat() - (intendedTarget.DrawPos - Origin).AngleFlat();
                weapon.RotationOffset = (angle + 180) % 360 - 180;
            }
            //if (this.def.impactReflection > 0)
            //{
            //    Vector3 dir = (this.ExactRotation.eulerAngles - origin).normalized;
            //    Rand.PushState();
            //    for (int i = 0; i < this.def.impactReflection; i++)
            //    {
            //        Vector3 c = ExactPosition - dir.RotatedBy(Rand.Range(-35.5f, 35.5f)) * Rand.Range(-1.5f, 1.5f);// 0.8f;
            //        SpawnBeam(b, c);
            //    }
            //    Rand.PopState();
            //}

            base.DeSpawn(mode);
        }
        */
        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {

            LaserGunDef defWeapon = equipmentDef as LaserGunDef;
            Vector3 a = Origin + ShotLine.direction * (defWeapon == null ? 0.9f : defWeapon.barrelLength);
            Vector3 b = ExactPosition;
            a.y = b.y = def.Altitude;
            SpawnBeam(a, b, hitThing);
            IDrawnWeaponWithRotation weapon = null;
            Pawn pawn = launcher as Pawn;
            LocalTargetInfo tgt = null;
            if (pawn != null && pawn.equipment != null)
            {
                if (pawn.TargetCurrentlyAimingAt != null)
                {
                    tgt = pawn.TargetCurrentlyAimingAt;
                }
                weapon = pawn.equipment.Primary as IDrawnWeaponWithRotation;
            }
            if (weapon == null)
            {
                Building_LaserGunCE turret = launcher as Building_LaserGunCE;
                if (turret != null)
                {
                    if (turret.TargetCurrentlyAimingAt != null)
                    {
                        tgt = turret.TargetCurrentlyAimingAt;
                    }
                    weapon = turret.Gun as IDrawnWeaponWithRotation;
                }
            }
            if (weapon != null)
            {
                float angle = (b - Origin).AngleFlat() - (tgt.CenterVector3 - Origin).AngleFlat();
                weapon.RotationOffset = (angle + 180) % 360 - 180;
            }
            /*
            if (this.def.impactReflection > 0)
            {
                Vector3 dir = (this.ExactRotation.eulerAngles - Origin).normalized;
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
        public float DamageAmount
        {
            get
            {
                float result = this.def.projectile.GetDamageAmount(1f, null);
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
                                    return result * 2;
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
                    return result + distance;
                }
                return result;
            }
        }

        public float ArmorPenetration
        {
            get
            {
                ProjectilePropertiesCE projectilePropertiesCE = (ProjectilePropertiesCE)this.def.projectile;
                float result = (this.def.projectile.damageDef.armorCategory == DamageArmorCategoryDefOf.Sharp) ? projectilePropertiesCE.armorPenetrationSharp : projectilePropertiesCE.armorPenetrationBlunt;
                if (this.def.projectile.Volkite())
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

                                    return result / distance;
                                }
                            }
                        }
                    }

                }
                return result;
            }
        }
        public void NotifyImpact(Thing hitThing, Map map, IntVec3 position)
        {
            Bullet bullet = this.GenerateVanillaBullet();
            BulletImpactData impactData = new BulletImpactData
            {
                bullet = bullet,
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
        private Bullet GenerateVanillaBullet()
        {
            Bullet bullet = new Bullet
            {
                def = this.def,
                intendedTarget = this.intendedTarget
            };
            Traverse.Create(bullet).Field("launcher").SetValue(this.launcher);
            return bullet;
        }
        /*
        protected void AddedEffect(Thing hitThing)
        {
            if (def != null && hitThing != null && hitThing is Pawn hitPawn)
            {
                Rand.PushState();
                var rand = Rand.Value; // This is a random percentage between 0% and 100%
                Rand.PopState();
                StatDef ResistHediffStat = def.ResistHediffStat;
                float AddHediffChance = def.AddHediffChance;
                float ResistHediffChance = def.ResistHediffChance;
                if (def.CanResistHediff == true)
                {
                    
                    //if (Def.ResistHediffChance!=0)
                    //{
                    //    rand = rand + Def.ResistHediffChance;
                    //}
                    //else
                    if (def.ResistHediffStat != null)
                    {
                        ResistHediffChance = hitPawn.GetStatValue(ResistHediffStat, true);
                    }
                    AddHediffChance = AddHediffChance * ResistHediffChance;
                }

                if (rand <= AddHediffChance) // If the percentage falls under the chance, success!
                {

                    var effectOnPawn = hitPawn?.health?.hediffSet?.GetFirstHediffOfDef(def.HediffToAdd);
                    Rand.PushState();
                    var randomSeverity = Rand.Range(0.15f, 0.30f);
                    Rand.PopState();
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

                    
                    //MoteMaker.ThrowText(hitThing.PositionHeld.ToVector3(), hitThing.MapHeld, "TST_PlagueBullet_FailureMote".Translate(Def.AddHediffChance), 12f);
                    
                }
            }
        }
        */
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
        private float suppressionAmount;
        private static RulePackDef cookOffDamageEvent = null;
        private static readonly FieldInfo bulletLauncher = typeof(Bullet).GetField("launcher", BindingFlags.Instance | BindingFlags.NonPublic);
    }
}
