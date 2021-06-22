using System;
using System.Collections.Generic;
using HoloEmitters.Things;
using RimWorld;
using Verse;
using Verse.AI;

namespace HoloEmitters
{
    // Token: 0x02000053 RID: 83
    public class JobDriver_ScanAtEmitter : JobDriver
    {
        // Token: 0x06000128 RID: 296 RVA: 0x0000C034 File Offset: 0x0000A434
        public JobDriver_ScanAtEmitter()
        {
            this.rotateToFace = (Verse.AI.TargetIndex)2;
        }

        // Token: 0x17000019 RID: 25
        // (get) Token: 0x06000129 RID: 297 RVA: 0x0000C044 File Offset: 0x0000A444
        private Corpse Corpse
        {
            get
            {
                return (Corpse)base.TargetA;
            }
        }

        // Token: 0x1700001A RID: 26
        // (get) Token: 0x0600012A RID: 298 RVA: 0x0000C074 File Offset: 0x0000A474
        private HoloEmitter Emitter
        {
            get
            {
                return (HoloEmitter)base.TargetB;
            }
        }

        // Token: 0x0600012B RID: 299 RVA: 0x0000C0A4 File Offset: 0x0000A4A4
        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Reserve.Reserve((Verse.AI.TargetIndex)1, 1, -1, null);
            yield return Toils_Reserve.Reserve((Verse.AI.TargetIndex)2, 1, -1, null);
            yield return Toils_Goto.GotoThing((Verse.AI.TargetIndex)1, (Verse.AI.PathEndMode)3);
            yield return Toils_Haul.StartCarryThing((Verse.AI.TargetIndex)1, false, false);
            yield return Toils_Haul.CarryHauledThingToCell((Verse.AI.TargetIndex)2);
            Toil t2 = Toils_General.Wait(1000);
            t2.AddFailCondition(() => !this.Emitter.GetComp<CompPowerTrader>().PowerOn);
            t2 = ToilEffects.WithProgressBar(t2, (Verse.AI.TargetIndex)1, () => (1000f - (float)this.ticksLeftThisToil) / 1000f, false, -0.5f);
            yield return t2;
            yield return new Toil
            {
                defaultCompleteMode = (Verse.AI.ToilCompleteMode)1,
                initAction = delegate ()
                {
                    this.Emitter.GetComp<CompHoloEmitter>().Scan(this.Corpse);
                }
            };
            yield return Toils_Reserve.Release((Verse.AI.TargetIndex)2);
            yield break;
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        // Token: 0x040000B6 RID: 182
        private const TargetIndex CorpseIndex = (Verse.AI.TargetIndex)1;

        // Token: 0x040000B7 RID: 183
        private const TargetIndex GraveIndex = (Verse.AI.TargetIndex)2;
    }
}
