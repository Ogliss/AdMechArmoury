using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace Momu
{
    public class ThoughtWorker_MomuWorkWithFriends : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            if (!p.RaceProps.Humanlike)
            {
                return false;
            }
            if (!p.def.defName.Contains("Momu"))
            {
                return ThoughtState.Inactive;
            }
            JobDriver curDriver = p.jobs.curDriver;
            if (curDriver == null)
            {
                return ThoughtState.Inactive;
            }
            if (p.skills == null)
            {
                return ThoughtState.Inactive;
            }
            if (curDriver.ActiveSkill == null)
            {
                return ThoughtState.Inactive;
            }
            SkillRecord skill = p.skills.GetSkill(curDriver.ActiveSkill);
            if (skill == null)
            {
                return ThoughtState.Inactive;
            }
            List<Pawn> momu = p.Map.mapPawns.AllPawnsSpawned.FindAll(x => x.def.defName.Contains("Momu"));
            if (momu.NullOrEmpty())
            {
                return ThoughtState.Inactive;
            }
            List<Pawn> FriendlyMomu = momu.FindAll(x => x.GetRelations(p) != null && x.relations.OpinionOf(p) > 50 && x != p);
            if (FriendlyMomu.NullOrEmpty())
            {
                return ThoughtState.Inactive;
            }
            if (!FriendlyMomu.Any(x => x.Position.InHorDistOf(p.Position, 10)))
            {
                return ThoughtState.Inactive;
            }
            return ThoughtState.ActiveDefault;
        }
    }
}
