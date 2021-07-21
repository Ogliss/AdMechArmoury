using AdeptusMechanicus.Lasers;
using HarmonyLib;
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
    [HarmonyPatch(typeof(TurretTop), "DrawTurret"), StaticConstructorOnStartup]
    class TuretTop_DrawTurret_LaserTurret_Patch
    {

        static bool Prefix(TurretTop __instance, Vector3 recoilDrawOffset, float recoilAngleOffset, float ___curRotationInt, Building_Turret ___parentTurret)
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

            Vector3 offset = new Vector3(___parentTurret.def.building.turretTopOffset.x, 0f, ___parentTurret.def.building.turretTopOffset.y).RotatedBy(___curRotationInt);
            float scale = ___parentTurret.def.building.turretTopDrawSize;
            offset = offset.RotatedBy(recoilAngleOffset);
            offset += recoilDrawOffset;
            Matrix4x4 Matrix = default(Matrix4x4);
            Matrix.SetTRS(___parentTurret.DrawPos + Altitudes.AltIncVect + offset, (___curRotationInt + (float)TurretTop.ArtworkRotation).ToQuat(), new Vector3(scale, 1f, scale));
            Graphics.DrawMesh(MeshPool.plane10, Matrix, material, 0);
            return false;
        }
    }
}
