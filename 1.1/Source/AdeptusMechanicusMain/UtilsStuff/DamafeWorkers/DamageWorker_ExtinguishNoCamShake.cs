using System.Collections.Generic;
using Verse;

namespace RimWorld
{
    // Token: 0x02000D55 RID: 3413
    public class DamageWorker_ExtinguishNoCamShake : DamageWorker
    {
        // Token: 0x06004BBA RID: 19386 RVA: 0x00236704 File Offset: 0x00234B04
        public override DamageWorker.DamageResult Apply(DamageInfo dinfo, Thing victim)
        {
            DamageWorker.DamageResult result = new DamageWorker.DamageResult();
            Fire fire = victim as Fire;
            if (fire == null || fire.Destroyed)
            {
                return result;
            }
            base.Apply(dinfo, victim);
            fire.fireSize -= dinfo.Amount * 0.01f;
            if (fire.fireSize <= 0.1f)
            {
                fire.Destroy(DestroyMode.Vanish);
            }
            return result;
        }

        public override void ExplosionStart(Explosion explosion, List<IntVec3> cellsToAffect)
        {
            if (this.def.explosionHeatEnergyPerCell > 1.401298E-45f)
            {
                GenTemperature.PushHeat(explosion.Position, explosion.Map, this.def.explosionHeatEnergyPerCell * (float)cellsToAffect.Count);
            }
            MoteMaker.MakeStaticMote(explosion.Position, explosion.Map, ThingDefOf.Mote_ExplosionFlash, explosion.radius * 6f);
        }
        
        // Token: 0x04003371 RID: 13169
        private const float DamageAmountToFireSizeRatio = 0.01f;
    }
}
