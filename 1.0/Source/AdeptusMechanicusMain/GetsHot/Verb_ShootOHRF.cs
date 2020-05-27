﻿using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    public class Verb_ShootOHRF : Verb_LaunchProjectileOH
    {
        protected override int ShotsPerBurst
        {
            get
            {
                if (caster.Position.InHorDistOf(this.currentTarget.Cell, this.verbProps.range / 2))
                {

                    return this.verbProps.burstShotCount * 2;
                }
                else
                {
                    return this.verbProps.burstShotCount;
                }
            }
        }

        public override void WarmupComplete()
        {
            base.WarmupComplete();
            if (base.CasterIsPawn && base.CasterPawn.skills != null)
            {
                float xp = 6f;
                if (this.currentTarget.Thing != null && this.currentTarget.Thing.def.category == ThingCategory.Pawn)
                {
                    if (this.currentTarget.Thing.HostileTo(this.caster))
                    {
                        xp = 240f;
                    }
                    else
                    {
                        xp = 50f;
                    }
                }
                base.CasterPawn.skills.Learn(SkillDefOf.Shooting, xp);
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
