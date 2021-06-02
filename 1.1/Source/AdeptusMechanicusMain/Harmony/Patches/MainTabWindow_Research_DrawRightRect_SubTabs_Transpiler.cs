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
using UnityEngine;

namespace AdeptusMechanicus.HarmonyInstance
{
    
    [HarmonyPatch(typeof(MainTabWindow_Research), "DrawRightRect")]
    public static class MainTabWindow_Research_DrawRightRect_SubTabs_Transpiler
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            MethodInfo CurTab = AccessTools.Property(typeof(MainTabWindow_Research), "CurTab").GetGetMethod(true);
            bool tabs = false;
            bool research = false;
            bool links = false;
            for (int i = 0; i < instructionsList.Count; i++)
            {
                var instruction = instructionsList[i];
                if (i > 1 && instruction.opcode == OpCodes.Ldarg_1 && instructionsList[i - 1].opcode == OpCodes.Stloc_0 && !tabs)
                {
                    //    Log.Message(i + " opcode: " + instruction.opcode + " operand: " + instruction.operand.ToString());
                    tabs = true;
                    yield return instruction;
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_0);
                    yield return new CodeInstruction(opcode: OpCodes.Call, CurTab);
                    instruction = new CodeInstruction(opcode: OpCodes.Call, operand: typeof(ResearchSubTabUtility).GetMethod("SubTabMenu"));


                }
                if (i > 1 && instruction.opcode == (research && !links ? OpCodes.Bne_Un_S : OpCodes.Bne_Un) && instructionsList[i - 1].OperandIs(CurTab))
                {
                    //    Log.Message(i + " opcode: " + instruction.opcode + " operand: " + instruction.operand.ToString());
                    yield return new CodeInstruction(opcode: OpCodes.Ldloc_S, research ? (links ? 18 : 16) : 14);
                    yield return new CodeInstruction(opcode: OpCodes.Call, operand: typeof(ResearchSubTabUtility).GetMethod("OnTabOrActiveSubTab"));
                    instruction = new CodeInstruction(OpCodes.Brfalse, instruction.operand);
                    if (research && !links)
                    {
                        links = true;
                    }
                    else
                    {
                        research = true;
                    }

                }
                yield return instruction;
            }
        }

    }
    
}
