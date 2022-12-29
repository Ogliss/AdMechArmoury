using System.Collections.Generic;
using Verse;
using HarmonyLib;
using System.Reflection.Emit;
using System.Reflection;
using AdeptusMechanicus.settings;
using RimWorld;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(PawnGenerator), "FinalLevelOfSkill")]
    public static class PawnGenerator_FinalLevelOfSkill_Transpiler
    {
        static bool patchedA = false;
        static bool patchedB = false;
        static bool patchedC = false;
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            FieldInfo ageSkillMaxFactorCurve = typeof(PawnGenerator).GetField("AgeSkillMaxFactorCurve", BindingFlags.NonPublic|BindingFlags.Static);
            FieldInfo ageSkillFactor = typeof(PawnGenerator).GetField("AgeSkillFactor", BindingFlags.NonPublic|BindingFlags.Static);
            MethodInfo evaluateCurve = typeof(SimpleCurve).GetMethod("Evaluate");
            MethodInfo pawnAge = typeof(Pawn_AgeTracker).GetMethod("get_AgeBiologicalYears");
            MethodInfo pawnAgeFloat = typeof(Pawn_AgeTracker).GetMethod("get_AgeBiologicalYearsFloat");
            MethodInfo newborn = typeof(DevelopmentalStageExtensions).GetMethod("Newborn");
            for (int i = 0; i < instructionsList.Count; i++)
            {
                CodeInstruction instruction = instructionsList[i];

                if (!patchedA && instruction.OperandIs(newborn))
                {
                    if (AMAMod.Dev) Log.Message($"FinalLevelOfSkill Newborn patched to AlienRaceUtility.RaceNewbornSkill at {i} opcode: {instruction.opcode} operand: {instruction.operand}");
                    yield return instruction;
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    instruction = new CodeInstruction(OpCodes.Call, typeof(AlienRaceUtility).GetMethod(nameof(AlienRaceUtility.RaceNewbornSkill)));
                    patchedA = true;
                }
                if (!patchedB && instruction.OperandIs(ageSkillMaxFactorCurve))
                {
                    if (AMAMod.Dev) Log.Message($"FinalLevelOfSkill AgeSkillMaxFactorCurve patched to AlienRaceUtility.RaceAgeSkillMaxFactorCurve at {i} opcode: {instruction.opcode} operand: {instruction.operand}");
                    yield return instruction;
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    instruction = new CodeInstruction(OpCodes.Call, typeof(AlienRaceUtility).GetMethod(nameof(AlienRaceUtility.RaceAgeSkillMaxFactorCurve)));
                    patchedB = true;
                }
                /*
                if (instructionsList.Count > i+1 && instruction.opcode == OpCodes.Stloc_0 && instructionsList[i + 1].opcode == OpCodes.Ldloc_0)
                {
                    if (AMAMod.Dev) Log.Message($"FinalLevelOfSkill Report patched at {i} opcode: {instruction.opcode} operand: {instruction.operand}");
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Call, typeof(PawnGenerator_FinalLevelOfSkill_Transpiler).GetMethod(nameof(PawnGenerator_FinalLevelOfSkill_Transpiler.Report)));
                }
                */
                if (!patchedC && instruction.OperandIs(ageSkillFactor))
                {
                    if (AMAMod.Dev) Log.Message($"FinalLevelOfSkill AgeSkillFactor patched to AlienRaceUtility.RaceAgeSkillFactor at {i} opcode: {instruction.opcode} operand: {instruction.operand}");
                    yield return instruction;
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    instruction = new CodeInstruction(OpCodes.Call, typeof(AlienRaceUtility).GetMethod(nameof(AlienRaceUtility.RaceAgeSkillFactor)));
                    patchedC = true;
                }
                if (instruction.OperandIs(pawnAge))
                {
                    if (AMAMod.Dev) Log.Message($"FinalLevelOfSkill AgeBiologicalYears patched to AgeBiologicalYearsFloat at {i} opcode: {instruction.opcode} operand: {instruction.operand}");
                    instruction = new CodeInstruction(instruction.opcode, pawnAgeFloat);
                }
                yield return instruction;
            }

        }

        public static float Report(float num, Pawn pawn, SkillDef sk)
        {
            if (AMAMod.Dev) Log.Message($"{sk.LabelCap} for {pawn} = {num}");
            return num;
        }
        /*
        static void Postfix(Pawn pawn, SkillDef sk, PawnGenerationRequest request, int __result)
        {
        //    if (AMAMod.Dev) Log.Message($"{pawn}'s final level for {sk.LabelCap} = {__result}");
        }
        */
    }
}

