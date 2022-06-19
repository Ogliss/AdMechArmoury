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
        public static bool ModPawn(this Pawn pawn) => (pawn.def.modContentPack != null && (pawn.def.modContentPack.Name.Contains("Mechanicus") || pawn.def.modContentPack.Name.Contains("Adeptus") || pawn.def.modContentPack.Name.Contains("Xenobiologis"))) || pawn.def.defName.StartsWith("OG_") || pawn.kindDef.defName.StartsWith("OG_") || pawn.kindDef.defName.Contains("_OG_");

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

        public static bool canDeepStrike(this PawnKindDef kindDef)
        {
            return canReserveDeployment(kindDef, out ReserveDeploymentExtension output) && output.Striker;
        }

        public static bool canDeepStrike(this Pawn pawn)
        {
            return canReserveDeployment(pawn, out ReserveDeploymentExtension output) && output.Striker;
        }

        public static float chanceDeepStrike(this Pawn pawn)
        {
            if (pawn.canReserveDeployment(out ReserveDeploymentExtension extension) && extension.Striker) return extension.DeepStrikeChance;
            return 0f;
        }
        public static bool canInfiltrate(this PawnKindDef kindDef)
        {
            return canReserveDeployment(kindDef, out ReserveDeploymentExtension output) && output.Infiltrator;
        }
        public static bool canInfiltrate(this Pawn pawn)
        {
            return canReserveDeployment(pawn, out ReserveDeploymentExtension output) && output.Infiltrator;
        }

        public static float chanceInfiltrate(this Pawn pawn)
        {
            if (pawn.canReserveDeployment(out ReserveDeploymentExtension extension) && extension.Infiltrator) return extension.InfiltrateChance;
            return 0f;
        }

        public static bool canReserveDeployment(this PawnKindDef kindDef, out ReserveDeploymentExtension output)
        {
            ReserveDeploymentExtension reserve = null;
            if ((reserve = kindDef.GetModExtensionFast<ReserveDeploymentExtension>()) != null)
            {
                return (output = reserve) != null;
            }
            if ((reserve = kindDef.race.GetModExtensionFast<ReserveDeploymentExtension>()) != null)
            {
                return (output = reserve) != null;
            }
            return (output = reserve) != null;
        }


        public static bool canReserveDeployment(this Pawn pawn, out ReserveDeploymentExtension output)
        {
            ReserveDeploymentExtension reserve = null;
            if ((reserve = pawn.kindDef.GetModExtensionFast<ReserveDeploymentExtension>()) != null)
            {
                return (output = reserve) != null;
            }
            if ((reserve = pawn.def.GetModExtensionFast<ReserveDeploymentExtension>()) != null)
            {
                return (output = reserve) != null;
            }
            if (pawn.apparel != null && pawn.apparel.WornApparel != null && pawn.apparel.WornApparel.Any(x => (reserve = x.def.GetModExtensionFast<ReserveDeploymentExtension>()) != null))
            {
                return (output = reserve) != null;
            }
            if (pawn.health.hediffSet.hediffs.Any(x => (reserve = x.def.GetModExtensionFast<ReserveDeploymentExtension>()) != null))
            {
                return (output = reserve) != null;
            }
            return (output = reserve) != null;
        }

        public static ReserveDeploymentExtension ReserveDeployment(this Pawn pawn)
        {
            if (canReserveDeployment(pawn, out ReserveDeploymentExtension Extension)) return Extension;
            return null;
        }

        /*
        public static bool abilityWeapon(this Pawn pawn)
        {
            bool flag1 = pawn.equipment != null;
            bool flag2 = pawn.equipment.Primary != null;
            bool flag3 = pawn.equipment.Primary.TryGetCompFast<CompAbilityItem>() != null;
            return flag1 && flag2 && flag3;
        }

        public static bool abilityEquipment(this Pawn pawn)
        {
            bool flag1 = pawn.apparel != null;
            bool flag2 = pawn.apparel.WornApparel != null;
            bool flag3 = pawn.apparel.WornApparel.Any(x=> x.TryGetCompFast<CompAbilityItem>() != null);
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
                if (pawn.HasPsylink)
                {
                    Level = pawn.psychicEntropy.Psylink.level;
                    result = true;
                }
                else
                if (pawn.health.hediffSet.hediffs.Any(x => x.GetType() == typeof(Hediff_Level)))
                {
                    Level = (pawn.health.hediffSet.hediffs.First(x => x.GetType() == typeof(Hediff_Level)) as Hediff_Level).level;
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
                    extension = pawn.def.GetModExtensionFast<ToolUserPskyerDefExtension>();
                }
                else
                if (pawn.kindDef.HasModExtension<ToolUserPskyerDefExtension>())
                {
                    extension = pawn.kindDef.GetModExtensionFast<ToolUserPskyerDefExtension>();
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
