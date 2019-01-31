using System;
using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
	// Token: 0x02001014 RID: 4116
	public class Verb_ShootRF : Verb_LaunchProjectile
	{
        protected override int ShotsPerBurst
        {
            get
            {
                if (caster.Position.InHorDistOf(this.currentTarget.Cell, this.verbProps.range / 2))
                {

                    return this.verbProps.ticksBetweenBurstShots;
                }
                else
                {
                    return this.verbProps.burstShotCount;
                }
            }
        }

        public override void WarmupComplete()
        {
            Pawn pawn = this.currentTarget.Thing as Pawn;
            base.WarmupComplete();

            if (pawn != null && !pawn.Downed && base.CasterIsPawn && base.CasterPawn.skills != null)
			{
				float num = (!pawn.HostileTo(this.caster)) ? 20f : 170f;
				float num2 = this.verbProps.AdjustedFullCycleTime(this, base.CasterPawn);
				base.CasterPawn.skills.Learn(SkillDefOf.Shooting, num * num2, false);
			}
		}

		protected override bool TryCastShot()
		{
			bool flag = base.TryCastShot();
			if (flag && base.CasterIsPawn)
			{
				base.CasterPawn.records.Increment(RecordDefOf.ShotsFired);
			}
			return flag;
		}
	}
}
