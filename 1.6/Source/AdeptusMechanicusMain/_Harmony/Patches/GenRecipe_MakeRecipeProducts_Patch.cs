using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace AdeptusMechanicus.HarmonyInstance
{
    /*
    [HarmonyPatch(typeof(GenRecipe), nameof(GenRecipe.MakeRecipeProducts))]
    [HarmonyPatch(MethodType.Normal)]
    public static class GenRecipe_MakeRecipeProducts_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref IEnumerable<Thing> __result, RecipeDef recipeDef, Pawn worker,
            List<Thing> ingredients)
        {
            if (recipeDef.defName.Contains("OG_Recipe_ReclaimBionics"))
            {
                float skillChance = worker.GetStatValue(StatDefOf.MedicalSurgerySuccessChance);

                List<Thing> result = __result as List<Thing> ?? __result.ToList();
                foreach (Corpse corpse in ingredients.OfType<Corpse>())
                    result.AddRange(
                        NewMedicalRecipesUtility.TraverseBody(recipeSettings, corpse, skillChance));

                if (recipeDef.Equals(AutopsyRecipeDefs.AutopsyBasic))
                {
                    worker.needs?.mood?.thoughts?.memories?.TryGainMemory(AutopsyRecipeDefs.HarvestedHumanlikeCorpse, null);
                    foreach (Pawn pawn in worker.Map.mapPawns.SpawnedPawnsInFaction(worker.Faction))
                        if (pawn != worker)
                            pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(
                                AutopsyRecipeDefs.KnowHarvestedHumanlikeCorpse, null);
                }

                __result = result;
            }
        }
    }
    */
}