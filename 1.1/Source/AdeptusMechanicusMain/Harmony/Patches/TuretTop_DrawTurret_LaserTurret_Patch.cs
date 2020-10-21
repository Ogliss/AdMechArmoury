﻿using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    [HarmonyPatch(typeof(TurretTop), "DrawTurret", new Type[] { }), StaticConstructorOnStartup]
    class TuretTop_DrawTurret_LaserTurret_Patch
    {
        static FieldInfo parentTurretField;
        static FieldInfo curRotationIntField;

        static TuretTop_DrawTurret_LaserTurret_Patch()
        {
            parentTurretField = typeof(TurretTop).GetField("parentTurret", BindingFlags.NonPublic | BindingFlags.Instance);
            curRotationIntField = typeof(TurretTop).GetField("curRotationInt", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        static bool Prefix(TurretTop __instance, float ___curRotationInt, Building_Turret ___parentTurret)
        {
            Building_LaserGun turret = ___parentTurret as Building_LaserGun;
            if (turret == null) return true;
            float rotation = ___curRotationInt;
            if (turret.TargetCurrentlyAimingAt.HasThing)
            {
                rotation = (turret.TargetCurrentlyAimingAt.CenterVector3 - turret.TrueCenter()).AngleFlat();
            }

            IDrawnWeaponWithRotation gunRotation = turret.gun as IDrawnWeaponWithRotation;
            if (gunRotation != null) rotation += gunRotation.RotationOffset;

            Material material = ___parentTurret.def.building.turretTopMat;
            SpinningLaserGunTurret spinningGun = turret.gun as SpinningLaserGunTurret;
            if (spinningGun != null)
            {
                spinningGun.turret = turret;
                material = spinningGun.Graphic.MatSingle;
            }

            Vector3 b = new Vector3(___parentTurret.def.building.turretTopOffset.x, 0f, ___parentTurret.def.building.turretTopOffset.y);
            float turretTopDrawSize = ___parentTurret.def.building.turretTopDrawSize;
            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetTRS(turret.DrawPos + Altitudes.AltIncVect + b, (rotation + (float)TurretTop.ArtworkRotation).ToQuat(), new Vector3(turretTopDrawSize, 1f, turretTopDrawSize));
            Graphics.DrawMesh(MeshPool.plane10, matrix, material, 0);

            return false;
        }
    }
}
