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
using AdeptusMechanicus.settings;
using System.Text;
using AdeptusMechanicus.ExtensionMethods;

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
        public static List<string> humansTags = new List<string>() {"I", "AM", "AS", "C" };
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
            factionColours = DefDatabase<FactionDef>.AllDefs.Where(x => x.GetModExtensionFast<FactionDefExtension>()?.factionColor != null || x.GetModExtensionFast<FactionDefExtension>()?.factionColorTwo != null).ToList();
            if (AMAMod.Dev) Log.Message("factions with colours: "+ factionColours.Count);
            IEnumerable<BackstorySettings> settings = DefDatabase<BackstorySettings>.AllDefs;
            if (!settings.EnumerableNullOrEmpty())
            {
                InsertTags();
            }
            /*
        //    Log.Message("AppDomain.CurrentDomain.GetAssemblies():\n" + System.AppDomain.CurrentDomain.GetAssemblies().Join(delimiter: "\n"));
        //    Log.Message("GenTypes.AllActiveAssemblies:\n" + Traverse.Create(typeof(GenTypes)).Property<IEnumerable<System.Reflection.Assembly>>("AllActiveAssemblies").Value.Join(delimiter: "\n"));
            */
            //    Log.Message("ArmouryMain ");
            if (AMAMod.Dev && !scenariosTesting.NullOrEmpty())
            {
                TestingScenarioUtility.SetUpTestScenarios(scenariosTesting);
            }
            HumanlikeRecipeUtility.AddHumanlikeRecipes();
            if (AdeptusIntergrationUtility.enabled_AlienRaces)
            {
                AlienRaceUtility.AlienRaces();
            }
            /*
            StringBuilder Memes = new StringBuilder("MemeDef's");
            foreach (var item in DefDatabase<MemeDef>.AllDefsListForReading)
            {
                Memes.AppendLine(item.defName);
            }
            StringBuilder Precepts = new StringBuilder("PreceptDef's");
            foreach (var item in DefDatabase<PreceptDef>.AllDefsListForReading)
            {
                Precepts.AppendLine(item.defName);
            }
        //    Log.Message(Memes.ToString());
        //    Log.Message(Precepts.ToString());
            */
        }

        public static void InsertTags()
        {
            IEnumerable<BackstorySettings> settings = DefDatabase<BackstorySettings>.AllDefs;
            IEnumerable<BackstoryTagItem> items = settings.SelectMany((BackstorySettings bs) => bs.backstoryTagInsertion);
       //     Log.Message("Checking " + items.Count() + " Backstroy Tags from " + settings.Count() + " BackstorySettings");
            foreach (BackstoryTagItem backstoryTagItem in items)
            {
            //    Log.Message("BackstorySettings for " + backstoryTagItem);
                using (List<string>.Enumerator enumerator2 = backstoryTagItem.backstories.GetEnumerator())
                {
                    while (enumerator2.MoveNext())
                    {
                        Backstory backstory;
                        if (BackstoryDatabase.TryGetWithIdentifier(enumerator2.Current, out backstory, false))
                        {
                            backstory.spawnCategories.AddRange(backstoryTagItem.spawnCategories);
                            if (backstoryTagItem.debug) Log.Message("Found Backstory with Identifer: " + enumerator2.Current + " Backstory: " + backstory.identifier + " spawnCategories: " + backstory.spawnCategories.ToCommaList());
                        }
                    }
                }
            }
        }
        
        public static void CopyFields<T>(T source, T destination, List<string> not = null, bool log = false)
        {
            var fields = source.GetType().GetFields();
            foreach (var field in fields)
            {
                if (!not.NullOrEmpty())
                {
                    if (not.Any(x=> x == field.Name))
                    {
                        Log.Message($"CopyFields skipping: {field.Name}");
                        continue;
                    }
                }
                field.SetValue(destination, field.GetValue(source));
            }
        }
    }
}