using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace AdeptusMechanicus.ExtensionMethods
{
    public static class Toil_IngestExtensions
    {
        public static Toil FinalizeIngested(this Toils_Ingest Ingest, Pawn ingester, TargetIndex ingestibleInd, BodyPartDef targetPart)
		{
			Toil toil = new Toil();
			toil.initAction = delegate ()
			{
				Pawn actor = toil.actor;
				Job curJob = actor.jobs.curJob;
				Thing thing = curJob.GetTarget(ingestibleInd).Thing;
				if (ingester.needs.mood != null && thing.def.IsNutritionGivingIngestible && thing.def.ingestible.chairSearchRadius > 10f)
				{
					if (!(ingester.Position + ingester.Rotation.FacingCell).HasEatSurface(actor.Map) && ingester.GetPosture() == PawnPosture.Standing && !ingester.IsWildMan())
					{
						ingester.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.AteWithoutTable, null);
					}
					Room room = ingester.GetRoom(RegionType.Set_Passable);
					if (room != null)
					{
						int scoreStageIndex = RoomStatDefOf.Impressiveness.GetScoreStageIndex(room.GetStat(RoomStatDefOf.Impressiveness));
						if (ThoughtDefOf.AteInImpressiveDiningRoom.stages[scoreStageIndex] != null)
						{
							ingester.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(ThoughtDefOf.AteInImpressiveDiningRoom, scoreStageIndex), null);
						}
					}
				}
				float num = ingester.needs.food.NutritionWanted;
				if (curJob.overeat)
				{
					num = Mathf.Max(num, 0.75f);
				}
				float num2 = thing.Ingested(ingester, num);
				if (!ingester.Dead)
				{
					ingester.needs.food.CurLevel += num2;
				}
				ingester.records.AddTo(RecordDefOf.NutritionEaten, num2);
			};
			toil.defaultCompleteMode = ToilCompleteMode.Instant;
			return toil;
		}
    }
}
