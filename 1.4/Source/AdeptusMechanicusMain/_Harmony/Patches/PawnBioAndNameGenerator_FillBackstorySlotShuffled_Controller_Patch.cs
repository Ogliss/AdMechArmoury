using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using Verse.Sound;
using System.Reflection;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(PawnBioAndNameGenerator), "FillBackstorySlotShuffled")]
    public static class PawnBioAndNameGenerator_FillBackstorySlotShuffled_Controller_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Pawn pawn, BackstorySlot slot, List<BackstoryCategoryFilter> backstoryCategories, FactionDef factionType, BackstorySlot? mustBeCompatibleTo)
        {
            bool act = pawn.def.modContentPack != null && pawn.def.modContentPack.Name.Contains("Adeptus Mechanicus");
            if (act || pawn.def.defName.StartsWith("OG_") || pawn.kindDef.defName.StartsWith("OG_") || pawn.kindDef.defName.Contains("_OG_"))
            {
                FillBackstorySlotShuffled(pawn, slot, backstoryCategories, factionType, mustBeCompatibleTo);
                return false;
            }
            return true;
        }
        public static bool FillBackstorySlotShuffled(Pawn pawn, BackstorySlot slot, List<BackstoryCategoryFilter> backstoryCategories, FactionDef factionType, BackstorySlot? mustBeCompatibleTo)
        {
            bool act = pawn.def.modContentPack != null && pawn.def.modContentPack.Name.Contains("Adeptus Mechanicus");
            if (act || pawn.def.defName.StartsWith("OG_") || pawn.kindDef.defName.StartsWith("OG_") || pawn.kindDef.defName.Contains("_OG_"))
            {
                /*
                BackstoryCategoryFilter backstoryCategoryFilter = backstoryCategories.RandomElementByWeight((BackstoryCategoryFilter c) => c.commonality);
                if (backstoryCategoryFilter == null)
                {

                    backstoryCategoryFilter = PawnBioAndNameGenerator_FillBackstorySlotShuffled_Controller_Patch.FallbackCategoryGroup;
                }
                List<string> lista = new List<string>();
                foreach (BackstoryCategoryFilter filter in backstoryCategories)
                {
                    foreach (string str in filter.categories)
                    {
                        if (!lista.Contains(str))
                        {
                            lista.Add(str);
                        }
                    }
                }
                */

                BackstoryCategoryFilter categoryFilter = backstoryCategories.RandomElementByWeight((BackstoryCategoryFilter c) => c.commonality);
                if (categoryFilter == null)
                {
                    categoryFilter = FallbackCategoryGroup;
                }
                IEnumerable<RimWorld.BackstoryDef> source = DefDatabase<RimWorld.BackstoryDef>.AllDefs.Where((RimWorld.BackstoryDef bs) => bs.shuffleable && categoryFilter.Matches(bs));
                source.Concat(DefDatabase<BackstoryDef>.AllDefs.Where((BackstoryDef bs) => bs.shuffleable && categoryFilter.Matches(bs)));
                if (AdeptusIntergrationUtility.enabled_AlienRaces)
                {
                    alienBackstories(categoryFilter, ref source);
                }
                if (source.EnumerableNullOrEmpty())
                {
                    string s = categoryFilter.categories.NullOrEmpty() ? (slot == BackstorySlot.Adulthood ? $"Adulthoods: {categoryFilter.categoriesAdulthood.ToCommaList()}" : $"Childhoods: {categoryFilter.categoriesChildhood.ToCommaList()}") : $"Catergories: {categoryFilter.categories.ToCommaList()}";
                    Log.Warning($"No {slot} backstories matching categoryFilter {s} for {pawn} of {pawn.Faction}");
                }
                PawnBioAndNameGenerator.tmpBackstories.Clear();
                if (!mustBeCompatibleTo.HasValue)
                {
                    PawnBioAndNameGenerator.tmpBackstories.AddRange(source.Where((RimWorld.BackstoryDef bs) => bs.slot == slot));
                }
                else
                {
                    IEnumerable<RimWorld.BackstoryDef> compatibleBackstories = source.Where((RimWorld.BackstoryDef bs) => bs.slot == mustBeCompatibleTo.Value);
                    PawnBioAndNameGenerator.tmpBackstories.AddRange(source.Where((RimWorld.BackstoryDef bs) => bs.slot == slot && compatibleBackstories.Any((RimWorld.BackstoryDef b) => !b.requiredWorkTags.OverlapsWithOnAnyWorkType(bs.workDisables))));
                }
                if (PawnBioAndNameGenerator.tmpBackstories.NullOrEmpty())
                {
                    string s = categoryFilter.categories.NullOrEmpty() ? (slot == BackstorySlot.Adulthood ? $"Adulthoods: {categoryFilter.categoriesAdulthood.ToCommaList()}" : $"Childhoods: {categoryFilter.categoriesChildhood.ToCommaList()}") : $"Catergories: {categoryFilter.categories.ToCommaList()}";
                    Log.Warning($"No {slot} tmpBackstories matching categoryFilter {s} for {pawn} of {pawn.Faction}");
                }
                if (!(from bs in PawnBioAndNameGenerator.tmpBackstories.TakeRandom(20)
                      where (slot != BackstorySlot.Adulthood || bs.requiredWorkTags == WorkTags.None || !bs.requiredWorkTags.OverlapsWithOnAnyWorkType(pawn.story.Childhood.workDisables)) ? true : false
                      select bs).TryRandomElementByWeight(x=> PawnBioAndNameGenerator.BackstorySelectionWeight(x), out var result))
                {
                    string s = categoryFilter.categories.NullOrEmpty() ? (slot == BackstorySlot.Adulthood ? $": {categoryFilter.categoriesAdulthood.ToCommaList()}" : $": {categoryFilter.categoriesChildhood.ToCommaList()}") : $": {categoryFilter.categories.ToCommaList()}";
                    Log.Error(string.Concat("No shuffled ", slot, " backstories matching categoryFilter", s, " found for ", pawn.ToStringSafe(), " of ", factionType.ToStringSafe(), ". Choosing random."));
                    result = DefDatabase<RimWorld.BackstoryDef>.AllDefs.Where((RimWorld.BackstoryDef bs) => bs.slot == slot && categoryFilter.Matches(bs)).RandomElement();
                }
                if (slot == BackstorySlot.Adulthood)
                {
                    pawn.story.Adulthood = result;
                }
                else
                {
                    pawn.story.Childhood = result;
                }
                PawnBioAndNameGenerator.tmpBackstories.Clear();
                return false;
            }
            return true;
        }

        public static void alienBackstories(BackstoryCategoryFilter categoryFilter, ref IEnumerable<RimWorld.BackstoryDef> source)
        {
            source.Concat(DefDatabase<AlienRace.AlienBackstoryDef>.AllDefs.Where((AlienRace.AlienBackstoryDef bs) => bs.shuffleable && categoryFilter.Matches(bs)));
        }

        // Token: 0x060040BF RID: 16575 RVA: 0x00159374 File Offset: 0x00157574
        public static List<BackstoryCategoryFilter> GetBackstoryCategoryFiltersFor(Pawn pawn, FactionDef faction)
        {
            if (!pawn.kindDef.backstoryFiltersOverride.NullOrEmpty<BackstoryCategoryFilter>())
            {
                return pawn.kindDef.backstoryFiltersOverride;
            }
            List<BackstoryCategoryFilter> list = new List<BackstoryCategoryFilter>();
            if (pawn.kindDef.backstoryFilters != null)
            {
                list.AddRange(pawn.kindDef.backstoryFilters);
            }
            if (faction != null && !faction.backstoryFilters.NullOrEmpty<BackstoryCategoryFilter>())
            {
                for (int i = 0; i < faction.backstoryFilters.Count; i++)
                {
                    BackstoryCategoryFilter item = faction.backstoryFilters[i];
                    if (!list.Contains(item))
                    {
                        list.Add(item);
                    }
                }
            }
            if (!list.NullOrEmpty<BackstoryCategoryFilter>())
            {
                return list;
            }
            Log.ErrorOnce(string.Concat(new object[]
            {
                "PawnKind ",
                pawn.kindDef,
                " generating with factionDef ",
                faction,
                ": no backstoryCategories in either."
            }), 1871521);
            return new List<BackstoryCategoryFilter>
            {
                PawnBioAndNameGenerator_FillBackstorySlotShuffled_Controller_Patch.FallbackCategoryGroup
            };
        }

        public static readonly BackstoryCategoryFilter FallbackCategoryGroup = new BackstoryCategoryFilter
        {
            categories = new List<string>
            {
                "Civil"
            },
            commonality = 1f
        };

        // Token: 0x06001503 RID: 5379 RVA: 0x000A3B95 File Offset: 0x000A1F95
        public static float BackstorySelectionWeight(KeyValuePair<string, RimWorld.BackstoryDef> bs)
        {
            return PawnBioAndNameGenerator_FillBackstorySlotShuffled_Controller_Patch.SelectionWeightFactorFromWorkTagsDisabled(bs.Value.workDisables);
        }
        public static float BackstorySelectionWeight(BackstoryDef bs)
        {
            return PawnBioAndNameGenerator_FillBackstorySlotShuffled_Controller_Patch.SelectionWeightFactorFromWorkTagsDisabled(bs.workDisables);
        }

        // Token: 0x06001504 RID: 5380 RVA: 0x000A3BA2 File Offset: 0x000A1FA2
        public static float BioSelectionWeight(PawnBio bio)
        {
            return PawnBioAndNameGenerator_FillBackstorySlotShuffled_Controller_Patch.SelectionWeightFactorFromWorkTagsDisabled(bio.adulthood.workDisables | bio.childhood.workDisables);
        }

        // Token: 0x06001505 RID: 5381 RVA: 0x000A3BC0 File Offset: 0x000A1FC0
        public static float SelectionWeightFactorFromWorkTagsDisabled(WorkTags wt)
        {
            float num = 1f;
            if ((wt & WorkTags.ManualDumb) != WorkTags.None)
            {
                num *= 0.4f;
            }
            if ((wt & WorkTags.ManualSkilled) != WorkTags.None)
            {
                num *= 1f;
            }
            if ((wt & WorkTags.Violent) != WorkTags.None)
            {
                num *= 0.5f;
            }
            if ((wt & WorkTags.Caring) != WorkTags.None)
            {
                num *= 0.9f;
            }
            if ((wt & WorkTags.Social) != WorkTags.None)
            {
                num *= 0.5f;
            }
            if ((wt & WorkTags.Intellectual) != WorkTags.None)
            {
                num *= 0.35f;
            }
            if ((wt & WorkTags.Firefighting) != WorkTags.None)
            {
                num *= 0.7f;
            }
            return num;
        }

        //    private static Func<Backstory, float> funcA;
        //    private static Func<PawnBio, float> funcB;

    }

}
