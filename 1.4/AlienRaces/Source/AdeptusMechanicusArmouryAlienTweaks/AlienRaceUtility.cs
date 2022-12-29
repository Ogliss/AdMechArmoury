using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;
using AlienRace;
using System.Text;
using AdeptusMechanicus.settings;
using HarmonyLib;
using UnityEngine;

namespace AdeptusMechanicus
{

    // AdeptusMechanicus.AlienRaceUtility
    public static class AlienRaceUtility
    {
        public static void AlienRaces()
        {
            AlienRace.ThingDef_AlienRace Human = DefDatabase<ThingDef>.GetNamedSilentFail("Human") as AlienRace.ThingDef_AlienRace;
            AlienRace.ThingDef_AlienRace Mechanicus = ArmouryMain.mechanicus as AlienRace.ThingDef_AlienRace;
            AlienRace.ThingDef_AlienRace Ogryn = ArmouryMain.ogryn as AlienRace.ThingDef_AlienRace;
            AlienRace.ThingDef_AlienRace Ratlin = ArmouryMain.ratlin as AlienRace.ThingDef_AlienRace;
            AlienRace.ThingDef_AlienRace Beastman = ArmouryMain.beastman as AlienRace.ThingDef_AlienRace;
            AlienRace.ThingDef_AlienRace astartes_Geneseed = ArmouryMain.geneseedAstartes as AlienRace.ThingDef_AlienRace;
            AlienRace.ThingDef_AlienRace custodes_Geneseed = ArmouryMain.geneseedCustodes as AlienRace.ThingDef_AlienRace;
            List<ThingDef> races = new List<ThingDef>();
            foreach (var item in settings.AMSettings.Instance.RaceSettings)
            {
                if (!item.hidden && !races.Contains(item.Race))
                {
                    if (item.Imperial)
                    {
                        races.Add(item.Race);
                    }
                }
            }
            if (astartes_Geneseed != null && !races.Contains(astartes_Geneseed))
            {
                races.Add(astartes_Geneseed);
            }
            if (custodes_Geneseed != null && !races.Contains(custodes_Geneseed))
            {
                races.Add(custodes_Geneseed);
            }
            if (AdeptusIntergrationUtility.enabled_AstraServoSkulls)
            {
                races.AddRange(DefDatabase<ThingDef>.AllDefsListForReading.Where(x => (x.defName.Contains("IG_Serv_ServoSkull") && x.defName.Contains("_Race")) || (x.defName.Contains("IG_Serv_Servitor") && x.defName.Contains("_Race"))));
            }

            if (Human != null)
            {
                List<string> Tags = new List<string>() { "I", "C", "AS" };
                List<ResearchProjectDef> projects = new List<ResearchProjectDef>();

                projects.AddRange(ArmouryMain.ReseachImperial);
                projects.AddRange(ArmouryMain.ReseachChaos);
                DoRacialRestrictionsFor(Human, Tags, whiteResearches: projects, racesNotXeno: races);
            }
            if (!races.NullOrEmpty())
            {
                List<string> Tags = new List<string>() { "I", "C" };
                List<ResearchProjectDef> projects = new List<ResearchProjectDef>();
                projects.AddRange(ArmouryMain.ReseachImperial);
                projects.AddRange(ArmouryMain.ReseachChaos);
                DoRacialRestrictionsFor(races, Tags, whiteResearches: projects, racesNotXeno: races);
            }
            if (Mechanicus != null)
            {
                List<string> Tags = new List<string>() { "I", "AS", "AM" };
                List<ResearchProjectDef> projects = new List<ResearchProjectDef>();
                projects.AddRange(ArmouryMain.ReseachImperial);
                projects.AddRange(ArmouryMain.ReseachMechanicus);
                DoRacialRestrictionsFor(Mechanicus, Tags, whiteResearches: projects, racesNotXeno: races, Logging: AMAMod.Dev);
            }

        }

        private static readonly BackstoryCategoryFilter NewbornCategoryGroup = new BackstoryCategoryFilter
        {
            categories = new List<string>
            {
                "Newborn"
            },
            commonality = 1f
        };


        public static BackstoryCategoryFilter RaceNewbornCategoryGroup(BackstoryCategoryFilter newborn, Pawn pawn)
        {
            return newborn;
        }
        

        public static BackstoryCategoryFilter RaceChildCategoryGroup(BackstoryCategoryFilter child, Pawn pawn)
        {
            return child;
        }
        
        public static BackstoryCategoryFilter RaceAdultCategoryGroup(BackstoryCategoryFilter adult, Pawn pawn)
        {
            return adult;
        }


        public static bool RaceNewbornSkill(bool newborn, Pawn pawn)
        {
            return newborn;
        }
        public static bool RaceNewbornTraits(bool newborn, Pawn pawn)
        {
            return newborn;
        }
        public static int[] RaceGrowthMomentAges(int[] GrowthMomentAges, Pawn pawn)
        {
            return GrowthMomentAges;
        }

        public static SimpleCurve RaceAgeSkillMaxFactorCurve(SimpleCurve curve, Pawn pawn)
        {
            return curve;
        }

        public static SimpleCurve RaceAgeSkillFactor(SimpleCurve curve, Pawn pawn)
        {
            return curve;
        }
        public static void alienBackstories(BackstoryCategoryFilter categoryFilter, ref IEnumerable<RimWorld.BackstoryDef> source, Pawn pawn, BackstorySlot slot)
        {
            source.Concat(AlienRace.HarmonyPatches.FilterBackstories(DefDatabase<AlienRace.AlienBackstoryDef>.AllDefs.Where((AlienRace.AlienBackstoryDef bs) => bs.shuffleable && categoryFilter.Matches(bs)), pawn, slot));
        }
        public static void DoRacialRestrictionsFor(ThingDef race, string whiteTag, List<string> blackTags = null, List<ResearchProjectDef> whiteResearches = null, List<ResearchProjectDef> blackResearches = null, List<ThingDef> whiteApparel = null, List<ThingDef> blackApparel = null, List<ThingDef> whiteWeapons = null, List<ThingDef> blackWeapons = null, List<ThingDef> whitePlants = null, List<ThingDef> blackPlants = null, List<ThingDef> whiteAnimals = null, List<ThingDef> blackAnimals = null, List<ThingDef> racesNotXeno = null, bool Logging = false)
        {
            List<ThingDef> races = new List<ThingDef>();
            races.Add(race);
            List<string> whiteTags = new List<string>();
            whiteTags.Add(whiteTag);
            DoRacialRestrictionsFor(races, whiteTags, blackTags, whiteResearches, blackResearches, whiteApparel, blackApparel, whiteWeapons, blackWeapons, whitePlants, blackPlants, whiteAnimals, blackAnimals, racesNotXeno, Logging);
        }

        public static void DoRacialRestrictionsFor(ThingDef race, List<string> whiteTags, List<string> blackTags = null, List<ResearchProjectDef> whiteResearches = null, List<ResearchProjectDef> blackResearches = null, List<ThingDef> whiteApparel = null, List<ThingDef> blackApparel = null, List<ThingDef> whiteWeapons = null, List<ThingDef> blackWeapons = null, List<ThingDef> whitePlants = null, List<ThingDef> blackPlants = null, List<ThingDef> whiteAnimals = null, List<ThingDef> blackAnimals = null, List<ThingDef> racesNotXeno = null, bool Logging = false)
        {
            List<ThingDef> races = new List<ThingDef>();
            races.Add(race);
            DoRacialRestrictionsFor(races, whiteTags, blackTags, whiteResearches, blackResearches, whiteApparel, blackApparel, whiteWeapons, blackWeapons, whitePlants, blackPlants, whiteAnimals, blackAnimals, racesNotXeno, Logging);
        }

        public static void DoRacialRestrictionsFor(List<ThingDef> races, string whiteTag, List<string> blackTags = null, List<ResearchProjectDef> whiteResearches = null, List<ResearchProjectDef> blackResearches = null, List<ThingDef> whiteApparel = null, List<ThingDef> blackApparel = null, List<ThingDef> whiteWeapons = null, List<ThingDef> blackWeapons = null, List<ThingDef> whitePlants = null, List<ThingDef> blackPlants = null, List<ThingDef> whiteAnimals = null, List<ThingDef> blackAnimals = null, List<ThingDef> racesNotXeno = null, bool Logging = false)
        {
            List<string> Tags = new List<string>();
            Tags.Add(whiteTag);
            DoRacialRestrictionsFor(races, Tags, blackTags, whiteResearches, blackResearches, whiteApparel, blackApparel, whiteWeapons, blackWeapons, whitePlants, blackPlants, whiteAnimals, blackAnimals, racesNotXeno, Logging);
        }

        public static void DoRacialRestrictionsFor(List<ThingDef> races, List<string> whiteTags, List<string> blackTags = null, List<ResearchProjectDef> whiteResearches = null, List<ResearchProjectDef> blackResearches = null, List<ThingDef> whiteApparel = null, List<ThingDef> blackApparel = null, List<ThingDef> whiteWeapons = null, List<ThingDef> blackWeapons = null, List<ThingDef> whitePlants = null, List<ThingDef> blackPlants = null, List<ThingDef> whiteAnimals = null, List<ThingDef> blackAnimals = null, List<ThingDef> racesNotXeno = null, bool Logging = false)
        {
            foreach (ThingDef race in races)
            {
                AlienRace.ThingDef_AlienRace alien = race as AlienRace.ThingDef_AlienRace;
                if (alien != null)
                {
                    debug = new StringBuilder("Try Do Racial Restrictions For " + alien.LabelCap + " (" + alien.defName + ")");
                    debug.AppendLine();
                    List<ThingDef> blackBuildings = new List<ThingDef>();
                    List<ThingDef> whiteBuildings = new List<ThingDef>();
                    List<RecipeDef> blackRecipes = new List<RecipeDef>();
                    List<RecipeDef> whiteRecipes = new List<RecipeDef>();
                    if (!whiteTags.NullOrEmpty())
                    {
                        foreach (string Tag in whiteTags)
                        {
                            List<RecipeDef> recipes = DefDatabase<RecipeDef>.AllDefsListForReading.FindAll(x => x.defName.Contains("OG" + Tag + "_"));
                            if (!recipes.NullOrEmpty())
                            {
                            //    if (Logging) debug.AppendLine(recipes.Count + "   OG" + Tag + " whiteRecipes");
                                whiteRecipes.AddRange(recipes);
                            }
                            List<ThingDef> buildings = DefDatabase<ThingDef>.AllDefsListForReading.FindAll(x => x.defName.Contains("OG" + Tag + "_") && (x.building != null || x.IsBuildingArtificial));
                            if (!buildings.NullOrEmpty())
                            {
                            //     if (Logging) debug.AppendLine(buildings.Count + "   OG" + Tag + " whiteBuildings");
                                whiteBuildings.AddRange(buildings);
                            }
                        }
                    }
                    if (!blackTags.NullOrEmpty())
                    {
                        foreach (string Tag in blackTags)
                        {
                            List<RecipeDef> recipes = DefDatabase<RecipeDef>.AllDefsListForReading.FindAll(x => x.defName.Contains("OG" + Tag + "_"));
                            if (!recipes.NullOrEmpty())
                            {
                            //     if (Logging) debug.AppendLine(recipes.Count + "   OG" + Tag + " blackRecipes");
                                blackRecipes.AddRange(recipes);
                            }
                            List<ThingDef> buildings = DefDatabase<ThingDef>.AllDefsListForReading.FindAll(x => x.defName.Contains("OG" + Tag + "_") && (x.building != null || x.IsBuildingArtificial));
                            if (!buildings.NullOrEmpty())
                            {
                            //     if (Logging) debug.AppendLine(buildings.Count + "   OG" + Tag + " blackBuildings");
                                blackBuildings.AddRange(buildings);
                            }
                        }
                    }
                    if (settings.AMSettings.Instance.RacialProductionRestriction)
                    {
                        if (!blackRecipes.NullOrEmpty() || !whiteRecipes.NullOrEmpty() && Logging)
                        {
                            debug.AppendLine("    Recipes: ");//+(blackRecipes.Count + whiteRecipes.Count));
                        }
                        RestrictRecipes(alien, whiteRecipes, blackRecipes, Logging);
                    }
                    if (settings.AMSettings.Instance.RacialConstructionRestriction)
                    {
                        if (!blackBuildings.NullOrEmpty() || !whiteBuildings.NullOrEmpty() && Logging)
                        {
                            debug.AppendLine("    Buildings: ");// + (blackBuildings.Count + whiteBuildings.Count));
                        }
                        RestrictBuildings(alien, whiteBuildings, blackBuildings, Logging);
                    }
                    if (settings.AMSettings.Instance.RacialResearchRestriction)
                    {
                        if (!blackResearches.NullOrEmpty() || !whiteResearches.NullOrEmpty() && Logging)
                        {
                            debug.AppendLine("    Research: ");// + (blackResearches.Count + whiteResearches.Count));
                        }
                        RestrictResearch(alien, whiteResearches, blackResearches, Logging);
                    }
                    if (!blackApparel.NullOrEmpty() || !whiteApparel.NullOrEmpty() && Logging)
                    {
                        debug.AppendLine("    Apparel: ");// + (blackApparel.Count + whiteApparel.Count));
                    }
                    RestrictApparel(alien, whiteApparel, blackApparel, Logging);
                    if (!blackWeapons.NullOrEmpty() || !whiteWeapons.NullOrEmpty() && Logging)
                    {
                        debug.AppendLine("    Weapons: ");// + (blackWeapons.Count + whiteWeapons.Count));
                    }
                    RestrictWeapons(alien, whiteWeapons, blackWeapons, Logging);
                    if (!blackPlants.NullOrEmpty() || !whitePlants.NullOrEmpty() && Logging)
                    {
                        debug.AppendLine("    Plants: ");// + (blackPlants.Count + whitePlants.Count));
                    }
                    RestrictPlants(alien, whitePlants, blackPlants, Logging);
                    if (!blackAnimals.NullOrEmpty() || !whiteAnimals.NullOrEmpty() && Logging)
                    {
                        debug.AppendLine("    Animals: ");// + (blackAnimals.Count + whiteAnimals.Count));
                    }
                    RestrictAnimals(alien, whiteAnimals, blackAnimals, Logging);
                    if (!racesNotXeno.NullOrEmpty())
                    {
                        debug.AppendLine("    Xenophobia: ");// + (blackAnimals.Count + whiteAnimals.Count));
                    }
                    Xenophobia(alien, racesNotXeno, Logging);
                    if (Logging) Log.Message(debug.ToString());
                }
            }
        }

        public static void Xenophobia(ThingDef race, List<ThingDef> Races, bool logging)
        {
            if (race is AlienRace.ThingDef_AlienRace alien && !Races.NullOrEmpty())
            {
                foreach (var item in Races)
                {
                    if (item != race)
                    {
                        if (alien.alienRace.generalSettings.notXenophobistTowards.NullOrEmpty()) alien.alienRace.generalSettings.notXenophobistTowards = new List<ThingDef>();
                        if (!alien.alienRace.generalSettings.notXenophobistTowards.Contains(item)) alien.alienRace.generalSettings.notXenophobistTowards.Add(item);
                    }
                }
            }
        }

        public static void RestrictBuildings(ThingDef race, List<ThingDef> listWhite = null, List<ThingDef> listBlack = null, bool Logging = false)
        {

            AlienRace.ThingDef_AlienRace alien = race as AlienRace.ThingDef_AlienRace;
            if (alien == null)
            {
                return;
            }
            List<ThingDef> list = alien.alienRace.raceRestriction.buildingList;
            List<ThingDef> whitelist = alien.alienRace.raceRestriction.whiteBuildingList;
            List<ThingDef> blacklist = alien.alienRace.raceRestriction.blackBuildingList;
            RestrictThings(ref list, ref whitelist, ref blacklist, ref RaceRestrictionSettings.buildingRestricted, listWhite, listBlack, Logging);
            /*
            alien.alienRace.raceRestriction.buildingList.AddRange(listWhite);
            alien.alienRace.raceRestriction.whiteBuildingList.AddRange(listWhite);
            foreach (ThingDef def in listWhite)
            {
                if (!RaceRestrictionSettings.buildingRestricted.Contains(def))
                {
                    RaceRestrictionSettings.buildingRestricted.Add(def);
                }
            }
            */
        }

        public static void RestrictWeapons(ThingDef race, List<ThingDef> listWhite = null, List<ThingDef> listBlack = null, bool Logging = false)
        {
            AlienRace.ThingDef_AlienRace alien = race as AlienRace.ThingDef_AlienRace;
            if (alien == null)
            {
                return;
            }
            List<ThingDef> list = alien.alienRace.raceRestriction.weaponList;
            List<ThingDef> whitelist = alien.alienRace.raceRestriction.whiteWeaponList;
            List<ThingDef> blacklist = alien.alienRace.raceRestriction.blackWeaponList;
            RestrictThings(ref list, ref whitelist, ref blacklist, ref RaceRestrictionSettings.weaponRestricted, listWhite, listBlack, Logging);
            /*
            alien.alienRace.raceRestriction.weaponList.AddRange(listWhite);
            alien.alienRace.raceRestriction.whiteWeaponList.AddRange(listWhite);
            foreach (ThingDef def in listWhite)
            {
                if (!RaceRestrictionSettings.weaponRestricted.Contains(def))
                {
                    RaceRestrictionSettings.weaponRestricted.Add(def);
                }
            }
            */
        }

        public static void RestrictPlants(ThingDef race, List<ThingDef> listWhite = null, List<ThingDef> listBlack = null, bool Logging = false)
        {
            AlienRace.ThingDef_AlienRace alien = race as AlienRace.ThingDef_AlienRace;
            if (alien == null)
            {
                return;
            }
            List<ThingDef> list = alien.alienRace.raceRestriction.plantList;
            List<ThingDef> whitelist = alien.alienRace.raceRestriction.whitePlantList;
            List<ThingDef> blacklist = alien.alienRace.raceRestriction.blackPlantList;
            RestrictThings(ref list, ref whitelist, ref blacklist, ref RaceRestrictionSettings.plantRestricted, listWhite, listBlack, Logging);
            /*
            alien.alienRace.raceRestriction.plantList.AddRange(listWhite);
            alien.alienRace.raceRestriction.whitePlantList.AddRange(listWhite);
            foreach (ThingDef def in listWhite)
            {
                if (!RaceRestrictionSettings.plantRestricted.Contains(def))
                {
                    RaceRestrictionSettings.plantRestricted.Add(def);
                }
            }
            */
        }

        public static void RestrictAnimals(ThingDef race, List<ThingDef> listWhite = null, List<ThingDef> listBlack = null, bool Logging = false)
        {
            AlienRace.ThingDef_AlienRace alien = race as AlienRace.ThingDef_AlienRace;
            if (alien == null)
            {
                return;
            }
            List<ThingDef> list = alien.alienRace.raceRestriction.petList;
            List<ThingDef> whitelist = alien.alienRace.raceRestriction.whitePetList;
            List<ThingDef> blacklist = alien.alienRace.raceRestriction.blackPetList;
            RestrictThings(ref list, ref whitelist, ref blacklist, ref RaceRestrictionSettings.petRestricted, listWhite, listBlack, Logging);
            /*
            alien.alienRace.raceRestriction.petList.AddRange(listWhite);
            alien.alienRace.raceRestriction.whitePetList.AddRange(listWhite);
            foreach (ThingDef def in listWhite)
            {
                if (!RaceRestrictionSettings.petRestricted.Contains(def))
                {
                    RaceRestrictionSettings.petRestricted.Add(def);
                }
            }
            */
        }

        public static void RestrictApparel(ThingDef race, List<ThingDef> listWhite = null, List<ThingDef> listBlack = null, bool Logging = false)
        {
            AlienRace.ThingDef_AlienRace alien = race as AlienRace.ThingDef_AlienRace;
            if (alien == null)
            {
                return;
            }
            RestrictThings(ref alien.alienRace.raceRestriction.apparelList, ref alien.alienRace.raceRestriction.whiteApparelList, ref alien.alienRace.raceRestriction.blackApparelList, ref RaceRestrictionSettings.apparelRestricted, listWhite, listBlack, Logging);
            /*
            alien.alienRace.raceRestriction.apparelList.AddRange(listWhite);
            alien.alienRace.raceRestriction.whiteApparelList.AddRange(listWhite);
            foreach (ThingDef def in listWhite)
            {
                if (!RaceRestrictionSettings.apparelRestricted.Contains(def))
                {
                    RaceRestrictionSettings.apparelRestricted.Add(def);
                }
            }
            */
        }
        public static void RestrictThings(ref List<ThingDef> list, ref List<ThingDef> whiteList, ref List<ThingDef> blackList, ref HashSet<ThingDef> restricted, List<ThingDef> listWhite = null, List<ThingDef> listBlack = null, bool Logging = false)
        {
            if (!listWhite.NullOrEmpty())
            {
                if (Logging) debug.AppendLine("        Whitelising: " + listWhite.Count);
                foreach (ThingDef def in listWhite)
                {
                    if (Logging) debug.AppendLine("            " + def.defName);
                    if (!list.Contains(def))
                    {
                        list.Add(def);
                    }
                    if (!restricted.Contains(def))
                    {
                        restricted.Add(def);
                    }
                    if (!whiteList.Contains(def))
                    {
                        whiteList.Add(def);
                    }
                }
            }
            if (!listBlack.NullOrEmpty())
            {
                if (Logging) debug.AppendLine("        blacklising: " + listBlack.Count);
                foreach (ThingDef def in listBlack)
                {
                    if (Logging) debug.AppendLine("            " + def.defName);
                    /*
                    if (!list.Contains(def))
                    {
                        list.Add(def);
                    }
                    if (!restricted.Contains(def))
                    {
                        restricted.Add(def);
                    }
                    */
                    if (!blackList.Contains(def))
                    {
                        blackList.Add(def);
                    }
                }
            }
        }

        public static void RestrictRecipes(ThingDef race, List<RecipeDef> listWhite, List<RecipeDef> listBlack = null, bool Logging = false)
        {
            AlienRace.ThingDef_AlienRace alien = race as AlienRace.ThingDef_AlienRace;
            if (alien == null)
            {
                return;
            }
            RestrictRecipes(ref alien.alienRace.raceRestriction.recipeList, ref alien.alienRace.raceRestriction.whiteRecipeList, ref alien.alienRace.raceRestriction.blackRecipeList, ref RaceRestrictionSettings.recipeRestricted, listWhite, listBlack, Logging);
            /*
            alien.alienRace.raceRestriction.recipeList.AddRange(listWhite);
            alien.alienRace.raceRestriction.whiteRecipeList.AddRange(listWhite);
            foreach (RecipeDef def in listWhite)
            {
                if (!RaceRestrictionSettings.recipeRestricted.Contains(def))
                {
                    RaceRestrictionSettings.recipeRestricted.Add(def);
                }
            }
            */
        }

        public static void RestrictRecipes(ref List<RecipeDef> list, ref List<RecipeDef> whiteList, ref List<RecipeDef> blackList, ref HashSet<RecipeDef> restricted, List<RecipeDef> listWhite = null, List<RecipeDef> listBlack = null, bool Logging = false)
        {
            list.AddRange(listWhite);
            if (!listWhite.NullOrEmpty())
            {
                if (Logging) debug.AppendLine("        Whitelising: "+ listWhite.Count);
                whiteList.AddRange(listWhite);
                foreach (RecipeDef def in listWhite)
                {
                    if (Logging) debug.AppendLine("            " + def.defName);
                    if (!restricted.Contains(def))
                    {
                        restricted.Add(def);
                    }
                }
            }
            if (!listBlack.NullOrEmpty())
            {
                if (Logging) debug.AppendLine("        blacklising: " + listBlack.Count);
                blackList.AddRange(listBlack);
                foreach (RecipeDef def in listBlack)
                {
                    if (Logging) debug.AppendLine("            " + def.defName);
                    if (!restricted.Contains(def))
                    {
                    //    restricted.Add(def);
                    }
                }
            }
        }

        public static void RestrictResearch(ThingDef race, List<ResearchProjectDef> listWhite = null, List<ResearchProjectDef> listBlack = null, bool Logging = false)
        {
            AlienRace.ThingDef_AlienRace alien = race as AlienRace.ThingDef_AlienRace;
            if (alien == null)
            {
                return;
            }
            if (alien.alienRace.raceRestriction.researchList.NullOrEmpty())
            {
                alien.alienRace.raceRestriction.researchList = new List<AlienRace.ResearchProjectRestrictions>();
                alien.alienRace.raceRestriction.researchList.Add(new AlienRace.ResearchProjectRestrictions());
                alien.alienRace.raceRestriction.researchList[0].projects = new List<ResearchProjectDef>();
                alien.alienRace.raceRestriction.researchList[0].apparelList = new List<ThingDef>();
            }
            if (!listWhite.NullOrEmpty())
            {
                if (Logging) debug.AppendLine("        Whitelising: " + listWhite.Count);
                alien.alienRace.raceRestriction.researchList[0].projects.AddRange(listWhite);
                foreach (ResearchProjectDef def in listWhite)
                {
                    if (Logging) debug.AppendLine("            " + def.defName);
                    if (!AlienRace.RaceRestrictionSettings.researchRestrictionDict.ContainsKey(key: def))
                    {
                        List<AlienRace.ThingDef_AlienRace> l2 = new List<AlienRace.ThingDef_AlienRace>();
                        l2.Add(alien);
                        AlienRace.RaceRestrictionSettings.researchRestrictionDict.Add(key: def, value: l2);
                    }
                    else
                    {
                        if (!AlienRace.RaceRestrictionSettings.researchRestrictionDict[key: def].Contains(alien))
                        {
                            AlienRace.RaceRestrictionSettings.researchRestrictionDict[key: def].Add(item: alien);
                        }
                    }
                }
            }
            if (!listBlack.NullOrEmpty())
            {
                if (Logging) debug.AppendLine("        blacklising: " + listBlack.Count);
                //    alien.alienRace.raceRestriction.researchList[0].projects.AddRange(listBlack);
                foreach (ResearchProjectDef def in listBlack)
                {
                    if (Logging) debug.AppendLine("            " + def.defName);
                    if (!AlienRace.RaceRestrictionSettings.researchRestrictionDict.ContainsKey(key: def))
                    {
                        List<AlienRace.ThingDef_AlienRace> l2 = new List<AlienRace.ThingDef_AlienRace>();
                        l2.Add(alien);
                        AlienRace.RaceRestrictionSettings.researchRestrictionDict.Add(key: def, value: l2);
                    }
                    else
                    {
                        if (!AlienRace.RaceRestrictionSettings.researchRestrictionDict[key: def].Contains(alien))
                        {
                            AlienRace.RaceRestrictionSettings.researchRestrictionDict[key: def].Add(item: alien);
                        }
                    }
                }
            }
        }
        public static bool CanResearch(ResearchProjectDef def)
        {
            return RaceRestrictionSettings.CanResearch((IEnumerable<ThingDef>)AccessTools.Field(typeof(AlienRace.HarmonyPatches), "colonistRaces").GetValue(null), def);
        }

        public static void RequiredRace(ResearchProjectDef project, ref Rect rect)
        {
            if (selected != project)
            {
                raceRestriction = RaceRestrictionSettings.researchRestrictionDict[key: project].ToList<ThingDef>();
            }
            if (!raceRestriction.NullOrEmpty())
            {
                for (int i = 0; i < raceRestriction.Count; i++)
                {
                    Widgets.LabelCacheHeight(ref rect, raceRestriction[i].LabelCap, true, false);
                    rect.yMin += rect.height;
                }
            }
        }
        static ResearchProjectDef selected = null;
        static List<ThingDef> raceRestriction = new List<ThingDef>();
        private static StringBuilder debug = new StringBuilder();
    }
}