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
using System.Reflection;
using UnityEngine;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Projectile), "Impact")]
    public static class AM_Projectile_Impact_ImpactSFX_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref Projectile __instance, ref Thing ___launcher, ref LocalTargetInfo ___intendedTarget, Thing hitThing)
        {
            if (__instance.def.HasModExtension<EffectProjectileExtension>())
            {
                EffectProjectileExtension effects = __instance.def.GetModExtension<EffectProjectileExtension>();
                if (!effects.ImpactMoteDef.NullOrEmpty())
                {

                //    MoteMaker.ThrowExplosionCell(c, explosion.Map, this.def.explosionCellMote, color);
                }

            }
        }

    }

}
