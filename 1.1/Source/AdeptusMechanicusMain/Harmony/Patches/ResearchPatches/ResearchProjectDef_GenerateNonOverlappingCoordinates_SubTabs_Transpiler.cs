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
using System.Reflection;
using System.Reflection.Emit;

namespace AdeptusMechanicus.HarmonyInstance
{
    
    [HarmonyPatch(typeof(ResearchProjectDef), "GenerateNonOverlappingCoordinates")]
    public static class ResearchProjectDef_GenerateNonOverlappingCoordinates_SubTabs_Transpiler
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            FieldInfo tab = AccessTools.Field(typeof(ResearchProjectDef), "tab");

            for (int i = 0; i < instructionsList.Count; i++)
            {
                var instruction = instructionsList[i];
                yield return instruction;
                if (i > 1 && instruction.opcode == OpCodes.Bne_Un && instructionsList[i - 1].OperandIs(tab))
                {
                //    Log.Message(i + " opcode: " + instruction.opcode + " operand: " + instruction.operand.ToString());
                    yield return new CodeInstruction(opcode: OpCodes.Ldloc_3);
                    yield return new CodeInstruction(opcode: OpCodes.Ldloc_S, 5);
                    yield return new CodeInstruction(opcode: OpCodes.Call, operand: typeof(ResearchSubTabUtility).GetMethod("SameSubTab"));
                    yield return new CodeInstruction(OpCodes.Brfalse, instruction.operand);

                }
            }

        }

    }
    
}
