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
    
    [HarmonyPatch(typeof(ThingDef), "get_PlayerAcquirable")]
    public static class ThingDef_get_PlayerAcquirable_ModOptions_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ThingDef __instance, ref bool __result)
        {
            if (__result)
            {
                if (__instance.defName.Contains("OGI_") && (__instance.defName.Contains("_Melee_") || __instance.defName.Contains("_Gun_")))
                {
                    if (!SettingsHelper.latest.AllowImperialWeapons)
                    {
                        __result = false;
                    }
                }
                else
                if (__instance.defName.Contains("OGAM_") && (__instance.defName.Contains("_Melee_") || __instance.defName.Contains("_Gun_")))
                {
                    if (!SettingsHelper.latest.AllowMechanicusWeapons)
                    {
                        __result = false;
                    }
                }
                else
                if (__instance.defName.Contains("OGE_") && (__instance.defName.Contains("_Melee_") || __instance.defName.Contains("_Gun_")))
                {
                    if (!SettingsHelper.latest.AllowEldarWeapons)
                    {
                        __result = false;
                    }
                }
                else
                if (__instance.defName.Contains("OGDE_") && (__instance.defName.Contains("_Melee_") || __instance.defName.Contains("_Gun_")))
                {
                    if (!SettingsHelper.latest.AllowDarkEldarWeapons)
                    {
                        __result = false;
                    }
                }
                else
                if (__instance.defName.Contains("OGC_") && (__instance.defName.Contains("_Melee_") || __instance.defName.Contains("_Gun_")))
                {
                    if (!SettingsHelper.latest.AllowChaosWeapons)
                    {
                        __result = false;
                    }
                }
                else
                if (__instance.defName.Contains("OGT_") && (__instance.defName.Contains("_Melee_") || __instance.defName.Contains("_Gun_")))
                {
                    if (!SettingsHelper.latest.AllowTauWeapons)
                    {
                        __result = false;
                    }
                }
                else
                if (__instance.defName.Contains("OGO_") && (__instance.defName.Contains("_Melee_") || __instance.defName.Contains("_Gun_")))
                {
                    if (!SettingsHelper.latest.AllowOrkWeapons)
                    {
                        __result = false;
                    }
                }
                else
                if (__instance.defName.Contains("OGN_") && (__instance.defName.Contains("_Melee_") || __instance.defName.Contains("_Gun_")))
                {
                    if (!SettingsHelper.latest.AllowNecronWeapons)
                    {
                        __result = false;
                    }
                }
                else
                if (__instance.defName.Contains("OGTY_") && (__instance.defName.Contains("_Melee_") || __instance.defName.Contains("_Gun_")))
                {
                    if (!SettingsHelper.latest.AllowTyranidWeapons)
                    {
                        __result = false;
                    }
                }
            }
        }
    }
    
}
