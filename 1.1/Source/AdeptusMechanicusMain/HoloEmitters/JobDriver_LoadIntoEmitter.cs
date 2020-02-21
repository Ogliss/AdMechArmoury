using System;
using System.Collections.Generic;
using System.Linq;
using HoloEmitters.Things;
using RimWorld;
using Verse;
using Verse.AI;

namespace HoloEmitters
{
    // Token: 0x0200005A RID: 90
    public class JobDriver_LoadIntoEmitter : JobDriver
    {
        // Token: 0x06000143 RID: 323 RVA: 0x0000CE1A File Offset: 0x0000B21A
        public JobDriver_LoadIntoEmitter()
        {
            this.rotateToFace = (Verse.AI.TargetIndex)2;
        }

        // Token: 0x06000144 RID: 324 RVA: 0x0000CE2C File Offset: 0x0000B22C
        private Pawn MakeGeniusPawn()
        {
            PawnGenerationRequest pawnGenerationRequest = new PawnGenerationRequest(PawnKindDefOf.SpaceRefugee, Faction.OfPlayer, (PawnGenerationContext)1, -1, true, false, false, false, false, false, 0f, true, true, true, false, false, false, false, null, null, null, null);
            Pawn pawn = PawnGenerator.GeneratePawn(pawnGenerationRequest);
            List<Trait> list = new List<Trait>();
            foreach (Trait trait in pawn.story.traits.allTraits)
            {
                if (trait.def == TraitDefOf.Psychopath || trait.def == TraitDefOf.Cannibal || trait.def == TraitDefOf.Pyromaniac || trait.def == TraitDefOf.BodyPurist)
                {
                    list.Add(trait);
                }
            }
            foreach (Trait item in list)
            {
                pawn.story.traits.allTraits.Remove(item);
            }
            List<SkillRecord> list2 = (from s in pawn.skills.skills
                                       where !s.TotallyDisabled
                                       select s).ToList<SkillRecord>();
            SkillRecord skillRecord = GenCollection.RandomElement<SkillRecord>(list2);
            skillRecord.Level = 20;
            skillRecord.passion = (RimWorld.Passion)2;
            list2.Remove(skillRecord);
            skillRecord = GenCollection.RandomElement<SkillRecord>(list2);
            skillRecord.Level = 20;
            skillRecord.passion = (RimWorld.Passion)2;
            return pawn;
        }

        // Token: 0x1700001B RID: 27
        // (get) Token: 0x06000145 RID: 325 RVA: 0x0000D00C File Offset: 0x0000B40C
        private Thing Disk => base.TargetThingA;

        // Token: 0x1700001C RID: 28
        // (get) Token: 0x06000146 RID: 326 RVA: 0x0000D038 File Offset: 0x0000B438
        private HoloEmitter Emitter
        {
            get
            {
                return (HoloEmitter)base.TargetThingB;
            }
        }

        // Token: 0x06000147 RID: 327 RVA: 0x0000D068 File Offset: 0x0000B468
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
                    Pawn simPawn = this.MakeGeniusPawn();
                    this.Emitter.GetComp<CompHoloEmitter>().SimPawn = simPawn;
                    this.Emitter.GetComp<CompHoloEmitter>().SetUpPawn();
                    this.Disk.Destroy(0);
                }
            };
            yield return Toils_Reserve.Release((Verse.AI.TargetIndex)2);
            yield break;
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        // Token: 0x040000BF RID: 191
        private const TargetIndex CorpseIndex = (Verse.AI.TargetIndex)1;

        // Token: 0x040000C0 RID: 192
        private const TargetIndex GraveIndex = (Verse.AI.TargetIndex)2;
    }
}
