using System.Collections.Generic;
using Verse;
using HarmonyLib;
using System.Reflection.Emit;
using System.Reflection;
using AdeptusMechanicus.settings;
using RimWorld;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(PawnGenerator), "GenerateTraits")]
    public static class PawnGenerator_GenerateTraits_Transpiler
    {
        static bool patchedA = false;
        static bool patchedB = false;
        static bool patchedC = false;
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            FieldInfo growthMomentAges = typeof(GrowthUtility).GetField("GrowthMomentAges", BindingFlags.Static | BindingFlags.Public);
            MethodInfo newborn = typeof(DevelopmentalStageExtensions).GetMethod("Newborn");
            for (int i = 0; i < instructionsList.Count; i++)
            {
                CodeInstruction instruction = instructionsList[i];

                if (!patchedA && instruction.OperandIs(newborn))
                {
                    if (AMAMod.Dev) Log.Message($"GenerateTraits Newborn patched to AlienRaceUtility.RaceNewbornTraits at {i} opcode: {instruction.opcode} operand: {instruction.operand}");
                    yield return instruction;
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    instruction = new CodeInstruction(OpCodes.Call, typeof(AlienRaceUtility).GetMethod(nameof(AlienRaceUtility.RaceNewbornTraits)));
                    patchedA = true;
                }
                
                if (instruction.OperandIs(growthMomentAges))
                {
                    if (AMAMod.Dev) Log.Message($"GenerateTraits GrowthMomentAges patched to AlienRaceUtility.RaceGrowthMomentAges at {i} opcode: {instruction.opcode} operand: {instruction.operand}");
                  
                    yield return instruction;
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    instruction = new CodeInstruction(OpCodes.Call, typeof(AlienRaceUtility).GetMethod(nameof(AlienRaceUtility.RaceGrowthMomentAges)));
                    
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

