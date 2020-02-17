using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Harmony;
using Verse.Sound;
using UnityEngine;
using System.Reflection;
using AdeptusMechanicus.settings;

namespace AdeptusMechanicus.Harmony
{

    [HarmonyPatch(typeof(Verb_Shoot), "HighlightFieldRadiusAroundTarget")]
    public static class AM_Verb_Shoot_HighlightFieldRadiusAroundTarget_CustomExplosiveProjectile_Patch
    {
        [HarmonyPostfix]
        public static void HighlightFieldRadiusAroundTarget_CustomExplosiveProjectile_Postfix(ref Verb_Shoot __instance, ref float __result)
        {
            if (__instance.Projectile != null)
            {
                if (__instance.Projectile.thingClass == typeof(Projectile_ExplosiveOG))
                {
                    ThingDef_BulletExplosiveOG bulletExplosiveOG = (ThingDef_BulletExplosiveOG)__instance.Projectile;
                    if (bulletExplosiveOG.explosionradius != 0)
                    {
                        __result = bulletExplosiveOG.explosionradius;
                    }
                }
            }
        }
    }
}
