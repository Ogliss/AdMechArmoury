using Verse;
using RimWorld;
using Verse.Sound;

namespace AdeptusMechanicus
{
	public class Projectile_WC : Projectile
    {
        #region Properties
        private int ticksToDetonation;

        public ThingDef_BulletWC Def
        {
            get
            {
                return this.def as ThingDef_BulletWC;
            }
        }
        #endregion Properties

        #region Overrides

        protected override void Impact(Thing hitThing)
        {
            Pawn pawn = hitThing as Pawn;
            if (hitThing != null)
            {
                Map map = base.Map;
                BattleLogEntry_RangedImpact battleLogEntry_RangedImpact = new BattleLogEntry_RangedImpact(this.launcher, hitThing, this.intendedTarget.Thing, this.equipmentDef, this.def, this.targetCoverDef);
                Find.BattleLog.Add(battleLogEntry_RangedImpact);
                float explodeRoll = Rand.Range(0, 100);
                float explodeChance = Def.DetonationChance;
                string logroll = string.Format("rolled {0} needs less than {1} to detonate", explodeRoll, explodeChance);
            //    Log.Message(logroll);
                if (explodeRoll < explodeChance && hitThing == this.intendedTarget.Thing & hitThing.def.category == ThingCategory.Pawn && hitThing.Spawned && hitThing!=null)
                {
                    if (this.def.projectile.explosionDelay == 0)
                    {

                        this.Destroy(DestroyMode.Vanish);
                        if (this.def.projectile.explosionEffect != null)
                        {
                            Effecter effecter = this.def.projectile.explosionEffect.Spawn();
                            effecter.Trigger(new TargetInfo(base.Position, map, false), new TargetInfo(base.Position, map, false));
                            effecter.Cleanup();
                        }
                        IntVec3 position = base.Position;
                        Map map2 = map;
                        float explosionRadius = Def.blastRadius;
                        DamageDef damageDef = Def.blastdamageDef;
                        Thing launcher = this.launcher;
                        int damageAmount = Def.blastdamageAmount;
                        float hitdmg = (float)base.DamageAmount*Def.blastdamageAmount;
                        float armorPenetration = Def.blastarmorPenetration;
                        SoundDef soundExplode = Def.blastsoundExplode;
                        ThingDef equipmentDef = this.equipmentDef;
                        ThingDef def = this.def;
                        Thing thing = this.intendedTarget.Thing;
                        float y = this.ExactRotation.eulerAngles.y;
                        ThingDef postExplosionSpawnThingDef = this.def.projectile.postExplosionSpawnThingDef;
                        float postExplosionSpawnChance = this.def.projectile.postExplosionSpawnChance;
                        int postExplosionSpawnThingCount = this.def.projectile.postExplosionSpawnThingCount;
                        ThingDef preExplosionSpawnThingDef = this.def.projectile.preExplosionSpawnThingDef;
                        GenExplosion.DoExplosion(position, map2, explosionRadius, damageDef, launcher, damageAmount, armorPenetration, soundExplode, equipmentDef, def, thing, postExplosionSpawnThingDef, postExplosionSpawnChance, postExplosionSpawnThingCount, this.def.projectile.applyDamageToExplosionCellsNeighbors, preExplosionSpawnThingDef, this.def.projectile.preExplosionSpawnChance, this.def.projectile.preExplosionSpawnThingCount, this.def.projectile.explosionChanceToStartFire, this.def.projectile.explosionDamageFalloff);
                        DamageInfo dinfo = new DamageInfo(damageDef, hitdmg, base.ArmorPenetration, y, launcher, null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, this.intendedTarget.Thing);
                        hitThing.TakeDamage(dinfo).AssociateWithLog(battleLogEntry_RangedImpact);
                        string msg = string.Format("{0} was lost to the warp", hitThing.LabelCap);
                        if (pawn.Dead)
                        {
                            pawn.Kill(dinfo);
                        }
                        if (hitThing.Faction == Faction.OfPlayer) {Messages.Message(msg, MessageTypeDefOf.PawnDeath);}
                        if (hitThing.Spawned == true)
                        {
                            if (((hitThing as Corpse)?.InnerPawn ?? hitThing) is Pawn hitPawn)
                                hitPawn.Corpse.Destroy(DestroyMode.KillFinalize);

                        }
                        ///this.intendedTarget.Thing.Destroy(DestroyMode.KillFinalize);


                    }
                    this.landed = true;
                    this.ticksToDetonation = this.def.projectile.explosionDelay;
                    GenExplosion.NotifyNearbyPawnsOfDangerousExplosive(this, this.def.projectile.damageDef, this.launcher.Faction);
                }
                else 
                {
                    if (hitThing != null)
                    {
                        this.Destroy(DestroyMode.Vanish);
                        DamageDef damageDef = this.def.projectile.damageDef;
                        float amount = (float)base.DamageAmount;
                        float armorPenetration = base.ArmorPenetration;
                        float y = this.ExactRotation.eulerAngles.y;
                        Thing launcher = this.launcher;
                        ThingDef equipmentDef = this.equipmentDef;
                        DamageInfo dinfo = new DamageInfo(damageDef, amount, armorPenetration, y, launcher, null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, this.intendedTarget.Thing);
                        hitThing.TakeDamage(dinfo).AssociateWithLog(battleLogEntry_RangedImpact);
                    }
                    else
                    {
                        this.Destroy(DestroyMode.Vanish);
                        SoundDefOf.BulletImpact_Ground.PlayOneShot(new TargetInfo(base.Position, map, false));
                        MoteMaker.MakeStaticMote(this.ExactPosition, map, ThingDefOf.Mote_ShotHit_Dirt, 1f);
                    }
                }
            }
        }

        #endregion Overrides
    }
}
