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
    [HarmonyPatch(typeof(Pawn_AgeTracker), "AgeTick")]
    public static class AM_Pawn_AgeTracker_AgeTick_ActivatableEffect_Patch
    {
        [HarmonyPostfix]
        public static void Notify_EquipmentAddedPostfix(Pawn_AgeTracker __instance)
        {
            Traverse traverse = Traverse.Create(__instance);
            Pawn pawn = (Pawn)AM_Pawn_AgeTracker_AgeTick_ActivatableEffect_Patch.pawn.GetValue(__instance);
            if (pawn!=null)
            {
                if (pawn.story!=null)
                {
                    if (pawn.story.adulthood == null)
                    {
                        if (pawn.isAdult())
                        {
                            List<BackstoryCategoryFilter> backstoryCategoryFiltersFor = AM_PawnBioAndNameGenerator_GiveShuffledBioTo_AdultAge_Patch.GetBackstoryCategoryFiltersFor(pawn, pawn.Faction.def);
                            AM_PawnBioAndNameGenerator_GiveShuffledBioTo_AdultAge_Patch.FillBackstorySlotShuffled(pawn, BackstorySlot.Adulthood, ref pawn.story.adulthood, pawn.story.childhood, backstoryCategoryFiltersFor, pawn.Faction.def);
                        }
                    }
                }
            }

        }
        public static FieldInfo pawn = typeof(Pawn_AgeTracker).GetField("pawn", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);

    }
}
