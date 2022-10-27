using System.Collections.Generic;
using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection;
using System.Reflection.Emit;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(IncidentWorker_Raid), "TryExecuteWorker")]
    public static class IncidentWorker_Raid_TryExecuteWorker_AnimalRaids_Transpiler
    {
        static FieldInfo pawn_Apparel = AccessTools.Field(typeof(Pawn),"apparel");
        static MethodInfo pawn_ApparelTracker_GetWornApparel = AccessTools.Method(typeof(Pawn_ApparelTracker), "get_WornApparel");
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);

            CodeInstruction loopInstruction = instructionsList.FindLast(x=> x.opcode == OpCodes.Brfalse_S);
            for (int i = 0; i < instructionsList.Count; i++)
            {
                CodeInstruction instruction = instructionsList[i];
                if (instruction.OperandIs(pawn_Apparel))
                {
                    if (instructionsList.Count > i+12 && instructionsList[i+12].opcode == OpCodes.Brfalse_S && instructionsList[i - 4].opcode == OpCodes.Br_S && instructionsList[i + 1].OperandIs(pawn_ApparelTracker_GetWornApparel))
                    {
                        Log.Message($"AdeptusMechanicus:: Patched Raid.TryExecuteWorker- Allowing animals in raids");
                        /*
                        Log.Message($"{i - 4}  opcode: {instructionsList[i - 4].opcode} operand: {instructionsList[i - 4].operand}");
                        Log.Message($"{i - 3}  opcode: {instructionsList[i - 3].opcode} operand: {instructionsList[i - 3].operand}");
                        Log.Message($"{i - 2}  opcode: {instructionsList[i - 2].opcode} operand: {instructionsList[i - 2].operand}");
                        Log.Message($"{i - 1}  opcode: {instructionsList[i - 1].opcode} operand: {instructionsList[i - 1].operand}");
                        Log.Message($"{i}  opcode: {instruction.opcode} operand: {instruction.operand}");
                        */
                        yield return instruction;
                        // if null, continue to loop increment
                        yield return new CodeInstruction(OpCodes.Brfalse_S, instructionsList[i + 12].operand);
                        // replace used
                        yield return new CodeInstruction(instructionsList[i - 3].opcode, instructionsList[i - 3].operand);
                        yield return new CodeInstruction(instructionsList[i - 2].opcode, instructionsList[i - 2].operand);
                        yield return new CodeInstruction(instructionsList[i - 1].opcode, instructionsList[i - 1].operand);
                        instruction = new CodeInstruction(instruction.opcode, instruction.operand);
                    }
                }
                yield return instruction;
            }

        }
    }
}
