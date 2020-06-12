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
    
    [HarmonyPatch(typeof(StockGenerator_WeaponsMelee), "HandlesThingDef")]
    public static class AM_StockGenerator_WeaponsMelee_HandlesThingDef_Settings_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ThingDef td, ref bool __result) 
        {

            if (td.defName.Contains("OGI_Melee_"))
            {
                if (!SettingsHelper.latest.AllowImperialWeapons)
                {
                    __result = false;
                }
            }
            if (td.defName.Contains("OGAM_Melee_"))
            {
                if (!SettingsHelper.latest.AllowMechanicusWeapons)
                {
                    __result = false;
                }
            }
            if (td.defName.Contains("OGE_Melee_"))
            {
                if (!SettingsHelper.latest.AllowEldarWeapons)
                {
                    __result = false;
                }
            }
            if (td.defName.Contains("OGDE_Melee_"))
            {
                if (!SettingsHelper.latest.AllowDarkEldarWeapons)
                {
                    __result = false;
                }
            }
            if (td.defName.Contains("OGC_Melee_"))
            {
                if (!SettingsHelper.latest.AllowChaosWeapons)
                {
                    __result = false;
                }
            }
            if (td.defName.Contains("OGT_Melee_"))
            {
                if (!SettingsHelper.latest.AllowTauWeapons)
                {
                    __result = false;
                }
            }
            if (td.defName.Contains("OGO_Melee_"))
            {
                if (!SettingsHelper.latest.AllowOrkWeapons)
                {
                    __result = false;
                }
            }
            if (td.defName.Contains("OGN_Melee_"))
            {
                if (!SettingsHelper.latest.AllowNecronWeapons)
                {
                    __result = false;
                }
            }
            if (td.defName.Contains("OGTY_Melee_"))
            {
                if (!SettingsHelper.latest.AllowTyranidWeapons)
                {
                    __result = false;
                }
            }
            if (!__result && (td.defName.Contains("OG_") && td.defName.Contains("_Melee_")))
            {
            //    log.message("returning false for " + td.LabelCap);
            }
        }
    }
    
}
