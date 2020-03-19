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
    [HarmonyPatch(typeof(PawnGenerator), "GeneratePawn", new[] { typeof(PawnGenerationRequest) })]
    public static class AM_PawnGenerator_GeneratePawn_FactionDefExtension_Patch
    {
        [HarmonyPostfix]
        public static void GeneratePawn_ForceFactionTraits_Postfix(ref Pawn __result)
        {
            if (__result!=null)
            {
                if (__result.Faction!=null && __result.RaceProps.Humanlike)
                {
                    if (__result.Faction.def.HasModExtension<FactionDefExtension>())
                    {
                        if (__result.Faction.def.GetModExtension<FactionDefExtension>() is FactionDefExtension Forced)
                        {
                            if (__result.RaceProps.Humanlike)
                            {
                                foreach (FactionTraitEntry item in Forced.ForcedTraits)
                                {
                                    if (!__result.story.traits.HasTrait(item.def))
                                    {
                                        int maxTraits;
                                        if (MoreTraitSlotsUtil.TryGetMaxTraitSlots(out int max))
                                        {
                                            maxTraits = max;
                                        }
                                        else { maxTraits = 4; }
                                        if (Rand.Chance(item.Chance))
                                        {
                                            if (__result.story.traits.allTraits.Count >= maxTraits)
                                            {
                                                if (!item.replaceiffull)
                                                {
                                                    return;
                                                }
                                                __result.story.traits.allTraits.Remove(__result.story.traits.allTraits.RandomElement());
                                            }
                                            Trait trait = new Trait(item.def, item.degree);
                                            __result.story.traits.GainTrait(trait);
                                        }
                                    }
                                }
                            }
                            foreach (HediffGiverSetDef item in Forced.hediffGivers)
                            {
                                foreach (var hdg in item.hediffGivers.Where(x => x is HediffGiver_StartWithHediff))
                                {
                                    HediffGiver_StartWithHediff hediffGiver_StartWith = (HediffGiver_StartWithHediff)hdg;
                                    if (__result.health.hediffSet.GetNotMissingParts().Any(x => hediffGiver_StartWith.partsToAffect.Contains(x.def)))
                                    {
                                        hediffGiver_StartWith.GiveHediff(__result);
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
