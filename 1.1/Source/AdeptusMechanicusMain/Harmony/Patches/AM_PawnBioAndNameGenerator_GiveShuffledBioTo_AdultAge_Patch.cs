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
    [HarmonyPatch(typeof(PawnBioAndNameGenerator), "GiveShuffledBioTo")]
    public static class AM_PawnBioAndNameGenerator_GiveShuffledBioTo_AdultAge_Patch
    {
        //    public static MethodInfo FillBackstorySlotShuffled = typeof(PawnBioAndNameGenerator).GetMethod("FillBackstorySlotShuffled", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
        [HarmonyPrefix]
        public static bool GiveShuffledBioTo_AdultAge_Prefix(Pawn pawn, FactionDef factionType, string requiredLastName, ref List<BackstoryCategoryFilter> backstoryCategories)
        {
            bool ext = pawn.kindDef.HasModExtension<BackstoryExtension>();
            if (ext)
            {
                BackstoryCategoryFilter backstoryCategoryFilter = backstoryCategories.RandomElementByWeight((BackstoryCategoryFilter c) => c.commonality);
                backstoryCategories.Clear();
                backstoryCategories.Add(backstoryCategoryFilter);
            }
            if (pawn.ageTracker.AgeBiologicalYears < 20)
            {
                bool act = pawn.RaceProps.lifeStageAges.Any(x => x.def.reproductive);
                if (act)
                {
                    GiveShuffledBioTo(pawn, factionType, requiredLastName, backstoryCategories);
                    return false;
                }
            }
            if (backstoryCategories.NullOrEmpty())
            {
                Log.Warning(string.Format("{0} backstoryCategories null", pawn.NameShortColored));
            }
            return true;
        }

        private static void GiveShuffledBioTo(Pawn pawn, FactionDef factionType, string requiredLastName, List<BackstoryCategoryFilter> backstoryCategories)
        {
            AM_PawnBioAndNameGenerator_GiveShuffledBioTo_AdultAge_Patch.FillBackstorySlotShuffled(pawn, BackstorySlot.Childhood, ref pawn.story.childhood, pawn.story.adulthood, backstoryCategories, factionType);
            if (pawn.ageTracker.AgeBiologicalYearsFloat >= pawn.RaceProps.lifeStageAges.First(x => x.def.reproductive).minAge)
            {
                AM_PawnBioAndNameGenerator_GiveShuffledBioTo_AdultAge_Patch.FillBackstorySlotShuffled(pawn, BackstorySlot.Adulthood, ref pawn.story.adulthood, pawn.story.childhood, backstoryCategories, factionType);
            }
            pawn.Name = PawnBioAndNameGenerator.GeneratePawnName(pawn, NameStyle.Full, requiredLastName);
        }

        // Token: 0x060040BA RID: 16570 RVA: 0x00158E54 File Offset: 0x00157054
        public static void FillBackstorySlotShuffled(Pawn pawn, BackstorySlot slot, ref Backstory backstory, Backstory backstoryOtherSlot, List<BackstoryCategoryFilter> backstoryCategories, FactionDef factionType)
        {
            BackstoryCategoryFilter backstoryCategoryFilter = backstoryCategories.RandomElementByWeight((BackstoryCategoryFilter c) => c.commonality);
            if (backstoryCategoryFilter == null)
            {
                backstoryCategoryFilter = AM_PawnBioAndNameGenerator_GiveShuffledBioTo_AdultAge_Patch.FallbackCategoryGroup;
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
            //    Log.Message(string.Format("backstoryCategories: {0}, used backstoryCategoryFilter: {1}", lista.ToCommaList(), backstoryCategoryFilter.categories.ToCommaList()));
            if (!(from bs in BackstoryDatabase.ShuffleableBackstoryList(slot, backstoryCategoryFilter).TakeRandom(20)
                  where slot != BackstorySlot.Adulthood || !bs.requiredWorkTags.OverlapsWithOnAnyWorkType(pawn.story.childhood.workDisables)
                  select bs).TryRandomElementByWeight(new Func<Backstory, float>(AM_PawnBioAndNameGenerator_GiveShuffledBioTo_AdultAge_Patch.BackstorySelectionWeight), out backstory))
            {
                Log.Message(string.Format("backstoryCategories: {0}, used backstoryCategoryFilter: {1}", lista.ToCommaList(), backstoryCategoryFilter.categories.ToCommaList()));
                Log.Error(string.Concat(new object[]
                {
                    "No shuffled ",
                    slot,
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
                AM_PawnBioAndNameGenerator_GiveShuffledBioTo_AdultAge_Patch.FallbackCategoryGroup
            };
        }
        private static readonly BackstoryCategoryFilter FallbackCategoryGroup = new BackstoryCategoryFilter
        {
            categories = new List<string>
            {
                "Civil"
            },
            commonality = 1f
        };

        // Token: 0x06001503 RID: 5379 RVA: 0x000A3B95 File Offset: 0x000A1F95
        private static float BackstorySelectionWeight(Backstory bs)
        {
            return AM_PawnBioAndNameGenerator_GiveShuffledBioTo_AdultAge_Patch.SelectionWeightFactorFromWorkTagsDisabled(bs.workDisables);
        }

        // Token: 0x06001504 RID: 5380 RVA: 0x000A3BA2 File Offset: 0x000A1FA2
        private static float BioSelectionWeight(PawnBio bio)
        {
            return AM_PawnBioAndNameGenerator_GiveShuffledBioTo_AdultAge_Patch.SelectionWeightFactorFromWorkTagsDisabled(bio.adulthood.workDisables | bio.childhood.workDisables);
        }

        // Token: 0x06001505 RID: 5381 RVA: 0x000A3BC0 File Offset: 0x000A1FC0
        private static float SelectionWeightFactorFromWorkTagsDisabled(WorkTags wt)
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
        private static Func<Backstory, float> funcA;
        private static Func<PawnBio, float> funcB;
    }
    
}
