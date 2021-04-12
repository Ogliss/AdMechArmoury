
using RimWorld;
using System;
using Verse;
using Verse.AI;

namespace AdeptusMechanicus
{
    // Token: 0x020001BE RID: 446
    public class ThinkNode_ConditionalReachedLifeStage : ThinkNode_Conditional
    {
        // Token: 0x06000956 RID: 2390 RVA: 0x0004D678 File Offset: 0x0004BA78
        protected override bool Satisfied(Pawn pawn)
        {
            LifeStageDef stage = pawn.ageTracker.CurLifeStage;
            LifeStageDef targetStage = pawn.RaceProps.lifeStageAges[pawn.RaceProps.lifeStageAges.Count - 1].def;
            return stage == targetStage;
        }

    }
}
