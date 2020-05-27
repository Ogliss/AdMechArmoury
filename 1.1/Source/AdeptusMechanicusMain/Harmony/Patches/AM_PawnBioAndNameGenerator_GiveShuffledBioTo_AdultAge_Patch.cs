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
        public static bool Prefix(Pawn pawn, FactionDef factionType, string requiredLastName, ref List<BackstoryCategoryFilter> backstoryCategories)
        {
            if (pawn == null)
            {
                return true;
            }
            if (!pawn.RaceProps.Humanlike)
            {
                return true;
            }
            if (pawn.ageTracker == null || pawn.health == null || pawn.story == null)
            {
                return true;
            }
            bool ext = pawn.kindDef.HasModExtension<BackstoryExtension>();
            if (ext)
            {
                BackstoryCategoryFilter backstoryCategoryFilter = backstoryCategories.RandomElementByWeight((BackstoryCategoryFilter c) => c.commonality);
                backstoryCategories.Clear();
                backstoryCategories.Add(backstoryCategoryFilter);
            }
            if (pawn.health.hediffSet.hediffs.Any(x => x.def.defName.Contains("TM_") && (x.def.defName.Contains("Undead") || x.def.defName.Contains("Lich"))))
            {
                return true;
            }
            if (pawn.ageTracker.AgeBiologicalYears < 20 )
            {
                //    Log.Message(string.Format("AdMech mod pawn: {0} {1} {2}",pawn.NameShortColored, pawn.kindDef, pawn.def.modContentPack.PackageIdPlayerFacing));
                bool act = pawn.RaceProps.lifeStageAges.Any(x => x.def.reproductive);
                if (act)
                {
                    /*
                    if (pawn.NameShortColored.NullOrEmpty())
                    {
                        Log.Message(string.Format("pawn.NameShortColored"));
                    }
                    if (pawn.kindDef == null)
                    {
                        Log.Message(string.Format("pawn.kindDef"));
                    }
                    if (pawn.def.modContentPack == null)
                    {
                        Log.Message(string.Format("pawn.def.modContentPack"));
                    }
                    else
                    {
                        Log.Message(string.Format("pawn.def.modContentPack {0}: {1}", pawn.def.modContentPack.PackageIdPlayerFacing, pawn.def.modContentPack.PackageIdPlayerFacing.ToString().Contains("AdMech")));

                    }
                    Log.Message(string.Format("pawn has reproductive stage: {0} {1}",pawn.NameShortColored, pawn.kindDef));
                    */
                    if (pawn.ageTracker.AgeBiologicalYears > pawn.RaceProps.lifeStageAges.First(x => x.def.reproductive).minAge)
                    {
                        //    Log.Message(string.Format("pawn age > min age: {0} {1} {2}", pawn.ageTracker.AgeBiologicalYears, pawn.RaceProps.lifeStageAges.First(x => x.def.reproductive).minAge, (pawn.ageTracker.AgeBiologicalYears > pawn.RaceProps.lifeStageAges.First(x => x.def.reproductive).minAge)));
                        AM_PawnBioAndNameGenerator_FillBackstorySlotShuffled_Test_Patch.FillBackstorySlotShuffled(pawn, BackstorySlot.Childhood, ref pawn.story.childhood, pawn.story.adulthood, backstoryCategories, factionType);
                        //    Traverse.Create(typeof(PawnBioAndNameGenerator)).Method("FillBackstorySlotShuffled", new object[] { pawn, BackstorySlot.Childhood, pawn.story.childhood, pawn.story.adulthood, backstoryCategories, factionType }).GetValue();
                        //    Traverse.Create(typeof(PawnBioAndNameGenerator)).Method("FillBackstorySlotShuffled", new object[] { pawn, BackstorySlot.Childhood, pawn.story.childhood, pawn.story.adulthood, backstoryCategories, factionType });

                        if (pawn.ageTracker.AgeBiologicalYearsFloat >= pawn.RaceProps.lifeStageAges.First(x => x.def.reproductive).minAge)
                        {
                            AM_PawnBioAndNameGenerator_FillBackstorySlotShuffled_Test_Patch.FillBackstorySlotShuffled(pawn, BackstorySlot.Adulthood, ref pawn.story.adulthood, pawn.story.childhood, backstoryCategories, factionType);
                            //    Traverse.Create(typeof(PawnBioAndNameGenerator)).Method("FillBackstorySlotShuffled", new object[] { pawn, BackstorySlot.Adulthood, pawn.story.adulthood, pawn.story.childhood, backstoryCategories, factionType }).GetValue();
                            //    Traverse.Create(typeof(PawnBioAndNameGenerator)).Method("FillBackstorySlotShuffled", new object[] { pawn, BackstorySlot.Adulthood, pawn.story.adulthood, pawn.story.childhood, backstoryCategories, factionType });
                        }
                        pawn.Name = PawnBioAndNameGenerator.GeneratePawnName(pawn, NameStyle.Full, requiredLastName);
                        return false;
                    }
                }
            }
            /*
            if (backstoryCategories.NullOrEmpty())
            {
                Log.Warning(string.Format("{0} backstoryCategories null", pawn.NameShortColored));
            }
            Log.Message(string.Format("{0} of {1} useing default", pawn.NameShortColored ,pawn.Faction));
            */
            return true;
        }
    }
    
}
