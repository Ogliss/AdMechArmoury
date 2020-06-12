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
    /*
    [HarmonyPatch(typeof(PawnRenderer), "RenderPawnInternal", new Type[] { typeof(Vector3), typeof(float), typeof(bool), typeof(Rot4), typeof(Rot4), typeof(RotDrawMode), typeof(bool), typeof(bool), typeof(bool) })]
    public static class AMA_PawnRenderer_RenderPawnInternal_Transpiler_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            int i = 0;
            foreach (var instruction in instructionsList)
            {
                if (instruction.operand as MethodInfo == typeof(GenDraw).GetMethod("DrawMeshNowOrLater"))
                {
                    i++;
                    if (i==6)
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_2);
                        yield return new CodeInstruction(OpCodes.Call, typeof(AMA_PawnRenderer_RenderPawnInternal_Transpiler_Patch).GetMethod("CenterCellValue"));
                    //    log.message(string.Format("operand: {0}, opcode: {1} : {2}", instruction.operand, instruction.opcode, OpCodes.Ldarg_2.OperandType));
                    }
                }
                yield return instruction;

            }
        }
        public static Vector3 CenterCellValue(Vector3 loc)
        {

        //    log.message(string.Format("Drawpos: {0}", loc));
            
            return loc;
        }
    }
    */
}
