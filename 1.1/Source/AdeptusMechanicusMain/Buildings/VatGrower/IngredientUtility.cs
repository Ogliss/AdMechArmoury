using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using Verse.AI;

namespace AdeptusMechanicus
{
    /// <summary>
    /// Utility functions for dealing with ingredients.
    /// </summary>
    public static class IngredientUtility
    {
        public static Thing FindClosestRequestForThingOrderProcessor(ThingOrderProcessor orderProcessor, Pawn finder)
        {
            Thing result = GenClosest.ClosestThingReachable(finder.Position, finder.Map, ThingRequest.ForGroup(ThingRequestGroup.HaulableEver), PathEndMode.OnCell, TraverseParms.For(finder),
                    validator:
                    delegate (Thing thing)
                    {
                        if (thing.IsForbidden(finder))
                        {
                            return false;
                        }

                        foreach (ThingOrderRequest request in orderProcessor.PendingRequests)
                        {
                            if (request.ThingMatches(thing))
                            {
                                return true;
                            }
                        }

                        return false;
                    });

            return result;
        }

        public static string FormatIngredientsInThingOrderProcessor(this ThingOrderProcessor orderProcessor, string format = "{0} x {1}", char delimiter = ',')
        {
            StringBuilder builder = new StringBuilder();

            //Ingredients Wanted
            foreach (ThingOrderRequest ingredient in orderProcessor.desiredIngredients)
            {
                builder.Append(string.Format(format, ingredient.LabelCap, ingredient.amount));
                if (orderProcessor.desiredIngredients.Count > 1 && ingredient != orderProcessor.desiredIngredients.Last())
                {
                    builder.Append(delimiter);
                    builder.Append(' ');
                }
            }

            return builder.ToString().TrimEndNewlines();
        }

        public static string FormatCachedIngredientsInThingOrderProcessor(this ThingOrderProcessor orderProcessor, string format = "{0} x {1}", char delimiter = ',')
        {
            StringBuilder builder = new StringBuilder();

            //Ingredients Requested
            foreach (ThingOrderRequest request in orderProcessor.PendingRequests)
            {
                builder.Append(string.Format(format, request.LabelCap, request.amount));
                if (orderProcessor.PendingRequests.Count() > 1 && request != orderProcessor.PendingRequests.Last())
                {
                    builder.Append(delimiter);
                    builder.Append(' ');
                }
            }

            return builder.ToString().TrimEndNewlines();
        }

        public static string FormatIngredientsInThingOwner(this ThingOwner thingOwner, string format = "{0} x {1}", char delimiter = ',')
        {
            StringBuilder builder = new StringBuilder();

            //Ingredients Wanted
            foreach (Thing ingredient in thingOwner)
            {
                builder.Append(string.Format(format, ingredient.LabelCapNoCount, ingredient.stackCount));
                if (thingOwner.Count > 1 && ingredient != thingOwner.Last())
                {
                    builder.Append(delimiter);
                    builder.Append(' ');
                }
            }

            return builder.ToString().TrimEndNewlines();
        }

        public static void FillOrderProcessorFromVatGrowerRecipe(ThingOrderProcessor orderProcessor, GrowerRecipeDef recipeDef)
        {
            foreach (IngredientCount ingredientCount in recipeDef.ingredients)
            {
                ThingFilter filterCopy = new ThingFilter();
                filterCopy.CopyAllowancesFrom(ingredientCount.filter);

                ThingOrderRequest copy = new ThingOrderRequest(filterCopy);
                copy.amount = (int)ingredientCount.GetBaseCount();

                orderProcessor.desiredIngredients.Add(copy);
            }
        }

        public static void FillOrderProcessorFromPawnKindDef(ThingOrderProcessor orderProcessor, PawnKindDef pawnKind)
        {
            ThingDef pawnThingDef = pawnKind.race;

            RaceProperties raceProps = pawnThingDef.race;

            //Assemble all required materials for a fully grown adult.
            //Meat is worth the full nutritional value.
            ThingDef meatDef = raceProps.meatDef;
            float meatBaseNutrition = meatDef.ingestible.CachedNutrition;
            float meatAmount = pawnThingDef.GetStatValueAbstract(StatDefOf.MeatAmount) * raceProps.baseBodySize;

            //Leather is worth half of the meat value.
            float leatherAmount = pawnThingDef.GetStatValueAbstract(StatDefOf.LeatherAmount) * raceProps.baseBodySize;

            int proteinAmount = (int)(meatAmount * meatBaseNutrition * 15f) + (int)(leatherAmount * meatBaseNutrition * 15f);
            int nutritionAmount = (int)(proteinAmount * 4.5f);

            {
                ThingOrderRequest orderRequest = new ThingOrderRequest(OGVatThingDefOf.OG_ProteinMash, proteinAmount);
                orderProcessor.desiredIngredients.Add(orderRequest);
            }

            {
                ThingOrderRequest orderRequest = new ThingOrderRequest(OGVatThingDefOf.OG_NutrientSolution, nutritionAmount);
                orderProcessor.desiredIngredients.Add(orderRequest);
            }
        }
    }
}
