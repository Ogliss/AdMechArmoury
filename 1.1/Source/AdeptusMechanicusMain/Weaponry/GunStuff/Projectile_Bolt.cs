using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    public class Projectile_Bolt : Projectile_Trailer
    {
        public override string trailDef => "OG_Mote_BoltTrailPuff";
        public override void Draw()
        {
            Mesh mesh = MeshPool.GridPlane(this.def.graphicData.drawSize);
            Mesh mesh2 = MeshPool.GridPlane(DefDatabase<ThingDef>.GetNamed("Mote_PlasmaGlow").graphicData.drawSize*2.5f);
            Graphics.DrawMesh(mesh, this.DrawPos, this.ExactRotation, Graphic.MatSingle, 0);
            Graphics.DrawMesh(mesh2, this.DrawPos, this.ExactRotation, DefDatabase<ThingDef>.GetNamed("Mote_PlasmaGlow").graphic.MatSingle, 0);
            base.Comps_PostDraw();
        }

        // Token: 0x060052C6 RID: 21190 RVA: 0x0026286F File Offset: 0x00260C6F
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.ticksToDetonation, "ticksToDetonation", 0, false);
        }

        // Token: 0x060052C7 RID: 21191 RVA: 0x00262889 File Offset: 0x00260C89
        public override void Tick()
        {
            base.Tick();
            if (this.ticksToDetonation > 0)
            {
                this.ticksToDetonation--;
                if (this.ticksToDetonation <= 0)
                {
                    this.Explode();
                }
            }
        }

        // Token: 0x060052C8 RID: 21192 RVA: 0x002628C0 File Offset: 0x00260CC0
        protected override void Impact(Thing hitThing)
        {
            if (this.def.projectile.explosionRadius>0)
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
            base.Impact(hitThing);
        }

        // Token: 0x060052C9 RID: 21193 RVA: 0x00262928 File Offset: 0x00260D28
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
            GenExplosion.DoExplosion(position, map2, explosionRadius, damageDef, launcher, damageAmount, armorPenetration, soundExplode, equipmentDef, def, thing, postExplosionSpawnThingDef, postExplosionSpawnChance, postExplosionSpawnThingCount, this.def.projectile.applyDamageToExplosionCellsNeighbors, preExplosionSpawnThingDef, this.def.projectile.preExplosionSpawnChance, this.def.projectile.preExplosionSpawnThingCount, this.def.projectile.explosionChanceToStartFire, this.def.projectile.explosionDamageFalloff);
        }

        // Token: 0x04003719 RID: 14105
        private int ticksToDetonation;
    }
}
