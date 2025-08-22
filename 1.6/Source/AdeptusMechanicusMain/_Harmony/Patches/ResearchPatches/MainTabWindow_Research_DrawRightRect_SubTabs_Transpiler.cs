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
            MethodInfo curTab = AccessTools.Property(typeof(MainTabWindow_Research), "CurTab").GetGetMethod(true);
            MethodInfo scrollWindow = AccessTools.Method(typeof(Widgets), "ScrollHorizontal");
            MethodInfo subTabMenu = AccessTools.Method(typeof(ResearchSubTabUtility), nameof(ResearchSubTabUtility.SubTabMenu));
            MethodInfo onTabOrActiveSubTab = AccessTools.Method(typeof(ResearchSubTabUtility), nameof(ResearchSubTabUtility.OnTabOrActiveSubTab));
            bool tabs = false;
            bool research = false;
            bool links = false;
        //    bool log = false;
            for (int i = 0; i < instructionsList.Count; i++)
            {
                var instruction = instructionsList[i];
                if (i >= 1 && instruction.opcode == OpCodes.Ldarg_1 && instructionsList[i - 1].opcode == OpCodes.Stloc_1 && !tabs)
                {
                //    Draws SubTabMenu
                    Log.Message("ResearchTab DrawRightRect SubTabMenu: " + i + " opcode: " + instruction.opcode + " operand: " + instruction.operand);
                    tabs = true;
                    yield return instruction;
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_0);
                    yield return new CodeInstruction(opcode: OpCodes.Call, curTab);
                    instruction = new CodeInstruction(opcode: OpCodes.Call, subTabMenu);


                }
                if (scrollWindow != null)
                {
                    if (instruction.opcode == OpCodes.Call && instruction.OperandIs(scrollWindow))
                    {
                    //    Enables vertical mousewheel scrolling while holding Ctrl 
                        Log.Message("ResearchTab DrawRightRect ScrollWindow: " + i + " opcode: " + instruction.opcode + " operand: " + instruction.operand);
                        instruction.operand = typeof(AdeptusWidgets).GetMethod("ScrollHorizontalAndVert");
                    }

                }
                yield return instruction;
            }
        }

    }
    
    [HarmonyPatch(typeof(MainTabWindow_Research), "ListProjects")]
    public static class MainTabWindow_Research_ListProjects_SubTabs_Transpiler
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            MethodInfo curTab = AccessTools.Property(typeof(MainTabWindow_Research), "CurTab").GetGetMethod(true);
            MethodInfo onTabOrActiveSubTab = AccessTools.Method(typeof(ResearchSubTabUtility), nameof(ResearchSubTabUtility.OnTabOrActiveSubTab));
            bool research = false;
            bool links = false;
        //    bool log = false;
            for (int i = 0; i < instructionsList.Count; i++)
            {
                var instruction = instructionsList[i];
                if (i > 3 && instruction.opcode == (research && !links ? OpCodes.Bne_Un_S : OpCodes.Bne_Un) && instructionsList[i - 1].OperandIs(curTab))
                {
                    /*
                    Log.Message($"ResearchTab ListProjects OnTabOrActiveSubTab: {i} opcode: {instruction.opcode} operand: {instruction.operand}");
                    Log.Message($"ResearchTab ListProjects OnTabOrActiveSubTab: i-1 opcode: {instructionsList[i - 1].opcode} operand: {instructionsList[i - 1].operand}");
                    Log.Message($"ResearchTab ListProjects OnTabOrActiveSubTab: i-2 opcode: {instructionsList[i - 2].opcode} operand: {instructionsList[i - 2].operand}");
                    Log.Message($"ResearchTab ListProjects OnTabOrActiveSubTab: i-3 opcode: {instructionsList[i - 3].opcode} operand: {instructionsList[i - 3].operand}");
                    Log.Message($"ResearchTab ListProjects OnTabOrActiveSubTab: i-4 opcode: {instructionsList[i - 4].opcode} operand: {instructionsList[i - 4].operand}");
                    */
                    if (instructionsList[i - 4].opcode == OpCodes.Ldfld)
                    {
                        // when the compiler generated field is called, add in its preceeding arg
                        yield return new CodeInstruction(instructionsList[i - 5].opcode, instructionsList[i - 5].operand);
                    }
                    // when the compiler generated field is called, add in its preceeding arg
                    yield return new CodeInstruction(instructionsList[i - 4].opcode, instructionsList[i - 4].operand);
                    yield return new CodeInstruction(OpCodes.Call, onTabOrActiveSubTab);
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
