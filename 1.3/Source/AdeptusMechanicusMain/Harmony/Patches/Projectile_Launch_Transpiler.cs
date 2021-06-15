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
using UnityEngine;
using System.Reflection.Emit;
using OgsCompOversizedWeapon;

namespace AdeptusMechanicus.HarmonyInstance
{
    
    [HarmonyPatch(typeof(Projectile), "Launch", new Type[] { typeof(Thing), typeof(Vector3), typeof(LocalTargetInfo), typeof(LocalTargetInfo), typeof(ProjectileHitFlags), typeof(bool), typeof(Thing), typeof(ThingDef) })]
    public static class Projectile_Launch_Transpiler
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            for (int i = 0; i < instructionsList.Count; i++)
            {
                var instruction = instructionsList[i];
                if (instruction.opcode == OpCodes.Ldc_R4 && instruction.OperandIs((float)0.3f))
                {
                    yield return instruction;
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_0);              // Projectile
                    instruction = new CodeInstruction(opcode: OpCodes.Call, operand: typeof(Projectile_Launch_Transpiler).GetMethod("CalculateAdjusted"));
                }
                yield return instruction;

            }

        }
        public static float CalculateAdjusted(float spread, Projectile instance)
        {
            if (instance.def.HasModExtension<ScattershotProjectileExtension>())
            {
                ScattershotProjectileExtension ext = instance.def.GetModExtensionFast<ScattershotProjectileExtension>();
                spread += ext.spread * (instance.usedTarget.Cell.DistanceTo(instance.Launcher.Position) / ext.perCells);
            }

            return spread;
        }

        public static float CalculateAdjustedForcedMiss(float forcedMiss, IntVec3 vector, Verb_LaunchProjectile instance)
        {
            float num = (float)vector.LengthHorizontalSquared;
            if (num < instance.verbProps.range * 0.25f)
            {
                return 0f;
            }
            if (num < instance.verbProps.range * 0.5f)
            {
                return forcedMiss * 0.5f;
            }
            if (num < instance.verbProps.range * 0.75f)
            {
                return forcedMiss * 0.8f;
            }
            return forcedMiss;
        }
    }
    
}
