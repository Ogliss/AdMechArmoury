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
using CombatExtended;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(ProjectileCE), "Draw")]
    public static class Projectile_Draw_ProjectileGlow_Patch_CE
    {
     //   [HarmonyPostfix]
        public static void Postfix(ProjectileCE __instance)
        {
            if (__instance != null)
            {
                if (__instance.def.HasModExtension<GlowerProjectileExtension>())
                {

                    GlowerProjectileExtension glower = __instance.def.GetModExtension<GlowerProjectileExtension>();
                    if (glower != null)
                    {
                        glower.Glow(__instance, __instance.ExactRotation);
                        /*
                        Mesh mesh2 = MeshPool.GridPlane(glower.GlowMoteDef.graphicData.drawSize * glower.GlowMoteSize);
                        Graphics.DrawMesh(mesh2, __instance.DrawPos, __instance.ExactRotation, glower.GlowMoteDef.graphic.MatSingle, 0);
                        */
                    }
                }
            }
        }
    }
}
