using System.Collections.Generic;
using Verse;
using HarmonyLib;
using System.Reflection.Emit;
using System.Reflection;
using AdeptusMechanicus.settings;
using RimWorld;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(PawnBioAndNameGenerator), "GetBackstoryCategoryFiltersFor")]
    public static class PawnBioAndNameGenerator_GetBackstoryCategoryFiltersFor_Transpiler
    {
        static bool patchedA = false;
        static bool patchedB = false;
        static bool patchedC = false;
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            FieldInfo newbornCategoryGroup = typeof(PawnBioAndNameGenerator).GetField("NewbornCategoryGroup", BindingFlags.NonPublic | BindingFlags.Static);
            for (int i = 0; i < instructionsList.Count; i++)
            {
                CodeInstruction instruction = instructionsList[i];

                if (!patchedA && instruction.OperandIs(newbornCategoryGroup))
                {
                    if (AMAMod.Dev) Log.Message($"FinalLevelOfSkill NewbornCategoryGroup patched to AlienRaceUtility.RaceNewbornCategoryGroup at {i} opcode: {instruction.opcode} operand: {instruction.operand}");
                    yield return instruction;
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    instruction = new CodeInstruction(OpCodes.Call, typeof(AlienRaceUtility).GetMethod(nameof(AlienRaceUtility.RaceNewbornCategoryGroup)));
                    patchedA = true;
                }
                yield return instruction;
            }

        }

    }
}

