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
using AdeptusMechanicus.Lasers;

namespace AdeptusMechanicus.HarmonyInstance
{
    
    [HarmonyPatch(typeof(Verb_LaunchProjectile), "TryCastShot")]
    public static class Verb_LaunchProjectile_TryCastShot_Transpiler
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            for (int i = 0; i < instructionsList.Count; i++)
            {
                var instruction = instructionsList[i];
                if (instruction.opcode == OpCodes.Stloc_S && ((LocalBuilder)instruction.operand).LocalIndex == 6)
                {
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_0);              // Verb
                    yield return new CodeInstruction(opcode: OpCodes.Ldloc_S, 4);              // Equipment
                    yield return new CodeInstruction(opcode: OpCodes.Ldloc_3);              // launcher
                    yield return new CodeInstruction(opcode: OpCodes.Call, operand: typeof(Verb_LaunchProjectile_TryCastShot_Transpiler).GetMethod("MuzzlePosition"));
                }
                if (instruction.OperandIs(AccessTools.Method(type: typeof(Projectile), name: nameof(Projectile.Launch), parameters: new Type[] { typeof(Thing), typeof(Vector3), typeof(LocalTargetInfo), typeof(LocalTargetInfo), typeof(ProjectileHitFlags), typeof(Thing), typeof(ThingDef) })))
                {
                    yield return new CodeInstruction(opcode: OpCodes.Ldloc_1);              // shootline
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_0);              // Verb
                    yield return new CodeInstruction(opcode: OpCodes.Call, operand: typeof(Verb_LaunchProjectile_TryCastShot_Transpiler).GetMethod("ExtraProjectiles"));
                    continue;
                }

                yield return instruction;

            }

        }
        public static void ExtraProjectiles(Projectile projectile2, Thing launcher, Vector3 origin, LocalTargetInfo usedTarget, LocalTargetInfo intendedTarget, ProjectileHitFlags hitFlags, Thing equipment, ThingDef targetCoverDef, ShootLine shootLine, Verb_LaunchProjectile instance)
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
            projectile2.Launch(launcher, origin, usedTarget, intendedTarget, hitFlags, equipment, targetCoverDef);
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
                    projectile3.Launch(launcher, origin, usedTarget, intendedTarget, hitFlags, equipment, targetCoverDef);
                }
            }
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
        public static Vector3 MuzzlePosition(Vector3 DrawPos, Verb_LaunchProjectile instance, Thing equipment, Thing launcher)
        {
            Vector3 result = DrawPos;
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
            if (equipment.def.HasComp(typeof(OgsCompOversizedWeapon.CompOversizedWeapon)))
            {
                OgsCompOversizedWeapon.CompOversizedWeapon compOversized = equipment.TryGetCompFast<OgsCompOversizedWeapon.CompOversizedWeapon>();
                if (compOversized != null)
                {
                    bool DualWeapon = compOversized.Props != null && compOversized.Props.isDualWeapon;
                    Vector3 offsetMainHand = default(Vector3);
                    Vector3 offsetOffHand = default(Vector3);
                    float offHandAngle = aimAngle;
                    float mainHandAngle = aimAngle;

                    Harmony_PawnRenderer_DrawEquipmentAiming_Transpiler.SetAnglesAndOffsets(equipment, equipment as ThingWithComps, aimAngle, launcher, ref offsetMainHand, ref offsetOffHand, ref offHandAngle, ref mainHandAngle, true, DualWeapon && !compOversized.FirstAttack);
                    Vector3 vector = DualWeapon && !compOversized.FirstAttack ? offsetOffHand : offsetMainHand;
                    //    Vector3 vector = compOversized.AdjustRenderOffsetFromDir(equippable.PrimaryVerb.CasterPawn, !compOversized.FirstAttack);
                    result += vector;

                }
            }
            result = equipment.MuzzlePositionFor(result, aimAngle);
            return result;
        }
    }
    
}
