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
using UnityEngine;
using System.Reflection.Emit;
using AdeptusMechanicus.settings;
using AlienRace;
using System.Reflection;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(PawnGenerator), "FinalLevelOfSkill")]
    public static class PawnGenerator_FinalLevelOfSkill_Transpiler
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            FieldInfo ageSkillMaxFactorCurve = typeof(PawnGenerator).GetField("AgeSkillMaxFactorCurve", BindingFlags.NonPublic|BindingFlags.Static);
            MethodInfo evaluateCurve = typeof(SimpleCurve).GetMethod("Evaluate");
            MethodInfo pawnAge = typeof(Pawn_AgeTracker).GetMethod("get_AgeBiologicalYears");
            MethodInfo pawnAgeFloat = typeof(Pawn_AgeTracker).GetMethod("get_AgeBiologicalYearsFloat");
            for (int i = 0; i < instructionsList.Count; i++)
            {
                CodeInstruction instruction = instructionsList[i];
                if (instruction.OperandIs(ageSkillMaxFactorCurve))
                {
                    if (AMAMod.Dev) Log.Message($"FinalLevelOfSkill AgeSkillMaxFactorCurve patched at {i} opcode: {instruction.opcode} operand: {instruction.operand}");
                    yield return instruction;
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    instruction = new CodeInstruction(OpCodes.Call, typeof(AlienRaceUtility).GetMethod("RaceAgeSkillMaxFactorCurve"));
                }
                if (instruction.OperandIs(pawnAge))
                {
                    if (AMAMod.Dev) Log.Message($"FinalLevelOfSkill AgeBiologicalYears patched at {i} opcode: {instruction.opcode} operand: {instruction.operand}");
                    instruction = new CodeInstruction(instruction.opcode, pawnAgeFloat);
                }
                yield return instruction;
            }

        }

    }
}

