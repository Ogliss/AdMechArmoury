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
using AdeptusMechanicus;
using AlienRace;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(PawnGenerator), "GenerateInitialHediffs")]
    public static class PawnGenerator_GenerateInitialHediffs_StartWithHediff_Patch
    {
        [HarmonyPostfix, HarmonyPriority(Priority.Last)]
        public static void Post(Pawn pawn, PawnGenerationRequest request)
        {
            Pawn_StoryTracker story = pawn.story;
            if (story == null)
            {
                return;
            }
            List<RimWorld.BackstoryDef> allBackstories = story.AllBackstories;
            if (allBackstories == null)
            {
                return;
            }
            IEnumerable<string> first = allBackstories.OfType<BackstoryDef>().SelectMany((BackstoryDef bd) => bd.forcedHediffs);
            PawnBioDef pawnBioDef = null;//  HarmonyPatches.bioReference;
            first.Concat(((pawnBioDef != null) ? pawnBioDef.forcedHediffs : null) ?? new List<string>(0)).Select(new Func<string, HediffDef>(DefDatabase<HediffDef>.GetNamedSilentFail)).ToList<HediffDef>().ForEach(delegate (HediffDef hd)
            {
                BodyPartRecord part = null;
                RecipeDef recipeDef = DefDatabase<RecipeDef>.AllDefs.FirstOrDefault((RecipeDef rd) => rd.addsHediff == hd);
                if (recipeDef != null)
                {
                    recipeDef.appliedOnFixedBodyParts.SelectMany((BodyPartDef bpd) => from bpr in pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined, null, null)
                                                                                      where bpr.def == bpd && !pawn.health.hediffSet.hediffs.Any((Hediff h) => h.def == hd && h.Part == bpr)
                                                                                      select bpr).TryRandomElement(out part);
                }
                pawn.health.AddHediff(hd, part, null, null);
            });
            var hediffGiverSet = pawn?.def?.race?.hediffGiverSets;
            if (hediffGiverSet == null) return;
            foreach (var item in hediffGiverSet)
            {
                var hediffGivers = item.hediffGivers;
                if (hediffGivers == null) return;
                if (hediffGivers.Any(y => y is HediffGiver_StartWithHediff))
                {
                    foreach (var hdg in hediffGivers.Where(x => x is HediffGiver_StartWithHediff))
                    {
                        HediffGiver_StartWithHediff hediffGiver_StartWith = (HediffGiver_StartWithHediff)hdg;
                        hediffGiver_StartWith.GiveHediff(pawn);
                    }
                }
            }
        }
    }
}
