using System.Collections.Generic;
using RimWorld;
using HarmonyLib;
using AdeptusMechanicus.settings;
using Verse;
using RimWorld.Planet;

namespace AdeptusMechanicus.HarmonyInstance
{

    [HarmonyPatch(typeof(PawnWeaponGenerator), "Reset")]
    public static class PawnWeaponGenerator_Reset_WeaponRestriction_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref List<ThingStuffPair> ___allWeaponPairs)
        {
            if (!AMAMod.settings.AllowImperialWeapons)
            {
                ___allWeaponPairs.RemoveAll(x => (x.thing.defName.Contains("OGI_")) && (x.thing.defName.Contains("_Gun_") || x.thing.defName.Contains("_Melee_")));
            }
            if (!AMAMod.settings.AllowMechanicusWeapons)
            {
                ___allWeaponPairs.RemoveAll(x => (x.thing.defName.Contains("OGAM_")) && (x.thing.defName.Contains("_Gun_") || x.thing.defName.Contains("_Melee_")));
            }
            if (!AMAMod.settings.AllowEldarWeapons)
            {
                ___allWeaponPairs.RemoveAll(x => (x.thing.defName.Contains("OGE_")) && (x.thing.defName.Contains("_Gun_") || x.thing.defName.Contains("_Melee_")));
            }
            if (!AMAMod.settings.AllowDarkEldarWeapons)
            {
                ___allWeaponPairs.RemoveAll(x => (x.thing.defName.Contains("OGDE_")) && (x.thing.defName.Contains("_Gun_") || x.thing.defName.Contains("_Melee_")));
            }
            if (!AMAMod.settings.AllowChaosWeapons)
            {
                ___allWeaponPairs.RemoveAll(x => (x.thing.defName.Contains("OGC_")) && (x.thing.defName.Contains("_Gun_") || x.thing.defName.Contains("_Melee_")));
            }
            if (!AMAMod.settings.AllowTauWeapons)
            {
                ___allWeaponPairs.RemoveAll(x => (x.thing.defName.Contains("OGT_") || x.thing.defName.Contains("OGK_")) && (x.thing.defName.Contains("_Gun_") || x.thing.defName.Contains("_Melee_")));
            }
            if (!AMAMod.settings.AllowOrkWeapons)
            {
                ___allWeaponPairs.RemoveAll(x => (x.thing.defName.Contains("OGO_")) && (x.thing.defName.Contains("_Gun_") || x.thing.defName.Contains("_Melee_")));
            }
            if (!AMAMod.settings.AllowNecronWeapons)
            {
                ___allWeaponPairs.RemoveAll(x => (x.thing.defName.Contains("OGN_")) && (x.thing.defName.Contains("_Gun_") || x.thing.defName.Contains("_Melee_")));
            }
            if (!AMAMod.settings.AllowTyranidWeapons)
            {
                ___allWeaponPairs.RemoveAll(x => (x.thing.defName.Contains("OGTY_")) && (x.thing.defName.Contains("_Gun_") || x.thing.defName.Contains("_Melee_")));
            }
            if (Find.World is World world)
            {
                if (world.GetComponent<RelicTracker>() is RelicTracker relicTracker)
                {
                    ___allWeaponPairs.RemoveAll(x => x.thing.HasModExtension<RelicExtension>() && !relicTracker.CanSpawn(x.thing));
                }
            }
        }
    }
    
}
