using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using HarmonyLib;
using AdeptusMechanicus.ExtensionMethods;
using UnityEngine;
using System.Reflection.Emit;
using AdeptusMechanicus.Lasers;
using AdeptusMechanicus.settings;
using System.Reflection;
using AdvancedGraphics;

namespace AdeptusMechanicus.HarmonyInstance
{
    
    [HarmonyPatch(typeof(Verb_LaunchProjectile), "TryCastShot")]
    public static class Verb_LaunchProjectile_TryCastShot_Transpiler
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            MethodInfo drawPos = AccessTools.Method(typeof(Thing), "get_DrawPos");
            MethodInfo shootLine_Dest = AccessTools.Method(typeof(ShootLine), "get_Dest");
            for (int i = 0; i < instructionsList.Count; i++)
            {
                var instruction = instructionsList[i];
                if (instruction.opcode == OpCodes.Stloc_S && ((LocalBuilder)instruction.operand).LocalIndex == 6)
                {
                //    Log.Message("MuzzlePosition patch at " + i);
                    yield return new CodeInstruction(OpCodes.Ldarg_0);              // Verb
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 4);              // Equipment
                    yield return new CodeInstruction(OpCodes.Ldloc_3);              // launcher
                    yield return new CodeInstruction(OpCodes.Call, operand: typeof(Verb_LaunchProjectile_TryCastShot_Transpiler).GetMethod("MuzzlePosition"));
                }
                
                if (instruction.OperandIs(AccessTools.Method(type: typeof(Projectile), name: nameof(Projectile.Launch), parameters: new Type[] { typeof(Thing), typeof(Vector3), typeof(LocalTargetInfo), typeof(LocalTargetInfo), typeof(ProjectileHitFlags), typeof(bool), typeof(Thing), typeof(ThingDef) })))
                {
                //    Log.Message("ExtraProjectiles patch at " + i);
                    yield return instruction;
                    if (instructionsList[i - 9].OperandIs(shootLine_Dest))
                    {
                    //    Log.Message($"TryCastShot_Transpiler {i - 13} {instructionsList[i - 13].opcode}, {instructionsList[i - 13].operand}");
                        yield return new CodeInstruction(instructionsList[i - 13].opcode, instructionsList[i - 13].operand);
                    }
                //    Log.Message($"TryCastShot_Transpiler {i - 12} {instructionsList[i - 12].opcode}, {instructionsList[i - 12].operand}");
                    yield return new CodeInstruction(instructionsList[i-12].opcode, instructionsList[i-12].operand);
                //    Log.Message($"TryCastShot_Transpiler {i - 11} {instructionsList[i - 11].opcode}, {instructionsList[i - 11].operand}");             // shootline
                    yield return new CodeInstruction(instructionsList[i-11].opcode, instructionsList[i-11].operand);
                //    Log.Message($"TryCastShot_Transpiler {i - 10} {instructionsList[i - 10].opcode}, {instructionsList[i - 10].operand}");             // shootline
                    yield return new CodeInstruction(instructionsList[i-10].opcode, instructionsList[i-10].operand);
                //    Log.Message($"TryCastShot_Transpiler {i - 9} {instructionsList[i - 9].opcode}, {instructionsList[i - 9].operand}");           // shootline
                    yield return new CodeInstruction(instructionsList[i-9].opcode, instructionsList[i-9].operand);
                //    Log.Message($"TryCastShot_Transpiler {i - 8} {instructionsList[i - 8].opcode}, {instructionsList[i - 8].operand}");            // shootline
                    yield return new CodeInstruction(instructionsList[i-8].opcode, instructionsList[i-8].operand);
                //    Log.Message($"TryCastShot_Transpiler {i - 7} {instructionsList[i - 7].opcode}, {instructionsList[i - 7].operand}");           // shootline
                    yield return new CodeInstruction(instructionsList[i-7].opcode, instructionsList[i-7].operand);
                //    Log.Message($"TryCastShot_Transpiler {i - 6} {instructionsList[i - 6].opcode}, {instructionsList[i - 6].operand}");           // shootline
                    yield return new CodeInstruction(instructionsList[i-6].opcode, instructionsList[i-6].operand);
                //    Log.Message($"TryCastShot_Transpiler {i - 5} {instructionsList[i - 5].opcode}, {instructionsList[i - 5].operand}");          // shootline
                    yield return new CodeInstruction(instructionsList[i-5].opcode, instructionsList[i-5].operand);
                //    Log.Message($"TryCastShot_Transpiler {i - 4} {instructionsList[i - 4].opcode}, {instructionsList[i - 4].operand}");         // shootline
                    yield return new CodeInstruction(instructionsList[i-4].opcode, instructionsList[i-4].operand);
                //    Log.Message($"TryCastShot_Transpiler {i - 3} {instructionsList[i - 3].opcode}, {instructionsList[i - 3].operand}");         // shootline
                    yield return new CodeInstruction(instructionsList[i-3].opcode, instructionsList[i-3].operand);
                //    Log.Message($"TryCastShot_Transpiler {i - 2} {instructionsList[i - 2].opcode}, {instructionsList[i - 2].operand}");          // shootline
                    yield return new CodeInstruction(instructionsList[i-2].opcode, instructionsList[i-2].operand);
                //    Log.Message($"TryCastShot_Transpiler {i - 1} {instructionsList[i - 1].opcode}, {instructionsList[i - 1].operand}");         // shootline
                    yield return new CodeInstruction(instructionsList[i-1].opcode, instructionsList[i-1].operand);              // shootline
                    yield return new CodeInstruction(OpCodes.Ldloc_1);              // shootline
                    yield return new CodeInstruction(OpCodes.Ldarg_0);              // Verb
                    yield return new CodeInstruction(OpCodes.Call, operand: typeof(Verb_LaunchProjectile_TryCastShot_Transpiler).GetMethod("ExtraProjectiles"));
                    continue;
                }
                
                yield return instruction;

            }

        }
        public static void ExtraProjectiles(Projectile projectile2, Thing launcher, Vector3 origin, LocalTargetInfo usedTarget, LocalTargetInfo intendedTarget, ProjectileHitFlags hitFlags, bool preventFriendlyFire, Thing equipment, ThingDef targetCoverDef, ShootLine shootLine, Verb_LaunchProjectile instance)
        {
            IDrawnWeaponWithRotation weapon = null;
            Pawn pawn = launcher as Pawn;
            if (pawn != null && pawn.equipment != null) weapon = pawn.equipment.Primary as IDrawnWeaponWithRotation;
            if (weapon == null)
            {
                Building_LaserGun turret = launcher as Building_LaserGun;
                if (turret != null)
                {
                    weapon = turret.gun as IDrawnWeaponWithRotation;
                }
            }
            if (weapon != null)
            {
                float angle = (usedTarget.CenterVector3 - origin).AngleFlat() - (intendedTarget.CenterVector3 - origin).AngleFlat();
                weapon.RotationOffset = (angle + 180) % 360 - 180;
            }
        //    projectile2.Launch(launcher, origin, usedTarget, intendedTarget, hitFlags, preventFriendlyFire, equipment, targetCoverDef);
            int extras = 0;
            IAdvancedVerb Props = instance.verbProps as IAdvancedVerb;
            if (Props != null)
            {
                extras = Props.ScattershotCount;
            }
            else
            {
                ScattershotProjectileExtension ext = projectile2.def.GetModExtensionFast<ScattershotProjectileExtension>();
                if (ext != null && ext.projectileCount.HasValue)
                {
                    extras = ext.projectileCount.Value;
                }
            }
            if (extras > 0)
            {
                for (int i = 0; i < extras; i++)
                {
                    Projectile projectile3 = (Projectile)GenSpawn.Spawn(projectile2.def, shootLine.Source, launcher.Map, WipeMode.Vanish);
                    projectile3.Launch(launcher, origin, usedTarget, intendedTarget, hitFlags, preventFriendlyFire, equipment, targetCoverDef);
                }
            }
        }
        /*
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
        */
        public static Vector3 MuzzlePosition(Vector3 DrawPos, Verb_LaunchProjectile instance, Thing equipment, Thing launcher)
        {
            Vector3 result = DrawPos;
            if (equipment == null || !AMAMod.settings.AllowMuzzlePosition)
            {
                return result;
            }
            Vector3 destination = instance.CurrentTarget.Cell.ToVector3Shifted();
            float aimAngle = 0f;
            if ((destination - result).MagnitudeHorizontalSquared() > 0.001f)
            {
                aimAngle = (destination - result).AngleFlat();
            }
            IDrawnWeaponWithRotation rotation = equipment as IDrawnWeaponWithRotation;
            if (rotation != null)
            {
                //    Log.Message(gunOG + " is IDrawnWeaponWithRotation with RotationOffset: "+ gunOG.RotationOffset);
                aimAngle += rotation.RotationOffset;
            }
            if (equipment.def.graphicData is GraphicData_Equippable equippable)
            {
                bool DualWeapon = equippable.isDualWeapon;
                Vector3 vector = equippable.OffsetPosFor(instance.CasterPawn.Rotation, DualWeapon && (instance.burstShotsLeft | 2) == 0).RotatedBy(aimAngle); 
                //    Vector3 vector = compOversized.AdjustRenderOffsetFromDir(equippable.PrimaryVerb.CasterPawn, !compOversized.FirstAttack);
                result += vector;
            }
            result = equipment.MuzzlePositionFor(result, aimAngle);
            return result;
        }

    }
    
    
}
