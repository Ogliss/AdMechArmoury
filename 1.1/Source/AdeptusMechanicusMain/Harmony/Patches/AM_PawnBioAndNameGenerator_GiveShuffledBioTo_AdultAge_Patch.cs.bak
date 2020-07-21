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
            if (pawn.ageTracker.AgeBiologicalYears < 20 && (pawn.def.defName.StartsWith("OG_") || pawn.kindDef.defName.StartsWith("OG_")))
            {
                //    Log.Message(string.Format("AdMech mod pawn: {0} {1} {2}",pawn.NameShortColored, pawn.kindDef, pawn.def.modContentPack.PackageIdPlayerFacing));
                bool act = pawn.RaceProps.lifeStageAges.Any(x => x.def.reproductive);
                if (act)
                {
                    if (pawn.ageTracker.AgeBiologicalYears > pawn.RaceProps.lifeStageAges.First(x => x.def.reproductive).minAge)
                    {
                        AM_PawnBioAndNameGenerator_FillBackstorySlotShuffled_Test_Patch.FillBackstorySlotShuffled(pawn, BackstorySlot.Childhood, ref pawn.story.childhood, pawn.story.adulthood, backstoryCategories, factionType);
                        if (pawn.ageTracker.AgeBiologicalYearsFloat >= pawn.RaceProps.lifeStageAges.First(x => x.def.reproductive).minAge)
                        {
                            AM_PawnBioAndNameGenerator_FillBackstorySlotShuffled_Test_Patch.FillBackstorySlotShuffled(pawn, BackstorySlot.Adulthood, ref pawn.story.adulthood, pawn.story.childhood, backstoryCategories, factionType);
                        }
                        pawn.Name = PawnBioAndNameGenerator.GeneratePawnName(pawn, NameStyle.Full, requiredLastName);
                        return false;
                    }
                }
            }
            return true;
        }
    }
    
}
