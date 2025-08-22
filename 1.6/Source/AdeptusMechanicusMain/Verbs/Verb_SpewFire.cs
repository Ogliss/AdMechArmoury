using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{

    // AdeptusMechanicus.Verb_SpewFire
    public class Verb_SpewFire : Verb
    {
        public AdvancedVerbProperties Props => this.verbProps as AdvancedVerbProperties;    
        public override bool TryCastShot()
        {
            if (this.currentTarget.HasThing && this.currentTarget.Thing.Map != this.caster.Map)
            {
                return false;
            }
            if (base.EquipmentSource != null)
            {
                CompChangeableProjectile comp = base.EquipmentSource.GetComp<CompChangeableProjectile>();
                if (comp != null)
                {
                    comp.Notify_ProjectileLaunched();
                }
                CompApparelReloadable comp2 = base.EquipmentSource.GetComp<CompApparelReloadable>();
                if (comp2 != null)
                {
                    comp2.UsedOnce();
                }
            }
            IntVec3 position = this.caster.Position;
            DamageDef damageDef = verbProps.defaultProjectile?.projectile.damageDef ?? DamageDefOf.Flame;
            int damage = verbProps.defaultProjectile?.projectile.damageAmountBase ?? -1;
            float armorP = verbProps.defaultProjectile?.projectile.GetArmorPenetration(EquipmentSource) ?? -1f;
            EffecterDef effecter = verbProps.sprayEffecterDef ?? EffecterDefOf.Fire_SpewShort;
        //    Log.Message($"{this.caster} am using the right verb! DamageDef: {damageDef.LabelCap} EffecterDef: {effecter.defName}");
            float num = Mathf.Atan2((float)(-(float)(this.currentTarget.Cell.z - position.z)), (float)(this.currentTarget.Cell.x - position.x)) * 57.29578f;
            FloatRange value = new FloatRange(num - 13f, num + 13f);
            GenExplosion.DoExplosion(position, this.caster.MapHeld, this.verbProps.range, damageDef, this.caster, damage, armorP, verbProps.defaultProjectile?.projectile.soundExplode ?? null, this.EquipmentSource?.def, verbProps.defaultProjectile, null, verbProps.spawnDef, verbProps.defaultProjectile?.projectile.postExplosionSpawnChance ?? 1f, verbProps.defaultProjectile?.projectile.postExplosionSpawnThingCount ?? 1, null, null, 255, false, null, 0f, 1, 0, false, null, null, new FloatRange?(value), false, 0.3f, 0f, verbProps.defaultProjectile?.projectile.soundExplode != null ? true : false , null, 1f, null, null);
            base.AddEffecterToMaintain(effecter.Spawn(this.caster.Position, this.currentTarget.Cell, this.caster.Map, 1f), this.caster.Position, this.currentTarget.Cell, 14 + (int)verbProps.range, this.caster.Map);
            this.lastShotTick = Find.TickManager.TicksGame;
            return true;
        }

        public override bool Available()
        {
            if (!base.Available())
            {
                return false;
            }
            if (this.CasterIsPawn)
            {
                Pawn casterPawn = this.CasterPawn;
                if (casterPawn.Faction != Faction.OfPlayer && casterPawn.mindState.MeleeThreatStillThreat && casterPawn.mindState.meleeThreat.Position.AdjacentTo8WayOrInside(casterPawn.Position))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
