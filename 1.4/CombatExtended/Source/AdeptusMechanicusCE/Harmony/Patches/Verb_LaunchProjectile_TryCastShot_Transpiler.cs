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
using AdeptusMechanicus.Lasers;
using CombatExtended;
using System.Reflection;
using AdvancedGraphics;

namespace AdeptusMechanicus.HarmonyInstance
{
    
    [HarmonyPatch(typeof(Verb_LaunchProjectileCE), "TryCastShot")]
    public static class Verb_LaunchProjectileCE_TryCastShot_Transpiler
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            FieldInfo sourceLoc = AccessTools.Field(typeof(Verb_LaunchProjectileCE), "sourceLoc");
            MethodInfo shooter = AccessTools.Property(typeof(Verb_LaunchProjectileCE), nameof(Verb_LaunchProjectileCE.Shooter)).GetGetMethod();
            for (int i = 0; i < instructionsList.Count; i++)
            {
                var instruction = instructionsList[i];
                if (i > 2 && instructionsList.Count > i + 2 && instruction.opcode == OpCodes.Ldfld && instruction.OperandIs(sourceLoc) && instructionsList[i-2].OperandIs(shooter))
                {
                    yield return instruction;
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_0);              // Verb
                    instruction = new CodeInstruction(opcode: OpCodes.Call, operand: typeof(Verb_LaunchProjectileCE_TryCastShot_Transpiler).GetMethod("MuzzlePosition"));
                    
                }
                yield return instruction;

            }

        }
        public static Vector2 MuzzlePosition(Vector2 sourceLoc, Verb_LaunchProjectileCE instance)
        {
        //    Log.Message("MuzzlePosition CE");
            Vector2 result = sourceLoc;
        //    Log.Message("MuzzlePosition CE sourceLoc: " + sourceLoc);
            string msg = "CE MuzzlePosition {0}: {1}, aimAngle: {2}";
            float aimAngle = 0f;
            if (instance.EquipmentSource is Thing equipment)
            {
            //    Log.Message("MuzzlePosition CE EquipmentSource: " + equipment);
                Thing launcher = instance.Shooter;
                Vector3 origin = new Vector3(sourceLoc.x, 0, sourceLoc.y);
                Vector3 destination = instance.CurrentTarget.Cell.ToVector3Shifted();
                if ((destination - origin).MagnitudeHorizontalSquared() > 0.001f)
                {
                    aimAngle = (destination - origin).AngleFlat();
                }
                IDrawnWeaponWithRotation rotation = equipment as IDrawnWeaponWithRotation;
                if (rotation != null)
                {
                //    Log.Message(rotation + " is IDrawnWeaponWithRotation with RotationOffset: " + rotation.RotationOffset);
                    aimAngle += rotation.RotationOffset;
                }
            //    Log.Message(string.Format(msg, "Original", sourceLoc, aimAngle));
                GunDrawExtension gunDrawExtension = equipment.def.GetModExtensionFast<GunDrawExtension>();
                if (gunDrawExtension != null)
                {
                //    Log.Message("gunDrawExtension");
                }
                else
                        if (instance.EquipmentSource.def.graphicData is GraphicData_Equippable equippable)
                {
                    bool DualWeapon = equippable.isDualWeapon;
                    Vector3 vector = equippable.OffsetPosFor(instance.CasterPawn.Rotation, DualWeapon && (instance.burstShotsLeft | 2) == 0).RotatedBy(aimAngle);
                    //    Vector3 vector = compOversized.AdjustRenderOffsetFromDir(equippable.PrimaryVerb.CasterPawn, !compOversized.FirstAttack);
                    origin += vector;
                }
                origin = equipment.MuzzlePositionFor(origin, aimAngle);
                result = new Vector2(origin.x, origin.z);
            }
        //    Log.Message(string.Format(msg, "result", result, aimAngle));
            return result;
        }
    }
    
}
