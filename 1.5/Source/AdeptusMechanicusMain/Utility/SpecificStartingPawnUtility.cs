using RimWorld;
using System;
using Verse;

namespace AdeptusMechanicus
{
    public static class SpecificStartingPawnUtility
    {

		public static Pawn NewGeneratedStartingPawn(PawnKindDef kindDef, int index = -1)
		{
			if (kindDef == null)
			{
				Log.Error("KindDef NULL");
			}
            PawnGenerationRequest request = (index < 0) ? StartingPawnUtility.DefaultStartingPawnRequest : StartingPawnUtility.GetGenerationRequest(index);
			request.KindDef = kindDef ?? Faction.OfPlayer.def.basicMemberKind;
            Pawn pawn = null;
            try
            {
                pawn = PawnGenerator.GeneratePawn(request);
            }
            catch (Exception arg)
            {
                Log.Error("There was an exception thrown by the PawnGenerator during generating a starting pawn. Trying one more time...\nException: " + arg);
                pawn = PawnGenerator.GeneratePawn(request);
            }
            pawn.relations.everSeenByPlayer = true;
            PawnComponentsUtility.AddComponentsForSpawn(pawn);
            StartingPawnUtility.GeneratePossessions(pawn);
            return pawn;
        }
	}
}
