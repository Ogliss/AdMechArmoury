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
        [HarmonyPostfix]
        public static void GiveShuffledBioTo_AdultAge_Postfix(Pawn pawn, FactionDef factionType, string requiredLastName, List<BackstoryCategoryFilter> backstoryCategories)
        {
        //    MethodInfo dynMethod = typeof(PawnBioAndNameGenerator).GetMethod("FillBackstorySlotShuffled", BindingFlags.NonPublic | BindingFlags.Instance);
            
            pawn.Name = PawnBioAndNameGenerator.GeneratePawnName(pawn, NameStyle.Full, requiredLastName);
            AM_PawnBioAndNameGenerator_GiveShuffledBioTo_AdultAge_Patch.FillBackstorySlotShuffled(pawn, BackstorySlot.Childhood, ref pawn.story.childhood, pawn.story.adulthood, backstoryCategories, factionType);
            //    dynMethod.Invoke(typeof(PawnBioAndNameGenerator), new object[] { pawn, BackstorySlot.Childhood, pawn.story.childhood, backstoryCategories, factionType });
            if (pawn.RaceProps.lifeStageAges.First(x => x.def.reproductive) != null)
            {
            //    Log.Message(string.Format("Pawn: {0}, Comes of age at: {1}, Adult: {2}", pawn.LabelShortCap, pawn.RaceProps.lifeStageAges.First(x => x.def.reproductive).minAge, pawn.ageTracker.AgeBiologicalYearsFloat));
            if (pawn.ageTracker.AgeBiologicalYearsFloat >= pawn.RaceProps.lifeStageAges.First(x => x.def.reproductive).minAge)
                {
            //        Log.Message(string.Format("Adult"));
                    AM_PawnBioAndNameGenerator_GiveShuffledBioTo_AdultAge_Patch.FillBackstorySlotShuffled(pawn, BackstorySlot.Adulthood, ref pawn.story.adulthood, pawn.story.childhood, backstoryCategories, factionType);
                    //    dynMethod.Invoke(typeof(PawnBioAndNameGenerator), new object[] { pawn, BackstorySlot.Adulthood, pawn.story.adulthood, backstoryCategories, factionType });
                }
            }
        }


        private static void FillBackstorySlotShuffled(Pawn pawn, BackstorySlot slot, ref Backstory backstory, Backstory backstoryOtherSlot, List<BackstoryCategoryFilter> backstoryCategories, FactionDef factionType)
        {
            AM_PawnBioAndNameGenerator_GiveShuffledBioTo_AdultAge_Patch.tmpBackstories.Clear();
            BackstoryCategoryFilter backstoryCategoryFilter = backstoryCategories.RandomElementByWeight((BackstoryCategoryFilter c) => c.commonality);
            AM_PawnBioAndNameGenerator_GiveShuffledBioTo_AdultAge_Patch.tmpBackstories = BackstoryDatabase.ShuffleableBackstoryList(slot, backstoryCategoryFilter);
            IEnumerable<Backstory> source = from bs in AM_PawnBioAndNameGenerator_GiveShuffledBioTo_AdultAge_Patch.tmpBackstories.TakeRandom(20)
                                            where slot != BackstorySlot.Adulthood || !bs.requiredWorkTags.OverlapsWithOnAnyWorkType(pawn.story.childhood.workDisables)
                                            select bs;
            if (AM_PawnBioAndNameGenerator_GiveShuffledBioTo_AdultAge_Patch.funcA == null)
			{
                AM_PawnBioAndNameGenerator_GiveShuffledBioTo_AdultAge_Patch.funcA = new Func<Backstory, float>(AM_PawnBioAndNameGenerator_GiveShuffledBioTo_AdultAge_Patch.BackstorySelectionWeight);
            }
            if (!source.TryRandomElementByWeight(AM_PawnBioAndNameGenerator_GiveShuffledBioTo_AdultAge_Patch.funcA, out backstory))
            {
                Log.Error(string.Concat(new object[]
                {
                    "No shuffled ",
                    slot,
                    " found for ",
                    pawn.ToStringSafe<Pawn>(),
                    " of ",
                    factionType.ToStringSafe<FactionDef>(),
                    ". Defaulting."
                }), false);
                backstory = (from kvp in BackstoryDatabase.allBackstories
                             where kvp.Value.slot == slot
                             select kvp).RandomElement<KeyValuePair<string, Backstory>>().Value;
            }
            AM_PawnBioAndNameGenerator_GiveShuffledBioTo_AdultAge_Patch.tmpBackstories.Clear();
        }

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
