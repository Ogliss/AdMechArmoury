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
    public static class Projectile_Impact_ImpactSFX_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(ref Projectile __instance, ref Thing ___launcher, ref LocalTargetInfo ___intendedTarget, Thing hitThing)
        {
            if (__instance.def.HasModExtension<EffectProjectileExtension>())
            {
                EffectProjectileExtension effects = __instance.def.GetModExtension<EffectProjectileExtension>();
                Vector3 vector = __instance.ExactPosition;
                effects.ThrowMote(vector, __instance.Map, __instance.def.projectile.damageDef.explosionCellMote, effects.explosionMoteSize,  __instance.def.projectile.damageDef.explosionColorCenter, __instance.def.projectile.damageDef.soundExplosion, ThingDef.Named(effects.ImpactMoteDef) ?? null, effects.ImpactMoteSizeRange?.RandomInRange ?? effects.ImpactMoteSize, ThingDef.Named(effects.ImpactGlowMoteDef) ?? null, effects.ImpactGlowMoteSizeRange?.RandomInRange ?? effects.ImpactGlowMoteSize, hitThing);
            }
        }

    }

}
