using RimWorld;
using System.Linq;
using Verse;


namespace AdeptusMechanicus
{
    public class Verb_LaunchProjectileOH : Verb_LaunchProjectile
    {
        public HediffDef HediffToAdd = OGHediffDefOf.PlasmaBurn;
        protected virtual float overheat
        {
            get
            {
                return EquipmentSource.GetStatValue(StatDefOf_OH.overheat);
            }
        }

        protected override bool TryCastShot()
        {
            string overheatString;
            float overheatsOn;
            StatPart_Overheat.GetOverheat((ThingDef_GunOH)EquipmentSource, out overheatString, out overheatsOn);
            float overheatRoll = (Rand.Range(0, 1000))/10f;
            //float overheatRoll = Rand.Range(0, 100);
            Pawn launcherPawn = caster as Pawn;
            if (overheatRoll < overheatsOn)
            {
                int DamageAmount = 0;
                if (overheatRoll < overheatsOn*2)
                {
                    DamageDef damageDef = Projectile.projectile.damageDef;
                    DamageAmount = Projectile.projectile.GetDamageAmount(EquipmentSource, null);
                    float ArmorPenetration = Projectile.projectile.GetArmorPenetration(EquipmentSource, null);
                    string msg = string.Format("{0}'s {1} critically overheated. ({2}/{3}) causing {4} damage", caster.LabelCap, EquipmentSource.LabelCap, overheatRoll, overheatsOn, DamageAmount);
                    EquipmentSource.HitPoints--;
                    Messages.Message(msg, MessageTypeDefOf.NegativeHealthEvent);

                    var overheatOnPawn = launcherPawn?.health?.hediffSet?.GetFirstHediffOfDef(HediffToAdd);
                    if (overheatOnPawn != null)
                    {
                        overheatOnPawn.Severity += DamageAmount;
                    }
                    else
                    {
                        foreach (var part in launcherPawn.RaceProps.body.AllParts.Where(x => x.def.defName == "Hand"))
                        {
                            Hediff hediff = HediffMaker.MakeHediff(HediffToAdd, launcherPawn, null);
                            hediff.Severity = DamageAmount;
                            launcherPawn.health.AddHediff(hediff, part, null);
                        }
                    }
                }
                else
                {
                    DamageDef damageDef = Projectile.projectile.damageDef;
                    DamageAmount = Projectile.projectile.GetDamageAmount(EquipmentSource, null)/10;
                    float ArmorPenetration = Projectile.projectile.GetArmorPenetration(EquipmentSource, null);
                    string msg = string.Format("{0}'s {1} overheated. ({2}/{3}) causing {4} damage", caster.LabelCap, EquipmentSource.LabelCap, overheatRoll, overheatsOn, DamageAmount);
                    EquipmentSource.HitPoints--;
                    Messages.Message(msg, MessageTypeDefOf.NegativeHealthEvent);


                    var overheatOnPawn = launcherPawn?.health?.hediffSet?.GetFirstHediffOfDef(HediffToAdd);
                    if (overheatOnPawn != null)
                    {
                        overheatOnPawn.Severity += DamageAmount;
                    }
                    else
                    {
                        foreach (var part in launcherPawn.RaceProps.body.AllParts.Where(x => x.def.defName == "Hand"))
                        {
                            Hediff hediff = HediffMaker.MakeHediff(HediffToAdd, launcherPawn, null);
                            hediff.Severity = DamageAmount;
                            launcherPawn.health.AddHediff(hediff, part, null);
                        }
                    }
                }
                return false;
            }
            return base.TryCastShot();
        }

        public virtual void CriticalOverheatExplosion()
        {
            Map map = EquipmentSource.Map;
            if (EquipmentSource.def.projectile.explosionEffect != null)
            {
                Effecter effecter = Projectile.projectile.explosionEffect.Spawn();
                effecter.Trigger(new TargetInfo(EquipmentSource.Position, map, false), new TargetInfo(EquipmentSource.Position, map, false));
                effecter.Cleanup();
            }
            IntVec3 position = EquipmentSource.Position;
            Map map2 = map;
            float explosionRadius = EquipmentSource.def.projectile.explosionRadius;
            DamageDef damageDef = EquipmentSource.def.projectile.damageDef;
            Thing launcher = EquipmentSource;
            int DamageAmount = Projectile.projectile.GetDamageAmount(EquipmentSource, null);
            float ArmorPenetration = Projectile.projectile.GetArmorPenetration(EquipmentSource, null);
            SoundDef soundExplode = EquipmentSource.def.projectile.soundExplode;
            ThingDef equipmentDef = EquipmentSource.def;
            ThingDef def = EquipmentSource.def;
            Thing thing = EquipmentSource;
            ThingDef postExplosionSpawnThingDef = EquipmentSource.def.projectile.postExplosionSpawnThingDef;
            float postExplosionSpawnChance = EquipmentSource.def.projectile.postExplosionSpawnChance;
            int postExplosionSpawnThingCount = EquipmentSource.def.projectile.postExplosionSpawnThingCount;
            ThingDef preExplosionSpawnThingDef = EquipmentSource.def.projectile.preExplosionSpawnThingDef;
            GenExplosion.DoExplosion(position, map2, explosionRadius, damageDef, launcher, DamageAmount, ArmorPenetration, soundExplode, equipmentDef, def, thing, postExplosionSpawnThingDef, postExplosionSpawnChance, postExplosionSpawnThingCount, EquipmentSource.def.projectile.applyDamageToExplosionCellsNeighbors, preExplosionSpawnThingDef, EquipmentSource.def.projectile.preExplosionSpawnChance, EquipmentSource.def.projectile.preExplosionSpawnThingCount, EquipmentSource.def.projectile.explosionChanceToStartFire, EquipmentSource.def.projectile.explosionDamageFalloff);
        }
    }
}
