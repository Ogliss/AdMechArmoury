using System;
using System.Text;
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
    public static class VerbTracker_PrimaryVerb_Patch
    {
        [HarmonyPostfix]
        public static void PrimaryVerb_Postfix(ref VerbTracker __instance,ref Verb __result)
        {
            if (__result.EquipmentSource!=null)
            {
                ThingWithComps weapon = __result.EquipmentSource;
                CompEquippable equippable = weapon.TryGetCompFast<CompEquippable>();
                if (equippable!=null)
                {
                    CompWeapon_GunSpecialRules GunExt = weapon.TryGetCompFast<CompWeapon_GunSpecialRules>();
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
    public class CompEquippable_PrimaryVerb_CompToggleFireMode_Patch
    {
        private static void Postfix(CompEquippable __instance, ref Verb __result)
        {
            var comp = __instance.parent.TryGetCompFast<CompToggleFireMode>();
            if (comp != null)
            {
                __result.verbProps = __instance.parent.TryGetCompFast<CompToggleFireMode>().ActiveProps;
                __result = comp.ActiveVerb;
            }
        }
    }
}