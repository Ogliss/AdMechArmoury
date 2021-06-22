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
    
    [HarmonyPatch(typeof(MainTabWindow_Research), "PostOpen")]
    public static class MainTabWindow_Research_PostOpen_SubTabs_Transpiler
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            CodeInstruction loop = null;
            for (int i = 0; i < instructionsList.Count; i++)
            {
                var instruction = instructionsList[i];
                if (instruction.opcode == OpCodes.Br_S && loop == null)
                {
                    loop = instruction;
                }
                if (loop != null && instructionsList[i].opcode == OpCodes.Ldloc_1)
                {
                    yield return instruction;
                    yield return new CodeInstruction(opcode: OpCodes.Call, operand: typeof(ResearchSubTabUtility).GetMethod("IsSubTab"));
                    yield return new CodeInstruction(OpCodes.Brtrue_S, loop.operand);
                    instruction = new CodeInstruction(opcode: OpCodes.Ldloc_1);

                }
                yield return instruction;
            }
        }

    }
    
}
