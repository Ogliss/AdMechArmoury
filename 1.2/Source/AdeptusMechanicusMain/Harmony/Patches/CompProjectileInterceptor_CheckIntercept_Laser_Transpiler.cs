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
using System.Reflection.Emit;
using System.Reflection;
using UnityEngine;
using AdeptusMechanicus.Lasers;

namespace AdeptusMechanicus.HarmonyInstance
{
    
    [HarmonyPatch(typeof(CompProjectileInterceptor), "CheckIntercept")]
    public static class CompProjectileInterceptor_CheckIntercept_Laser_Transpiler
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            bool acted = false;
            bool watch = false;
            for (int i = 0; i < instructionsList.Count; i++)
            {
                var instruction = instructionsList[i];
                if (instruction.opcode == OpCodes.Newobj && instruction.OperandIs((ConstructorInfo)typeof(Effecter).GetConstructors()[0]))
                {
                //    Log.Message(i + " opcode: " + instruction.opcode + " operand: " + instruction.operand);
                    watch = true;
                }
                if (!acted && watch && instruction.opcode == OpCodes.Ldarg_3)
                {
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_0);              // CompProjectileInterceptor
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_1);              // Projectile
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_2);              // lastExactPos
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_3);              // newExactPos
                    instruction = new CodeInstruction(opcode: OpCodes.Call, operand: typeof(CompProjectileInterceptor_CheckIntercept_Laser_Transpiler).GetMethod("LaserIntercept"));
                    acted = true;
                    watch = false;
                }
                yield return instruction;
            }
            
        }
        public static Vector3 LaserIntercept(CompProjectileInterceptor __instance, Projectile projectile, Vector3 lastExactPos, Vector3 newExactPos)
        {
            
            LaserBeam beam = projectile as LaserBeam;
            if (beam != null)
            {
                Vector3 vec = LaserIntersectionPoint(newExactPos, lastExactPos, __instance.parent.Position.ToVector3Shifted(), __instance.Props.radius - 0.5f)[0];
                vec.y = AltitudeLayer.MoteOverhead.AltitudeFor();
                beam.destination = vec;
                return vec;
            }
            return newExactPos;
        }
        public static Vector3[] LaserIntersectionPoint(Vector3 p1, Vector3 p2, Vector3 center, float radius)
        {
            Vector3 dp = new Vector3();
            Vector3[] sect;
            float a, b, c;
            float bb4ac;
            float mu1;
            float mu2;

            //  get the distance between X and Z on the segment
            dp.x = p2.x - p1.x;
            dp.z = p2.z - p1.z;
            //   I don't get the math here
            a = dp.x * dp.x + dp.z * dp.z;
            b = 2 * (dp.x * (p1.x - center.x) + dp.z * (p1.z - center.z));
            c = center.x * center.x + center.z * center.z;
            c += p1.x * p1.x + p1.z * p1.z;
            c -= 2 * (center.x * p1.x + center.z * p1.z);
            c -= radius * radius;
            bb4ac = b * b - 4 * a * c;
            if (Mathf.Abs(a) < float.Epsilon || bb4ac < 0)
            {
                //  line does not intersect
                return new Vector3[] { Vector3.zero, Vector3.zero };
            }
            mu1 = (-b + Mathf.Sqrt(bb4ac)) / (2 * a);
            mu2 = (-b - Mathf.Sqrt(bb4ac)) / (2 * a);
            sect = new Vector3[2];
            sect[0] = new Vector3(p1.x + mu1 * (p2.x - p1.x), 0, p1.z + mu1 * (p2.z - p1.z));
            sect[1] = new Vector3(p1.x + mu2 * (p2.x - p1.x), 0, p1.z + mu2 * (p2.z - p1.z));

            return sect;
        }
    }
    
}
