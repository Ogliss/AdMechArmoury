using System;
using System.Collections.Generic;
using System.Linq;
using AdeptusMechanicus.settings;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
	
    // Token: 0x02000025 RID: 37
    public class Verb_ShootEquipment : Verb_UseEquipment
    {
        // Token: 0x170006AA RID: 1706
        // (get) Token: 0x0600219B RID: 8603 RVA: 0x000CC040 File Offset: 0x000CA240
        bool RapidFire => verbProperties.RapidFire;
        float RapidFireRange => verbProperties.range / 2;
        bool BodyBurstSize => verbProperties.TyranidBurstBodySize;

        protected override int ShotsPerBurst
        {
            get
            {
                if (RapidFire && AMASettings.Instance.AllowRapidFire)
                {
                    if (caster.Position.InHorDistOf(((Pawn)caster).TargetCurrentlyAimingAt.Cell, RapidFireRange))
                    {
                        return verbProperties.burstShotCount * 2;
                    }
                }
                return this.verbProps.burstShotCount;
            }
        }

        // Token: 0x0600219C RID: 8604 RVA: 0x000CC050 File Offset: 0x000CA250
        public override void WarmupComplete()
        {
            base.WarmupComplete();
            Pawn pawn = this.currentTarget.Thing as Pawn;
            if (pawn != null && !pawn.Downed && this.CasterIsPawn && this.CasterPawn.skills != null)
            {
                float num = pawn.HostileTo(this.caster) ? 170f : 20f;
                float num2 = this.verbProps.AdjustedFullCycleTime(this, this.CasterPawn);
                this.CasterPawn.skills.Learn(SkillDefOf.Shooting, num * num2, false);
            }
        }

        // Token: 0x0600219D RID: 8605 RVA: 0x000CC0DB File Offset: 0x000CA2DB
        protected override bool TryCastShot()
        {
            bool flag = base.TryCastShot();
            if (flag && this.CasterIsPawn)
            {
                this.CasterPawn.records.Increment(RecordDefOf.ShotsFired);
            }
            return flag;
        }
    }
}
