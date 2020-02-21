using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Harmony;
using RimWorld;
using Verse;

namespace Momu
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            var harmony = HarmonyInstance.Create("rimworld.ogliss.momu.harmony");
            var type = typeof(HarmonyPatches);
            
            harmony.Patch(
                original: AccessTools.Method(type: typeof(FoodUtility), name: "AddFoodPoisoningHediff"),
                prefix: new HarmonyMethod(type: type, name: nameof(Pre_AddFoodPoisoningHediff_CompCheck)),
                postfix: null);

            harmony.Patch(
                AccessTools.Method(typeof(ApparelUtility), "HasPartsToWear", new[] { typeof(Pawn), typeof(ThingDef) }), null,
                new HarmonyMethod(type, nameof(Post_HasPartsToWear_BodyTypeRestriction)));

            harmony.Patch(
                AccessTools.Method(typeof(FoodUtility), "WillEat", new[] { typeof(Pawn), typeof(Thing), typeof(Pawn) }), null,
                new HarmonyMethod(type, nameof(Post_WillEat_IngredientRestriction)));
            
        }

        public static void Post_WillEat_IngredientRestriction(Pawn p, Thing food, Pawn getter, ref bool __result)
        {
            bool flag = (Find.Selector.SingleSelectedThing == p || Find.Selector.SingleSelectedThing == getter) && Prefs.DevMode && DebugSettings.godMode;
            if (p != null)
            {
                if (getter != null)
                {
                    if (food != null)
                    {
                        if (p.def.defName.Contains("Momu"))
                        {
                            if (food.def.defName.Contains("Meal"))
                            {
                                CompIngredients ingredients = food.TryGetComp<CompIngredients>();
                                if (ingredients != null)
                                {
                                    if (ingredients.ingredients.All(ingred => ingred.ingestible.foodType == FoodTypeFlags.VegetableOrFruit || ingred.ingestible.foodType == FoodTypeFlags.VegetarianAnimal || ingred.ingestible.foodType == FoodTypeFlags.VegetarianRoughAnimal || ingred.ingestible.foodType == FoodTypeFlags.Seed || ingred.ingestible.foodType == FoodTypeFlags.Plant))
                                    {
                                        __result = true;
                                    }
                                    else
                                    {
                                        __result = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void Post_HasPartsToWear_BodyTypeRestriction(Pawn p, ThingDef apparel, ref bool __result)
        {
            if (apparel.HasComp(typeof(CompApparelBodyRestriction)))
            {
                CompProperties_ApparelBodyRestriction bodyRestriction = apparel.GetCompProperties<CompProperties_ApparelBodyRestriction>();
                if (bodyRestriction != null)
                {
                    if (!bodyRestriction.AllowedBodyTypes.Contains(p.story.bodyType))
                    {
                        __result = false;
                    }
                }
            }
            return;
        }

        public static bool Pre_AddFoodPoisoningHediff_CompCheck(Pawn pawn, Thing ingestible, FoodPoisonCause cause)
        {
            CompFoodPoisonProtection compFood = pawn.TryGetComp<CompFoodPoisonProtection>();
            if (compFood != null)
            {
                if (!compFood.Props.Poisonable)
                {
                    return false;
                }
                if (!compFood.Props.FoodTypeFlags.NullOrEmpty<FoodTypeFlags>())
                {
                    foreach (var ftf in compFood.Props.FoodTypeFlags)
                    {
                        if (ftf == ingestible.def.ingestible.foodType)
                        {
                            return false;
                        }
                    }
                }
                if (!compFood.Props.FoodPoisonCause.NullOrEmpty<FoodPoisonCause>())
                {
                    foreach (var fpc in compFood.Props.FoodPoisonCause)
                    {
                        if (fpc == cause)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

    }

}