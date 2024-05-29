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
    
//    [HarmonyPatch(typeof(StockGenerator_WeaponsMelee), "HandlesThingDef")]
    public static class StockGenerator_WeaponsMelee_HandlesThingDef_Settings_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ThingDef td, ref bool __result) 
        {

            if (td.defName.Contains("OGI_Melee_"))
            {
                if (!AMAMod.settings.AllowImperialWeapons)
                {
                    __result = false;
                }
            }
            if (td.defName.Contains("OGAM_Melee_"))
            {
                if (!AMAMod.settings.AllowMechanicusWeapons)
                {
                    __result = false;
                }
            }
            if (td.defName.Contains("OGE_Melee_"))
            {
                if (!AMAMod.settings.AllowEldarWeapons)
                {
                    __result = false;
                }
            }
            if (td.defName.Contains("OGDE_Melee_"))
            {
                if (!AMAMod.settings.AllowDarkEldarWeapons)
                {
                    __result = false;
                }
            }
            if (td.defName.Contains("OGC_Melee_"))
            {
                if (!AMAMod.settings.AllowChaosWeapons)
                {
                    __result = false;
                }
            }
            if (td.defName.Contains("OGT_Melee_"))
            {
                if (!AMAMod.settings.AllowTauWeapons)
                {
                    __result = false;
                }
            }
            if (td.defName.Contains("OGO_Melee_"))
            {
                if (!AMAMod.settings.AllowOrkWeapons)
                {
                    __result = false;
                }
            }
            if (td.defName.Contains("OGN_Melee_"))
            {
                if (!AMAMod.settings.AllowNecronWeapons)
                {
                    __result = false;
                }
            }
            if (td.defName.Contains("OGTY_Melee_"))
            {
                if (!AMAMod.settings.AllowTyranidWeapons)
                {
                    __result = false;
                }
            }
            if (td.HasModExtension<RelicExtension>())
            {
           //     Log.Message("Trader Spawned Relic: " + td);
                if (Find.World.GetComponent<RelicTracker>() is RelicTracker relicTracker)
                {
                    __result = relicTracker.CanSpawn(td);
                 //   Log.Message("Relic: " + td.LabelCap+" Allow: "+__result);
                }
            }
            if (!__result && (td.defName.Contains("OG_") && td.defName.Contains("_Melee_")))
            {
            //    log.message("returning false for " + td.LabelCap);
            }
        }
    }
    
}
