using Verse;
using HarmonyLib;
using System;
using UnityEngine;
using System.Reflection;
using OgsCompOversizedWeapon;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Projectile), "Launch", new Type[] { typeof(Thing), typeof(Vector3), typeof(LocalTargetInfo), typeof(LocalTargetInfo), typeof(ProjectileHitFlags), typeof(Thing), typeof(ThingDef)})]
    public static class Projectile_Launch_MuzzlePosition_Patch
    {
        static PropertyInfo StartingTicksToImpactProp = typeof(Projectile).GetProperty("StartingTicksToImpact", BindingFlags.NonPublic | BindingFlags.Instance);
        static void Postfix(Projectile __instance, Vector3 ___destination, Thing launcher, ref Vector3 ___origin, LocalTargetInfo intendedTarget, Thing equipment, ref int ___ticksToImpact)
        {
            if (__instance is LaserBeam)
            {
                return;
            }
            if (equipment != null)
            {
                float aimAngle = (___destination - ___origin).AngleFlat();
                OgsCompOversizedWeapon.CompOversizedWeapon compOversized = equipment.TryGetComp<OgsCompOversizedWeapon.CompOversizedWeapon>();
                CompEquippable equippable = equipment.TryGetComp<CompEquippable>();
                if (compOversized != null)
                {
                    bool DualWeapon = compOversized.Props != null && compOversized.Props.isDualWeapon;
                    Vector3 offsetMainHand = default(Vector3);
                    Vector3 offsetOffHand = default(Vector3);
                    float offHandAngle = aimAngle;
                    float mainHandAngle = aimAngle;

                    Harmony_PawnRenderer_DrawEquipmentAiming_Transpiler.SetAnglesAndOffsets(equipment, equipment as ThingWithComps, aimAngle, launcher as Pawn, ref offsetMainHand, ref offsetOffHand, ref offHandAngle, ref mainHandAngle, true, DualWeapon && !compOversized.FirstAttack);
                    Vector3 vector = DualWeapon && !compOversized.FirstAttack ? offsetOffHand : offsetMainHand;
                //    Vector3 vector = compOversized.AdjustRenderOffsetFromDir(equippable.PrimaryVerb.CasterPawn, !compOversized.FirstAttack);
                    ___origin += vector;

                }
                if (equipment.def.HasModExtension<BarrelOffsetExtension>())
                {
                    BarrelOffsetExtension ext = equipment.def.GetModExtension<BarrelOffsetExtension>();
                    if (launcher as Pawn != null)
                    {
                        ___origin += (___destination - ___origin).normalized * (equipment?.def.graphic.drawSize.magnitude * (ext.barrellength + (0.2f * __instance.Graphic.drawSize.magnitude)) ?? 1f);
                        ___ticksToImpact = Mathf.CeilToInt((float)StartingTicksToImpactProp.GetValue(__instance));
                        if (___ticksToImpact < 1) ___ticksToImpact = 1;
                    }
                    //    origin = MuzzlePosition(launcher, usedTarget, equipment?.def.graphic.drawSize.magnitude * 5f ?? 5f);
                }
            }
        }

    }
}
