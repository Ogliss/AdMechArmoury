using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus.ExtensionMethods
{
    public static class PawnExtensions
    {
        public static bool isAdult(this Pawn pawn)
        {
            float adultage = 18f;
            if (pawn.RaceProps.lifeStageAges.Any(x => x.def.reproductive) && pawn.def != ThingDefOf.Human)
            {
                foreach (LifeStageAge item in pawn.RaceProps.lifeStageAges)
                {
                    if (item.def.reproductive || item.def.defName.Contains("Adult") || item == pawn.RaceProps.lifeStageAges.Last())
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

        public static bool canDeepStrike(this Pawn pawn)
        {
            bool kindDefFlag = pawn.kindDef.HasModExtension<DeepStrikeExtension>();
            bool apparelFlag = pawn.apparel != null && pawn.apparel.WornApparel != null && pawn.apparel.WornApparel.Any(x => x.def.HasModExtension<DeepStrikeExtension>());
            bool hediffFlag = pawn.health.hediffSet.hediffs.Any(x => x.def.HasModExtension<DeepStrikeExtension>());
            bool raceDefFlag = pawn.def.HasModExtension<DeepStrikeExtension>();
            return kindDefFlag || apparelFlag || hediffFlag || raceDefFlag;
        }

        public static bool canDeepStrike(this Pawn pawn, out DeepStrikeExtension Extension)
        {
            Extension = null;
            bool kindDefFlag = pawn.kindDef.HasModExtension<DeepStrikeExtension>();
            bool apparelFlag = pawn.apparel != null && pawn.apparel.WornApparel != null && pawn.apparel.WornApparel.Any(x => x.def.HasModExtension<DeepStrikeExtension>());
            bool hediffFlag = pawn.health.hediffSet.hediffs.Any(x => x.def.HasModExtension<DeepStrikeExtension>());
            bool raceDefFlag = pawn.def.HasModExtension<DeepStrikeExtension>();

            if (kindDefFlag)
            {
                Extension = pawn.kindDef.GetModExtension<DeepStrikeExtension>();
            }
            if (raceDefFlag)
            {
                Extension = pawn.def.GetModExtension<DeepStrikeExtension>();
            }
            if (hediffFlag)
            {
                Extension = pawn.health.hediffSet.hediffs.Where(x => x.def.HasModExtension<DeepStrikeExtension>()).Single().def.GetModExtension<DeepStrikeExtension>();
            }
            if (apparelFlag)
            {
                Extension = pawn.apparel.WornApparel.Where(x => x.def.HasModExtension<DeepStrikeExtension>()).Single().def.GetModExtension<DeepStrikeExtension>();
            }
            return kindDefFlag || apparelFlag || hediffFlag || raceDefFlag;
        }

        public static DeepStrikeExtension DeepStrike(this Pawn pawn)
        {
            DeepStrikeExtension Extension = null;
            bool kindDefFlag = pawn.kindDef.HasModExtension<DeepStrikeExtension>();
            bool apparelFlag = pawn.apparel != null && pawn.apparel.WornApparel != null && pawn.apparel.WornApparel.Any(x => x.def.HasModExtension<DeepStrikeExtension>());
            bool hediffFlag = pawn.health.hediffSet.hediffs.Any(x => x.def.HasModExtension<DeepStrikeExtension>());
            bool raceDefFlag = pawn.def.HasModExtension<DeepStrikeExtension>();

            if (kindDefFlag)
            {
                Extension = pawn.kindDef.GetModExtension<DeepStrikeExtension>();
            }
            if (raceDefFlag)
            {
                Extension = pawn.def.GetModExtension<DeepStrikeExtension>();
            }
            if (hediffFlag)
            {
                Extension = pawn.health.hediffSet.hediffs.Where(x => x.def.HasModExtension<DeepStrikeExtension>()).Single().def.GetModExtension<DeepStrikeExtension>();
            }
            if (apparelFlag)
            {
                Extension = pawn.apparel.WornApparel.Where(x => x.def.HasModExtension<DeepStrikeExtension>()).Single().def.GetModExtension<DeepStrikeExtension>();
            }
            return Extension;
        }

        public static float chanceDeepStrike(this Pawn pawn)
        {
            float f = 0f;
            if (pawn.canDeepStrike(out DeepStrikeExtension extension))
            {
                f = extension.DeepStrikeChance;
            }
            return f;
        }

        public static bool canInfiltrate(this Pawn pawn)
        {
            bool kindDefFlag = pawn.kindDef.HasModExtension<InfiltratorExtension>();
            bool apparelFlag = pawn.apparel != null && pawn.apparel.WornApparel != null && pawn.apparel.WornApparel.Any(x => x.def.HasModExtension<InfiltratorExtension>());
            bool hediffFlag = pawn.health.hediffSet.hediffs.Any(x => x.def.HasModExtension<InfiltratorExtension>());
            bool raceDefFlag = pawn.def.HasModExtension<InfiltratorExtension>();
            return kindDefFlag || apparelFlag || hediffFlag || raceDefFlag;
        }

        public static bool canInfiltrate(this Pawn pawn, out InfiltratorExtension Extension)
        {
            Extension = null;
            bool kindDefFlag = pawn.kindDef.HasModExtension<InfiltratorExtension>();
            bool apparelFlag = pawn.apparel != null && pawn.apparel.WornApparel != null && pawn.apparel.WornApparel.Any(x => x.def.HasModExtension<InfiltratorExtension>());
            bool hediffFlag = pawn.health.hediffSet.hediffs.Any(x => x.def.HasModExtension<InfiltratorExtension>());
            bool raceDefFlag = pawn.def.HasModExtension<InfiltratorExtension>();

            if (kindDefFlag)
            {
                Extension = pawn.kindDef.GetModExtension<InfiltratorExtension>();
            }
            if (raceDefFlag)
            {
                Extension = pawn.def.GetModExtension<InfiltratorExtension>();
            }
            if (hediffFlag)
            {
                Extension = pawn.health.hediffSet.hediffs.Where(x => x.def.HasModExtension<InfiltratorExtension>()).Single().def.GetModExtension<InfiltratorExtension>();
            }
            if (apparelFlag)
            {
                Extension = pawn.apparel.WornApparel.Where(x => x.def.HasModExtension<InfiltratorExtension>()).Single().def.GetModExtension<InfiltratorExtension>();
            }
            return kindDefFlag || apparelFlag || hediffFlag || raceDefFlag;
        }

        public static InfiltratorExtension Infiltrate(this Pawn pawn)
        {
            InfiltratorExtension Extension = null;
            bool kindDefFlag = pawn.kindDef.HasModExtension<InfiltratorExtension>();
            bool apparelFlag = pawn.apparel != null && pawn.apparel.WornApparel != null && pawn.apparel.WornApparel.Any(x => x.def.HasModExtension<InfiltratorExtension>());
            bool hediffFlag = pawn.health.hediffSet.hediffs.Any(x => x.def.HasModExtension<InfiltratorExtension>());
            bool raceDefFlag = pawn.def.HasModExtension<InfiltratorExtension>();
            if (kindDefFlag)
            {
                Extension = pawn.kindDef.GetModExtension<InfiltratorExtension>();
            }
            if (raceDefFlag)
            {
                Extension = pawn.def.GetModExtension<InfiltratorExtension>();
            }
            if (hediffFlag)
            {
                Extension = pawn.health.hediffSet.hediffs.Where(x => x.def.HasModExtension<InfiltratorExtension>()).Single().def.GetModExtension<InfiltratorExtension>();
            }
            if (apparelFlag)
            {
                Extension = pawn.apparel.WornApparel.Where(x => x.def.HasModExtension<InfiltratorExtension>()).Single().def.GetModExtension<InfiltratorExtension>();
            }
            return Extension;
        }

        public static float chanceInfiltrate(this Pawn pawn)
        {
            float f = 0f;
            if (pawn.canInfiltrate(out InfiltratorExtension extension))
            {
                f = extension.InfiltrateChance;
            }
            return f;
        }
        /*
        public static bool abilityWeapon(this Pawn pawn)
        {
            bool flag1 = pawn.equipment != null;
            bool flag2 = pawn.equipment.Primary != null;
            bool flag3 = pawn.equipment.Primary.TryGetComp<CompAbilityItem>() != null;
            return flag1 && flag2 && flag3;
        }

        public static bool abilityEquipment(this Pawn pawn)
        {
            bool flag1 = pawn.apparel != null;
            bool flag2 = pawn.apparel.WornApparel != null;
            bool flag3 = pawn.apparel.WornApparel.Any(x=> x.TryGetComp<CompAbilityItem>() != null);
            return flag1 && flag2 && flag3;
        }

        public static bool abilityImplant(this Pawn pawn)
        {
            return false;
        }
        */
        public static bool isPsyker(this Pawn pawn)
        {
            return pawn.isPsyker(out int Level);
        }

        public static bool isPsyker(this Pawn pawn, out int Level)
        {
            return pawn.isPsyker(out Level, out float Mult);
        }

        public static bool isPsyker(this Pawn pawn, out int Level, out float Mult)
        {
            bool result = false;
            Mult = 0f;
            Level = 0;

            if (pawn.RaceProps.Humanlike)
            {
                if (pawn.health.hediffSet.hediffs.Any(x => x.GetType() == typeof(Hediff_ImplantWithLevel)))
                {
                    Level = (pawn.health.hediffSet.hediffs.First(x => x.GetType() == typeof(Hediff_ImplantWithLevel)) as Hediff_ImplantWithLevel).level;
                    result = true;
                }
                else
                if (pawn.story.traits.HasTrait(TraitDefOf.PsychicSensitivity))
                {
                    result = pawn.story.traits.DegreeOfTrait(TraitDefOf.PsychicSensitivity) > 0;
                    Level = pawn.story.traits.DegreeOfTrait(TraitDefOf.PsychicSensitivity);
                }
                else
                {
                    TraitDef Corruptionpsyker = DefDatabase<TraitDef>.GetNamedSilentFail("Psyker");
                    if (Corruptionpsyker!=null)
                    {
                        result = true;
                        pawn.story.traits.HasTrait(Corruptionpsyker);
                        Level = pawn.story.traits.DegreeOfTrait(Corruptionpsyker);
                    }
                }
                Mult = pawn.GetStatValue(StatDefOf.PsychicSensitivity) * (pawn.needs.mood.CurInstantLevelPercentage - pawn.health.hediffSet.PainTotal);
            }
            else
            {
                ToolUserPskyerDefExtension extension = null;
                if (pawn.def.HasModExtension<ToolUserPskyerDefExtension>())
                {
                    extension = pawn.def.GetModExtension<ToolUserPskyerDefExtension>();
                }
                else
                if (pawn.kindDef.HasModExtension<ToolUserPskyerDefExtension>())
                {
                    extension = pawn.kindDef.GetModExtension<ToolUserPskyerDefExtension>();
                }
                if (extension != null)
                {
                    result = true;
                    Level = extension.Level;
                }
                if (pawn.needs != null && pawn.needs.mood != null)
                {
                    Mult = pawn.GetStatValue(StatDefOf.PsychicSensitivity) * (pawn.needs.mood.CurInstantLevelPercentage - pawn.health.hediffSet.PainTotal);
                }
                else
                {
                    Mult = pawn.GetStatValue(StatDefOf.PsychicSensitivity) * (1 - pawn.health.hediffSet.PainTotal);
                }
            }

            return result;
        }

    }
}
