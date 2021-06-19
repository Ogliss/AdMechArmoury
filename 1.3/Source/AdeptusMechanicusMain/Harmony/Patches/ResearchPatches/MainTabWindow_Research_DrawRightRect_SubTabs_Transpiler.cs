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
            MethodInfo ScrollWindow = AccessTools.Method(typeof(Widgets), "ScrollHorizontal");
            bool tabs = false;
            bool research = false;
            bool links = false;
        //    bool log = false;
            for (int i = 0; i < instructionsList.Count; i++)
            {
                var instruction = instructionsList[i];
                if (i > 1 && instruction.opcode == OpCodes.Ldarg_1 && instructionsList[i - 1].opcode == OpCodes.Stloc_0 && !tabs)
                {
                //    Draws SubTabMenu
                //    Log.Message("ResearchTab DrawRightRect SubTabMenu: " + i + " opcode: " + instruction.opcode + " operand: " + instruction.operand);
                    tabs = true;
                    yield return instruction;
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_0);
                    yield return new CodeInstruction(opcode: OpCodes.Call, CurTab);
                    instruction = new CodeInstruction(opcode: OpCodes.Call, operand: typeof(ResearchSubTabUtility).GetMethod("SubTabMenu"));


                }
                if (ScrollWindow != null)
                {
                    if (instruction.opcode == OpCodes.Call && instruction.OperandIs(ScrollWindow))
                    {
                    //    Enables vertical mousewheel scrolling while holding Ctrl 
                    //    Log.Message("ResearchTab DrawRightRect ScrollWindow: " + i + " opcode: " + instruction.opcode + " operand: " + instruction.operand);
                        instruction.operand = typeof(AdeptusWidgets).GetMethod("ScrollHorizontalAndVert");
                    }

                }
                if ( i > 1 && instruction.opcode == OpCodes.Ceq && instructionsList[i - 1].OperandIs(CurTab))
                {
                //    controls which research are displayed 
                //    Log.Message("ResearchTab DrawRightRect OnTabOrActiveSubTab: " + i + " opcode: " + instruction.opcode + " operand: " + instruction.operand);
                    yield return new CodeInstruction(opcode: OpCodes.Ldloc_S, research ? (links ? 30 : 21) : 18);
                    yield return new CodeInstruction(opcode: OpCodes.Call, operand: typeof(ResearchSubTabUtility).GetMethod("OnTabOrActiveSubTab"));
                    yield return new CodeInstruction(OpCodes.Ldc_I4_1);
                //    instruction = new CodeInstruction(OpCodes.Brfalse, instruction.operand);
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
