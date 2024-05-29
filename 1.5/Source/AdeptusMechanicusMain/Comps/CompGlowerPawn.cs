using System;
using AdeptusMechanicus;
using RimWorld;

namespace Verse
{
    public class CompProperties_GlowerPawn : CompProperties_Glower
    {
        public CompProperties_GlowerPawn()
        {
            this.compClass = typeof(CompGlowerPawn);
        }
        
    }

    public class CompGlowerPawn : CompGlower
    {
        public new CompProperties_GlowerPawn Props
        {
            get
            {
                return (CompProperties_GlowerPawn)this.props;
            }
        }
        public override void PostPreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
        {
            base.PostPreApplyDamage(ref dinfo, out absorbed);
        }
        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            if (dinfo.HitPart?.parent?.def == AdeptusBodyPartDefOf.OG_SwarmCore && dinfo.HitPart.def.spawnThingOnRemoved is ThingDef thingDef && thingDef?.race != null)
            {
                PawnKindDef kindDef = thingDef.race.AnyPawnKind;
                if (kindDef != null)
                {

                }
            }
        }

        public IntVec3 vec3 = IntVec3.Invalid;

        public override void CompTick()
        {
            base.CompTick();
            Map map = this.parent.Map;
            if (map!=null)
            {
                IntVec3 @int = this.parent.Position;
                if ((vec3 == IntVec3.Invalid || (vec3 != IntVec3.Invalid && vec3 != @int)) && Find.TickManager.TicksGame >= this.nextUpdateTick)
                {
                    map.glowGrid.DeRegisterGlower(this);

                    map.mapDrawer.MapMeshDirty(this.parent.Position, MapMeshFlagDefOf.Things);
                    map.glowGrid.RegisterGlower(this);
                    this.nextUpdateTick = Find.TickManager.TicksGame + 50;

                    map.mapDrawer.MapMeshDirty(this.parent.Position, MapMeshFlagDefOf.Things);
                }
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                Rand.PushState();
                this.nextUpdateTick = Find.TickManager.TicksGame + Rand.Range(0, 100);
                Rand.PopState();
            }

        }

        public override void PostDeSpawn(Map map)
        {
            map.glowGrid.DeRegisterGlower(this);
            base.PostDeSpawn(map);
        }

        public const int updatePeriodInTicks = 50;

        public int nextUpdateTick;
    }
}
