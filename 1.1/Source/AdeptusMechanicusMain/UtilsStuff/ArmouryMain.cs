using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using AdeptusMechanicus;
using Verse.Sound;

namespace AdeptusMechanicus
{
    [StaticConstructorOnStartup]
    public static class ArmouryMain
    {
        public static IEnumerable<RecipeDef> humanRecipes;
        public static IEnumerable<ThingDef> TechHediffItems; 
        public static IEnumerable<RecipeDef> TechHediffRecipes;
        public static List<ResearchProjectDef> ReseachImperial = new List<ResearchProjectDef>();
        public static List<ResearchProjectDef> ReseachMechanicus = new List<ResearchProjectDef>();
        public static List<ResearchProjectDef> ReseachChaos = new List<ResearchProjectDef>();
        public static List<ScenarioDef> scenariosTesting = new List<ScenarioDef>();
        public static List<FactionDef> factionColours = new List<FactionDef>();
        public static ThingDef mechanicus;
        public static ThingDef astartes;
        public static ThingDef ogryn;
        public static ThingDef ratlin;
        public static ThingDef beastman;
        public static ThingDef geneseedAstartes;
        public static ThingDef geneseedCustodes;
        public static Texture2D expandTex;
        public static Texture2D collapseTex;

        static ArmouryMain()
        {
            collapseTex = TexButton.Collapse;
            if (collapseTex == null)
            {
                Log.Error("collapseTex == null");
            }
            expandTex = TexButton.Reveal;
            if (expandTex == null)
            {
                Log.Error("expandTex == null");
            }
            humanRecipes = DefDatabase<RecipeDef>.AllDefs.Where(x => x.AllRecipeUsers.Contains(ThingDefOf.Human));
            TechHediffRecipes = from x in DefDatabase<RecipeDef>.AllDefs
                                where TechHediffItems.Any(z => x.IsIngredient(z)) && x.targetsBodyPart
                                select x;
            TechHediffItems = from x in DefDatabase<ThingDef>.AllDefs
                              where x.isTechHediff && x.BaseMarketValue > 0
                              select x;
            ReseachImperial = DefDatabase<ResearchProjectDef>.AllDefs.Where(x => x.defName.Contains("OG_Imperial_Tech_")).ToList();
            ReseachMechanicus = DefDatabase<ResearchProjectDef>.AllDefs.Where(x => x.defName.Contains("OG_Mechanicus_Tech_")).ToList();
            ReseachChaos = DefDatabase<ResearchProjectDef>.AllDefs.Where(x => x.defName.Contains("OG_Chaos_Tech_")).ToList();
            scenariosTesting = DefDatabase<ScenarioDef>.AllDefs.Where(x => x.defName.StartsWith("OGAM_TestScenario_")).ToList();
            mechanicus = DefDatabase<ThingDef>.GetNamedSilentFail("OG_Human_Mechanicus");
            astartes = DefDatabase<ThingDef>.GetNamedSilentFail("OG_Human_Astartes");
            ogryn = DefDatabase<ThingDef>.GetNamedSilentFail("OG_Abhuman_Ogryn");
            ratlin = DefDatabase<ThingDef>.GetNamedSilentFail("OG_Abhuman_Ratling");
            beastman = DefDatabase<ThingDef>.GetNamedSilentFail("OG_Abhuman_Beastman");
            geneseedAstartes = DefDatabase<ThingDef>.GetNamedSilentFail("AstarteSpaceMarine");
            geneseedCustodes = DefDatabase<ThingDef>.GetNamedSilentFail("AdaptusCustodes");
            factionColours = DefDatabase<FactionDef>.AllDefs.Where(x => x.GetModExtension<FactionDefExtension>()?.factionColor != null || x.GetModExtension<FactionDefExtension>()?.factionColorTwo != null).ToList();
            Log.Message("factions with colours: "+ factionColours.Count);
            /*
            Log.Message("AppDomain.CurrentDomain.GetAssemblies():\n" + System.AppDomain.CurrentDomain.GetAssemblies().Join(delimiter: "\n"));
            Log.Message("GenTypes.AllActiveAssemblies:\n" + Traverse.Create(typeof(GenTypes)).Property<IEnumerable<System.Reflection.Assembly>>("AllActiveAssemblies").Value.Join(delimiter: "\n"));
            */
            //    Log.Message("ArmouryMain ");
            if (DefDatabase<ScenarioDef>.AllDefs.Any(x=> x.defName.Contains("OGAM_TestScenario_")))
            {
                foreach (ScenarioDef ScenDef in scenariosTesting)
                {
                    if (ScenDef.defName.Contains("Imperial"))
                    {
                        TryAddWeaponsToScenario(ScenDef, "I");
                        TryAddWeaponsToScenario(ScenDef, "IG");
                        
                    }
                    else if (ScenDef.defName.Contains("Mechanicus"))
                    {
                        TryAddWeaponsToScenario(ScenDef, "AM");
                    }
                    else if (ScenDef.defName.Contains("Astartes"))
                    {
                        TryAddWeaponsToScenario(ScenDef, "AA");
                    }
                    else if (ScenDef.defName.Contains("Chaos"))
                    {
                        TryAddWeaponsToScenario(ScenDef, "C");
                    }
                    else if (ScenDef.defName.Contains("Eldar"))
                    {
                        if (ScenDef.defName.Contains("DarkEldar"))
                        {
                            TryAddWeaponsToScenario(ScenDef, "DE");
                        }
                        else TryAddWeaponsToScenario(ScenDef, "E");
                    }
                    else if (ScenDef.defName.Contains("Tau"))
                    {
                        TryAddWeaponsToScenario(ScenDef, "T");
                        TryAddWeaponsToScenario(ScenDef, "K");
                        TryAddWeaponsToScenario(ScenDef, "V");
                    }
                    else if (ScenDef.defName.Contains("Ork"))
                    {
                        TryAddWeaponsToScenario(ScenDef, "O");
                    }
                    else if (ScenDef.defName.Contains("Necron"))
                    {
                        TryAddWeaponsToScenario(ScenDef, "N");
                    }
                    else if (ScenDef.defName.Contains("Tyranid"))
                    {
                        TryAddWeaponsToScenario(ScenDef, "TY");
                    }
                }
            }
            //    Log.Message("ArmouryMain 0");
            //    Log.Message("ArmouryMain 1");
            foreach (RecipeDef item in humanRecipes)
            {
                if (item.recipeUsers.NullOrEmpty())
                {
                    item.recipeUsers = new List<ThingDef>();
                }
                //    Log.Message("Checking Human recipes " + item);
                if (mechanicus != null)
                {
                    //    Log.Message("ArmouryMain 1 mechanicus");
                    if (!item.AllRecipeUsers.Contains(mechanicus))
                    {
                        item.recipeUsers.Add(mechanicus);
                    }
                }
                if (astartes != null)
                {
                    if (!item.AllRecipeUsers.EnumerableNullOrEmpty())
                    {
                        if (!item.AllRecipeUsers.Contains(astartes))
                        {
                            //    Log.Message("ArmouryMain 1 astartes");
                        //    Log.Message("Adding " + item + " to astartes");
                            item.recipeUsers.Add(astartes);
                        //    Log.Message("Added " + item + " to astartes");
                        }
                    }
                }
                if (AdeptusIntergrationUtility.enabled_GeneSeed)
                {
                    if (geneseedAstartes != null)
                    {
                        if (!item.AllRecipeUsers.EnumerableNullOrEmpty())
                        {
                            if (!item.AllRecipeUsers.Contains(geneseedAstartes))
                            {
                                //    Log.Message("ArmouryMain 1 astartes");
                                //    Log.Message("Adding " + item + " to astartes");
                                item.recipeUsers.Add(geneseedAstartes);
                                //    Log.Message("Added " + item + " to astartes");
                            }
                        }
                    }
                    if (geneseedCustodes != null)
                    {
                        if (!item.AllRecipeUsers.EnumerableNullOrEmpty())
                        {
                            if (!item.AllRecipeUsers.Contains(geneseedCustodes))
                            {
                                //    Log.Message("ArmouryMain 1 astartes");
                                //    Log.Message("Adding " + item + " to astartes");
                                item.recipeUsers.Add(geneseedCustodes);
                                //    Log.Message("Added " + item + " to astartes");
                            }
                        }
                    }
                }
                if (ogryn != null)
                {
                //        Log.Message("ArmouryMain 1 ogryn");
                    if (!item.AllRecipeUsers.Contains(ogryn))
                    {
                        item.recipeUsers.Add(ogryn);
                    }
                }
                if (ratlin != null)
                {
                //       Log.Message("ArmouryMain 1 ratlin");
                    if (!item.AllRecipeUsers.Contains(ratlin))
                    {
                        item.recipeUsers.Add(ratlin);
                    }
                }
                if (beastman != null)
                {
                //        Log.Message("ArmouryMain 1 beastman");
                    if (!item.AllRecipeUsers.Contains(beastman))
                    {
                        item.recipeUsers.Add(beastman);
                    }
                }
            }
            //    Log.Message("ArmouryMain 2");
            //    Log.Message("ArmouryMain 3");
            //    Log.Message("ArmouryMain 4");
            if (AdeptusIntergrationUtility.enabled_AlienRaces)
            {
                AlienRaces();
            }

        }

        public static void TryAddWeaponsToScenario(ScenarioDef ScenDef, string Tag)
        {
            List<ThingDef> things = DefDatabase<ThingDef>.AllDefsListForReading.FindAll(x => ((x.PlayerAcquirable || Tag == "TY") && (x.IsWeapon || x.IsApparel) &&  x.defName.Contains("OG" + Tag) && !x.defName.Contains("TOGGLEDEF_S")));
            foreach (ThingDef Weapon in things)
            {
                bool hasweapon = false;
                List<ScenPart> parts = Traverse.Create(ScenDef.scenario).Field("parts").GetValue<List<ScenPart>>();
                foreach (ScenPart scenpart in parts.Where(x => x.def == ScenPartDefOf.StartingThing_Defined))
                {
                    ThingDef td = Traverse.Create(scenpart).Field("thingDef").GetValue<ThingDef>();
                    if (td == Weapon)
                    {
                        hasweapon = true;
                        break;
                    }
                }
                if (hasweapon)
                {
                    continue;
                }
                ScenPart_StartingThing_Define StartingThing = new ScenPart_StartingThing_Define() { def = ScenPartDefOf.StartingThing_Defined };
                if (Weapon.MadeFromStuff)
                {
                    ThingDef stuffdef = DefDatabase<ThingDef>.AllDefsListForReading.Where(x => x.IsStuff && Weapon.stuffCategories.Any(y => x.stuffProps.categories.Contains(y))).RandomElement();
                    if (stuffdef != null)
                    {
                        StartingThing.ThingDefStuff = stuffdef;
                    }
                }
                StartingThing.ThingDef = Weapon;
                parts.Add(StartingThing);
            }
        }

        [MayRequireAlienRaces]
        public static void AlienRaces()
        {
            AlienRace.ThingDef_AlienRace Human = DefDatabase<ThingDef>.GetNamedSilentFail("Human") as AlienRace.ThingDef_AlienRace;
            if (Human != null)
            {
                List<string> Tags = new List<string>() { "I", "C" };
                List<ResearchProjectDef> projects = new List<ResearchProjectDef>();
                projects.AddRange(ReseachImperial);
                projects.AddRange(ReseachChaos);
                DoRacialRestrictionsFor(Human, Tags, projects);
            }
            AlienRace.ThingDef_AlienRace Mechanicus = mechanicus as AlienRace.ThingDef_AlienRace;
            AlienRace.ThingDef_AlienRace Ogryn = ogryn as AlienRace.ThingDef_AlienRace;
            AlienRace.ThingDef_AlienRace Ratlin = ratlin as AlienRace.ThingDef_AlienRace;
            AlienRace.ThingDef_AlienRace Beastman = beastman as AlienRace.ThingDef_AlienRace;
            if (Mechanicus != null)
            {
                List<string> Tags = new List<string>() { "I", "AM", "C" };
                List<ResearchProjectDef> projects = new List<ResearchProjectDef>();
                projects.AddRange(ReseachImperial);
                projects.AddRange(ReseachMechanicus);
                projects.AddRange(ReseachChaos);
                DoRacialRestrictionsFor(Mechanicus, Tags, projects);
            }
            List<ThingDef> races = new List<ThingDef>();
            if (Ogryn != null) races.Add(Ogryn);
            if (Ratlin != null) races.Add(Ratlin);
            if (Beastman != null) races.Add(Beastman);

            if (AdeptusIntergrationUtility.enabled_GeneSeed)
            {
                AlienRace.ThingDef_AlienRace GeneseedAstartes = geneseedAstartes as AlienRace.ThingDef_AlienRace;
                AlienRace.ThingDef_AlienRace GeneseedCustodes = geneseedCustodes as AlienRace.ThingDef_AlienRace;
                if (GeneseedAstartes != null) races.Add(GeneseedAstartes);
                if (GeneseedCustodes != null) races.Add(GeneseedCustodes);
            }
            if (AdeptusIntergrationUtility.enabled_AstraServoSkulls)
            {
                races.AddRange(DefDatabase<ThingDef>.AllDefsListForReading.Where(x=> (x.defName.Contains("IG_Serv_ServoSkull") && x.defName.Contains("_Race")) || (x.defName.Contains("IG_Serv_Servitor") && x.defName.Contains("_Race"))));
            }

            if (!races.NullOrEmpty())
            {
                List<string> Tags = new List<string>() { "I", "C" };
                List<ResearchProjectDef> projects = new List<ResearchProjectDef>();
                projects.AddRange(ReseachImperial);
                projects.AddRange(ReseachChaos);
                DoRacialRestrictionsFor(races, Tags, projects);
            }
        }

        [MayRequireAlienRaces]
        public static void DoRacialRestrictionsFor(ThingDef race, string Tag, List<ResearchProjectDef> researches = null, List<ThingDef> apparel = null, List<ThingDef> weapons = null, List<ThingDef> plants = null, List<ThingDef> animals = null)
        {
            List<ThingDef> races = new List<ThingDef>();
            races.Add(race);
            List<string> Tags = new List<string>();
            Tags.Add(Tag);
            DoRacialRestrictionsFor(races, Tags, researches, apparel, weapons, plants, animals);
        }

        [MayRequireAlienRaces]
        public static void DoRacialRestrictionsFor(ThingDef race, List<string> Tags, List<ResearchProjectDef> researches = null, List<ThingDef> apparel = null, List<ThingDef> weapons = null, List<ThingDef> plants = null, List<ThingDef> animals = null)
        {
            List<ThingDef> races = new List<ThingDef>();
            races.Add(race);
            DoRacialRestrictionsFor(races, Tags, researches, apparel, weapons, plants, animals);
        }

        [MayRequireAlienRaces]
        public static void DoRacialRestrictionsFor(List<ThingDef> races, string Tag, List<ResearchProjectDef> researches = null, List<ThingDef> apparel = null, List<ThingDef> weapons = null, List<ThingDef> plants = null, List<ThingDef> animals = null)
        {
            List<string> Tags = new List<string>();
            Tags.Add(Tag);
            DoRacialRestrictionsFor(races, Tags, researches, apparel, weapons, plants, animals);
        }

        [MayRequireAlienRaces]
        public static void DoRacialRestrictionsFor(List<ThingDef> races, List<string> Tags, List<ResearchProjectDef> researches = null, List<ThingDef> apparel = null, List<ThingDef> weapons = null, List<ThingDef> plants = null, List<ThingDef> animals = null)
        {
            foreach (ThingDef race in races)
            {
                AlienRace.ThingDef_AlienRace alien = race as AlienRace.ThingDef_AlienRace;
                if (alien != null)
                {
                    string msg = "Try Do Racial Restrictions For " + alien.LabelCap + " (" + alien.defName + ")";
                    foreach (string Tag in Tags)
                    {
                        List<RecipeDef> recipes = DefDatabase<RecipeDef>.AllDefsListForReading.FindAll(x => x.defName.Contains("OG" + Tag + "_"));
                        if (!recipes.NullOrEmpty())
                        {
                            msg += "\n" + recipes.Count + " OG" + Tag +" Recipes";
                            RestrictRecipes(alien, recipes);
                        }
                        List<ThingDef> buildings = DefDatabase<ThingDef>.AllDefsListForReading.FindAll(x => x.defName.Contains("OG" + Tag + "_") && (x.building != null || x.IsBuildingArtificial));
                        if (!buildings.NullOrEmpty())
                        {
                            msg += "\n" + buildings.Count + " OG" + Tag + " Buildings";
                            RestrictBuildings(alien, buildings);
                        }
                    }
                    if (!researches.NullOrEmpty())
                    {
                        msg += "\n" + researches.Count + " Reseaches";
                        RestrictResearch(alien, researches);
                    }
                    if (!apparel.NullOrEmpty())
                    {
                        msg += "\n" + apparel.Count + " Apparel";
                        RestrictApparel(alien, apparel);
                    }
                    if (!weapons.NullOrEmpty())
                    {
                        msg += "\n" + weapons.Count + " Weapons";
                        RestrictWeapons(alien, weapons);
                    }
                    if (!plants.NullOrEmpty())
                    {
                        msg += "\n" + plants.Count + " Plants";
                        RestrictPlants(alien, plants);
                    }
                    if (!animals.NullOrEmpty())
                    {
                        msg += "\n" + animals.Count + " Animals";
                        RestrictAnimals(alien, animals);
                    }
                //    Log.Message(msg);
                }
            }
        }

        [MayRequireAlienRaces]
        public static void RestrictRecipes(ThingDef race, List<RecipeDef> things)
        {
            AlienRace.ThingDef_AlienRace alien = race as AlienRace.ThingDef_AlienRace;
            if (alien == null)
            {
                return;
            }
            alien.alienRace.raceRestriction.recipeList.AddRange(things);
            foreach (RecipeDef def in things)
            {
                if (!AlienRace.RaceRestrictionSettings.recipeRestrictionDict.ContainsKey(key: def))
                {
                    AlienRace.RaceRestrictionSettings.recipeRestrictionDict.Add(key: def, value: new List<AlienRace.ThingDef_AlienRace>());
                }
                if (!AlienRace.RaceRestrictionSettings.recipeRestrictionDict[key: def].Contains(alien))
                {
                    AlienRace.RaceRestrictionSettings.recipeRestrictionDict[key: def].Add(item: alien as AlienRace.ThingDef_AlienRace);
                }
            }
        }

        [MayRequireAlienRaces]
        public static void RestrictBuildings(ThingDef race, List<ThingDef> things)
        {

            AlienRace.ThingDef_AlienRace alien = race as AlienRace.ThingDef_AlienRace;
            if (alien == null)
            {
                return;
            }
            alien.alienRace.raceRestriction.buildingList.AddRange(things);
            foreach (ThingDef def in things)
            {
                if (!AlienRace.RaceRestrictionSettings.buildingRestrictionDict.ContainsKey(key: def))
                {
                    AlienRace.RaceRestrictionSettings.buildingRestrictionDict.Add(key: def, value: new List<AlienRace.ThingDef_AlienRace>());
                }
                if (!AlienRace.RaceRestrictionSettings.buildingRestrictionDict[key: def].Contains(alien))
                {
                    AlienRace.RaceRestrictionSettings.buildingRestrictionDict[key: def].Add(item: alien as AlienRace.ThingDef_AlienRace);
                }
            }
        }

        [MayRequireAlienRaces]
        public static void RestrictResearch(ThingDef race, List<ResearchProjectDef> list)
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
            alien.alienRace.raceRestriction.researchList[0].projects.AddRange(list);
            foreach (ResearchProjectDef def in list)
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

        [MayRequireAlienRaces]
        public static void RestrictWeapons(ThingDef race, List<ThingDef> list)
        {
            AlienRace.ThingDef_AlienRace alien = race as AlienRace.ThingDef_AlienRace;
            if (alien == null)
            {
                return;
            }
            alien.alienRace.raceRestriction.weaponList.AddRange(list);
            foreach (ThingDef def in list)
            {
                if (!AlienRace.RaceRestrictionSettings.weaponRestrictionDict.ContainsKey(key: def))
                    AlienRace.RaceRestrictionSettings.weaponRestrictionDict.Add(key: def, value: new List<AlienRace.ThingDef_AlienRace>());
                if (!AlienRace.RaceRestrictionSettings.weaponRestrictionDict[key: def].Contains(alien))
                {
                    AlienRace.RaceRestrictionSettings.weaponRestrictionDict[key: def].Add(item: alien as AlienRace.ThingDef_AlienRace);
                }
            }
        }

        [MayRequireAlienRaces]
        public static void RestrictPlants(ThingDef race, List<ThingDef> list)
        {
            AlienRace.ThingDef_AlienRace alien = race as AlienRace.ThingDef_AlienRace;
            if (alien == null)
            {
                return;
            }
            alien.alienRace.raceRestriction.plantList.AddRange(list);
            foreach (ThingDef def in list)
            {
                if (!AlienRace.RaceRestrictionSettings.plantRestrictionDict.ContainsKey(key: def))
                    AlienRace.RaceRestrictionSettings.plantRestrictionDict.Add(key: def, value: new List<AlienRace.ThingDef_AlienRace>());
                if (!AlienRace.RaceRestrictionSettings.plantRestrictionDict[key: def].Contains(alien))
                {
                    AlienRace.RaceRestrictionSettings.plantRestrictionDict[key: def].Add(item: alien as AlienRace.ThingDef_AlienRace);
                }
            }
        }
        
        [MayRequireAlienRaces]
        public static void RestrictAnimals(ThingDef race, List<ThingDef> list)
        {
            AlienRace.ThingDef_AlienRace alien = race as AlienRace.ThingDef_AlienRace;
            if (alien == null)
            {
                return;
            }
            alien.alienRace.raceRestriction.petList.AddRange(list);
            foreach (ThingDef def in list)
            {
                if (!AlienRace.RaceRestrictionSettings.tameRestrictionDict.ContainsKey(key: def))
                    AlienRace.RaceRestrictionSettings.tameRestrictionDict.Add(key: def, value: new List<AlienRace.ThingDef_AlienRace>());
                if (!AlienRace.RaceRestrictionSettings.tameRestrictionDict[key: def].Contains(alien))
                {
                    AlienRace.RaceRestrictionSettings.tameRestrictionDict[key: def].Add(item: alien as AlienRace.ThingDef_AlienRace);
                }
            }
        }

        [MayRequireAlienRaces]
        public static void RestrictApparel(ThingDef race, List<ThingDef> list)
        {
            AlienRace.ThingDef_AlienRace alien = race as AlienRace.ThingDef_AlienRace;
            if (alien == null)
            {
                return;
            }
            alien.alienRace.raceRestriction.apparelList.AddRange(list);
            foreach (ThingDef def in list)
            {
                if (!AlienRace.RaceRestrictionSettings.apparelRestrictionDict.ContainsKey(key: def))

                    AlienRace.RaceRestrictionSettings.apparelRestrictionDict.Add(key: def, value: new List<AlienRace.ThingDef_AlienRace>());
                if (!AlienRace.RaceRestrictionSettings.apparelRestrictionDict[key: def].Contains(alien))
                {
                    AlienRace.RaceRestrictionSettings.apparelRestrictionDict[key: def].Add(item: alien as AlienRace.ThingDef_AlienRace);
                }
            }
        }
        
        public static void CopyFields<T>(T source, T destination)
        {
            var fields = source.GetType().GetFields();
            foreach (var field in fields)
            {
                field.SetValue(destination, field.GetValue(source));
            }
        }
    }

}