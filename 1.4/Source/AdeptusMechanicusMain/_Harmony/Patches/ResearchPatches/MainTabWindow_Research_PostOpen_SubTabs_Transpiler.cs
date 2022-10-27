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
            MethodInfo allTabDefs = AccessTools.Method(typeof(DefDatabase<ResearchTabDef>), "get_AllDefs");
            for (int i = 0; i < instructionsList.Count; i++)
            {
                var instruction = instructionsList[i];
                if (instruction.OperandIs(allTabDefs))
                {
                //    Log.Message("ResearchTab PostOpen filterSubTabs: " + i + " opcode: " + instruction.opcode + " operand: " + instruction.operand);
                    yield return instruction;
                    instruction = new CodeInstruction(opcode: OpCodes.Call, operand: typeof(ResearchSubTabUtility).GetMethod("filterSubTabs"));
                }
                yield return instruction;
            }
        }

    }
    
}
