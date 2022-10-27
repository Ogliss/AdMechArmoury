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
    public static class Pawn_AgeTracker_RecalculateLifeStageIndex_CommingOfAge_Patch
    {
        [HarmonyPostfix]
        public static void Post_RecalculateLifeStageIndex(Pawn ___pawn)
        {
            if (___pawn != null && ___pawn.RaceProps.Humanlike && ___pawn.RaceProps.IsFlesh)
            {
                if (___pawn.story != null && ___pawn.Map != null)
                {
                    if (___pawn.story.adulthood == null)
                    {
                        if (isAdult(___pawn))
                        {
                            List<BackstoryCategoryFilter> backstoryCategoryFiltersFor = PawnBioAndNameGenerator_FillBackstorySlotShuffled_Controller_Patch.GetBackstoryCategoryFiltersFor(___pawn, ___pawn.Faction?.def ?? null);
                            PawnBioAndNameGenerator_FillBackstorySlotShuffled_Controller_Patch.FillBackstorySlotShuffled(___pawn, BackstorySlot.Adulthood, backstoryCategoryFiltersFor, ___pawn.Faction?.def ?? null, BackstorySlot.Childhood);
                        }
                    }
                }
            }
        }
        public static bool isAdult(Pawn pawn)
        {
            float adultage = 18f;
            if (pawn.RaceProps.lifeStageAges.Any(x => x.def.reproductive) && pawn.def != ThingDefOf.Human)
            {
                foreach (LifeStageAge item in pawn.RaceProps.lifeStageAges)
                {
                    if (item.def.reproductive)
                    {
                        adultage = item.minAge;
                        break;
                    }
                }
                if (AdeptusIntergrationUtility.enabled_AlienRaces)
                {
                    float alienage = AlienAdult(pawn);
                    if (alienage > -1f)
                    {
                        adultage = alienage;
                    }
                }
            }
            return pawn.ageTracker.AgeBiologicalYearsFloat >= adultage;
        }

        public static float AlienAdult(Pawn pawn)
        {
            AlienRace.ThingDef_AlienRace race = pawn.def as AlienRace.ThingDef_AlienRace;
            if (race != null)
            {
                return race.alienRace.generalSettings.minAgeForAdulthood;
            }
            return -1;
        }

    }

}
