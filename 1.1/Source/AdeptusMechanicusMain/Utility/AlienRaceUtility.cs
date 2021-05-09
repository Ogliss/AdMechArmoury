using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;
using AlienRace;

namespace AdeptusMechanicus
{
    [MayRequireAlienRaces]
    public static class AlienRaceUtility
    {
        public static void AlienRaces()
        {
            AlienRace.ThingDef_AlienRace Human = DefDatabase<ThingDef>.GetNamedSilentFail("Human") as AlienRace.ThingDef_AlienRace;
            if (Human != null)
            {
                List<string> Tags = new List<string>() { "I", "C" };
                List<ResearchProjectDef> projects = new List<ResearchProjectDef>();
                projects.AddRange(ArmouryMain.ReseachImperial);
                projects.AddRange(ArmouryMain.ReseachChaos);
            //    DoRacialRestrictionsFor(Human, Tags, whiteResearches: projects);
            }
            AlienRace.ThingDef_AlienRace Mechanicus = ArmouryMain.mechanicus as AlienRace.ThingDef_AlienRace;
            AlienRace.ThingDef_AlienRace Ogryn = ArmouryMain.ogryn as AlienRace.ThingDef_AlienRace;
            AlienRace.ThingDef_AlienRace Ratlin = ArmouryMain.ratlin as AlienRace.ThingDef_AlienRace;
            AlienRace.ThingDef_AlienRace Beastman = ArmouryMain.beastman as AlienRace.ThingDef_AlienRace;
            if (Mechanicus != null)
            {
                DoRacialRestrictionsFor(Mechanicus, "AM", whiteResearches: ArmouryMain.ReseachMechanicus);
            }
            List<ThingDef> races = new List<ThingDef>();
            if (Ogryn != null) races.Add(Ogryn);
            if (Ratlin != null) races.Add(Ratlin);
            if (Beastman != null) races.Add(Beastman);

            if (AdeptusIntergrationUtility.enabled_GeneSeed)
            {
                AlienRace.ThingDef_AlienRace GeneseedAstartes = ArmouryMain.geneseedAstartes as AlienRace.ThingDef_AlienRace;
                AlienRace.ThingDef_AlienRace GeneseedCustodes = ArmouryMain.geneseedCustodes as AlienRace.ThingDef_AlienRace;
                if (GeneseedAstartes != null) races.Add(GeneseedAstartes);
                if (GeneseedCustodes != null) races.Add(GeneseedCustodes);
            }
            if (AdeptusIntergrationUtility.enabled_AstraServoSkulls)
            {
                races.AddRange(DefDatabase<ThingDef>.AllDefsListForReading.Where(x => (x.defName.Contains("IG_Serv_ServoSkull") && x.defName.Contains("_Race")) || (x.defName.Contains("IG_Serv_Servitor") && x.defName.Contains("_Race"))));
            }

            if (!races.NullOrEmpty())
            {
                List<string> Tags = new List<string>() { "I", "C" };
                List<ResearchProjectDef> projects = new List<ResearchProjectDef>();
                projects.AddRange(ArmouryMain.ReseachImperial);
                projects.AddRange(ArmouryMain.ReseachChaos);
            //    DoRacialRestrictionsFor(races, Tags, whiteResearches: projects);
            }
        }

        public static void DoRacialRestrictionsFor(ThingDef race, string whiteTag, List<string> blackTags = null, List<ResearchProjectDef> whiteResearches = null, List<ResearchProjectDef> blackResearches = null, List<ThingDef> whiteApparel = null, List<ThingDef> blackApparel = null, List<ThingDef> whiteWeapons = null, List<ThingDef> blackWeapons = null, List<ThingDef> whitePlants = null, List<ThingDef> blackPlants = null, List<ThingDef> whiteAnimals = null, List<ThingDef> blackAnimals = null)
        {
            List<ThingDef> races = new List<ThingDef>();
            races.Add(race);
            List<string> whiteTags = new List<string>();
            whiteTags.Add(whiteTag);
            DoRacialRestrictionsFor(races, whiteTags, blackTags, whiteResearches, blackResearches, whiteApparel, blackApparel, whiteWeapons, blackWeapons, whitePlants, blackPlants, whiteAnimals, blackAnimals);
        }

        public static void DoRacialRestrictionsFor(ThingDef race, List<string> whiteTags, List<string> blackTags = null, List<ResearchProjectDef> whiteResearches = null, List<ResearchProjectDef> blackResearches = null, List<ThingDef> whiteApparel = null, List<ThingDef> blackApparel = null, List<ThingDef> whiteWeapons = null, List<ThingDef> blackWeapons = null, List<ThingDef> whitePlants = null, List<ThingDef> blackPlants = null, List<ThingDef> whiteAnimals = null, List<ThingDef> blackAnimals = null)
        {
            List<ThingDef> races = new List<ThingDef>();
            races.Add(race);
            DoRacialRestrictionsFor(races, whiteTags, blackTags, whiteResearches, blackResearches, whiteApparel, blackApparel, whiteWeapons, blackWeapons, whitePlants, blackPlants, whiteAnimals, blackAnimals);
        }

        public static void DoRacialRestrictionsFor(List<ThingDef> races, string whiteTag, List<string> blackTags = null, List<ResearchProjectDef> whiteResearches = null, List<ResearchProjectDef> blackResearches = null, List<ThingDef> whiteApparel = null, List<ThingDef> blackApparel = null, List<ThingDef> whiteWeapons = null, List<ThingDef> blackWeapons = null, List<ThingDef> whitePlants = null, List<ThingDef> blackPlants = null, List<ThingDef> whiteAnimals = null, List<ThingDef> blackAnimals = null)
        {
            List<string> Tags = new List<string>();
            Tags.Add(whiteTag);
            DoRacialRestrictionsFor(races, Tags, blackTags, whiteResearches, blackResearches, whiteApparel, blackApparel, whiteWeapons, blackWeapons, whitePlants, blackPlants, whiteAnimals, blackAnimals);
        }

        public static void DoRacialRestrictionsFor(List<ThingDef> races, List<string> whiteTags, List<string> blackTags = null, List<ResearchProjectDef> whiteResearches = null, List<ResearchProjectDef> blackResearches = null, List<ThingDef> whiteApparel = null, List<ThingDef> blackApparel = null, List<ThingDef> whiteWeapons = null, List<ThingDef> blackWeapons = null, List<ThingDef> whitePlants = null, List<ThingDef> blackPlants = null, List<ThingDef> whiteAnimals = null, List<ThingDef> blackAnimals = null)
        {
            foreach (ThingDef race in races)
            {
                AlienRace.ThingDef_AlienRace alien = race as AlienRace.ThingDef_AlienRace;
                if (alien != null)
                {
                    string msg = "Try Do Racial Restrictions For " + alien.LabelCap + " (" + alien.defName + ")";
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
                                msg += "\n" + recipes.Count + " OG" + Tag + " whiteRecipes";
                                whiteRecipes.AddRange(recipes);
                            }
                            List<ThingDef> buildings = DefDatabase<ThingDef>.AllDefsListForReading.FindAll(x => x.defName.Contains("OG" + Tag + "_") && (x.building != null || x.IsBuildingArtificial));
                            if (!buildings.NullOrEmpty())
                            {
                                msg += "\n" + buildings.Count + " OG" + Tag + " whiteBuildings";
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
                                msg += "\n" + recipes.Count + " OG" + Tag + " blackRecipes";
                                blackRecipes.AddRange(recipes);
                            }
                            List<ThingDef> buildings = DefDatabase<ThingDef>.AllDefsListForReading.FindAll(x => x.defName.Contains("OG" + Tag + "_") && (x.building != null || x.IsBuildingArtificial));
                            if (!buildings.NullOrEmpty())
                            {
                                msg += "\n" + buildings.Count + " OG" + Tag + " blackBuildings";
                                blackBuildings.AddRange(buildings);
                            }
                        }
                    }
                    RestrictRecipes(alien, whiteRecipes, blackRecipes);
                    RestrictBuildings(alien, whiteBuildings, blackBuildings);
                    if (!whiteResearches.NullOrEmpty())
                    {
                        msg += "\n" + whiteResearches.Count + " whiteResearches";
                    }
                    if (!blackResearches.NullOrEmpty())
                    {
                        msg += "\n" + blackResearches.Count + " blackResearches";
                    }
                    RestrictResearch(alien, whiteResearches, blackResearches);
                    if (!whiteApparel.NullOrEmpty())
                    {
                        msg += "\n" + whiteApparel.Count + " whiteApparel";
                    }
                    if (!blackApparel.NullOrEmpty())
                    {
                        msg += "\n" + blackApparel.Count + " blackApparel";
                    }
                    RestrictApparel(alien, whiteApparel, blackApparel);
                    if (!whiteWeapons.NullOrEmpty())
                    {
                        msg += "\n" + whiteWeapons.Count + " whiteWeapons";
                    }
                    if (!blackWeapons.NullOrEmpty())
                    {
                        msg += "\n" + blackWeapons.Count + " blackWeapons";
                    }
                    RestrictWeapons(alien, whiteWeapons, blackWeapons);
                    if (!whitePlants.NullOrEmpty())
                    {
                        msg += "\n" + whitePlants.Count + " whitePlants";
                    }
                    if (!blackPlants.NullOrEmpty())
                    {
                        msg += "\n" + blackPlants.Count + " blackPlants";
                    }
                    RestrictPlants(alien, whitePlants, blackPlants);
                    if (!whiteAnimals.NullOrEmpty())
                    {
                        msg += "\n" + whiteAnimals.Count + " whiteAnimals";
                    }
                    if (!blackAnimals.NullOrEmpty())
                    {
                        msg += "\n" + blackAnimals.Count + " blackAnimals";
                    }
                    RestrictAnimals(alien, whiteAnimals, blackAnimals);
                    //    Log.Message(msg);
                }
            }
        }

        public static void RestrictBuildings(ThingDef race, List<ThingDef> listWhite = null, List<ThingDef> listBlack = null)
        {

            AlienRace.ThingDef_AlienRace alien = race as AlienRace.ThingDef_AlienRace;
            if (alien == null)
            {
                return;
            }
            List<ThingDef> list = alien.alienRace.raceRestriction.buildingList;
            List<ThingDef> whitelist = alien.alienRace.raceRestriction.whiteBuildingList;
            List<ThingDef> blacklist = alien.alienRace.raceRestriction.blackBuildingList;
            RestrictThings(ref list, ref whitelist, ref blacklist, ref RaceRestrictionSettings.buildingRestricted, listWhite, listBlack);
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

        public static void RestrictWeapons(ThingDef race, List<ThingDef> listWhite = null, List<ThingDef> listBlack = null)
        {
            AlienRace.ThingDef_AlienRace alien = race as AlienRace.ThingDef_AlienRace;
            if (alien == null)
            {
                return;
            }
            List<ThingDef> list = alien.alienRace.raceRestriction.weaponList;
            List<ThingDef> whitelist = alien.alienRace.raceRestriction.whiteWeaponList;
            List<ThingDef> blacklist = alien.alienRace.raceRestriction.blackWeaponList;
            RestrictThings(ref list, ref whitelist, ref blacklist, ref RaceRestrictionSettings.weaponRestricted, listWhite, listBlack);
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

        public static void RestrictPlants(ThingDef race, List<ThingDef> listWhite = null, List<ThingDef> listBlack = null)
        {
            AlienRace.ThingDef_AlienRace alien = race as AlienRace.ThingDef_AlienRace;
            if (alien == null)
            {
                return;
            }
            List<ThingDef> list = alien.alienRace.raceRestriction.plantList;
            List<ThingDef> whitelist = alien.alienRace.raceRestriction.whitePlantList;
            List<ThingDef> blacklist = alien.alienRace.raceRestriction.blackPlantList;
            RestrictThings(ref list, ref whitelist, ref blacklist, ref RaceRestrictionSettings.plantRestricted, listWhite, listBlack);
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

        public static void RestrictAnimals(ThingDef race, List<ThingDef> listWhite = null, List<ThingDef> listBlack = null)
        {
            AlienRace.ThingDef_AlienRace alien = race as AlienRace.ThingDef_AlienRace;
            if (alien == null)
            {
                return;
            }
            List<ThingDef> list = alien.alienRace.raceRestriction.petList;
            List<ThingDef> whitelist = alien.alienRace.raceRestriction.whitePetList;
            List<ThingDef> blacklist = alien.alienRace.raceRestriction.blackPetList;
            RestrictThings(ref list, ref whitelist, ref blacklist, ref RaceRestrictionSettings.petRestricted, listWhite, listBlack);
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

        public static void RestrictApparel(ThingDef race, List<ThingDef> listWhite = null, List<ThingDef> listBlack = null)
        {
            AlienRace.ThingDef_AlienRace alien = race as AlienRace.ThingDef_AlienRace;
            if (alien == null)
            {
                return;
            }
            List<ThingDef> list = alien.alienRace.raceRestriction.apparelList;
            List<ThingDef> whitelist = alien.alienRace.raceRestriction.whiteApparelList;
            List<ThingDef> blacklist = alien.alienRace.raceRestriction.blackApparelList;
            RestrictThings(ref list, ref whitelist, ref blacklist, ref RaceRestrictionSettings.apparelRestricted, listWhite, listBlack);
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
        public static void RestrictThings(ref List<ThingDef> list, ref List<ThingDef> whiteList, ref List<ThingDef> blackList, ref HashSet<ThingDef> restricted, List<ThingDef> listWhite = null, List<ThingDef> listBlack = null)
        {
            if (!listWhite.NullOrEmpty())
            {
                foreach (ThingDef def in listWhite)
                {
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
                foreach (ThingDef def in listBlack)
                {
                    if (!list.Contains(def))
                    {
                        list.Add(def);
                    }
                    if (!restricted.Contains(def))
                    {
                        restricted.Add(def);
                    }
                    if (!blackList.Contains(def))
                    {
                        blackList.Add(def);
                    }
                }
            }
        }

        public static void RestrictRecipes(ThingDef race, List<RecipeDef> listWhite, List<RecipeDef> listBlack = null)
        {
            AlienRace.ThingDef_AlienRace alien = race as AlienRace.ThingDef_AlienRace;
            if (alien == null)
            {
                return;
            }
            RestrictRecipes(ref alien.alienRace.raceRestriction.recipeList, ref alien.alienRace.raceRestriction.whiteRecipeList, ref alien.alienRace.raceRestriction.blackRecipeList, ref RaceRestrictionSettings.recipeRestricted, listWhite, listBlack);
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

        public static void RestrictRecipes(ref List<RecipeDef> list, ref List<RecipeDef> whiteList, ref List<RecipeDef> blackList, ref HashSet<RecipeDef> restricted, List<RecipeDef> listWhite = null, List<RecipeDef> listBlack = null)
        {
            list.AddRange(listWhite);
            if (!listWhite.NullOrEmpty())
            {
                whiteList.AddRange(listWhite);
                foreach (RecipeDef def in listWhite)
                {
                    if (!restricted.Contains(def))
                    {
                        restricted.Add(def);
                    }
                }
            }
            if (!listBlack.NullOrEmpty())
            {
                blackList.AddRange(listBlack);
                foreach (RecipeDef def in listBlack)
                {
                    if (!restricted.Contains(def))
                    {
                        restricted.Add(def);
                    }
                }
            }
        }

        public static void RestrictResearch(ThingDef race, List<ResearchProjectDef> listWhite = null, List<ResearchProjectDef> listBlack = null)
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
                alien.alienRace.raceRestriction.researchList[0].projects.AddRange(listWhite);
                foreach (ResearchProjectDef def in listWhite)
                {
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
            //    alien.alienRace.raceRestriction.researchList[0].projects.AddRange(listBlack);
                foreach (ResearchProjectDef def in listBlack)
                {
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

    }
}