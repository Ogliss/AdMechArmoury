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
using AdeptusMechanicus.settings;

namespace AdeptusMechanicus.HarmonyInstance
{
    [StaticConstructorOnStartup]
    [HarmonyPatch(typeof(Projectile), "Draw")]
    public static class Projectile_Draw_ProjectileGlow_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Projectile __instance/*, Vector3 ___origin, Vector3 ___destination, float ___ticksToImpact*/)
        {
            if (__instance != null && AMAMod.settings.AllowProjectileGlow && __instance as Bullet_Explosive == null)
            {
                if (__instance.def.HasModExtension<GlowerProjectileExtension>())
                {
                    GlowerProjectileExtension glower = __instance.def.GetModExtensionFast<GlowerProjectileExtension>();
                    if (glower != null)
                    {
                        glower.Glow(__instance, __instance.ExactRotation);
                        /*
                        Material material = glower.GlowMoteDef.graphic.MatSingle;
                        if (glower.useGraphicColor)
                        {
                            material.color = __instance.DrawColor;
                        }
                        else
                        if (glower.useGraphicColorTwo)
                        {
                            material.color = __instance.DrawColorTwo;
                        }

                        Mesh mesh2 = MeshPool.GridPlane(glower.GlowMoteDef.graphicData.drawSize * glower.GlowMoteSize);
                        Graphics.DrawMesh(mesh2, __instance.DrawPos, __instance.ExactRotation, material, 0);
                        */
                    }
                }
                /*
                if (__instance.def.HasModExtension<ScattershotProjectileExtension>())
                {
                    ScattershotProjectileExtension scattershot = __instance.def.GetModExtensionFast<ScattershotProjectileExtension>();
                    if (scattershot != null)
                    {

                        for (int i = 0; i < length; i++)
                        {

                            float num = ArcHeightFactor * GenMath.InverseParabola(DistanceCoveredFraction());
                            Vector3 drawPos = __instance.DrawPos;
                            Vector3 position = drawPos + new Vector3(0f, 0f, 1f) * num;
                            if (__instance.def.projectile.shadowSize > 0f)
                            {
                                DrawShadow(__instance, drawPos, num);
                            }
                            Graphics.DrawMesh(MeshPool.GridPlane(__instance.def.graphicData.drawSize), position, __instance.ExactRotation, __instance.def.DrawMatSingle, 0);
                        }
                    }
                }
                */
            }
        }

        private static float StartingTicksToImpact(Projectile projectile, Vector3 origin, Vector3 destination)
        {
                float num = (origin - destination).magnitude / projectile.def.projectile.SpeedTilesPerTick;
                if (num <= 0f)
                {
                    num = 0.001f;
                }
                return num;
        }
        private static float DistanceCoveredFraction(float ticksToImpact, float StartingTicksToImpact)
        {
            return Mathf.Clamp01(1f - (float)ticksToImpact / StartingTicksToImpact);
        }
        private static float ArcHeightFactor(Projectile projectile, Vector3 origin, Vector3 destination)
        {
            float num = projectile.def.projectile.arcHeightFactor;
            float num2 = (destination - origin).MagnitudeHorizontalSquared();
            if (num * num > num2 * 0.2f * 0.2f)
            {
                num = Mathf.Sqrt(num2) * 0.2f;
            }
            return num;
        }
        private static void DrawShadow(Projectile projectile, Vector3 drawLoc, float height)
        {
            if (shadowMaterial == null)
            {
                return;
            }
            float num = projectile.def.projectile.shadowSize * Mathf.Lerp(1f, 0.6f, height);
            Vector3 s = new Vector3(num, 1f, num);
            Vector3 b = new Vector3(0f, -0.01f, 0f);
            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetTRS(drawLoc + b, Quaternion.identity, s);
            Graphics.DrawMesh(MeshPool.plane10, matrix, shadowMaterial, 0);
        }

        // Token: 0x04000EA0 RID: 3744
        private static readonly Material shadowMaterial = MaterialPool.MatFrom("Things/Skyfaller/SkyfallerShadowCircle", ShaderDatabase.Transparent);

    }
}
