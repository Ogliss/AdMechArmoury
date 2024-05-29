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
                if (!AMAMod.settings.AllowImperialWeapons && (__instance.defName.StartsWith("OGI_") || __instance.defName.StartsWith("OGOA_") || __instance.defName.StartsWith("OGAA_") || __instance.defName.StartsWith("OGIQ_") || __instance.defName.StartsWith("OGAM_") || __instance.defName.StartsWith("OGIG_") || __instance.defName.StartsWith("OGAS_")) && (__instance.defName.Contains("_Melee_") || __instance.defName.Contains("_Gun_")))
                    __result = false;
                else
                if (!AMAMod.settings.AllowAssassinorumWeapons && __instance.defName.StartsWith("OGOA_") && (__instance.defName.Contains("_Melee_") || __instance.defName.Contains("_Gun_")))
                    __result = false;
                else
                if (!AMAMod.settings.AllowAstartesWeapons && __instance.defName.StartsWith("OGAA_") && (__instance.defName.Contains("_Melee_") || __instance.defName.Contains("_Gun_")))
                    __result = false;
                else
                if (!AMAMod.settings.AllowInquisitorialWeapons && __instance.defName.StartsWith("OGIQ_") && (__instance.defName.Contains("_Melee_") || __instance.defName.Contains("_Gun_")))
                    __result = false;
                else
                if (!AMAMod.settings.AllowMechanicusWeapons && __instance.defName.StartsWith("OGAM_") && (__instance.defName.Contains("_Melee_") || __instance.defName.Contains("_Gun_")))
                    __result = false;
                else
                if (!AMAMod.settings.AllowMilitarumWeapons && __instance.defName.StartsWith("OGIG_") && (__instance.defName.Contains("_Melee_") || __instance.defName.Contains("_Gun_")))
                    __result = false;
                else
                if (!AMAMod.settings.AllowSororitasWeapons && __instance.defName.StartsWith("OGAS_") && (__instance.defName.Contains("_Melee_") || __instance.defName.Contains("_Gun_")))
                    __result = false;
                else
                if (__instance.defName.StartsWith("OGE_") && (__instance.defName.Contains("_Melee_") || __instance.defName.Contains("_Gun_")))
                {
                    if (!AMAMod.settings.AllowEldarWeapons)
                    {
                        __result = false;
                    }
                }
                else
                if (__instance.defName.StartsWith("OGDE_") && (__instance.defName.Contains("_Melee_") || __instance.defName.Contains("_Gun_")))
                {
                    if (!AMAMod.settings.AllowDarkEldarWeapons)
                    {
                        __result = false;
                    }
                }
                else
                if (__instance.defName.StartsWith("OGC_") && (__instance.defName.Contains("_Melee_") || __instance.defName.Contains("_Gun_")))
                {
                    if (!AMAMod.settings.AllowChaosWeapons)
                    {
                        __result = false;
                    }
                }
                else
                if (__instance.defName.StartsWith("OGT_") && (__instance.defName.Contains("_Melee_") || __instance.defName.Contains("_Gun_")))
                {
                    if (!AMAMod.settings.AllowTauWeapons)
                    {
                        __result = false;
                    }
                }
                else
                if (__instance.defName.StartsWith("OGO_") && (__instance.defName.Contains("_Melee_") || __instance.defName.Contains("_Gun_")))
                {
                    if (!AMAMod.settings.AllowOrkWeapons)
                    {
                        __result = false;
                    }
                }
                else
                if (__instance.defName.StartsWith("OGN_") && (__instance.defName.Contains("_Melee_") || __instance.defName.Contains("_Gun_")))
                {
                    if (!AMAMod.settings.AllowNecronWeapons)
                    {
                        __result = false;
                    }
                }
                else
                if (__instance.defName.StartsWith("OGTY_") && (__instance.defName.Contains("_Melee_") || __instance.defName.Contains("_Gun_")))
                {
                    if (!AMAMod.settings.AllowTyranidWeapons)
                    {
                        __result = false;
                    }
                }
            }
        }
    }
    
}
