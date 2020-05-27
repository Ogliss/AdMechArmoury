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
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{

    [HarmonyPatch(typeof(Pawn_AgeTracker), "RecalculateLifeStageIndex")]
    public static class AM_Pawn_AgeTracker_RecalculateLifeStageIndex_CommingOfAge_Patch
    {
        public static FieldInfo pawn = typeof(Pawn_AgeTracker).GetField("pawn", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
        [HarmonyPostfix]
        public static void Post_RecalculateLifeStageIndex(Pawn ___pawn)
        {
            if (___pawn != null)
            {
                if (___pawn.story != null && ___pawn.Map != null && ___pawn.RaceProps.Humanlike)
                {
                    if (___pawn.story.adulthood == null)
                    {
                        if (___pawn.isAdult())
                        {
                            List<BackstoryCategoryFilter> backstoryCategoryFiltersFor = AM_PawnBioAndNameGenerator_FillBackstorySlotShuffled_Test_Patch.GetBackstoryCategoryFiltersFor(___pawn, ___pawn.Faction.def);
                            AM_PawnBioAndNameGenerator_FillBackstorySlotShuffled_Test_Patch.FillBackstorySlotShuffled(___pawn, BackstorySlot.Adulthood, ref ___pawn.story.adulthood, ___pawn.story.childhood, backstoryCategoryFiltersFor, ___pawn.Faction.def);
                        }
                    }
                }
            }
        }
    }

}
