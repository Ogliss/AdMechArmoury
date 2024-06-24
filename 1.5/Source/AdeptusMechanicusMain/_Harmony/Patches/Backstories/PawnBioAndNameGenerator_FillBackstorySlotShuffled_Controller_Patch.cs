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
using AlienRace;

namespace AdeptusMechanicus.HarmonyInstance
{
        [HarmonyPatch(typeof(PawnBioAndNameGenerator), "FillBackstorySlotShuffled")]
    public static class PawnBioAndNameGenerator_FillBackstorySlotShuffled_Controller_Patch
    {
    //    [HarmonyPrefix]
        public static bool Prefix(Pawn pawn, BackstorySlot slot, List<BackstoryCategoryFilter> backstoryCategories, FactionDef factionType, BackstorySlot? mustBeCompatibleTo)
        {
            bool act = pawn.def.modContentPack != null && pawn.def.modContentPack.Name.Contains("Adeptus Mechanicus");
            if (act || pawn.def.defName.StartsWith("OG_") || pawn.kindDef.defName.StartsWith("OG_") || pawn.kindDef.defName.Contains("_OG_"))
            {
                return FillBackstorySlotShuffled(pawn, slot, backstoryCategories, factionType, mustBeCompatibleTo);
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

                StringBuilder s = new StringBuilder($"Potential {slot} BackstoryCategoryFilters for {pawn} of {pawn.Faction}");
                foreach (var item in backstoryCategories)
                {
                    LogContents(slot, item,ref s);
                }
            //    Log.Message($"{slot} BackstoryCategoryFilters {backstoryCategories.SelectMany(x=> x.categories, y=> y.categoriesAdulthood)} for {pawn} of {pawn.Faction}");
                BackstoryCategoryFilter categoryFilter = backstoryCategories.RandomElementByWeight((c) => c.commonality) ?? FallbackCategoryGroup;
                s.AppendLine();
                s.AppendLine($"Chosen {slot} Id:{backstoryCategories.IndexOf(categoryFilter)} categoryFilter for {pawn} of {pawn.Faction}");
                LogContents(slot, categoryFilter,ref s);

                IEnumerable<RimWorld.BackstoryDef> source = DefDatabase<RimWorld.BackstoryDef>.AllDefs.Where((bs) => bs.shuffleable && categoryFilter.Matches(bs));
                //   source.Concat(DefDatabase<BackstoryDef>.AllDefs.Where((bs) => bs.shuffleable && categoryFilter.Matches(bs) && bs.Approved(pawn) && (slot != BackstorySlot.Adulthood || bs.linkedBackstory == null || pawn.story.Childhood == bs.linkedBackstory)));
                s.AppendLine($"there are {source.Count()} suitable backstories for {slot} in this catergory");
                if (AdeptusIntergrationUtility.enabled_AlienRaces)
                {
                    AlienBackstoryUtility.alienBackstories(categoryFilter, ref source, pawn, slot);
                }
                if (source.EnumerableNullOrEmpty())
                {
                    string s2 = categoryFilter.categories.NullOrEmpty() ? slot == BackstorySlot.Adulthood ? $"Adulthoods: {categoryFilter.categoriesAdulthood.ToCommaList()}" : $"Childhoods: {categoryFilter.categoriesChildhood.ToCommaList()}" : $"Catergories: {categoryFilter.categories.ToCommaList()}";
                    Log.Warning($"No {slot} backstories matching categoryFilter {s2} for {pawn} of {pawn.Faction}");
                }
                PawnBioAndNameGenerator.tmpBackstories.Clear();
                if (mustBeCompatibleTo == null)
                {
                    PawnBioAndNameGenerator.tmpBackstories.AddRange(from bs in source
                                                                    where bs.slot == slot
                                                                    select bs);
                }
                else
                {
                    IEnumerable<RimWorld.BackstoryDef> compatibleBackstories = from bs in source
                                                                      where bs.slot == mustBeCompatibleTo.Value
                                                                      select bs;
                    PawnBioAndNameGenerator.tmpBackstories.AddRange(from bs in source
                                                                    where bs.slot == slot && compatibleBackstories.Any((RimWorld.BackstoryDef b) => !b.requiredWorkTags.OverlapsWithOnAnyWorkType(bs.workDisables))
                                                                    select bs);
                }
                s.AppendLine($"there are now {PawnBioAndNameGenerator.tmpBackstories.Count()} suitable backstories for {slot} in tmpBackstories");
               
                if (PawnBioAndNameGenerator.tmpBackstories.NullOrEmpty())
                {
                    string s2 = categoryFilter.categories.NullOrEmpty() ? slot == BackstorySlot.Adulthood ? $"Adulthoods: {categoryFilter.categoriesAdulthood.ToCommaList()}" : $"Childhoods: {categoryFilter.categoriesChildhood.ToCommaList()}" : $"Catergories: {categoryFilter.categories.ToCommaList()}";
                    Log.Warning($"No {slot} tmpBackstories matching categoryFilter {s2} for {pawn} of {pawn.Faction}" + (mustBeCompatibleTo.HasValue ? $" Compatible to: {mustBeCompatibleTo.Value}" : ""));
                }
                List<RimWorld.BackstoryDef> list = PawnBioAndNameGenerator.tmpBackstories.Where(bs => slot != BackstorySlot.Adulthood || bs.requiredWorkTags == WorkTags.None || !bs.requiredWorkTags.OverlapsWithOnAnyWorkType(pawn.story.Childhood.workDisables)).ToList();
                s.AppendLine($"there are now {list.Count()} suitable backstories for {slot} in list");
                if (list.NullOrEmpty() && !PawnBioAndNameGenerator.tmpBackstories.NullOrEmpty())
                {
                    s.AppendLine($"list empty for {slot} checking for explanation!!");
                    foreach (var item in PawnBioAndNameGenerator.tmpBackstories)
                    {
                        bool canUse = false;
                        string why ="Not valid";
                        if (slot != BackstorySlot.Adulthood)
                        {
                            canUse = true;
                            why = "slot is not Adulthood";
                        }
                        if (item.requiredWorkTags == WorkTags.None)
                        {
                            canUse = true;
                            why = "bs has no required work tages";

                        }
                        if (!item.requiredWorkTags.OverlapsWithOnAnyWorkType(pawn.story.Childhood.workDisables))
                        {
                            canUse = true;
                            why = $"bs has no conflicting required work tags with {pawn.story.Childhood}";
                        }
                        s.AppendLine($"Can use use {item.defName}: {canUse} : {why}");
                    }
                }
                /*
                foreach (var item in list)
                {
                    s.AppendLine($"    Backstory: {item.defName} Weight: {PawnBioAndNameGenerator.BackstorySelectionWeight(item)}");
                }
                */
                LogMessage(s.ToString());
                RimWorld.BackstoryDef backstoryDef;
                if (!(from bs in PawnBioAndNameGenerator.tmpBackstories.TakeRandom(20)
                      where slot != BackstorySlot.Adulthood || bs.requiredWorkTags == WorkTags.None || !bs.requiredWorkTags.OverlapsWithOnAnyWorkType(pawn.story.Childhood.workDisables)
                      select bs).TryRandomElementByWeight(new Func<RimWorld.BackstoryDef, float>(PawnBioAndNameGenerator.BackstorySelectionWeight), out backstoryDef))
                {
                    Log.Error(string.Concat(new object[]
                    {
                    "ADEPTUS: No shuffled ",
                    slot,
                    " found for ",
                    pawn.ToStringSafe<Pawn>(),
                    " of ",
                    factionType.ToStringSafe<FactionDef>(),
                    ". Choosing random."
                    }));

                    return true;
                    /*
                    backstoryDef = (from bs in DefDatabase<RimWorld.BackstoryDef>.AllDefs
                                    where bs.slot == slot
                                    select bs).RandomElement<RimWorld.BackstoryDef>();
                    */
                }
                if (slot == BackstorySlot.Adulthood)
                {
                    pawn.story.Adulthood = backstoryDef;
                }
                else
                {
                    pawn.story.Childhood = backstoryDef;
                }
                /*
                if (!(from bs in PawnBioAndNameGenerator.tmpBackstories.TakeRandom(20)
                      where slot != BackstorySlot.Adulthood || bs.requiredWorkTags == WorkTags.None || !bs.requiredWorkTags.OverlapsWithOnAnyWorkType(pawn.story.Childhood.workDisables) ? true : false
                      select bs).TryRandomElementByWeight(x => PawnBioAndNameGenerator.BackstorySelectionWeight(x), out var result))
                {
                    string s2 = categoryFilter.categories.NullOrEmpty() ? slot == BackstorySlot.Adulthood ? $": {categoryFilter.categoriesAdulthood.ToCommaList()}" : $": {categoryFilter.categoriesChildhood.ToCommaList()}" : $": {categoryFilter.categories.ToCommaList()}";
                    Log.Error(string.Concat("No shuffled ", slot, " backstories matching categoryFilter", s2, " found for ", pawn.ToStringSafe(), " of ", factionType.ToStringSafe(), ". Choosing random."));
                    result = DefDatabase<RimWorld.BackstoryDef>.AllDefs.Where((bs) => bs.slot == slot && categoryFilter.Matches(bs)).RandomElement();
                }
                if (slot == BackstorySlot.Adulthood)
                {
                    pawn.story.Adulthood = result;
                }
                else
                {
                    pawn.story.Childhood = result;
                }
                */
                PawnBioAndNameGenerator.tmpBackstories.Clear();
                return false;
            }
            return true;
        }

        public static void LogContents(BackstorySlot slot, BackstoryCategoryFilter filter, ref StringBuilder s)
        {
            Type filterType = typeof(BackstoryCategoryFilter);

            s.Append("\n");
            // List of field names to log
            List<string> fieldNames = new List<string>
                {
                    "categories",
                    "exclude",
                    "categoriesChildhood",
                    "excludeChildhood",
                    "categoriesAdulthood",
                    "excludeAdulthood"
                };
            foreach (string fieldName in fieldNames)
            {
                FieldInfo fieldInfo = filterType.GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);
                if (fieldInfo != null)
                {
                    List<string> list = fieldInfo.GetValue(filter) as List<string>;
                    if (list != null)
                    {
                        s.AppendLine($"    {fieldName}:");
                        foreach (string item in list)
                        {
                            s.AppendLine($"        - {item}");
                        }
                    }
                }
            }
        }

        private static void LogMessage(string message)
        {
            // Assuming Verse.Log is the logging utility in the RimWorld modding framework
            Log.Message(message);
        }

        // Token: 0x060040BF RID: 16575 RVA: 0x00159374 File Offset: 0x00157574
        public static List<BackstoryCategoryFilter> GetBackstoryCategoryFiltersFor(Pawn pawn, FactionDef faction)
        {
            if (!pawn.kindDef.backstoryFiltersOverride.NullOrEmpty())
            {
                return pawn.kindDef.backstoryFiltersOverride;
            }
            List<BackstoryCategoryFilter> list = new List<BackstoryCategoryFilter>();
            if (pawn.kindDef.backstoryFilters != null)
            {
                list.AddRange(pawn.kindDef.backstoryFilters);
            }
            if (faction != null && !faction.backstoryFilters.NullOrEmpty())
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
            if (!list.NullOrEmpty())
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
                FallbackCategoryGroup
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
            return SelectionWeightFactorFromWorkTagsDisabled(bs.Value.workDisables);
        }
        public static float BackstorySelectionWeight(BackstoryDef bs)
        {
            return SelectionWeightFactorFromWorkTagsDisabled(bs.workDisables);
        }

        // Token: 0x06001504 RID: 5380 RVA: 0x000A3BA2 File Offset: 0x000A1FA2
        public static float BioSelectionWeight(PawnBio bio)
        {
            return SelectionWeightFactorFromWorkTagsDisabled(bio.adulthood.workDisables | bio.childhood.workDisables);
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
