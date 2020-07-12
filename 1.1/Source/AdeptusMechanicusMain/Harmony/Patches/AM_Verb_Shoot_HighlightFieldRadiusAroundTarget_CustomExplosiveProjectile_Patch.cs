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
using UnityEngine;
using System.Reflection;
using AdeptusMechanicus.settings;

namespace AdeptusMechanicus.HarmonyInstance
{

    [HarmonyPatch(typeof(Verb_LaunchProjectile), "HighlightFieldRadiusAroundTarget")]
    public static class AM_Verb_Shoot_HighlightFieldRadiusAroundTarget_CustomExplosiveProjectile_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref Verb_LaunchProjectile __instance, ref float __result)
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

    [HarmonyPatch(typeof(AbilitesExtended.Verb_EquipmentLaunchProjectile), "HighlightFieldRadiusAroundTarget")]
    public static class AM_Verb_UseEquipment_HighlightFieldRadiusAroundTarget_CustomExplosiveProjectile_Patch
    {
        [HarmonyPostfix]
        public static void HighlightFieldRadiusAroundTarget_CustomExplosiveProjectile_Postfix(ref AbilitesExtended.Verb_EquipmentLaunchProjectile __instance, ref float __result)
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
