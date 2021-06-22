using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace AdeptusMechanicus
{
    public class DamageWorker_BombNoCamShake : DamageWorker_AddInjury
    {
        public override void ExplosionStart(Explosion explosion, List<IntVec3> cellsToAffect)
        {
            if (this.def.explosionHeatEnergyPerCell > 1.401298E-45f)
            {
                GenTemperature.PushHeat(explosion.Position, explosion.Map, this.def.explosionHeatEnergyPerCell * (float)cellsToAffect.Count);
            }
            AdeptusMoteMaker.MakeStaticMote(explosion.DrawPos, explosion.Map, ThingDefOf.Mote_ExplosionFlash, explosion.radius * 6f);
        }
    }
}
