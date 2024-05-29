using AdeptusMechanicus;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
    public class DamageWorker_ExtinguishNoCamShake : DamageWorker
    {
        public override DamageWorker.DamageResult Apply(DamageInfo dinfo, Thing victim)
        {
            DamageWorker.DamageResult result = new DamageWorker.DamageResult();
            Fire fire = victim as Fire;
            if (fire == null || fire.Destroyed)
            {
                Thing thing = (victim != null) ? victim.GetAttachment(ThingDefOf.Fire) : null;
                if (thing != null)
                {
                    fire = (Fire)thing;
                }
            }
            if (fire != null && !fire.Destroyed)
            {
                base.Apply(dinfo, victim);
                fire.fireSize -= dinfo.Amount * 0.01f;
                if (fire.fireSize < 0.1f)
                {
                    fire.Destroy(DestroyMode.Vanish);
                }
            }
            Pawn pawn = victim as Pawn;
            if (pawn != null)
            {
                Hediff hediff = HediffMaker.MakeHediff(dinfo.Def.hediff, pawn, null);
                hediff.Severity = dinfo.Amount;
                pawn.health.AddHediff(hediff, null, new DamageInfo?(dinfo), null);
            }
            return result;
        }

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

        private const float DamageAmountToFireSizeRatio = 0.01f;
    }
}
