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

namespace AdeptusMechanicus.HarmonyInstance
{
    
//    [HarmonyPatch(typeof(PawnApparelGenerator), "GenerateStartingApparelFor")]
    public static class PawnApparelGenerator_GenerateStartingApparelFor_FactionColors_Patch
    {
    //    [HarmonyPostfix]
        public static void Postfix(ref Pawn pawn, PawnGenerationRequest request)
        {

            foreach (Apparel thing in pawn.apparel.WornApparel)
            {
                CompColorableTwoFaction FactionColorable = thing.TryGetComp<CompColorableTwoFaction>();
                if (FactionColorable != null && pawn.Faction !=null)
                {
                    FactionColorable.FactionDef = pawn.Faction.def;
                }
            }

        }
    }
    
}
