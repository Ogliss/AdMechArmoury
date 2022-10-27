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
    /*
    [HarmonyPatch(typeof(ApparelUtility), "HasPartsToWear")]
    public static class Class_Method_Name_Transpiler
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            
            for (int i = 0; i < instructionsList.Count; i++)
            {
				CodeInstruction instruction = instructionsList[i];
				Log.Message($"{i}  opcode: {instruction.opcode} operand: {instruction.operand}");
				yield return instruction;
			}
            
        }
    }
    */
}
