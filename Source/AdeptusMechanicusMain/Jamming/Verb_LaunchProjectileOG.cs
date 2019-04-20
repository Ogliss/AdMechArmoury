using RimWorld;
using System.Linq;
using Verse;


namespace AdeptusMechanicus
{
    public class Verb_LaunchProjectileOG : Verb_LaunchProjectile
    {
        protected virtual float Reliable
        {
            get
            {
                return EquipmentSource.GetStatValue(StatDefOf_OG.reliability);
            }
        }

        public VerbPropertiesOG VerbProps
        {
            get
            {
                return verbProps as VerbPropertiesOG;
            }
        }

        protected override bool TryCastShot()
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
            StatPart_Reliability.GetReliability((ThingDef_GunOG)EquipmentSource, out reliabilityString, out jamsOn);
            logcount++;
            if (logging == true) { Log.Message(lmsg); }
            jamsOn = jamsOn++;
            float jamRoll = 0;
            logcount++;
            lmsg = string.Format("log {0} jamsOn {1}", logcount, jamsOn);
            if (logging == true) { Log.Message(lmsg); }
            if (VerbProps.overheat == true) { jamRoll = (Rand.Range(0, 100)); }
            else { jamRoll = (Rand.Range(0, 1000)) / 10f; }
            logcount++;
            lmsg = string.Format("log {0} jamRoll {1}", logcount, jamRoll);
            if (logging == true) { Log.Message(lmsg); }
            if (jamRoll < jamsOn && canJam==true)
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
                if (EquipmentSource.HitPoints>0)
                {
                    EquipmentSource.HitPoints--;
                }
                float defaultCooldownTime = this.verbProps.defaultCooldownTime*2;
                return false;
            }
            if (canDamageWeapon)
            {
                if (extraWeaponDamage != 0f)
                {
                    if (EquipmentSource.HitPoints - (int)extraWeaponDamage>=0)
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
            return base.TryCastShot();
        }
        public virtual void CriticalOverheatExplosion()
        {
            int logcount = 0;
            //bool logging = VerbPropsOG.logging;
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
    }
}
