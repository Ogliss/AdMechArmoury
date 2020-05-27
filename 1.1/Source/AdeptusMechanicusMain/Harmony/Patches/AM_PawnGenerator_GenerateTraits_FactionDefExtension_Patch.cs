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
    [HarmonyPatch(typeof(PawnGenerator), "GenerateTraits")]
    public static class AM_PawnGenerator_GenerateTraits_FactionDefExtension_Patch
    {
        [HarmonyPrefix, HarmonyPriority(Priority.First)]
        public static void Postfix(ref Pawn pawn, ref PawnGenerationRequest request)
        {
            if (pawn != null)
            {
                if (pawn.Faction!=null && pawn.RaceProps.Humanlike)
                {
                    if (pawn.story.childhood==null)
                    {
                        pawn.story.childhood = BackstoryDatabase.RandomBackstory(BackstorySlot.Childhood);
                        PawnBioAndNameGenerator.GiveAppropriateBioAndNameTo(pawn, request.FixedLastName, pawn.Faction.def);
                        if (pawn.story.adulthood == null)
                        {
                            pawn.story.adulthood = BackstoryDatabase.RandomBackstory(BackstorySlot.Adulthood);
                            PawnBioAndNameGenerator.GiveAppropriateBioAndNameTo(pawn, request.FixedLastName, pawn.Faction.def);
                        }
                        Log.Message(string.Format("reroll {0} : {1}({2}) : {3} : {4} : {5}", pawn.NameShortColored, pawn.KindLabel, pawn.kindDef.defName, pawn.story.childhood, pawn.story.adulthood, pawn.Faction));
                    }
                    if (pawn.Faction.def.HasModExtension<FactionDefExtension>())
                    {
                        if (pawn.Faction.def.GetModExtension<FactionDefExtension>() is FactionDefExtension Forced)
                        {
                            if (pawn.RaceProps.Humanlike)
                            {
                                foreach (FactionTraitEntry item in Forced.ForcedTraits)
                                {
                                    Log.Message(string.Format("{0} : {1}", pawn.NameShortColored, item.def.LabelCap));
                                    if (!pawn.story.traits.HasTrait(item.def))
                                    {
                                        int maxTraits;
                                        if (MoreTraitSlotsUtil.TryGetMaxTraitSlots(out int max))
                                        {
                                            maxTraits = max;
                                        }
                                        else { maxTraits = 4; }
                                        if (Rand.Chance(item.Chance))
                                        {
                                            if (pawn.story.traits.allTraits.Count >= maxTraits)
                                            {
                                                if (!item.replaceiffull)
                                                {
                                                    return;
                                                }
                                                pawn.story.traits.allTraits.Remove(pawn.story.traits.allTraits.RandomElement());
                                            }
                                            Trait trait = new Trait(item.def, item.degree);
                                            pawn.story.traits.GainTrait(trait);
                                        }
                                    }
                                }
                            }
                            foreach (HediffGiverSetDef item in Forced.hediffGivers)
                            {
                                foreach (var hdg in item.hediffGivers.Where(x => x is HediffGiver_StartWithHediff))
                                {
                                    HediffGiver_StartWithHediff hediffGiver_StartWith = (HediffGiver_StartWithHediff)hdg;
                                    if (pawn.health.hediffSet.GetNotMissingParts().Any(x => hediffGiver_StartWith.partsToAffect.Contains(x.def)))
                                    {
                                        hediffGiver_StartWith.GiveHediff(pawn);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    
}
