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
    class ArmouryMain
    {
        public static IEnumerable<ThingDef> TechHediffItems; 
        public static IEnumerable<RecipeDef> TechHediffRecipes; 
        static ArmouryMain()
        {
            //    Log.Message("ArmouryMain ");
            if (DefDatabase<ScenarioDef>.AllDefs.Any(x=> x.defName.Contains("OGAM_TestScenario_")))
            {
                foreach (ScenarioDef ScenDef in DefDatabase<ScenarioDef>.AllDefs.Where(x => x.defName.Contains("OGAM_TestScenario_")))
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
            //    Log.Message("ArmouryMain 0");
            ThingDef mechanicus = DefDatabase<ThingDef>.GetNamedSilentFail("OG_Human_Mechanicus");
            ThingDef astartes = DefDatabase<ThingDef>.GetNamedSilentFail("OG_Human_Astartes");
            ThingDef ogryn = DefDatabase<ThingDef>.GetNamedSilentFail("OG_Abhuman_Ogryn");
            ThingDef ratlin = DefDatabase<ThingDef>.GetNamedSilentFail("OG_Abhuman_Ratling");
            ThingDef beastman = DefDatabase<ThingDef>.GetNamedSilentFail("OG_Abhuman_Beastman");
            //    Log.Message("ArmouryMain 1");
            foreach (RecipeDef item in DefDatabase<RecipeDef>.AllDefs)
            {
                if (item.AllRecipeUsers.Contains(ThingDefOf.Human))
                {
                    if (mechanicus!=null)
                    {
                    //    Log.Message("ArmouryMain 1 mechanicus");
                        if (!item.AllRecipeUsers.Contains(mechanicus))
                        {
                            item.recipeUsers.Add(mechanicus);
                        }
                    }
                    if (astartes != null)
                    {
                    //    Log.Message("ArmouryMain 1 astartes");
                        if (!item.AllRecipeUsers.Contains(astartes))
                        {
                            item.recipeUsers.Add(astartes);
                        }
                    }
                    if (ogryn != null)
                    {
                        //    Log.Message("ArmouryMain 1 ogryn");
                        if (!item.AllRecipeUsers.Contains(ogryn))
                        {
                            item.recipeUsers.Add(ogryn);
                        }
                    }
                    if (ratlin != null)
                    {
                        //    Log.Message("ArmouryMain 1 ratlin");
                        if (!item.AllRecipeUsers.Contains(ratlin))
                        {
                            item.recipeUsers.Add(ratlin);
                        }
                    }
                    if (beastman != null)
                    {
                        //    Log.Message("ArmouryMain 1 beastman");
                        if (!item.AllRecipeUsers.Contains(beastman))
                        {
                            item.recipeUsers.Add(beastman);
                        }
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

        private static void TryAddWeaponsStartingThingToTestScenario(ScenarioDef ScenDef, string Tag)
        {
            List<ThingDef> things = DefDatabase<ThingDef>.AllDefsListForReading.FindAll(x => (x.defName.Contains("OG" + Tag + "_Gun_") || x.defName.Contains("OG" + Tag + "_Melee_") || x.defName.Contains("OG" + Tag + "_Apparel_") || x.defName.Contains("OG" + Tag + "_Armour_") || x.defName.Contains("OG" + Tag + "_Wargear_") || x.defName.Contains("OG" + Tag + "_GrenadePack_")) && (!x.defName.Contains("TOGGLEDEF_") || x.defName.Contains("TOGGLEDEF_S")));

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
                }
            }
        }


    }

}