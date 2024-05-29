using Verse.AI;
using Verse;
using System.Collections.Generic;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.ThinkNode_ConditionalIsMemberOfRace
    public class ThinkNode_ConditionalIsMemberOfRace : ThinkNode_Conditional
    {
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            ThinkNode_ConditionalIsMemberOfRace thinkNode_ConditionalIsMemberOfRace = (ThinkNode_ConditionalIsMemberOfRace)base.DeepCopy(resolve);
            thinkNode_ConditionalIsMemberOfRace.races = new List<ThingDef>(races);
            return thinkNode_ConditionalIsMemberOfRace;
        }

        public override bool Satisfied(Pawn pawn)
        {
            return races.Contains(pawn.def);
        }


        public List<ThingDef> races;
    }
}