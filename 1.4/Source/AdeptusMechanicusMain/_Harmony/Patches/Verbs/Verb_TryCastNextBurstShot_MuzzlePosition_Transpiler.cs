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
using UnityEngine;
using AdeptusMechanicus.settings;
using AdvancedGraphics;

namespace AdeptusMechanicus.HarmonyInstance
{
    
    [HarmonyPatch(typeof(Verb), "TryCastNextBurstShot")]
    public static class Verb_TryCastNextBurstShot_MuzzlePosition_Transpiler
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            List<CodeInstruction> list = instructions.ToList<CodeInstruction>();
            for (int i = 0; i < list.Count; i++)
            {
                CodeInstruction instruction = list[i];
                if (instruction.OperandIs(AccessTools.Method(typeof(MoteMaker), "MakeStaticMote", new[] { typeof(IntVec3), typeof(Map), typeof(ThingDef), typeof(float) })))
                {
                //    Log.Message(i + " opcode: " + instruction.opcode + " operand: " + instruction.operand);
                    yield return new CodeInstruction(OpCodes.Ldarg_0, null);
                    instruction.operand = AccessTools.Method(typeof(Verb_TryCastNextBurstShot_MuzzlePosition_Transpiler), "ThrowMuzzleFlash", null, null);
                //    Log.Message(i + " opcode: " + instruction.opcode + " operand: " + instruction.operand);
                }
                yield return instruction;
            }
        }
        public static void ThrowMuzzleFlash(IntVec3 cell, Map map, FleckDef moteDef, float scale, Verb verb)
        {
            bool skip = false;
            if (!AMAMod.settings.AllowMuzzlePosition)
            {
                skip = true;
            }
            if (verb.GetProjectile() != null)
            {
                if (verb.GetProjectile() as Lasers.LaserBeamDef != null)
                {
                    skip = true;
                }
                if (verb.GetProjectile().GetType().ToString().Contains("Lasers.LaserBeamDef"))
                {
                    skip = true;
                }
            }
            if (verb.EquipmentSource != null && !skip)
            {
                if (verb.verbProps.range > 1.48f)
                {
                    FleckDef mote = moteDef;
                    Vector3 origin = verb.CasterIsPawn ? verb.CasterPawn.Drawer.DrawPos : verb.Caster.DrawPos;
                    Vector3 a = verb.CurrentTarget.CenterVector3;
                    float aimAngle = 0f;
                    if ((a - origin).MagnitudeHorizontalSquared() > 0.001f)
                    {
                        aimAngle = (a - origin).AngleFlat();
                    }
                    if (verb.Caster is Building_TurretGun _TurretGun)
                    {
                        origin += new Vector3 (_TurretGun.def.building.turretTopOffset.x, 0, _TurretGun.def.building.turretTopOffset.y);
                    }
                    else
                    {
                        if (verb.EquipmentSource.def.graphicData is GraphicData_Equippable equippable)
                        {
                            bool DualWeapon = equippable.isDualWeapon;
                            Vector3 vector = equippable.OffsetPosFor(verb.CasterPawn.Rotation, DualWeapon && (verb.burstShotsLeft | 2) == 0).RotatedBy(aimAngle);
                            //    Vector3 vector = compOversized.AdjustRenderOffsetFromDir(equippable.PrimaryVerb.CasterPawn, !compOversized.FirstAttack);
                            origin += vector;
                        }
                    }

                    origin = verb.MuzzlePositionFor(aimAngle, true);
                    if (verb.GetProjectile() != null) origin.y = verb.GetProjectile().Altitude;
                    AdeptusFleckMaker.Static(origin, map, mote, scale);
                    return;
                }
            }

            {
                AdeptusFleckMaker.Static(cell.ToVector3Shifted(), map, moteDef, scale);
            }
        }   
    }
    
}
