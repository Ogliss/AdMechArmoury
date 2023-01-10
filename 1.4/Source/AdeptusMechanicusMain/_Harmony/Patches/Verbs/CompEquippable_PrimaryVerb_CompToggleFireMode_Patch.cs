using System;
using System.Text;
using Verse;
using RimWorld;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using Verse.Sound;
using System.Reflection;
using AdeptusMechanicus.ExtensionMethods;
using UnityEngine;
using AdeptusMechanicus.settings;
using System.Collections.Generic;

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

    [HarmonyPatch(typeof(CompEquippable), "get_VerbProperties")]
    public class CompEquippable_getVerbProperties_QualityVerbs_Patch
    {
        private static List<VerbProperties> Postfix(List<VerbProperties> __result, CompEquippable __instance)
        {

            if (!__result.NullOrEmpty())
            {
                List<VerbProperties> result = new List<VerbProperties>();
                foreach (var item in __result)
                {
                    if (item is AdvancedVerbProperties props && props.minQuality > QualityCategory.Awful)
                    {
                        if (__instance.parent.TryGetQuality(out QualityCategory quality) && quality < props.minQuality)
                        {
                            continue;
                        }
                    }
                    result.Add(item);
                }
                return result;
            }
            return __result;
        }
    }
    [HarmonyPatch(typeof(CompEquippable), "get_Tools")]
    public class CompEquippable_getTools_QualityVerbs_Patch
    {
        private static List<Tool> Postfix(List<Tool> __result, CompEquippable __instance)
        {
            if (!__result.NullOrEmpty())
            {
                List<Tool> result = new List<Tool>();
                foreach (var item in __result)
                {
                    if (item is AdvancedTool props && props.minQuality > QualityCategory.Awful)
                    {
                        if (__instance.parent.TryGetQuality(out QualityCategory quality) && quality < props.minQuality)
                        {
                            continue;
                        }
                    }
                    result.Add(item);
                }
                return result;
            }
            return __result;
        }
    }
}