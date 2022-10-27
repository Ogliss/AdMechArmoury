using AdeptusMechanicus.ExtensionMethods;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.PawnGroupKindWorker_DeepStrike

    public class PawnGroupKindWorker_DeepStrike : PawnGroupKindWorker
	{
		public override float MinPointsToGenerateAnything(PawnGroupMaker groupMaker, FactionDef faction, PawnGroupMakerParms parms = null)
		{
			IEnumerable<PawnGenOption> source;
			if (parms == null)
			{
				source = groupMaker.options.Where((PawnGenOption x) => x.kind.isFighter && x.kind.canDeepStrike());
				if (!source.Any())
				{
					source = groupMaker.options;
				}
			}
			else
			{
				source = groupMaker.options.Where((PawnGenOption x) => PawnGroupMakerUtility.PawnGenOptionValid(x, parms) && x.kind.canDeepStrike());
			}
			return source.Min((PawnGenOption g) => g.Cost);
		}

		public override bool CanGenerateFrom(PawnGroupMakerParms parms, PawnGroupMaker groupMaker)
		{
			if (!base.CanGenerateFrom(parms, groupMaker))
			{
				return false;
			}
			if (!PawnGroupMakerUtility.ChoosePawnGenOptionsByPoints(parms.points, groupMaker.options, parms).Any())
			{
				return false;
			}
			return true;
		}

		public override void GeneratePawns(PawnGroupMakerParms parms, PawnGroupMaker groupMaker, List<Pawn> outPawns, bool errorOnZeroResults = true)
		{
			if (!CanGenerateFrom(parms, groupMaker))
			{
				if (errorOnZeroResults)
				{
					Log.Error("Cannot generate pawns for " + parms.faction + " with " + parms.points + ". Defaulting to a single random cheap group.");
				}
			}
			else
			{
				bool allowFood = parms.raidStrategy == null || parms.raidStrategy.pawnsCanBringFood || (parms.faction != null && !parms.faction.HostileTo(Faction.OfPlayer));
				Predicate<Pawn> validatorPostGear = (parms.raidStrategy != null) ? ((Predicate<Pawn>)((Pawn p) => parms.raidStrategy.Worker.CanUsePawn(parms.points, p, outPawns))) : null;
				bool flag = false;
				using (IEnumerator<PawnGenOptionWithXenotype> enumerator = PawnGroupMakerUtility.ChoosePawnGenOptionsByPoints(parms.points, groupMaker.options.FindAll(x=> x.kind.canDeepStrike()), parms).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(enumerator.Current.Option.kind, parms.faction, PawnGenerationContext.NonPlayer, fixedIdeo: parms.ideo, tile: parms.tile, forceGenerateNewPawn: false, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: true, colonistRelationChanceFactor: 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowFood: allowFood, allowAddictions: true, inhabitant: parms.inhabitants, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, biocodeWeaponChance: 0f, biocodeApparelChance: 0f, extraPawnForExtraRelationChance: null, relationWithExtraPawnChanceFactor: 1f, validatorPreGear: null, validatorPostGear: validatorPostGear));
						if (parms.forceOneDowned && !flag)
						{
							pawn.health.forceDowned = true;
							pawn.mindState.canFleeIndividual = false;
							flag = true;
						}
						outPawns.Add(pawn);
					}
				}
			}
		}

		public override IEnumerable<PawnKindDef> GeneratePawnKindsExample(PawnGroupMakerParms parms, PawnGroupMaker groupMaker)
		{
			foreach (PawnGenOptionWithXenotype item in PawnGroupMakerUtility.ChoosePawnGenOptionsByPoints(parms.points, groupMaker.options, parms))
			{
				yield return item.Option.kind;
			}
		}
	}

}
