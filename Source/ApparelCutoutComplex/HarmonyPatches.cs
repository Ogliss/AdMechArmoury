using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace OgsTools
{
    [StaticConstructorOnStartup]
    public static partial class HarmonyPatches
    {
        //For alternating fire on some weapons
        public static Dictionary<Thing, int> AlternatingFireTracker = new Dictionary<Thing, int>();

        // Verse.Pawn_HealthTracker
        public static bool StopPreApplyDamageCheck;

        public static int? tempDamageAmount = null;
        public static int? tempDamageAbsorbed = null;

        static HarmonyPatches()
        {
            var harmony = new Harmony("rimworld.Ogliss.jecstools.main");
            //Allow fortitude to soak damage
            var type = typeof(HarmonyPatches);
            
            //optionally use "CutoutComplex" shader for apparel that wants it
            harmony.Patch(AccessTools.Method(typeof(ApparelGraphicRecordGetter), nameof(ApparelGraphicRecordGetter.TryGetGraphicApparel)), null, null, new HarmonyMethod(type, nameof(CutOutComplexApparel_Transpiler)));
        }
        
        //added 2018/12/13 - Mehni.
        //Uses CutoutComplex shader for apparel that wants it.
        private static IEnumerable<CodeInstruction> CutOutComplexApparel_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo shader = AccessTools.Method(typeof(HarmonyPatches), nameof(HarmonyPatches.Shader));
            FieldInfo cutOut = AccessTools.Field(typeof(ShaderDatabase), nameof(ShaderDatabase.Cutout));

            foreach (CodeInstruction codeInstruction in instructions)
            {
                if (codeInstruction.opcode == OpCodes.Ldsfld && codeInstruction.operand == cutOut)
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0); //apparel
                    yield return new CodeInstruction(OpCodes.Call, shader); //return shader type
                    continue; //skip instruction.
                }
                yield return codeInstruction;
            }
        }

        private static Shader Shader(Apparel apparel)
        {
            if (apparel.def.graphicData.shaderType.Shader == ShaderDatabase.CutoutComplex)
                return ShaderDatabase.CutoutComplex;

            return ShaderDatabase.Cutout;
        }
    }
}