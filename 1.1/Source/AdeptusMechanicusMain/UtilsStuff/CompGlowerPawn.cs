using System;
using RimWorld;

namespace Verse
{
    // Token: 0x0200024E RID: 590
    public class CompProperties_GlowerPawn : CompProperties_Glower
    {
        // Token: 0x06000AB1 RID: 2737 RVA: 0x0005598C File Offset: 0x00053D8C
        public CompProperties_GlowerPawn()
        {
            this.compClass = typeof(CompGlowerPawn);
        }
        
    }
    // Token: 0x02000C69 RID: 3177
    public class CompGlowerPawn : CompGlower
    {
        public new CompProperties_GlowerPawn Props
        {
            get
            {
                return (CompProperties_GlowerPawn)this.props;
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

                    map.mapDrawer.MapMeshDirty(this.parent.Position, MapMeshFlag.Things);
                    map.glowGrid.RegisterGlower(this);
                    this.nextUpdateTick = Find.TickManager.TicksGame + 50;

                    map.mapDrawer.MapMeshDirty(this.parent.Position, MapMeshFlag.Things);
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

        // Token: 0x04000001 RID: 1
        public const int updatePeriodInTicks = 50;

        // Token: 0x04000002 RID: 2
        public int nextUpdateTick;
    }
}
