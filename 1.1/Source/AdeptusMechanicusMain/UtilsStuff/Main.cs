using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using AdeptusMechanicus;

namespace AdeptusMechanicus
{
    [StaticConstructorOnStartup]
    public class ArmouryMain
    {
        public static IEnumerable<ThingDef> TechHediffItems; 
        public static IEnumerable<RecipeDef> TechHediffRecipes; 
        static ArmouryMain()
        {
            /*
            Log.Message("AppDomain.CurrentDomain.GetAssemblies():\n" + System.AppDomain.CurrentDomain.GetAssemblies().Join(delimiter: "\n"));
            Log.Message("GenTypes.AllActiveAssemblies:\n" + Traverse.Create(typeof(GenTypes)).Property<IEnumerable<System.Reflection.Assembly>>("AllActiveAssemblies").Value.Join(delimiter: "\n"));
            */
            //    Log.Message("ArmouryMain ");
            if (DefDatabase<ScenarioDef>.AllDefs.Any(x=> x.defName.Contains("OGAM_TestScenario_")))
            {
                List<ScenarioDef> scenariosTesting = DefDatabase<ScenarioDef>.AllDefs.Where(x => x.defName.StartsWith("OGAM_TestScenario_")).ToList();
                foreach (ScenarioDef ScenDef in scenariosTesting)
                {
                    if (ScenDef.defName.Contains("Imperial"))
                    {
                        TryAddWeaponsStartingThingToTestScenario(ScenDef, "I");
                        TryAddWeaponsStartingThingToTestScenario(ScenDef, "IG");
                    }
                    else if (ScenDef.defName.Contains("Mechanicus"))
                    {
                        TryAddWeaponsStartingThingToTestScenario(ScenDef, "AM");
                    }
                    else if (ScenDef.defName.Contains("Astartes"))
                    {
                        TryAddWeaponsStartingThingToTestScenario(ScenDef, "AA");
                    }
                    else if (ScenDef.defName.Contains("Chaos"))
                    {
                        TryAddWeaponsStartingThingToTestScenario(ScenDef, "C");
                    }
                    else if (ScenDef.defName.Contains("Eldar") && !ScenDef.defName.Contains("DarkEldar"))
                    {
                        TryAddWeaponsStartingThingToTestScenario(ScenDef, "E");
                    }
                    else if (ScenDef.defName.Contains("DarkEldar"))
                    {
                        TryAddWeaponsStartingThingToTestScenario(ScenDef, "DE");
                    }
                    else if (ScenDef.defName.Contains("Tau"))
                    {
                        TryAddWeaponsStartingThingToTestScenario(ScenDef, "T");
                        TryAddWeaponsStartingThingToTestScenario(ScenDef, "K");
                    }
                    else if (ScenDef.defName.Contains("Ork"))
                    {
                        TryAddWeaponsStartingThingToTestScenario(ScenDef, "O");
                    }
                    else if (ScenDef.defName.Contains("Necron"))
                    {
                        TryAddWeaponsStartingThingToTestScenario(ScenDef, "N");
                    }
                    else if (ScenDef.defName.Contains("Tyranid"))
                    {
                        TryAddWeaponsStartingThingToTestScenario(ScenDef, "TY");
                    }
                }
            }
            if (AdeptusIntergrationUtil.enabled_CombatExtended)
            {

                if (DefDatabase<ScenarioDef>.AllDefs.Any(x => x.defName.StartsWith("OGAM_Scenario_")))
                {
                    foreach (ScenarioDef ScenDef in DefDatabase<ScenarioDef>.AllDefs.Where(x => x.defName.StartsWith("OGAM_Scenario_")))
                    {
                        TryAddCEAmmoScenario(ScenDef);
                    }
                }
            }
            //    Log.Message("ArmouryMain 0");
            ThingDef mechanicus = DefDatabase<ThingDef>.GetNamedSilentFail("OG_Human_Mechanicus");
            ThingDef astartes = DefDatabase<ThingDef>.GetNamedSilentFail("OG_Human_Astartes");
            ThingDef ogryn = DefDatabase<ThingDef>.GetNamedSilentFail("OG_Abhuman_Ogryn");
            ThingDef ratlin = DefDatabase<ThingDef>.GetNamedSilentFail("OG_Abhuman_Ratling");
            ThingDef beastman = DefDatabase<ThingDef>.GetNamedSilentFail("OG_Abhuman_Beastman");
            //    Log.Message("ArmouryMain 1");
            IEnumerable<RecipeDef> humanRecipes = DefDatabase<RecipeDef>.AllDefs.Where(x=> x.AllRecipeUsers.Contains(ThingDefOf.Human));
            foreach (RecipeDef item in humanRecipes)
            {
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
            TechHediffItems = from x in DefDatabase<ThingDef>.AllDefs
                     where x.isTechHediff && x.BaseMarketValue > 0
                     select x;
            //    Log.Message("ArmouryMain 3");
            TechHediffRecipes = from x in DefDatabase<RecipeDef>.AllDefs
                                          where TechHediffItems.Any(z=> x.IsIngredient(z)) && x.targetsBodyPart
                                          select x;
            //    Log.Message("ArmouryMain 4");
        }

        public static Listing_Standard BeginSection_OnePointTwo(ref Listing_Standard listing_Main, float f)
        {
            return listing_Main.BeginSection_NewTemp(f);
        }
        public static Listing_Standard BeginSection_OnePointOne(ref Listing_Standard listing_Main, float f)
        {
            return listing_Main.BeginSection_NewTemp(f);
        }
        private static void TryAddWeaponsStartingThingToTestScenario(ScenarioDef ScenDef, string Tag)
        {
            List<ThingDef> things = DefDatabase<ThingDef>.AllDefsListForReading.FindAll(x => (x.PlayerAcquirable && (x.IsWeapon || x.IsApparel) &&  x.defName.Contains("OG" + Tag) && !x.defName.Contains("TOGGLEDEF_S")));
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
                    }
                }
                if (!hasweapon)
                {
                    ScenPart_StartingThing_Defined _Defined = new ScenPart_StartingThing_Defined() { def = ScenPartDefOf.StartingThing_Defined };
                    Traverse.Create(_Defined).Field("thingDef").SetValue(Weapon);
                    if (Weapon.MadeFromStuff)
                    {
                        ThingDef stuffdef = DefDatabase<ThingDef>.AllDefsListForReading.Where(x => x.IsStuff && Weapon.stuffCategories.Any(y=> x.stuffProps.categories.Contains(y))).RandomElement();
                        if (stuffdef!=null)
                        {
                            Traverse.Create(_Defined).Field("stuff").SetValue(stuffdef);
                        }
                    }
                    parts.Add(_Defined);
                    if (AdeptusIntergrationUtil.enabled_CombatExtended && Weapon.IsRangedWeapon)
                    {
                        AddAmmoCE(Weapon, ref parts, -1, 1, false); // Prefs.DevMode
                    }
                }
            }
        }
        private static void TryAddCEAmmoScenario(ScenarioDef ScenDef)
        {
            Log.Message("Trying to add ammo to Scenario " + ScenDef);
            List<ScenPart> parts = Traverse.Create(ScenDef.scenario).Field("parts").GetValue<List<ScenPart>>().Where(x => x.def == ScenPartDefOf.StartingThing_Defined).ToList();
            List<Pair<ThingDef, int>> guns = new List<Pair<ThingDef, int>>();
            foreach (ScenPart scenpart in parts)
            {
                ThingDef x = Traverse.Create(scenpart).Field("thingDef").GetValue<ThingDef>();
                int mult = Traverse.Create(scenpart).Field("count").GetValue<int>();
                if (x.PlayerAcquirable && x.IsWeapon && x.defName.StartsWith("OG"))
                {
                    if (x.IsRangedWeapon)
                    {
                        guns.Add(new Pair<ThingDef, int>(x, mult));
                    }
                }
            }
            foreach (var x in guns)
            {
                AddAmmoCE(x.First, ref parts, 100, x.Second, true);
            }

        }

        public static void AddAmmoCE(ThingDef Weapon, ref List<ScenPart> parts, int count = -1, int mult = 1, bool logging = false)
        {
            if (Weapon.HasComp(typeof(CombatExtended.CompAmmoUser)))
            {
                if (logging) Log.Message("Trying to add ammo for " + Weapon);
                CombatExtended.CompProperties_AmmoUser ammo = Weapon.GetCompProperties<CombatExtended.CompProperties_AmmoUser>();
                foreach (var item in ammo.ammoSet.ammoTypes)
                {
                    CombatExtended.AmmoDef ammoDef = item.ammo;
                    if (count == -1)
                    {
                        bool hasammo = false;
                        foreach (ScenPart scenpart in parts.Where(x => x.def == ScenPartDefOf.StartingThing_Defined))
                        {
                            ThingDef td = Traverse.Create(scenpart).Field("thingDef").GetValue<ThingDef>();
                            if (td == ammoDef)
                            {
                                hasammo = true;
                                break;
                            }
                        }
                        if (hasammo)
                        {
                            continue;
                        }
                    }
                    for (int i = 0; i < mult; i++)
                    {
                        int c = count > 0 && count < item.ammo.stackLimit ? count : item.ammo.stackLimit;
                        if (logging) Log.Message("Trying to add " + ammoDef + " X " + c);
                        ScenPart_StartingThing_Defined _Defined = new ScenPart_StartingThing_Defined() { def = ScenPartDefOf.StartingThing_Defined };
                        Traverse.Create(_Defined).Field("thingDef").SetValue(ammoDef);
                        Traverse.Create(_Defined).Field("count").SetValue(c);
                        if (ammoDef.MadeFromStuff)
                        {
                            ThingDef stuffdef = DefDatabase<ThingDef>.AllDefsListForReading.Where(x => x.IsStuff && ammoDef.stuffCategories.Any(y => x.stuffProps.categories.Contains(y))).RandomElement();
                            if (stuffdef != null)
                            {
                                Traverse.Create(_Defined).Field("stuff").SetValue(stuffdef);
                            }
                        }
                        parts.Add(_Defined);
                        if (count != -1)
                        {
                            break;
                        }
                    }
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