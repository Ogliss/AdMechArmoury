using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace AdeptusMechanicus
{

    public class DamageWorker_FlameNoCamShake : DamageWorker_Flame
    {
		public override void ExplosionStart(Explosion explosion, List<IntVec3> cellsToAffect)
        {
            if (this.def.explosionHeatEnergyPerCell > 1E-45f)
            {
                GenTemperature.PushHeat(explosion.Position, explosion.Map, this.def.explosionHeatEnergyPerCell * (float)cellsToAffect.Count);
            }
            if (explosion.doVisualEffects)
            {
                FleckMaker.Static(explosion.Position, explosion.Map, FleckDefOf.ExplosionFlash, explosion.radius * 6f);
                if (explosion.Map == Find.CurrentMap)
                {
                    float magnitude = (explosion.Position.ToVector3Shifted() - Find.Camera.transform.position).magnitude;
                    //    Find.CameraDriver.shaker.DoShake(4f * explosion.radius * explosion.screenShakeFactor / magnitude);
                }
                this.ExplosionVisualEffectCenter(explosion);
            }
        }
    }
}
