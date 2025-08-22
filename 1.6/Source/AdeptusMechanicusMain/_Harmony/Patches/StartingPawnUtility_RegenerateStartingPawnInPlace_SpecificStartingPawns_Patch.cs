using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using Verse.Sound;
using AdeptusMechanicus;
using AdeptusMechanicus.ExtensionMethods;
using RimWorld.Planet;

namespace AdeptusMechanicus.HarmonyInstance
{
	
    [HarmonyPatch(typeof(StartingPawnUtility), "RegenerateStartingPawnInPlace")]
    public static class StartingPawnUtility_RegenerateStartingPawnInPlace_SpecificStartingPawns_Patch
	{
        [HarmonyPrefix]
        public static bool Prefix(int index, ref Pawn __result)
        {
			Scenario scenario = Find.Scenario;
            if (scenario !=null)
            {
                if (scenario.AllParts.Any(x => x is ScenPart_ConfigPage_ConfigureStartingPawnsSpecific))
				{
					Pawn pawn = Find.GameInitData.startingAndOptionalPawns[index];
					PawnKindDef kindDef = pawn.kindDef;
					PawnUtility.TryDestroyStartingColonistFamily(pawn);
					pawn.relations.ClearAllRelations();
					PawnComponentsUtility.RemoveComponentsOnDespawned(pawn);
					Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Discard);
					Find.GameInitData.startingAndOptionalPawns[index] = null;
					for (int i = 0; i < Find.GameInitData.startingAndOptionalPawns.Count; i++)
					{
						if (Find.GameInitData.startingAndOptionalPawns[i] != null)
						{
							PawnUtility.TryDestroyStartingColonistFamily(Find.GameInitData.startingAndOptionalPawns[i]);
						}
					}
					__result = SpecificStartingPawnUtility.NewGeneratedStartingPawn(kindDef);
					Find.GameInitData.startingAndOptionalPawns[index] = __result;
					return false;
				}
			}
			return true;
        }
    
        [HarmonyPostfix]
        public static void Postfix(int index, ref Pawn __result)
        {

        }
    }
    
}
