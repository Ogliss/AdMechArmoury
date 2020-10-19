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

namespace AdeptusMechanicus.HarmonyInstance
{
    /*
    [HarmonyPatch(typeof(ApparelUtility), "HasPartsToWear")]
    public static class Class_Method_Name_Transpiler
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            
            int i = 0;
            foreach (var instruction in instructionsList)
            {
                Log.Message(i+" opcode: " + instruction.opcode + " operand: " + instruction.operand);
                i++;
                yield return instruction;
            }
            
}
    }
    */
}
