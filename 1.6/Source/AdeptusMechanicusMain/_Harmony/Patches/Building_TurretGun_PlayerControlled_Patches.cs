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
using AdeptusMechanicus.settings;

namespace AdeptusMechanicus.HarmonyInstance
{
    
    [HarmonyPatch(typeof(RimWorld.Building_TurretGun), "get_PlayerControlled")]
    public static class Building_TurretGun_PlayerControlled_TargetableTurrets_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(RimWorld.Building_TurretGun __instance, ref bool __result)
        {
            if (AMAMod.Dev && __instance is Building_TurretGun Turret)
            {
                __result = Turret.PlayerControlled;
            }
        }
    }

    [HarmonyPatch(typeof(RimWorld.Building_TurretGun), "get_CanSetForcedTarget")]
    public static class Building_TurretGun_CanSetForcedTarget_TargetableTurrets_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(RimWorld.Building_TurretGun __instance, ref bool __result)
        {
            if (AMAMod.Dev && __instance is Building_TurretGun Turret)
            {
                __result = Turret.CanSetForcedTarget;
            }
        }
    }
    
    [HarmonyPatch(typeof(RimWorld.Building_TurretGun), "get_IsMortar")]
    public static class Building_TurretGun_IsMortar_TargetableTurrets_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(RimWorld.Building_TurretGun __instance, ref bool __result)
        {
            if (AMAMod.Dev && __instance is Building_TurretGun Turret)
            {
                __result = Turret.IsMortar;
            }
        }
    }
    
    [HarmonyPatch(typeof(RimWorld.Building_TurretGun), "get_IsMortarOrProjectileFliesOverhead")]
    public static class Building_TurretGun_IsMortarOrProjectileFliesOverhead_TargetableTurrets_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(RimWorld.Building_TurretGun __instance, ref bool __result)
        {
            if (AMAMod.Dev && __instance is Building_TurretGun Turret)
            {
                __result = Turret.IsMortarOrProjectileFliesOverhead;
            }
        }
    }
    
}
