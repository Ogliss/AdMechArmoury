﻿using System;
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
using AdeptusMechanicus.ExtensionMethods;
using UnityEngine;
using AdeptusMechanicus.settings;

namespace AdeptusMechanicus.HarmonyInstance
{
    /*
    [HarmonyPatch(typeof(VerbTracker), "get_PrimaryVerb")]
    public static class AM_VerbTracker_PrimaryVerb_Patch
    {
        [HarmonyPostfix]
        public static void PrimaryVerb_Postfix(ref VerbTracker __instance,ref Verb __result)
        {
            if (__result.EquipmentSource!=null)
            {
                ThingWithComps weapon = __result.EquipmentSource;
                CompEquippable equippable = weapon.TryGetComp<CompEquippable>();
                if (equippable!=null)
                {
                    CompWeapon_GunSpecialRules GunExt = weapon.TryGetComp<CompWeapon_GunSpecialRules>();
                    if (GunExt!=null)
                    {
                        __result = equippable.AllVerbs[GunExt.CurrentMode];
                    }
                }
            }
        }
    }
    */
    [HarmonyPatch(typeof(CompEquippable), "get_PrimaryVerb")]
    public static class AM_VerbTracker_PrimaryVerb_Patch
    {
        [HarmonyPostfix]
        public static void PrimaryVerb_Postfix(ref CompEquippable __instance, ref Verb __result)
        {
            if (__instance.parent.TryGetComp<CompToggleFireMode>() != null)
            {
                __result.verbProps = __instance.parent.TryGetComp<CompToggleFireMode>().Active;
            }
        }
    }
}