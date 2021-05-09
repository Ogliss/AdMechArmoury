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
        public static bool Prefix(Pawn pawn, BackstorySlot slot, ref Backstory backstory, Backstory backstoryOtherSlot, List<BackstoryCategoryFilter> backstoryCategories, FactionDef factionType)
        {
            bool act = pawn.def.modContentPack != null && pawn.def.modContentPack.Name.Contains("Adeptus Mechanicus");
            if (act || pawn.def.defName.StartsWith("OG_") || pawn.kindDef.defName.StartsWith("OG_") || pawn.kindDef.defName.Contains("_OG_"))
            {
                FillBackstorySlotShuffled(pawn, slot, ref backstory, backstoryOtherSlot, backstoryCategories, factionType);
                return false;
            }
            return true;
        }
        public static bool FillBackstorySlotShuffled(Pawn pawn, BackstorySlot slot, ref Backstory backstory, Backstory backstoryOtherSlot, List<BackstoryCategoryFilter> backstoryCategories, FactionDef factionType)
        {
            bool act = pawn.def.modContentPack != null && pawn.def.modContentPack.Name.Contains("Adeptus Mechanicus");
            if (act || pawn.def.defName.StartsWith("OG_") || pawn.kindDef.defName.StartsWith("OG_") || pawn.kindDef.defName.Contains("_OG_"))
            {
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
                if (!(from bs in BackstoryDatabase.ShuffleableBackstoryList(slot, backstoryCategoryFilter).TakeRandom(20)
                      where slot != BackstorySlot.Adulthood || !bs.requiredWorkTags.OverlapsWithOnAnyWorkType(pawn.story.childhood.workDisables)
                      select bs).TryRandomElementByWeight(new Func<Backstory, float>(PawnBioAndNameGenerator_FillBackstorySlotShuffled_Controller_Patch.BackstorySelectionWeight), out backstory))
                {
                    Log.Warning(string.Concat(new object[]
                    {
                        "low number of backstories ",
                        slot,
                        " categories used ",
                        backstoryCategoryFilter.categories.ToCommaList(),
                        " found for ",
                        pawn.ToStringSafe<Pawn>(),
                        " of ",
                        factionType.ToStringSafe<FactionDef>(),
                        ". trying random."
                    }));
                    if (!BackstoryDatabase.allBackstories.Where(bs => backstoryCategoryFilter.categories.Any(cat => bs.Value.spawnCategories.Contains(cat)) && bs.Value.slot == slot && (slot != BackstorySlot.Adulthood || !bs.Value.requiredWorkTags.OverlapsWithOnAnyWorkType(pawn.story.childhood.workDisables))).TryRandomElementByWeight<KeyValuePair<string, Backstory>>(new Func<KeyValuePair<string, Backstory>, float>(PawnBioAndNameGenerator_FillBackstorySlotShuffled_Controller_Patch.BackstorySelectionWeight), out KeyValuePair<string, Backstory> b))
                    {
                        Log.Error(string.Concat(new object[]
                        {
                        "No shuffled ",
                        slot,
                        " categories used ",
                        backstoryCategoryFilter.categories.ToCommaList(),
                        " found for ",
                        pawn.ToStringSafe<Pawn>(),
                        " of ",
                        factionType.ToStringSafe<FactionDef>(),
                        ". Choosing random."
                        }), false);
                        backstory = (from kvp in BackstoryDatabase.allBackstories
                                     where kvp.Value.slot == slot
                                     select kvp).RandomElement<KeyValuePair<string, Backstory>>().Value;
                    }
                    else
                    {
                        backstory = b.Value;
                    }
                }
                return false;
            }
            return true;
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
            }), 1871521, false);
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
        public static float BackstorySelectionWeight(KeyValuePair<string, Backstory> bs)
        {
            return PawnBioAndNameGenerator_FillBackstorySlotShuffled_Controller_Patch.SelectionWeightFactorFromWorkTagsDisabled(bs.Value.workDisables);
        }
        public static float BackstorySelectionWeight(Backstory bs)
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
        private static List<Backstory> tmpBackstories = new List<Backstory>();
        //    private static Func<Backstory, float> funcA;
        //    private static Func<PawnBio, float> funcB;

    }

}
