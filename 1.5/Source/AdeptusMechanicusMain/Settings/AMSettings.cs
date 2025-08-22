using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Verse;
using RimWorld;
using System.Text;
using ExtraHives;
using ThingDefOf = RimWorld.ThingDefOf;
using Corruption.Core;
using static UnityEngine.GraphicsBuffer;

namespace AdeptusMechanicus.settings
{
    public class AMSettings : ModSettings
    {
        public AMSettings()
        {
            AMSettings.Instance = this;
        }
        public void ApplySettingsStartUp()
        {
            imperialCategories = new List<ThingCategoryDef>(DefDatabase<ThingCategoryDef>.AllDefs.Where(x => x.defName.StartsWith("OG_") && x.defName.Contains("_Imperial")));
            AstartesCategories = new List<ThingCategoryDef>(DefDatabase<ThingCategoryDef>.AllDefs.Where(x => x.defName.StartsWith("OG_") && x.defName.Contains("_Astartes")));
            AssassinorumCategories = new List<ThingCategoryDef>(DefDatabase<ThingCategoryDef>.AllDefs.Where(x => x.defName.StartsWith("OG_") && x.defName.Contains("_Assassinorum")));
            IquisitorialCategories = new List<ThingCategoryDef>(DefDatabase<ThingCategoryDef>.AllDefs.Where(x => x.defName.StartsWith("OG_") && x.defName.Contains("_Inquisition")));
            mechanicusCategories = new List<ThingCategoryDef>(DefDatabase<ThingCategoryDef>.AllDefs.Where(x => x.defName.StartsWith("OG_") && x.defName.Contains("_Mechanicus")));
            militarumCategories = new List<ThingCategoryDef>(DefDatabase<ThingCategoryDef>.AllDefs.Where(x => x.defName.StartsWith("OG_") && x.defName.Contains("_Militarum")));
            sororitasCategories = new List<ThingCategoryDef>(DefDatabase<ThingCategoryDef>.AllDefs.Where(x => x.defName.StartsWith("OG_") && x.defName.Contains("_Sororitas")));

            AMAMod.updateFactions_Required = true;
            AMAMod.updateIncidents_Disabled = true;
            AMAMod.updateWeapons_Allowed = true;
            AMAMod.updateFactions_PawnKinds = true;
            GenerateRaceSettings();
            ApplySettings();
        }

        public List<ThingCategoryDef> imperialCategories;
        public List<ThingCategoryDef> AstartesCategories;
        public List<ThingCategoryDef> AssassinorumCategories;
        public List<ThingCategoryDef> IquisitorialCategories;
        public List<ThingCategoryDef> mechanicusCategories;
        public List<ThingCategoryDef> militarumCategories;
        public List<ThingCategoryDef> sororitasCategories;

        public void RegenerateRaceSettings()
        {
            raceSettings = new List<RaceSettingHandle>();
            raceSettingsActive = new List<RaceSettingHandle>();
            GenerateRaceSettings();
        }

        public List<FactionSettingHandle> GenerateFactionSettingsFor(ModContentPack modContent = null)
        {
            ModContentPack cont = modContent ?? this.Mod.Content;
            List<FactionSettingHandle> factionSettings = new List<FactionSettingHandle>();
            foreach (var f in DefDatabase<FactionDef>.AllDefsListForReading.Where(x => (x.modContentPack != null && x.modContentPack == modContent) || (x.basicMemberKind != null && x.basicMemberKind.modContentPack != null && x.basicMemberKind.modContentPack == modContent)))
            {
                if (!factionSettings.Any(x => x.factionDefNane == f.defName))
                {
                    factionSettings.Add(new FactionSettingHandle(f, cont));
                }
            }
            return factionSettings;
        }
        
        public void GenerateFactionSettings(ModContentPack modContent = null)
        {
            ModContentPack cont = modContent ?? this.Mod.Content;
            if (cont == null)
            {
                Log.Error($"Couldnt find Mod content pack");
            }
            if (factionSettings == null)
            {
                factionSettings = new List<FactionSettingHandle>();
            }
            List<FactionSettingHandle> newSettings = new List<FactionSettingHandle>();

            foreach (var f in DefDatabase<FactionDef>.AllDefsListForReading.Where(x => !x.isPlayer && ((x.modContentPack != null && x.modContentPack == cont) || (x.basicMemberKind != null && x.basicMemberKind.modContentPack != null && x.basicMemberKind.modContentPack == cont))))
            {
                if (!factionSettings.Any(x => x.factionDefNane == f.defName) && !newSettings.Any(x => x.factionDefNane == f.defName))
                {
                    newSettings.Add(new FactionSettingHandle(f, cont));
                }
            }
            if (!newSettings.NullOrEmpty()) factionSettings.AddRange(newSettings);
        //    Log.Message($"Generated settings for {newSettings.Count} new factions, Total: {factionSettings.Count}");
        }
        
        public void GenerateRaceSettings()
        {
            if (raceSettings == null)
            {
                raceSettings = new List<RaceSettingHandle>();
            }
            foreach (var p in DefDatabase<ThingDef>.AllDefsListForReading.Where(x => x.race != null && x.race.Humanlike))
            {
                if (!raceSettings.Any(x => x.raceDefNane == p.defName))
                {
                    raceSettings.Add(new RaceSettingHandle(p));
                }
            }
        }

        public void ApplySettings()
        {
            if (AMAMod.updateIncidents_Disabled) UpdateScenarioDisabledIncidents();
            if (AMAMod.updateFactions_Required) UpdateFactionsRequiredAtGameStart();
            if (AMAMod.updateWeapons_Allowed) UpdateWeapons_New();
            if (AMAMod.updateFactions_PawnKinds) UpdateFactionPawnKinds();
        }

        List<ThingDef> backupThingDefs;
        List<RecipeDef> backupRecipeDefs;
        public void UpdateFactionPawnKinds()
        {

        }
        /*
        public void UpdateWeapons()
        {
            if (backupThingDefs == null)
            {
                backupThingDefs = new List<ThingDef>(DefDatabase<ThingDef>.AllDefsListForReading);
            }
            if (backupRecipeDefs == null)
            {
                backupRecipeDefs = new List<RecipeDef>(DefDatabase<RecipeDef>.AllDefsListForReading);
            }
            if (!AllowImperialWeapons)
                ProcessWeaponTags(new List<string>() { "OGI_", "OGIG_", "OGAS_", "OGAA_", "OGOA_", "OGAM_", "OGIQ_" }, AllowImperialWeapons);
            else
            {
                ProcessWeaponTag("OGI_", AllowImperialWeapons);
                ProcessWeaponTag("OGOA_", AllowAssassinorumWeapons);
                ProcessWeaponTag("OGAA_", AllowAstartesWeapons);
                ProcessWeaponTag("OGIQ_", AllowInquisitorialWeapons);
                ProcessWeaponTag("OGAM_", AllowMechanicusWeapons);
                ProcessWeaponTag("OGIG_", AllowMilitarumWeapons);
                ProcessWeaponTag("OGAS_", AllowSororitasWeapons);
            }
            ProcessWeaponTag("OGE_", AllowEldarWeapons);
            ProcessWeaponTag("OGDE_", AllowDarkEldarWeapons);
            ProcessWeaponTag("OGC_", AllowChaosWeapons);
            ProcessWeaponTag("OGT_", AllowTauWeapons);
            ProcessWeaponTag("OGK_", AllowKrootWeapons);
            ProcessWeaponTag("OGV_", AllowVespuidWeapons);
            ProcessWeaponTag("OGO_", AllowOrkWeapons);
            ProcessWeaponTag("OGN_", AllowNecronWeapons);
            ProcessWeaponTag("OGTY_", AllowTyranidWeapons);
        }
        */
        public void UpdateWeapons_New()
        {
            if (backupThingDefs == null)
            {
                backupThingDefs = new List<ThingDef>(DefDatabase<ThingDef>.AllDefsListForReading);
            }
            if (backupRecipeDefs == null)
            {
                backupRecipeDefs = new List<RecipeDef>(DefDatabase<RecipeDef>.AllDefsListForReading);
            }
            List<ThingCategoryDef> whitelist = new List<ThingCategoryDef>();
            List<ThingCategoryDef> blacklist = new List<ThingCategoryDef>();

            if (AMAMod.Dev) Log.Message("pre _Imperial");
            if (AllowImperialWeapons) whitelist.AddRange(DefDatabase<ThingCategoryDef>.AllDefs.Where(x=> x.defName.StartsWith("OG_Weapons_") && x.defName.Contains("_Imperial")));
            else blacklist.AddRange(DefDatabase<ThingCategoryDef>.AllDefs.Where(x => x.defName.StartsWith("OG_Weapons_") && x.defName.Contains("_Imperial")));

            if (AMAMod.Dev) Log.Message("pre _Assassinorum");
            if (AllowAssassinorumWeapons) whitelist.AddRange(DefDatabase<ThingCategoryDef>.AllDefs.Where(x => x.defName.StartsWith("OG_Weapons_") && x.defName.Contains("_Assassinorum")));
            else blacklist.AddRange(DefDatabase<ThingCategoryDef>.AllDefs.Where(x => x.defName.StartsWith("OG_Weapons_") && x.defName.Contains("_Assassinorum")));

            if (AMAMod.Dev) Log.Message("pre _Astartes");
            if (AllowAstartesWeapons) whitelist.AddRange(DefDatabase<ThingCategoryDef>.AllDefs.Where(x => x.defName.StartsWith("OG_Weapons_") && x.defName.Contains("_Astartes")));
            else blacklist.AddRange(DefDatabase<ThingCategoryDef>.AllDefs.Where(x => x.defName.StartsWith("OG_Weapons_") && x.defName.Contains("_Astartes")));

            if (AMAMod.Dev) Log.Message("pre _Inquisition");
            if (AllowInquisitorialWeapons) whitelist.AddRange(DefDatabase<ThingCategoryDef>.AllDefs.Where(x => x.defName.StartsWith("OG_Weapons_") && x.defName.Contains("_Inquisition")));
            else blacklist.AddRange(DefDatabase<ThingCategoryDef>.AllDefs.Where(x => x.defName.StartsWith("OG_Weapons_") && x.defName.Contains("_Inquisition")));

            if (AMAMod.Dev) Log.Message("pre _Mechanicus");
            if (AllowMechanicusWeapons) whitelist.AddRange(DefDatabase<ThingCategoryDef>.AllDefs.Where(x => x.defName.StartsWith("OG_Weapons_") && x.defName.Contains("_Mechanicus")));
            else blacklist.AddRange(DefDatabase<ThingCategoryDef>.AllDefs.Where(x => x.defName.StartsWith("OG_Weapons_") && x.defName.Contains("_Mechanicus")));

            if (AMAMod.Dev) Log.Message("pre _Militarum");
            if (AllowMilitarumWeapons) whitelist.AddRange(DefDatabase<ThingCategoryDef>.AllDefs.Where(x => x.defName.StartsWith("OG_Weapons_") && x.defName.Contains("_Militarum")));
            else blacklist.AddRange(DefDatabase<ThingCategoryDef>.AllDefs.Where(x => x.defName.StartsWith("OG_Weapons_") && x.defName.Contains("_Militarum")));

            if (AMAMod.Dev) Log.Message("pre _Sororitas");
            if (AllowSororitasWeapons) whitelist.AddRange(DefDatabase<ThingCategoryDef>.AllDefs.Where(x => x.defName.StartsWith("OG_Weapons_") && x.defName.Contains("_Sororitas")));
            else blacklist.AddRange(DefDatabase<ThingCategoryDef>.AllDefs.Where(x => x.defName.StartsWith("OG_Weapons_") && x.defName.Contains("_Sororitas")));

            if (AMAMod.Dev) Log.Message("pre _Asuryani");
            if (AllowEldarWeapons) whitelist.AddRange(DefDatabase<ThingCategoryDef>.AllDefs.Where(x => x.defName.StartsWith("OG_Weapons_") && x.defName.Contains("_Asuryani")));
            else blacklist.AddRange(DefDatabase<ThingCategoryDef>.AllDefs.Where(x => x.defName.StartsWith("OG_Weapons_") && x.defName.Contains("_Asuryani")));

            if (AMAMod.Dev) Log.Message("pre _Drukhari");
            if (AllowDarkEldarWeapons) whitelist.AddRange(DefDatabase<ThingCategoryDef>.AllDefs.Where(x => x.defName.StartsWith("OG_Weapons_") && x.defName.Contains("_Drukhari")));
            else blacklist.AddRange(DefDatabase<ThingCategoryDef>.AllDefs.Where(x => x.defName.StartsWith("OG_Weapons_") && x.defName.Contains("_Drukhari")));

            if (AMAMod.Dev) Log.Message("pre _Chaos");
            if (AllowChaosWeapons) whitelist.AddRange(DefDatabase<ThingCategoryDef>.AllDefs.Where(x => x.defName.StartsWith("OG_Weapons_") && x.defName.Contains("_Chaos")));
            else blacklist.AddRange(DefDatabase<ThingCategoryDef>.AllDefs.Where(x => x.defName.StartsWith("OG_Weapons_") && x.defName.Contains("_Chaos")));

            if (AMAMod.Dev) Log.Message("pre _Tau");
            if (AllowTauWeapons) whitelist.AddRange(DefDatabase<ThingCategoryDef>.AllDefs.Where(x => x.defName.StartsWith("OG_Weapons_") && x.defName.Contains("_Tau")));
            else blacklist.AddRange(DefDatabase<ThingCategoryDef>.AllDefs.Where(x => x.defName.StartsWith("OG_Weapons_") && x.defName.Contains("_Tau")));

            if (AMAMod.Dev) Log.Message("pre _Kroot");
            if (AllowKrootWeapons) whitelist.AddRange(DefDatabase<ThingCategoryDef>.AllDefs.Where(x => x.defName.StartsWith("OG_Weapons_") && x.defName.Contains("_Kroot")));
            else blacklist.AddRange(DefDatabase<ThingCategoryDef>.AllDefs.Where(x => x.defName.StartsWith("OG_Weapons_") && x.defName.Contains("_Kroot")));

            if (AMAMod.Dev) Log.Message("pre _Vespid");
            if (AllowVespuidWeapons) whitelist.AddRange(DefDatabase<ThingCategoryDef>.AllDefs.Where(x => x.defName.StartsWith("OG_Weapons_") && x.defName.Contains("_Vespid")));
            else blacklist.AddRange(DefDatabase<ThingCategoryDef>.AllDefs.Where(x => x.defName.StartsWith("OG_Weapons_") && x.defName.Contains("_Vespid")));

            if (AMAMod.Dev) Log.Message("pre _Ork");
            if (AllowOrkWeapons) whitelist.AddRange(DefDatabase<ThingCategoryDef>.AllDefs.Where(x => x.defName.StartsWith("OG_Weapons_") && x.defName.Contains("_Ork")));
            else blacklist.AddRange(DefDatabase<ThingCategoryDef>.AllDefs.Where(x => x.defName.StartsWith("OG_Weapons_") && x.defName.Contains("_Ork")));

            if (AMAMod.Dev) Log.Message("pre _Necron");
            if (AllowNecronWeapons) whitelist.AddRange(DefDatabase<ThingCategoryDef>.AllDefs.Where(x => x.defName.StartsWith("OG_") && x.defName.Contains("_Necron")));
            else blacklist.AddRange(DefDatabase<ThingCategoryDef>.AllDefs.Where(x => x.defName.StartsWith("OG_Weapons_") && x.defName.Contains("_Necron")));

            if (AMAMod.Dev) Log.Message("pre _Tyranid");
            if (AllowTyranidWeapons) whitelist.AddRange(DefDatabase<ThingCategoryDef>.AllDefs.Where(x => x.defName.StartsWith("OG_") && x.defName.Contains("_Tyranid")));
            else blacklist.AddRange(DefDatabase<ThingCategoryDef>.AllDefs.Where(x => x.defName.StartsWith("OG_Weapons_") && x.defName.Contains("_Tyranid")));
            ProcessWeaponTags(whitelist, blacklist);
        }
        /*
        public void ProcessWeaponTags(List<string> tags, bool allow)
        {
            foreach (var tag in tags)
            {
                ProcessWeaponTag(tag, allow);
            }
        }
        */
        public void ProcessWeaponTags(List<ThingCategoryDef> whiteTags, List<ThingCategoryDef> blackTags)
        {
            if (AMAMod.Dev) Log.Message($"Processing Tags");
            if (AMAMod.Dev) Log.Message($"Black:{blackTags.Count()}");
            List<ThingDef> blackTaggedThingDefs = DefDatabase<ThingDef>.AllDefsListForReading.Where(x => !x.thingCategories.NullOrEmpty() && x.thingCategories.Any(y => blackTags.NotNullAndContains(y)) && !x.thingCategories.Any(y => whiteTags.NotNullAndContains(y) && !(y.defName.StartsWith("OG_") && y.defName.Contains("_Imperial")))).ToList();
            if (AMAMod.Dev) Log.Message($"White:{whiteTags.Count()}");
            List<ThingDef> whiteTaggedThingDefs = backupThingDefs.Where(x =>!x.thingCategories.NullOrEmpty() && x.thingCategories.Any(y => whiteTags.NotNullAndContains(y)) && !DefDatabase<ThingDef>.AllDefsListForReading.Contains(x) && !blackTaggedThingDefs.Contains(x)).ToList();
             
            if (!whiteTaggedThingDefs.EnumerableNullOrEmpty())
            {
                if (AMAMod.Dev) Log.Message($"{whiteTaggedThingDefs.Count()} ThingDefs tagged for addition");
                DefDatabase<ThingDef>.defsList.AddRange(whiteTaggedThingDefs);
                DefDatabase<ThingDef>.Add(whiteTaggedThingDefs);
                IEnumerable<RecipeDef> whiteTaggedRecipeDefs = backupRecipeDefs.Where(x => whiteTaggedThingDefs.Contains(x.ProducedThingDef));
                if (!whiteTaggedRecipeDefs.EnumerableNullOrEmpty())
                {
                    if (AMAMod.Dev) Log.Message($"readded {whiteTaggedRecipeDefs.Count()} RecipeDefs tagged {whiteTags.Select(x => x.defName).ToCommaList()}\nDefsRemoved: {whiteTaggedRecipeDefs.Select(x => x.defName).ToCommaList()}");

                    DefDatabase<RecipeDef>.Add(whiteTaggedRecipeDefs);
                }
            }
            if (AMAMod.Dev) Log.Message($"White:{whiteTags.Count()} processed");
            if (!blackTaggedThingDefs.EnumerableNullOrEmpty())
            {
                if (AMAMod.Dev) Log.Message($"{blackTaggedThingDefs.Count()} ThingDefs tagged for removal {blackTags.Select(x => x.defName).ToCommaList()}\nDefsRemoved: {blackTaggedThingDefs.Select(x => x.defName).ToCommaList()}");
                List<RecipeDef> blackTaggedRecipeDefs = DefDatabase<RecipeDef>.AllDefsListForReading.Where(x => blackTaggedThingDefs.Contains(x.ProducedThingDef)).ToList();

                for (int i = 0; i < blackTaggedThingDefs.Count(); i++)
                {
                    DefDatabase<ThingDef>.Remove(blackTaggedThingDefs[i]);
                }
                DefDatabase<ThingDef>.defsList.RemoveAll(x => blackTaggedThingDefs.Contains(x));
                DefDatabase<ThingDef>.defsByName.RemoveAll(x => blackTaggedThingDefs.Contains(x.Value) || blackTaggedThingDefs.Any(y => y.defName == x.Key));
                DefDatabase<ThingDef>.SetIndices();
                /*
                */
                if (!blackTaggedRecipeDefs.EnumerableNullOrEmpty())
                {
                    if (AMAMod.Dev) Log.Message($"removed {blackTaggedRecipeDefs.Count()} RecipeDefs tagged {blackTags.Select(x => x.defName).ToCommaList()}\nDefsRemoved: {blackTaggedRecipeDefs.Select(x => x.defName).ToCommaList()}");

                    for (int i = blackTaggedThingDefs.Count-1; i < 0; i--)
                    {
                        DefDatabase<RecipeDef>.Remove(blackTaggedRecipeDefs[i]);
                    }
                    /*
                    DefDatabase<RecipeDef>.defsList.RemoveAll(x => blackTaggedRecipeDefs.Contains(x));
                    DefDatabase<RecipeDef>.defsByName.RemoveAll(x => blackTaggedRecipeDefs.Contains(x.Value));
                    DefDatabase<RecipeDef>.SetIndices();
                    */
                }
            }
            if (AMAMod.Dev) Log.Message($"Black:{blackTags.Count()} processed");

        }
        /*
        public void ProcessWeaponTag(string tag, bool allow)
        {
            if (!allow)
            {
                DefDatabase<ThingDef>.defsList.RemoveAll(x => (x.defName.Contains(tag)) && (x.IsWeapon));
                DefDatabase<ThingDef>.defsByName.RemoveAll(x => (x.Value.defName.Contains(tag)) && (x.Value.defName.Contains("_Gun_") || x.Value.defName.Contains("_Melee_")));
                DefDatabase<ThingDef>.defsByShortHash.RemoveAll(x => (x.Value.defName.Contains(tag)) && (x.Value.defName.Contains("_Gun_") || x.Value.defName.Contains("_Melee_")));
                DefDatabase<RecipeDef>.defsList.RemoveAll(x => (x.defName.Contains(tag)) && (x.ProducedThingDef.IsWeapon));
                DefDatabase<RecipeDef>.defsByName.RemoveAll(x => (x.Value.defName.Contains(tag)) && (x.Value.defName.Contains("_Gun_") || x.Value.defName.Contains("_Melee_")));
                DefDatabase<RecipeDef>.defsByShortHash.RemoveAll(x => (x.Value.defName.Contains(tag)) && (x.Value.defName.Contains("_Gun_") || x.Value.defName.Contains("_Melee_")));
            }
            else
            {
                if (AMAMod.Dev)
                {
                    if (backupThingDefs.EnumerableNullOrEmpty())
                        Log.Warning($"couldnt find any backupThingDefs");
                    if (backupRecipeDefs.EnumerableNullOrEmpty())
                        Log.Warning($"couldnt find any backupRecipeDefs");
                }
                IEnumerable<ThingDef> taggedThingDefs = backupThingDefs.Where(x =>  x.defName.Contains(tag) && (x.IsWeapon));
                if (taggedThingDefs.EnumerableNullOrEmpty() && AMAMod.Dev)
                    Log.Warning($"couldnt find any taggedThingDefs for {tag}");
                else Log.Message($"{taggedThingDefs.Count()} taggedThingDefs for {tag}");
                IEnumerable<ThingDef> thingDefs = taggedThingDefs.Where(x => !DefDatabase<ThingDef>.AllDefsListForReading.Contains(x));
                IEnumerable<RecipeDef> taggedRecipeDefs = backupRecipeDefs.Where(x => x.defName.Contains(tag) && (x.ProducedThingDef.IsWeapon));
                if (taggedRecipeDefs.EnumerableNullOrEmpty() && AMAMod.Dev)
                    Log.Warning($"couldnt find any taggedRecipeDefs for {tag}");
                else Log.Message($"{taggedRecipeDefs.Count()} taggedRecipeDefs for {tag}");
                IEnumerable<RecipeDef> recipeDefs = taggedRecipeDefs.Where(x => !DefDatabase<RecipeDef>.AllDefsListForReading.Contains(x));
                if (thingDefs.EnumerableNullOrEmpty() && AMAMod.Dev)
                    Log.Warning($"No thingDefs for {tag} to add");
                else
                {
                    Log.Message($"{thingDefs.Count()} thingDefs for {tag} to add");
                    DefDatabase<ThingDef>.Add(thingDefs);
                    DefDatabase<ThingDef>.InitializeShortHashDictionary();
                }
                if (recipeDefs.EnumerableNullOrEmpty() && AMAMod.Dev)
                    Log.Warning($"No recipeDefs for {tag} to add");
                else
                {
                    Log.Message($"{recipeDefs.Count()} recipeDefs for {tag} to add");
                    DefDatabase<RecipeDef>.Add(recipeDefs);
                }
            }
        }
        */
       /* 
        public void ProcessWeaponTag(ThingCategoryDef tag, bool allow)
        {
            if (!allow)
            {
                IEnumerable<ThingDef> taggedThingDefs = DefDatabase<ThingDef>.AllDefsListForReading.Where(x => x.thingCategories.NotNullAndContains(tag) && x.IsWeapon);
                if (!taggedThingDefs.EnumerableNullOrEmpty())
                {
                    IEnumerable<RecipeDef> taggedRecipeDefs = DefDatabase<RecipeDef>.AllDefsListForReading.Where(x => taggedThingDefs.Contains(x.ProducedThingDef));
                    DefDatabase<ThingDef>.defsList.RemoveAll(x=> taggedThingDefs.Contains(x));
                    DefDatabase<ThingDef>.defsByName.RemoveAll(x => taggedThingDefs.Contains(x.Value));
                    DefDatabase<ThingDef>.defsByShortHash.RemoveAll(x => taggedThingDefs.Contains(x.Value));

                    if (!taggedRecipeDefs.EnumerableNullOrEmpty())
                    {
                        if (AMAMod.Dev) Log.Message($"{taggedRecipeDefs.Count()} taggedRecipeDefs for {tag} to remove");
                        DefDatabase<RecipeDef>.defsList.RemoveAll(x => taggedRecipeDefs.Contains(x));
                        DefDatabase<RecipeDef>.defsByName.RemoveAll(x => taggedRecipeDefs.Contains(x.Value));
                        DefDatabase<RecipeDef>.defsByShortHash.RemoveAll(x => taggedRecipeDefs.Contains(x.Value));
                    }
                    else if (AMAMod.Dev)
                    {
                        Log.Warning($"couldnt find any taggedRecipeDefs for {tag} to remove");
                    }
                }
                else if (AMAMod.Dev)
                {
                    Log.Warning($"couldnt find any taggedThingDefs for {tag} to remove");
                }
            }
            else
            {
                if (AMAMod.Dev)
                {
                    if (backupThingDefs.EnumerableNullOrEmpty())
                        Log.Warning($"couldnt find any backupThingDefs");
                    if (backupRecipeDefs.EnumerableNullOrEmpty())
                        Log.Warning($"couldnt find any backupRecipeDefs");
                }
                IEnumerable<ThingDef> taggedThingDefs = backupThingDefs.Where(x =>  x.thingCategories.NotNullAndContains(tag) && (x.IsWeapon));
               
                if (taggedThingDefs.EnumerableNullOrEmpty())
                {
                    if (AMAMod.Dev) Log.Warning($"couldnt find any taggedThingDefs for {tag}");
                }
                else
                {
                    if (AMAMod.Dev) Log.Message($"{taggedThingDefs.Count()} taggedThingDefs for {tag}");
                    IEnumerable<ThingDef> thingDefs = taggedThingDefs.Where(x => !DefDatabase<ThingDef>.AllDefsListForReading.Contains(x));
                    if (thingDefs.EnumerableNullOrEmpty())
                    {
                        if (AMAMod.Dev) Log.Warning($"couldnt find any thingDefs for {tag} that need to be added");
                    }
                    else
                    {
                        if (AMAMod.Dev) Log.Message($"{taggedThingDefs.Count()} thingDefs for {tag} that need to be added");

                        IEnumerable<RecipeDef> taggedRecipeDefs = backupRecipeDefs.Where(x => x.ProducedThingDef.IsWeapon && x.ProducedThingDef.thingCategories.NotNullAndContains(tag));
                        if (taggedRecipeDefs.EnumerableNullOrEmpty())
                        {
                            if (AMAMod.Dev) Log.Warning($"couldnt find any taggedRecipeDefs for {tag}");
                        }
                        else
                        {
                            if (AMAMod.Dev) Log.Message($"{taggedRecipeDefs.Count()} taggedRecipeDefs for {tag}");
                            IEnumerable<RecipeDef> recipeDefs = taggedRecipeDefs.Where(x => !DefDatabase<RecipeDef>.AllDefsListForReading.NotNullAndContains(x));

                            if (thingDefs.EnumerableNullOrEmpty())
                            {
                                if (AMAMod.Dev) Log.Warning($"No thingDefs for {tag} to add");
                            }
                            else
                            {
                                if (AMAMod.Dev) Log.Message($"{thingDefs.Count()} thingDefs for {tag} to add");
                                DefDatabase<ThingDef>.Add(thingDefs);
                                DefDatabase<ThingDef>.InitializeShortHashDictionary();
                            }
                            if (recipeDefs.EnumerableNullOrEmpty())
                            {
                                if (AMAMod.Dev) Log.Warning($"No recipeDefs for {tag} to add");
                            }
                            else
                            {
                                if (AMAMod.Dev) Log.Message($"{recipeDefs.Count()} recipeDefs for {tag} to add");
                                DefDatabase<RecipeDef>.Add(recipeDefs);
                            }
                        }


                    }
                }

            }
        }
        */
        /*
        public void ProcessWeaponList(List<ThingDef> list, bool allow)
        {
            if (!allow)
            {
                DefDatabase<ThingDef>.defsList.RemoveAll(x => list.Contains(x));
                DefDatabase<ThingDef>.defsByName.RemoveAll(x => list.Contains(x.Value));
                DefDatabase<ThingDef>.defsByShortHash.RemoveAll(x => list.Contains(x.Value));
                DefDatabase<RecipeDef>.defsList.RemoveAll(x => list.Contains(x.ProducedThingDef));
                DefDatabase<RecipeDef>.defsByName.RemoveAll(x => list.Contains(x.Value.ProducedThingDef));
                DefDatabase<RecipeDef>.defsByShortHash.RemoveAll(x => list.Contains(x.Value.ProducedThingDef));
            }
            else
            {
                DefDatabase<ThingDef>.Add(backupThingDefs.Where(x => !DefDatabase<ThingDef>.AllDefsListForReading.Contains(x) && list.Contains(x)));
                DefDatabase<RecipeDef>.Add(backupRecipeDefs.Where(x => !DefDatabase<RecipeDef>.AllDefsListForReading.Contains(x) && list.Contains(x.ProducedThingDef)));
            }
        }
        */
        public void UpdateScenarioDisabledIncidents()
        {
            StringBuilder builder = new StringBuilder("Adeptus Mechanicus:: Updating Incident Settings");
            if (Find.Scenario?.parts is List<ScenPart> parts)
            {
                UpdateScenarioParts(parts);
                builder.AppendLine("Current Scenario Updated");
            }
            foreach (var item in DefDatabase<ScenarioDef>.AllDefsListForReading)
            {
                if (item.scenario.parts is List<ScenPart> parts2)
                {
                    UpdateScenarioParts(parts2);
                    builder.AppendLine($"Updated {item.LabelCap}");
                }
            }
            if (AMAMod.Dev) Log.Message(builder.ToString());
        }

        private void UpdateScenarioParts(List<ScenPart> parts)
        {

            // Monolith Appears
            if (AdeptusIncidentDefOf.OGN_MonolithAppears != null)
            {
                if (!AllowNecronMonolith && !parts.Any(x => (x is ScenPart_DisableIncident disableIncident && disableIncident.incident == AdeptusIncidentDefOf.OGN_MonolithAppears)))
                    parts.Add(new ScenPart_DisableIncident() { def = AdeptusScenPartDefOf.DisableIncident, incident = AdeptusIncidentDefOf.OGN_MonolithAppears });
                else if (parts.Find(x => (x is ScenPart_DisableIncident disableIncident && disableIncident.incident == AdeptusIncidentDefOf.OGN_MonolithAppears)) is ScenPart part)
                    parts.Remove(part);
            }
            // Deamonic Infestations
            if (AdeptusIncidentDefOf.OG_Chaos_Deamon_Daemonic_Infestation != null)
            {
                if (!AllowChaosDeamonicInfestation && !parts.Any(x => (x is ScenPart_DisableIncident disableIncident && disableIncident.incident == AdeptusIncidentDefOf.OG_Chaos_Deamon_Daemonic_Infestation)))
                    parts.Add(new ScenPart_DisableIncident() { def = AdeptusScenPartDefOf.DisableIncident, incident = AdeptusIncidentDefOf.OG_Chaos_Deamon_Daemonic_Infestation });
                else if (parts.Find(x => (x is ScenPart_DisableIncident disableIncident && disableIncident.incident == AdeptusIncidentDefOf.OG_Chaos_Deamon_Daemonic_Infestation)) is ScenPart part)
                    parts.Remove(part);
            }
            // Deamonic Incursions
            if (AdeptusIncidentDefOf.OG_Chaos_Deamon_Deamonic_Incursion != null)
            {
                if (!AllowChaosDeamonicIncursion && !parts.Any(x => (x is ScenPart_DisableIncident disableIncident && disableIncident.incident == AdeptusIncidentDefOf.OG_Chaos_Deamon_Deamonic_Incursion)))
                    parts.Add(new ScenPart_DisableIncident() { def = AdeptusScenPartDefOf.DisableIncident, incident = AdeptusIncidentDefOf.OG_Chaos_Deamon_Deamonic_Incursion });
                else if (parts.Find(x => (x is ScenPart_DisableIncident disableIncident && disableIncident.incident == AdeptusIncidentDefOf.OG_Chaos_Deamon_Deamonic_Incursion)) is ScenPart part)
                    parts.Remove(part);
            }
            // Tyranid Infestations
            if (AdeptusIncidentDefOf.OG_Tyranid_Infestation != null)
            {
                if (!AllowTyranidInfestation && !parts.Any(x => (x is ScenPart_DisableIncident disableIncident && disableIncident.incident == AdeptusIncidentDefOf.OG_Tyranid_Infestation)))
                    parts.Add(new ScenPart_DisableIncident() { def = AdeptusScenPartDefOf.DisableIncident, incident = AdeptusIncidentDefOf.OG_Tyranid_Infestation });
                else if (parts.Find(x => (x is ScenPart_DisableIncident disableIncident && disableIncident.incident == AdeptusIncidentDefOf.OG_Tyranid_Infestation)) is ScenPart part)
                    parts.Remove(part);
            }
        }

        public void UpdateFactionsRequiredAtGameStart(bool allowed, List<FactionDef> defs)
        {
            foreach (var factionDef in defs)
            {
                factionDef.startingCountAtWorldCreation = allowed ? 1 : 0;
            }
        }

        public void UpdateFactionsRequiredAtGameStart()
        {
            AMAMod.updateFactions_Required = false;
            if (AMAMod.Dev) Log.Message("Adeptus Mechanicus:: Updating Faction Settings");
            foreach (var factionDef in DefDatabase<FactionDef>.AllDefsListForReading.Where(x=> !x.isPlayer && x.defName.StartsWith("OG_")))
            {
                // Update Imperial factions
                if (factionDef.defName.Contains("OG_Astartes_"))
                {
                    continue;
                }
                if (factionDef.defName.Contains("OG_Mechanicus_"))
                {
                    factionDef.startingCountAtWorldCreation = AllowAdeptusMechanicus ? 1 : 0;
                    continue;
                }
                if (factionDef.defName.Contains("OG_Militarum_"))
                {
                    factionDef.startingCountAtWorldCreation = AllowAdeptusMilitarum ? 1 : 0;
                    continue;
                }
                if (factionDef.defName.Contains("OG_Sororitas_"))
                {
                    factionDef.startingCountAtWorldCreation = AllowAdeptusSororitas ? 1 : 0;
                    continue;
                }
                // Update Chaos factions
                if (factionDef.defName.Contains("OG_Chaos_"))
                {
                    if (factionDef.defName.Contains("Deamon"))
                    {
                        factionDef.startingCountAtWorldCreation = AllowChaosDeamons ? 1 : 0;
                        continue;
                    }
                    if (factionDef.defName.Contains("Marine"))
                    {
                        factionDef.startingCountAtWorldCreation = AllowChaosMarine ? 1 : 0;
                        continue;
                    }
                    if (factionDef.defName.Contains("Guard"))
                    {
                        factionDef.startingCountAtWorldCreation = AllowChaosGuard ? 1 : 0;
                        continue;
                    }
                    if (factionDef.defName.Contains("Mechanicus"))
                    {
                        factionDef.startingCountAtWorldCreation = AllowChaosMechanicus ? 1 : 0;
                        continue;
                    }
                }
                // Update Eldar factions
                if (factionDef.defName.Contains("OG_Eldar_") || factionDef.defName.Contains("OG_DarkEldar_"))
                {
                    if (factionDef.defName.Contains("Craftworld"))
                    {
                        factionDef.startingCountAtWorldCreation = AllowEldarCraftworld ? 1 : 0;
                        continue;
                    }
                    if (factionDef.defName.Contains("Exodite"))
                    {
                        factionDef.startingCountAtWorldCreation = AllowEldarExodite ? 1 : 0;
                        continue;
                    }
                    if (factionDef.defName.Contains("Harlequin"))
                    {
                        factionDef.startingCountAtWorldCreation = AllowEldarHarlequinn ? 1 : 0;
                        continue;
                    }
                    factionDef.startingCountAtWorldCreation = AllowDarkEldar ? 1 : 0;
                    continue;
                }
                // update Tau factions
                if (factionDef.defName.Contains("OG_Kroot_"))
                {
                    factionDef.startingCountAtWorldCreation = AllowKroot ? 1 : 0;
                    continue;
                }
                if (factionDef.defName.Contains("OG_Tau_"))
                {
                    factionDef.startingCountAtWorldCreation = AllowTau ? 1 : 0;
                    continue;
                }
                if (factionDef.defName.Contains("OG_Vespid_") || factionDef.defName.Contains("OG_Vespid_Feral_"))
                {
                    factionDef.startingCountAtWorldCreation = AllowVespid ? 1 : 0;
                    continue;
                }

                // update ork factions
                if (factionDef.defName.Contains("OG_Ork_"))
                {
                    if (factionDef.defName.Contains("Feral"))
                    {
                        factionDef.startingCountAtWorldCreation = AllowOrkFeral ? 1 : 0;
                        continue;
                    }
                    if (factionDef.defName.Contains("Hulk") || factionDef.defName.Contains("Rok"))
                    {
                        factionDef.startingCountAtWorldCreation = AllowOrkRok ? 1 : 0;
                        continue;
                    }
                    factionDef.startingCountAtWorldCreation = AllowOrkTek ? 1 : 0;
                    continue;
                }

                // update necron factions
                if (factionDef.defName.Contains("OG_Necron_"))
                {
                    factionDef.startingCountAtWorldCreation = AllowNecron ? 1 : 0;
                    continue;
                }
                // update tyranid factions
                if (factionDef.defName.Contains("OG_Tyranid_") || factionDef.defName.Contains("OG_Genestealer_Cult"))
                {
                    factionDef.startingCountAtWorldCreation = AllowTyranid ? 1 : 0;
                    continue;
                }
            }
        }

        #region Settings_Vars
        public static AMSettings Instance;
        // Armoury Settings;
        public bool ShowArmourySettings = true;

        public bool ArmouryGeneralSpecialRules = false;
        public bool AllowDeepStrike = true;
        public bool AllowInfiltrate = true;

        public bool ShowAllowedWeaponSpecialRules = false;
        public bool AllowRapidFire = true;
        public bool AllowGetsHot = true;
        public bool AllowJams = true;
        public bool AllowMultiShot = true;
        public bool AllowUserEffects = true;
        public bool AllowForceWeaponEffect = true;
        public bool AllowRendingMeleeEffect = true;
        public bool AllowRendingRangedEffect = true;

        public bool ShowAllowedWeapons = false;
        public bool AllowImperialWeapons = true;
        public bool allowAstartesWeapons = true;
        public bool AllowAstartesWeapons => AllowImperialWeapons && allowAstartesWeapons ;
        public bool allowAssassinorumWeapons = true;
        public bool AllowAssassinorumWeapons => AllowImperialWeapons && allowAssassinorumWeapons;
        public bool allowMilitarumWeapons = true;
        public bool AllowMilitarumWeapons => AllowImperialWeapons && allowMilitarumWeapons;
        public bool allowSororitasWeapons = true;
        public bool AllowSororitasWeapons => AllowImperialWeapons && allowSororitasWeapons;
        public bool allowMechanicusWeapons = true;
        public bool AllowMechanicusWeapons => AllowImperialWeapons && allowMechanicusWeapons;
        public bool allowInquisitorialWeapons = true;
        public bool AllowInquisitorialWeapons => AllowImperialWeapons && allowInquisitorialWeapons;
        public bool AllowChaosWeapons = true;
        public bool AllowEldarWeapons = true;
        public bool AllowDarkEldarWeapons = true;
        public bool AllowTauWeapons = true;
        public bool AllowKrootWeapons = true;
        public bool AllowVespuidWeapons = true;
        public bool AllowOrkWeapons = true;
        public bool AllowNecronWeapons = true;
        public bool AllowTyranidWeapons = false;

        // Armoury Misc Options
        public bool ShowMiscOptions = false;
        public bool RacialResearchRestriction = true;
        public bool RacialConstructionRestriction = true;
        public bool RacialProductionRestriction = true;

        // Armoury Performance Options
        public bool ShowPerformanceOptions = false;
        public bool AllowProjectileTrail = true;
        public bool AllowProjectileGlow = true;
        public bool AllowMuzzlePosition = true;
        public bool AllowPauldronDrawer = true;
        public bool AllowExtraPartDrawer = true;
        public bool AllowHediffPartDrawer = true;
        public bool AllowDynmanicPartDrawer = true; 

        // Xenobiologis Settings

        public bool ShowXenobiologisSettings = false;
        public bool ShowAllowedRaceSettings = false;
        public bool ForceRelations = true;

        // Imperium Settings

        public bool ShowImperium = false;
        public bool ShowAstartes = false;
        public bool ShowMechanicus = false;
        public bool ShowMilitarum = false;
        public bool ShowSororitas = false;
        public bool ShowInquisition = false;
        public bool allowAdeptusAstartes = false;
        public bool AllowAdeptusAstartes => AllowAstartesWeapons && allowAdeptusAstartes;
        public float AstartePunchingFactor = 1f, AstarteSplitFactor = 1f, AstarteScale = 1f;
        public bool AstarteUseOrgans, AstarteEasyMode, AstartesMaleOnly, AstartesAgeMatters, AstartesHumansOnly;
        public bool allowAdeptusMechanicus = true;
        public bool AllowAdeptusMechanicus => AllowMechanicusWeapons && allowAdeptusMechanicus;
        public bool allowAdeptusMilitarum = true;
        public bool AllowAdeptusMilitarum => AllowMilitarumWeapons && allowAdeptusMilitarum;
        public bool allowAdeptusSororitas = false;
        public bool AllowAdeptusSororitas => AllowSororitasWeapons && allowAdeptusSororitas;
        public bool allowOfficoAssassinorum = false;
        public bool AllowOfficoAssassinorum => AllowSororitasWeapons && allowOfficoAssassinorum;

        // Chaos Settings

        public bool ShowChaos = false;
        public bool allowChaosMarine = false;
        public bool AllowChaosMarine => allowChaosMarine && AllowChaosWeapons;
        public bool allowChaosGuard = false;
        public bool AllowChaosGuard => allowChaosGuard && AllowChaosWeapons;
        public bool allowChaosMechanicus = false;
        public bool AllowChaosMechanicus => allowChaosMechanicus && AllowChaosWeapons;
        public bool allowWarpstorm = true;
        public bool AllowWarpstorm => allowWarpstorm && AllowChaosWeapons;
        public bool allowChaosDeamons = true;
        public bool AllowChaosDeamons => allowChaosDeamons && AllowChaosWeapons;
        public bool allowChaosDeamonicIncursion = true;
        public bool AllowChaosDeamonicIncursion => allowChaosDeamonicIncursion && AllowChaosWeapons;
        public bool allowChaosDeamonicInfestation = true;
        public bool AllowChaosDeamonicInfestation => allowChaosDeamonicInfestation && AllowChaosWeapons;

        // End times intergration

        public bool EndTimesIntergrateDeamons = true;
        public bool EndTimesIntergrateDeamonsGreat = true;
        public bool EndTimesIntergrateDeamonsSmall = true;

        // Playable Chaos Settings

        // Aeldari Settings

        public bool ShowAeldari = false;
        public bool allowEldarCraftworld = true;
        public bool AllowEldarCraftworld => allowEldarCraftworld && AllowEldarWeapons;
        public bool allowEldarExodite = false;
        public bool AllowEldarExodite => allowEldarExodite && AllowEldarWeapons;
        public bool allowEldarHarlequinn = false;
        public bool AllowEldarHarlequinn => allowEldarHarlequinn && AllowEldarWeapons;
        public bool allowEldarWraithguard = true;
        public bool AllowEldarWraithguard => allowEldarWraithguard && AllowEldarWeapons;
        public bool allowDarkEldar = true;
        public bool AllowDarkEldar => allowDarkEldar && AllowDarkEldarWeapons;

        // Playable Aeldari Settings



        // Tau Settings

        public bool ShowTau = false;
        public bool allowTau = true;
        public bool AllowTau => AllowTauWeapons && allowTau;
        public bool allowKroot = true;
        public bool AllowKroot => AllowKrootWeapons && allowKroot;
        public bool allowKrootAuxiliaries = true;
        public bool AllowKrootAuxiliaries => AllowTau && AllowKroot && allowKrootAuxiliaries;
        public bool allowVespid = false;
        public bool AllowVespid => AllowVespuidWeapons && allowVespid;
        public bool allowVespidAuxiliaries = false;
        public bool AllowVespidAuxiliaries => AllowTau && AllowVespuidWeapons && allowVespidAuxiliaries;
        public bool allowGueVesaAuxiliaries = false;
        public bool AllowGueVesaAuxiliaries => AllowTau && allowGueVesaAuxiliaries;

        // Playable Tau Settings

        // Orkz Settings

        public bool ShowOrk = false;
        public bool allowOrkTek = true;
        public bool AllowOrkTek => AllowOrkWeapons && allowOrkTek;
        public bool allowOrkFeral = true;
        public bool AllowOrkFeral => AllowOrkWeapons && allowOrkFeral;
        public bool allowOrkRok = true;
        public bool AllowOrkRok => AllowOrkWeapons && allowOrkRok;

        // Playable Orkz Settings

        // temp option
        public bool DisableVEMPatch = false;

        public bool OrkoidFightyness = true;

        public float OrkoidFightynessStatisfied;
        public string OrkoidFightynessStatisfiedBuffer;
        public float OrkoidMinHealing;
        public string OrkoidMinHealingBuffer;
        
        public float FungusMedChance;
        public string FungusMedChanceBuffer;

        public float FungusSpawnChance;
        public string FungusSpawnChanceBuffer;
        public float FungusSquigChance;
        public string FungusSquigChanceBuffer;
        public float FungusSnotChance;
        public string FungusSnotChanceBuffer;
        public float FungusGrotChance;
        public string FungusGrotChanceBuffer;
        public float FungusOrkChance;
        public string FungusOrkChanceBuffer;

        public float CocoonSpawnChance;
        public string CocoonSpawnChanceBuffer;
        public float CocoonSquigChance;
        public string CocoonSquigChanceBuffer;
        public float CocoonSnotChance;
        public string CocoonSnotChanceBuffer;
        public float CocoonGrotChance;
        public string CocoonGrotChanceBuffer;
        public float CocoonOrkChance;
        public string CocoonOrkChanceBuffer;

        // Necron Settings

        public bool ShowNecron = false;
        public bool allowNecron = true;
        public bool AllowNecron => AllowNecronWeapons && allowNecron;
        public bool allowNecronMonolith = true;
        public bool AllowNecronMonolith => AllowNecron && allowNecronMonolith;
        public bool allowNecronWellBeBack = true;
        public bool AllowNecronWellBeBack => AllowNecron && allowNecronWellBeBack;

        // Tyranid Settings

        public bool ShowTyranid = false;
        public bool allowTyranid = true;
        public bool AllowTyranid => allowTyranid && AllowTyranidWeapons;
        public bool allowTyranidInfestation = false;
        public bool AllowTyranidInfestation => AllowTyranid && allowTyranidInfestation;

        public bool rimTime = false;
        #endregion
        // Compatability Patch Settings
        private List<PatchDescription> _PatchesCompatabilityScribeHelper = new List<PatchDescription>();
        public List<PatchDescription> DisabledPatchSetting = new List<PatchDescription>();

        // Racial Restriction Settings
        public List<RaceSettingHandle> RaceSettings => raceSettingsActive ??= raceSettings.FindAll(x=> x.Loaded);
        private List<RaceSettingHandle> raceSettingsActive;
        private List<RaceSettingHandle> raceSettings;
        // Faction Settings
        public List<FactionSettingHandle> FactionSettings => factionSettingsActive ??= factionSettings.FindAll(x=> x.Loaded);
        private List<FactionSettingHandle> factionSettingsActive;
        private List<FactionSettingHandle> factionSettings;

        public override void ExposeData()
        {
            base.ExposeData();
            // Armoury Data
            {
                Scribe_Values.Look(ref this.ShowArmourySettings, "AMA_ShowArmourySettings", false);
                Scribe_Values.Look(ref this.ArmouryGeneralSpecialRules, "AMA_ShowSpecialRules", false);
                Scribe_Values.Look(ref this.AllowDeepStrike, "AMA_AllowDeepStrike", true);
                Scribe_Values.Look(ref this.AllowInfiltrate, "AMA_AllowInfiltrate", true);

                Scribe_Values.Look(ref this.ShowAllowedWeaponSpecialRules, "AMA_ShowWeaponSpecialRules", false);
                Scribe_Values.Look(ref this.AllowRapidFire, "AMA_AllowRapidFire", true);
                Scribe_Values.Look(ref this.AllowGetsHot, "AMA_AllowGetsHot", true);
                Scribe_Values.Look(ref this.AllowJams, "AMA_AllowJams", true);
                Scribe_Values.Look(ref this.AllowMultiShot, "AMA_AllowMultiShot", true);
                Scribe_Values.Look(ref this.AllowUserEffects, "AMA_AllowUserEffects", true);
                Scribe_Values.Look(ref this.AllowForceWeaponEffect, "AMA_AllowForceWeaponEffect", true);
                Scribe_Values.Look(ref this.AllowRendingMeleeEffect, "AMA_AllowRendingMeleeEffect", true);
                Scribe_Values.Look(ref this.AllowRendingRangedEffect, "AMA_AllowRendingRangedEffect", true);

                Scribe_Values.Look(ref this.ShowAllowedWeapons, "AMA_ShowAllowedWeapons", false);
                Scribe_Values.Look(ref this.AllowImperialWeapons, "AMA_AllowImperialWeapons", true);
                Scribe_Values.Look(ref this.allowMechanicusWeapons, "AMA_AllowMechanicusWeapons", true);
                Scribe_Values.Look(ref this.allowAssassinorumWeapons, "AMA_AllowAssassinorumWeapons", true);
                Scribe_Values.Look(ref this.allowAstartesWeapons, "AMA_AllowAstartesWeapons", true);
                Scribe_Values.Look(ref this.allowMilitarumWeapons, "AMA_AllowMilitarumWeapons", true);
                Scribe_Values.Look(ref this.allowInquisitorialWeapons, "AMA_AllowInquisitorialWeapons", true);
                Scribe_Values.Look(ref this.allowSororitasWeapons, "AMA_AllowSororitasWeapons", true);
                Scribe_Values.Look(ref this.AllowChaosWeapons, "AMA_AllowChaosWeapons", true);
                Scribe_Values.Look(ref this.AllowEldarWeapons, "AMA_AllowEldarWeapons", true);
                Scribe_Values.Look(ref this.AllowDarkEldarWeapons, "AMA_AllowDarkEldarWeapons", true);
                Scribe_Values.Look(ref this.AllowTauWeapons, "AMA_AllowTauWeapons", true);
                Scribe_Values.Look(ref this.AllowOrkWeapons, "AMA_AllowOrkWeapons", true);
                Scribe_Values.Look(ref this.AllowNecronWeapons, "AMA_AllowNecronWeapons", true);
                Scribe_Values.Look(ref this.AllowTyranidWeapons, "AMA_AllowTyranidWeapons", true);

                Scribe_Values.Look(ref this.AllowProjectileTrail, "AMA_AllowProjectileTrail", true);
                Scribe_Values.Look(ref this.AllowProjectileGlow, "AMA_AllowProjectileGlow", true);
                Scribe_Values.Look(ref this.AllowMuzzlePosition, "AMA_AllowMuzzlePosition", true);
                Scribe_Values.Look(ref this.AllowPauldronDrawer, "AMA_AllowPauldronDrawer", true);
                Scribe_Values.Look(ref this.AllowExtraPartDrawer, "AMA_AllowExtraPartDrawer", true);
                Scribe_Values.Look(ref this.AllowHediffPartDrawer, "AMA_AllowHediffPartDrawer", true);
                Scribe_Values.Look(ref this.AllowDynmanicPartDrawer, "AMA_AllowDynmanicPartDrawer", true);

                Scribe_Values.Look(ref this.RacialConstructionRestriction, "AMA_RacialConstructionRestriction", true);
                Scribe_Values.Look(ref this.RacialProductionRestriction, "AMA_RacialProductionRestriction", true);
                Scribe_Values.Look(ref this.RacialResearchRestriction, "AMA_RacialResearchRestriction", true);
            }

            // Xenobiologis Data
            {
                Scribe_Values.Look(ref this.ShowXenobiologisSettings, "AMXB_ShowXenobiologisSettings", true);
                Scribe_Values.Look(ref this.ShowAllowedRaceSettings, "AMXB_ShowAllowedRaceSettings", true);
                Scribe_Values.Look(ref this.ForceRelations, "AMXB_ForceRelations", true);
                Scribe_Values.Look(ref this.ShowImperium, "AMXB_ShowImperium", false);
                Scribe_Values.Look(ref this.allowAdeptusAstartes, "AMXB_AllowAdeptusAstartes", false);
                Scribe_Values.Look(ref this.allowAdeptusMechanicus, "AMXB_AllowAdeptusMechanicus", true);
                Scribe_Values.Look(ref this.allowAdeptusMilitarum, "AMXB_AllowAdeptusMilitarum", true);
                Scribe_Values.Look(ref this.allowAdeptusSororitas, "AMXB_AllowAdeptusSororitas", false);
                Scribe_Values.Look(ref this.ShowChaos, "AMXB_ShowChaos", false);
                Scribe_Values.Look(ref this.allowChaosMarine, "AMXB_AllowChaosMarine", false);
                Scribe_Values.Look(ref this.allowChaosGuard, "AMXB_AllowChaosGuard", false);
                Scribe_Values.Look(ref this.allowChaosMechanicus, "AMXB_AllowChaosMechanicus", false);
                Scribe_Values.Look(ref this.allowWarpstorm, "AMXB_AllowWarpstorm", true);
                Scribe_Values.Look(ref this.allowChaosDeamons, "AMXB_AllowChaosDeamons", true);
                Scribe_Values.Look(ref this.allowChaosDeamonicIncursion, "AMXB_AllowChaosDeamonicIncursion", true);
                Scribe_Values.Look(ref this.allowChaosDeamonicInfestation, "AMXB_AllowChaosDeamonicInfestation", true);
                Scribe_Values.Look(ref this.EndTimesIntergrateDeamons, "AMXB_EndTimesChaosDeamonIntergration", true);
                Scribe_Values.Look(ref this.EndTimesIntergrateDeamonsGreat, "AMXB_EndTimesChaosDeamonIntergration_GreatPortal", true);
                Scribe_Values.Look(ref this.EndTimesIntergrateDeamonsSmall, "AMXB_EndTimesChaosDeamonIntergration_SmallPortal", true);
                Scribe_Values.Look(ref this.ShowAeldari, "AMXB_ShowDarkEldar", false);
                Scribe_Values.Look(ref this.allowDarkEldar, "AMXB_AllowDarkEldar", true);
                Scribe_Values.Look(ref this.ShowAeldari, "AMXB_ShowEldar", false);
                Scribe_Values.Look(ref this.allowEldarCraftworld, "AMXB_AllowEldarCraftworld", true);
                Scribe_Values.Look(ref this.allowEldarExodite, "AMXB_AllowEldarExodite", false);
                Scribe_Values.Look(ref this.allowEldarHarlequinn, "AMXB_AllowEldarHarlequinn", false);
                Scribe_Values.Look(ref this.allowEldarWraithguard, "AMXB_AllowEldarWraithguard", true);
                Scribe_Values.Look(ref this.ShowTau, "AMXB_ShowTau", false);
                Scribe_Values.Look(ref this.allowTau, "AMXB_AllowTau", true);
                Scribe_Values.Look(ref this.allowGueVesaAuxiliaries, "AMXB_AllowGueVesaAuxiliaries", true);
                Scribe_Values.Look(ref this.allowKrootAuxiliaries, "AMXB_AllowKrootAuxiliaries", true);
                Scribe_Values.Look(ref this.allowKroot, "AMXB_AllowKroot", true);
                Scribe_Values.Look(ref this.allowVespidAuxiliaries, "AMXB_AllowVespidAuxiliaries", true);
                Scribe_Values.Look(ref this.allowVespid, "AMXB_AllowVespid", false);
                Scribe_Values.Look(ref this.ShowNecron, "AMXB_ShowNecron", true);
                Scribe_Values.Look(ref this.allowNecron, "AMXB_AllowNecron", true);
                Scribe_Values.Look(ref this.allowNecronMonolith, "AMXB_AllowNecronMonolith", true);
                Scribe_Values.Look(ref this.allowNecronWellBeBack, "AMXB_AllowNecronWellBeBack", true);
                Scribe_Values.Look(ref this.ShowTyranid, "AMXB_ShowTyranid", false);
                Scribe_Values.Look(ref this.allowTyranid, "AMXB_AllowTyranid", true);
                Scribe_Values.Look(ref this.allowTyranidInfestation, "AMXB_AllowTyranidInfestation", true);

            }
            // Astartes Data
            {
                Scribe_Values.Look(ref this.ShowAstartes, "AMAA_ShowAstartes", true);
                Scribe_Values.Look(ref this.AstartePunchingFactor, "AMAA_AstartePunchingFactor", 1f);
                Scribe_Values.Look(ref this.AstarteSplitFactor, "AMAA_AstarteSplitFactor", 1f);
                Scribe_Values.Look(ref this.AstarteScale, "AMAA_AstarteScale", 1f);
                // Astartes Playable Race Extras
                Scribe_Values.Look(ref this.AstarteEasyMode, "AMAA_AstarteEasyMode", false);
                Scribe_Values.Look(ref this.AstartesAgeMatters, "AMAA_AstartesAgeMatters", true);
                Scribe_Values.Look(ref this.AstartesMaleOnly, "AMAA_AstartesMaleOnly", true);
                Scribe_Values.Look(ref this.AstarteUseOrgans, "AMAA_AstarteUseOrgans", true);
                Scribe_Values.Look(ref this.AstartesHumansOnly, "AMAA_AstartesHumansOnly", true);

            }


            // Tau Data
            {
                // Tau Playable Race Extras

            }

            // Ork Data
            {
                Scribe_Values.Look(ref this.ShowOrk, "AMXB_AllowOrk", true);
                Scribe_Values.Look(ref this.allowOrkTek, "AMXB_AllowOrkTek", true);
                Scribe_Values.Look(ref this.allowOrkFeral, "AMXB_AllowOrkFeral", true);
                Scribe_Values.Look(ref this.allowOrkRok, "AMXB_AllowOrkRok", true);

                // Orkz Playable Race Extras

                Scribe_Values.Look(ref this.OrkoidFightyness, "AMO_AllowOrkoidFightyness", true);
                Scribe_Values.Look(ref this.OrkoidFightynessStatisfied, "AMO_OrkoidFightynessStatisfied", 24000);
                Scribe_Values.Look(ref this.OrkoidFightynessStatisfiedBuffer, "AMO_OrkoidFightynessStatisfiedBuffer", "24000");
                Scribe_Values.Look(ref this.OrkoidMinHealing, "AMO_OrkoidMinHealing", 0.001f);
                Scribe_Values.Look(ref this.OrkoidMinHealingBuffer, "AMO_OrkoidMinHealingBuffer", "0.001");

                Scribe_Values.Look(ref this.FungusMedChance, "AMO_FungusMedChance", 0.01f);
                Scribe_Values.Look(ref this.FungusMedChanceBuffer, "AMO_FungusMedChanceBuffer", "0.01");

                Scribe_Values.Look(ref this.FungusSpawnChance, "AMO_FungusSpawnChance", 0.025f);
                Scribe_Values.Look(ref this.FungusSpawnChanceBuffer, "AMO_FungusSpawnChanceBuffer", "0.025");
                Scribe_Values.Look(ref this.FungusSquigChance, "AMO_FungusSquigChance", 1f);
                Scribe_Values.Look(ref this.FungusSquigChanceBuffer, "AMO_FungusSquigChanceBuffer", "1");
                Scribe_Values.Look(ref this.FungusSnotChance, "AMO_FungusSnotChance", 0.85f);
                Scribe_Values.Look(ref this.FungusSnotChanceBuffer, "AMO_FungusSnotChanceBuffer", "0.85");
                Scribe_Values.Look(ref this.FungusGrotChance, "AMO_FungusGrotChance", 0.1f);
                Scribe_Values.Look(ref this.FungusGrotChanceBuffer, "AMO_FungusGrotChanceBuffer", "0.1");
                Scribe_Values.Look(ref this.FungusOrkChance, "AMO_FungusOrkChance", 0.05f);
                Scribe_Values.Look(ref this.FungusOrkChanceBuffer, "AMO_FungusOrkChanceBuffer", "0.05");

                Scribe_Values.Look(ref this.CocoonSpawnChance, "AMO_CocoonSpawnChance", 0.25f);
                Scribe_Values.Look(ref this.CocoonSpawnChanceBuffer, "AMO_CocoonSpawnChanceBuffer", "0.25");
                Scribe_Values.Look(ref this.CocoonSquigChance, "AMO_CocoonSquigChance", 0.15f);
                Scribe_Values.Look(ref this.CocoonSquigChanceBuffer, "AMO_CocoonSquigChanceBuffer", "0.15");
                Scribe_Values.Look(ref this.CocoonSnotChance, "AMO_CocoonSnotChance", 0.2f);
                Scribe_Values.Look(ref this.CocoonSnotChanceBuffer, "AMO_CocoonSnotChanceBuffer", "0.2");
                Scribe_Values.Look(ref this.CocoonGrotChance, "AMO_CocoonGrotChance", 0.35f);
                Scribe_Values.Look(ref this.CocoonGrotChanceBuffer, "AMO_CocoonGrotChanceBuffer", "0.35");
                Scribe_Values.Look(ref this.CocoonOrkChance, "AMO_CocoonOrkChance", 0.3f);
                Scribe_Values.Look(ref this.CocoonOrkChanceBuffer, "AMO_CocoonOrkChanceBuffer", "0.3");
            }

            if (Scribe.mode == LoadSaveMode.LoadingVars || Scribe.mode == LoadSaveMode.PostLoadInit)
            {

            }
            Scribe_Collections.Look<RaceSettingHandle>(ref this.raceSettings, "raceSettings"/*, LookMode.Def, LookMode.Value, ref RaceKeyWorkingList, ref RaceValueWorkingList*/);
            Scribe_Collections.Look<FactionSettingHandle>(ref this.factionSettings, "factionSettings"/*, LookMode.Def, LookMode.Value, ref RaceKeyWorkingList, ref RaceValueWorkingList*/);
            if (Scribe.mode == LoadSaveMode.Saving && !DisabledPatchSetting.NullOrEmpty())
            {
                // create the data structure we're going to save.
                _PatchesCompatabilityScribeHelper = DisabledPatchSetting;
            }
            Scribe_Collections.Look(ref _PatchesCompatabilityScribeHelper, "patches");
            // finally, when the scribe finishes, we need to transform this back to a data structure we understand.
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                DisabledPatchSetting = _PatchesCompatabilityScribeHelper;
            }
            /*
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                // create the data structure we're going to save.
                _CompatabilityPatchesScribeHelper = PatchDisabled.ToDictionary(
                    // delegate to transform a dict item into a key, we want the file property of the old key. ( PatchDescription => string )
                    k => k.Key.file,

                    // same for the value, which is just the value. ( bool => bool )
                    v => v.Value);
            }
            Scribe_Collections.Look(ref _CompatabilityPatchesScribeHelper, "patches", LookMode.Value, LookMode.Value);
            // finally, when the scribe finishes, we need to transform this back to a data structure we understand.
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                // for each stored patch, update the value in our dictionary.
                if (!_CompatabilityPatchesScribeHelper.EnumerableNullOrEmpty())
                {
                    foreach (var storedPatch in _CompatabilityPatchesScribeHelper)
                    {
                        var index = AMAMod.Patches.FindIndex(p => p.file == storedPatch.Key);
                        if (index >= 0) // match found
                        {
                            var patch = AMAMod.Patches[index];
                            PatchDisabled[patch] = storedPatch.Value;
                        }
                    }
                }
            }
            */
            if (TrashableKeyPairs.EnumerableNullOrEmpty())
            {
                TrashableKeyPairs = new Dictionary<string, bool>();
            }
            Scribe_Collections.Look<string, bool>(ref this.TrashableKeyPairs, "TrashableKeyPairs");
        }

        public Dictionary<string, bool> TrashableKeyPairs;

        public bool CanTrash(Building b, Pawn pawn = null) 
        { 
            return CanTrash(b.def, b.Stuff, pawn);
        }
        public bool CanTrash(ThingDef thingDef, ThingDef stuffdef = null, Pawn pawn = null)
        {
            bool setting = true;
            if (TrashableKeyPairs.ContainsKey(thingDef.defName))
            {
                setting = TrashableKeyPairs.GetValueOrDefault(thingDef.defName);
            }
            else
            {
                TrashableKeyPairs.SetOrAdd(thingDef.defName, setting);
            }
            return setting && thingDef != ThingDefOf.Wall;
        }

    }
}