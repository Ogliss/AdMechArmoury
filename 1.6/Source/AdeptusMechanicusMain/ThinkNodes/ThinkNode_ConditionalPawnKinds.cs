using Verse.AI;
using Verse;
using System.Collections.Generic;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.ThinkNode_ConditionalPawnKinds
    public class ThinkNode_ConditionalPawnKinds : ThinkNode_Conditional
    {
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            ThinkNode_ConditionalPawnKinds thinkNode_ConditionalPawnKind = (ThinkNode_ConditionalPawnKinds)base.DeepCopy(resolve);
            thinkNode_ConditionalPawnKind.pawnKinds = pawnKinds;
            return thinkNode_ConditionalPawnKind;
        }

        public override bool Satisfied(Pawn pawn)
        {
            return pawnKinds.Contains(pawn.kindDef);
        }

        public List<PawnKindDef> pawnKinds;
    }
}