using AdeptusMechanicus.Lasers;
using AdeptusMechanicus.settings;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{

    [HarmonyPatch(typeof(PawnRenderer), "DrawEquipmentAiming", new Type[] { typeof(Thing), typeof(Vector3), typeof(float) }), StaticConstructorOnStartup]
    public static class PawnRenderer_Draw_WquipmentAiming_GunDrawing_Patch
    {
        [HarmonyPrefix, HarmonyPriority(Priority.First)]
        static void Prefix(Pawn ___pawn, ref Thing eq, ref Vector3 drawLoc, ref float aimAngle, PawnRenderer __instance)
        {
            if (___pawn == null) return;

            IDrawnWeaponWithRotation gun = eq as IDrawnWeaponWithRotation;
            if (gun == null) return;

            Stance_Busy stance_Busy = ___pawn.stances.curStance as Stance_Busy;
            if (stance_Busy != null && !stance_Busy.neverAimWeapon && stance_Busy.focusTarg.IsValid)
            {
                drawLoc -= new Vector3(0f, 0f, 0.4f).RotatedBy(aimAngle);
                float f = gun.RotationOffset;
                /*
                if ((aimAngle > 330 && aimAngle < 360) || aimAngle > 0 && aimAngle < 30)
                {
                   if (AMAMod.Dev && f != 0) Log.Message(___pawn.Name + " aiming "+eq.LabelCap+" @: "+drawLoc+" Angle: "+aimAngle + " Offet: "+ f);
                }
                */
                aimAngle = (aimAngle + gun.RotationOffset) % 360;
                drawLoc += new Vector3(0f, 0f, 0.4f).RotatedBy(aimAngle);
            }
        }
    }
}
