using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace AdeptusMechanicus
{
    public class JobGiver_ServitorIdle : ThinkNode_JobGiver
    {
        public override Job TryGiveJob(Pawn pawn)
        {
            return new Job(JobDefOf.Wait_Wander)
            {
                expiryInterval = 600
            };
        }
    }
}
