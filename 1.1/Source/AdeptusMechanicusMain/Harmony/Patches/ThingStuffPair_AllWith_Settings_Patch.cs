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
    
    [HarmonyPatch(typeof(ThingStuffPair), "AllWith")]
    public static class ThingStuffPair_AllWith_Settings_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref List<ThingStuffPair> __result)
        {
            List<ThingStuffPair> list = new List<ThingStuffPair>();
            
            if (!AMAMod.settings.AllowImperialWeapons)
            {
                __result.RemoveAll(x => (x.thing.defName.Contains("OGI_")) && (x.thing.defName.Contains("_Gun_") || x.thing.defName.Contains("_Melee_")));
            }
            if (!AMAMod.settings.AllowMechanicusWeapons)
            {
                __result.RemoveAll(x => (x.thing.defName.Contains("OGAM_")) && (x.thing.defName.Contains("_Gun_") || x.thing.defName.Contains("_Melee_")));
            }
            if (!AMAMod.settings.AllowEldarWeapons)
            {
                __result.RemoveAll(x => (x.thing.defName.Contains("OGE_")) && (x.thing.defName.Contains("_Gun_") || x.thing.defName.Contains("_Melee_")));
            }
            if (!AMAMod.settings.AllowDarkEldarWeapons)
            {
                __result.RemoveAll(x => (x.thing.defName.Contains("OGDE_")) && (x.thing.defName.Contains("_Gun_") || x.thing.defName.Contains("_Melee_")));
            }
            if (!AMAMod.settings.AllowChaosWeapons)
            {
                __result.RemoveAll(x => (x.thing.defName.Contains("OGC_")) && (x.thing.defName.Contains("_Gun_") || x.thing.defName.Contains("_Melee_")));
            }
            if (!AMAMod.settings.AllowTauWeapons)
            {
                __result.RemoveAll(x => (x.thing.defName.Contains("OGT_") || x.thing.defName.Contains("OGK_")) && (x.thing.defName.Contains("_Gun_") || x.thing.defName.Contains("_Melee_")));
            }
            if (!AMAMod.settings.AllowOrkWeapons)
            {
                __result.RemoveAll(x => (x.thing.defName.Contains("OGO_")) && (x.thing.defName.Contains("_Gun_") || x.thing.defName.Contains("_Melee_")));
            }
            if (!AMAMod.settings.AllowNecronWeapons)
            {
                __result.RemoveAll(x => (x.thing.defName.Contains("OGN_")) && (x.thing.defName.Contains("_Gun_") || x.thing.defName.Contains("_Melee_")));
            }
            if (!AMAMod.settings.AllowTyranidWeapons)
            {
                __result.RemoveAll(x => (x.thing.defName.Contains("OGTY_")) && (x.thing.defName.Contains("_Gun_") || x.thing.defName.Contains("_Melee_")));
            }
            
            /*
            foreach (ThingStuffPair item in __result)
            {
                if (item.thing.defName.Contains("OGI_Gun_") || item.thing.defName.Contains("OGI_Melee_"))
                {
                    if (!AMAMod.settings.AllowImperialWeapons)
                    {
                        continue;
                    }
                }
                if (item.thing.defName.Contains("OGAM_Gun_") || item.thing.defName.Contains("OGAM_Melee_"))
                {
                    if (!AMAMod.settings.AllowMechanicusWeapons)
                    {
                        continue;
                    }
                }
                if (item.thing.defName.Contains("OGE_Gun_") || item.thing.defName.Contains("OGE_Melee_"))
                {
                    if (!AMAMod.settings.AllowEldarWeapons)
                    {
                        continue;
                    }
                }
                if (item.thing.defName.Contains("OGDE_Gun_") || item.thing.defName.Contains("OGDE_Melee_"))
                {
                    if (!AMAMod.settings.AllowDarkEldarWeapons)
                    {
                        continue;
                    }
                }
                if (item.thing.defName.Contains("OGC_Gun_") || item.thing.defName.Contains("OGC_Melee_"))
                {
                    if (!AMAMod.settings.AllowChaosWeapons)
                    {
                        continue;
                    }
                }
                if ((item.thing.defName.Contains("OGT_") || item.thing.defName.Contains("OGK_")) && (item.thing.defName.Contains("_Gun_") || item.thing.defName.Contains("_Melee_")))
                {
                    if (!AMAMod.settings.AllowTauWeapons)
                    {
                        continue;
                    }
                }
                if (item.thing.defName.Contains("OGO_Gun_") || item.thing.defName.Contains("OGO_Melee_"))
                {
                    if (!AMAMod.settings.AllowOrkWeapons)
                    {
                        continue;
                    }
                }
                if (item.thing.defName.Contains("OGN_Gun_") || item.thing.defName.Contains("OGN_Melee_"))
                {
                    if (!AMAMod.settings.AllowNecronWeapons)
                    {
                        continue;
                    }
                }
                if (item.thing.defName.Contains("OGTY_Gun_") || item.thing.defName.Contains("OGTY_Melee_"))
                {
                    if (!AMAMod.settings.AllowTyranidWeapons)
                    {
                        continue;
                    }
                }
                list.Add(item);
            //    log.message("Allowed "+item.thing.LabelCap);
            }
            
            __result = list;
            */

        }
    }
    
}
