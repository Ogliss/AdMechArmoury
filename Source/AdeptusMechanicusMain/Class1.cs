using AbilityUser;
using RimWorld;
using System;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
    // Token: 0x020000FE RID: 254
    public class Verb_UseAbilityOG : Verb_UseAbility
    {
        public VerbProperties_AbilityOG VerbProps
        {
            get
            {
                return verbProps as VerbProperties_AbilityOG;
            }
        }


        public override float HighlightFieldRadiusAroundTarget(out bool needLOSToCenter)
        {
            needLOSToCenter = true;
            ThingDef projectile = VerbProps.defaultProjectile;
            if (projectile == null)
            {
                return 0f;
            }
            if (projectile.thingClass == typeof(Projectile_ExplosiveOG))
            {
                ThingDef_BulletExplosiveOG bulletExplosiveOG = (ThingDef_BulletExplosiveOG)this.Projectile;
                return bulletExplosiveOG.explosionradius;
            }
            return projectile.projectile.explosionRadius;
        }

        protected override int ShotsPerBurst
        {
            get
            {
                if (VerbProps.rapidfire == true && caster.Position.InHorDistOf(this.currentTarget.Cell, this.verbProps.range / 2))
                {
                    return this.verbProps.burstShotCount * 2;
                }
                else
                {
                    return this.verbProps.burstShotCount;
                }
                //return this.verbProps.warmupTime;
                //return this.verbProps.defaultCooldownTime;
            }
        }

        public override void WarmupComplete()
        {
            base.WarmupComplete();
            if (base.CasterIsPawn && base.CasterPawn.skills != null)
            {
                float xp = 6f;
                if (this.currentTarget.Thing != null && this.currentTarget.Thing.def.category == ThingCategory.Pawn)
                {
                    if (this.currentTarget.Thing.HostileTo(this.caster))
                    {
                        xp = 240f;
                    }
                    else
                    {
                        xp = 50f;
                    }
                }
                base.CasterPawn.skills.Learn(SkillDefOf.Shooting, xp);
            }
        }


        // Token: 0x0600062E RID: 1582 RVA: 0x00053394 File Offset: 0x00051594
        protected override bool TryCastShot()
        {
            bool flag = base.CasterPawn.equipment.Primary != null && base.CasterPawn.equipment.Primary.def.IsRangedWeapon;
            bool result;
            if (flag)
            {

                int logcount = 0;
                bool logging = VerbProps.logging;
                bool canDamageWeapon = VerbProps.canDamageWeapon;
                float extraWeaponDamage = VerbProps.extraWeaponDamage;
                bool canJam = VerbProps.canJam;
                logcount++;
                string msg = string.Format("");
                string lmsg = string.Format("log {0}", logcount);
                string reliabilityString;
                float jamsOn;
                if (canJam)
                {
                    StatPart_Reliability.GetReliability((ThingDef_GunOG)CasterPawn.equipment.Primary, out reliabilityString, out jamsOn);
                    logcount++;
                    if (logging == true) { Log.Message(lmsg); }
                    jamsOn = jamsOn++;
                }
                else
                {
                    jamsOn = 0;
                }
                float jamRoll = 0;
                logcount++;
                lmsg = string.Format("log {0} jamsOn {1}", logcount, jamsOn);
                if (logging && canJam) { Log.Message(lmsg); }
                if (VerbProps.overheat == true) { jamRoll = (Rand.Range(0, 100)); }
                else { jamRoll = (Rand.Range(0, 1000)) / 10f; }
                logcount++;
                lmsg = string.Format("log {0} jamRoll {1}", logcount, jamRoll);
                if (logging && canJam) { Log.Message(lmsg); }
                if (jamRoll < jamsOn && canJam == true)
                {
                    logcount++;
                    lmsg = string.Format("log {0} VerbPropsCP.overheat {1}", logcount, VerbProps.overheat);
                    if (logging == true) { Log.Message(lmsg); }
                    if (VerbProps.overheat == true)
                    {
                        DamageDef damageDef = Projectile.projectile.damageDef;
                        HediffDef HediffToAdd = damageDef.hediff;
                        float ArmorPenetration = Projectile.projectile.GetArmorPenetration(EquipmentSource, null);
                        float overheatsOn = VerbProps.overheatsOn;
                        logcount++;
                        lmsg = string.Format("log {0} overheatsOn {1}", logcount, overheatsOn);
                        if (logging == true) { Log.Message(lmsg); }
                        int DamageAmount = 0;
                        float overheatRoll = (Rand.Range(0, 1000)) / 10f;
                        logcount++;
                        lmsg = string.Format("log {0} overheatRoll {1}", logcount, overheatRoll);
                        if (logging == true) { Log.Message(lmsg); }
                        Pawn launcherPawn = caster as Pawn;
                        if (overheatRoll < overheatsOn)
                        {
                            DamageAmount = Projectile.projectile.GetDamageAmount(EquipmentSource, null);
                            msg = string.Format("{0}'s {1} critically overheated. ({2}/{3}) causing {4} damage", caster.LabelCap, EquipmentSource.LabelCap, jamRoll, jamsOn, DamageAmount);
                            if (VerbProps.criticaloverheatExplosion == true) { CriticalOverheatExplosion(); }
                        }
                        else
                        {
                            DamageAmount = Projectile.projectile.GetDamageAmount(EquipmentSource, null) / 10;
                            msg = string.Format("{0}'s {1} overheated. ({2}/{3}) causing {4} damage", caster.LabelCap, EquipmentSource.LabelCap, jamRoll, jamsOn, DamageAmount);
                        }
                        var overheatOnPawn = launcherPawn?.health?.hediffSet?.GetFirstHediffOfDef(HediffToAdd);
                        if (overheatOnPawn != null)
                        {
                            overheatOnPawn.Severity += DamageAmount;
                        }
                        else
                        {
                            foreach (var part in launcherPawn.RaceProps.body.AllParts.Where(x => x.def.labelShort == "Hand"))
                            {
                                logcount++;
                                lmsg = string.Format("log {0} part.def.hitPoints {1}", logcount, launcherPawn.health.hediffSet.PartIsMissing(part));
                                if (logging == true) { Log.Message(lmsg); }
                                if (launcherPawn.health.hediffSet.PartIsMissing(part) == false)
                                {
                                    logcount++;
                                    lmsg = string.Format("log {0} part.customLabel {1}", logcount, part.def.hitPoints);
                                    if (logging == true) { Log.Message(lmsg); }
                                    Hediff hediff = HediffMaker.MakeHediff(HediffToAdd, launcherPawn, null);
                                    hediff.Severity = Rand.Range(0, DamageAmount);
                                    launcherPawn.health.AddHediff(hediff, part, null);
                                }
                            }
                        }
                        Messages.Message(msg, MessageTypeDefOf.NegativeHealthEvent);
                    }
                    else
                    {
                        msg = string.Format("{0}'s {1} had a weapon jam. ({2}/{3})", caster.LabelCap, EquipmentSource.LabelCap, jamRoll, jamsOn);
                        Messages.Message(msg, MessageTypeDefOf.SilentInput);
                    }
                    if (EquipmentSource.HitPoints > 0)
                    {
                        EquipmentSource.HitPoints--;
                    }
                    float defaultCooldownTime = this.verbProps.defaultCooldownTime * 2;
                    return false;
                }
                if (canDamageWeapon)
                {
                    if (extraWeaponDamage != 0f)
                    {
                        if (EquipmentSource.HitPoints - (int)extraWeaponDamage >= 0)
                        {
                            EquipmentSource.HitPoints = EquipmentSource.HitPoints - (int)extraWeaponDamage;
                        }
                        else if (EquipmentSource.HitPoints - (int)extraWeaponDamage < 0)
                        {
                            EquipmentSource.HitPoints = 0;
                        }
                    }
                    else
                    {
                        if (EquipmentSource.HitPoints > 0)
                        {
                            EquipmentSource.HitPoints--;
                        }
                    }
                }

                Thing primary = base.CasterPawn.equipment.Primary;
                ThingDef defaultProjectile = primary.def.Verbs.FirstOrDefault<VerbProperties>().defaultProjectile;
                Type thingClass = defaultProjectile.thingClass;
                defaultProjectile.thingClass = this.Projectile.thingClass;
                bool flag2 = false;
                SoundInfo info = SoundInfo.InMap(new TargetInfo(base.CasterPawn.Position, base.CasterPawn.Map, false), MaintenanceType.None);
                SoundDef.Named(primary.def.Verbs.FirstOrDefault<VerbProperties>().soundCast.ToString()).PlayOneShot(info);
                bool? flag3 = base.TryLaunchProjectile(defaultProjectile, this.currentTarget);
                bool flag4 = flag3 != null;
                bool flag5 = flag4;
                if (flag5)
                {
                    bool? flag6 = flag3;
                    bool flag7 = true;
                    bool flag8 = flag6.GetValueOrDefault() == flag7 & flag6 != null;
                    bool flag9 = flag8;
                    if (flag9)
                    {
                        flag2 = true;
                    }
                    flag6 = flag3;
                    flag7 = false;
                    bool flag10 = flag6.GetValueOrDefault() == flag7 & flag6 != null;
                    bool flag11 = flag10;
                    if (flag11)
                    {
                        flag2 = false;
                    }
                }
                this.PostCastShot(flag2, out flag2);
                bool flag12 = !flag2;
                bool flag13 = flag12;
                if (flag13)
                {
                    base.Ability.Notify_AbilityFailed(base.UseAbilityProps.refundsPointsAfterFailing);
                }
                defaultProjectile.thingClass = thingClass;
                this.burstShotsLeft = 0;
                result = flag2;
            }
            else
            {
                Messages.Message("MustHaveRangedWeapon".Translate(base.CasterPawn.LabelCap), MessageTypeDefOf.RejectInput, true);
                result = false;
            }
            return result;
        }

        /*
         
        // Token: 0x0600062E RID: 1582 RVA: 0x00053394 File Offset: 0x00051594
        protected override bool TryCastShot()
        {
            bool flag = base.CasterPawn.equipment.Primary != null && base.CasterPawn.equipment.Primary.def.IsRangedWeapon;
            bool result;
            if (flag)
            {

                Thing primary = base.CasterPawn.equipment.Primary;
                ThingDef defaultProjectile = primary.def.Verbs.FirstOrDefault<VerbProperties>().defaultProjectile;
                Type thingClass = defaultProjectile.thingClass;
                defaultProjectile.thingClass = this.Projectile.thingClass;
                bool flag2 = false;
                SoundInfo info = SoundInfo.InMap(new TargetInfo(base.CasterPawn.Position, base.CasterPawn.Map, false), MaintenanceType.None);
                SoundDef.Named(primary.def.Verbs.FirstOrDefault<VerbProperties>().soundCast.ToString()).PlayOneShot(info);
                bool? flag3 = base.TryLaunchProjectile(defaultProjectile, this.currentTarget);
                bool flag4 = flag3 != null;
                bool flag5 = flag4;
                if (flag5)
                {
                    bool? flag6 = flag3;
                    bool flag7 = true;
                    bool flag8 = flag6.GetValueOrDefault() == flag7 & flag6 != null;
                    bool flag9 = flag8;
                    if (flag9)
                    {
                        flag2 = true;
                    }
                    flag6 = flag3;
                    flag7 = false;
                    bool flag10 = flag6.GetValueOrDefault() == flag7 & flag6 != null;
                    bool flag11 = flag10;
                    if (flag11)
                    {
                        flag2 = false;
                    }
                }
                this.PostCastShot(flag2, out flag2);
                bool flag12 = !flag2;
                bool flag13 = flag12;
                if (flag13)
                {
                    base.Ability.Notify_AbilityFailed(base.UseAbilityProps.refundsPointsAfterFailing);
                }
                defaultProjectile.thingClass = thingClass;
                this.burstShotsLeft = 0;
                result = flag2;
            }
            else
            {
                Messages.Message("MustHaveRangedWeapon".Translate(base.CasterPawn.LabelCap), MessageTypeDefOf.RejectInput, true);
                result = false;
            }
            return result;
        }
 
         */

        public virtual void CriticalOverheatExplosion()
        {
            int logcount = 0;
            //bool logging = VerbProps.logging;
            bool logging = true;
            string lmsg = string.Format("log {0}", logcount);
            logcount++; lmsg = string.Format("log {0}", logcount); if (logging == true) { Log.Message(lmsg); }
            Map map = caster.Map;
            logcount++; lmsg = string.Format("log {0} EquipmentSource.def.projectile.explosionEffect: {1}", logcount, Projectile.projectile.explosionEffect); if (logging == true) { Log.Message(lmsg); }
            if (Projectile.projectile.explosionEffect != null)
            {
                Effecter effecter = Projectile.projectile.explosionEffect.Spawn();
                effecter.Trigger(new TargetInfo(EquipmentSource.Position, map, false), new TargetInfo(EquipmentSource.Position, map, false));
                effecter.Cleanup();
            }
            IntVec3 position = caster.Position;
            Map map2 = map;
            float explosionRadius = Projectile.projectile.explosionRadius;
            DamageDef damageDef = Projectile.projectile.damageDef;
            Thing launcher = EquipmentSource;
            int DamageAmount = Projectile.projectile.GetDamageAmount(EquipmentSource, null);
            logcount++; lmsg = string.Format("log {0} DamageAmount {1}", logcount, DamageAmount); if (logging == true) { Log.Message(lmsg); }
            float ArmorPenetration = Projectile.projectile.GetArmorPenetration(EquipmentSource, null);
            SoundDef soundExplode = Projectile.projectile.soundExplode;
            ThingDef equipmentDef = EquipmentSource.def;
            ThingDef def = EquipmentSource.def;
            Thing thing = EquipmentSource;
            ThingDef postExplosionSpawnThingDef = Projectile.projectile.postExplosionSpawnThingDef;
            float postExplosionSpawnChance = Projectile.projectile.postExplosionSpawnChance;
            int postExplosionSpawnThingCount = Projectile.projectile.postExplosionSpawnThingCount;
            ThingDef preExplosionSpawnThingDef = Projectile.projectile.preExplosionSpawnThingDef;
            GenExplosion.DoExplosion(position, map2, explosionRadius, damageDef, launcher, DamageAmount, ArmorPenetration, soundExplode);//, equipmentDef, def, thing, postExplosionSpawnThingDef, postExplosionSpawnChance, postExplosionSpawnThingCount, EquipmentSource.def.projectile.applyDamageToExplosionCellsNeighbors, preExplosionSpawnThingDef, EquipmentSource.def.projectile.preExplosionSpawnChance, EquipmentSource.def.projectile.preExplosionSpawnThingCount, EquipmentSource.def.projectile.explosionChanceToStartFire, EquipmentSource.def.projectile.explosionDamageFalloff);
            return;
        }

        protected virtual float Reliable
        {
            get
            {
                return EquipmentSource.GetStatValue(StatDefOf_OG.reliability);
            }
        }

    }
}
