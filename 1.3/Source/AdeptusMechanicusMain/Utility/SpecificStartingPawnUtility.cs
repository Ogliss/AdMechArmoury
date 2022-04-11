using RimWorld;
using System;
using Verse;

namespace AdeptusMechanicus
{
    public static class SpecificStartingPawnUtility
    {

		public static Pawn NewGeneratedStartingPawn(PawnKindDef kindDef)
		{
			if (kindDef == null)
			{
				Log.Error("KindDef NULL");
			}
			PawnGenerationRequest request = new PawnGenerationRequest(kindDef ?? Faction.OfPlayer.def.basicMemberKind, Faction.OfPlayer, PawnGenerationContext.PlayerStarter, -1, true, false, false, false, true, TutorSystem.TutorialMode, 20f, false, true, true, true, false, false, false, false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, null, false, false, false);
			Pawn pawn = null;
			try
			{
				pawn = PawnGenerator.GeneratePawn(request);
			}
			catch (Exception arg)
			{
				Log.Error("There was an exception thrown by the PawnGenerator during generating a Specific starting pawn("+kindDef+"). Trying one more time...\nException: " + arg);
				pawn = PawnGenerator.GeneratePawn(request);
			}
			pawn.relations.everSeenByPlayer = true;
			PawnComponentsUtility.AddComponentsForSpawn(pawn);
			return pawn;
		}
	}
}
