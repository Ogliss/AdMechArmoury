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
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{

    [HarmonyPatch(typeof(ResearchProjectDef), "CanBeResearchedAt")]
    public static class ResearchProjectDef_CanBeResearchedAt_RaceBench_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ResearchProjectDef __instance, Building_ResearchBench bench, bool ignoreResearchBenchPowerStatus, ref bool __result)
        {
            if (!__result)
            {
                if ((__instance.requiredResearchBuilding == AdeptusThingDefOf.HiTechResearchBench && bench.def.defName.Contains("HiTechResearchBench")) || (__instance.requiredResearchBuilding == AdeptusThingDefOf.SimpleResearchBench && bench.def.defName.Contains("ResearchBench")))
                {
                    if (raceTableFor(bench, __instance))
                    {
                        if (!ignoreResearchBenchPowerStatus)
                        {
                            CompPowerTrader comp = bench.TryGetCompFast<CompPowerTrader>();
                            if (comp != null && !comp.PowerOn)
                            {
                                return;
                            }
                        }
                        if (!__instance.requiredResearchFacilities.NullOrEmpty())
                        {
                            CompAffectedByFacilities affectedByFacilities = bench.TryGetCompFast<CompAffectedByFacilities>();
                            if (affectedByFacilities == null)
                            {
                                return;
                            }
                            List<Thing> linkedFacilitiesListForReading = affectedByFacilities.LinkedFacilitiesListForReading;
                            int i;
                            for (i = 0; i < __instance.requiredResearchFacilities.Count; i++)
                            {
                                if (linkedFacilitiesListForReading.Find((Thing x) => x.def == __instance.requiredResearchFacilities[i] && affectedByFacilities.IsFacilityActive(x)) == null)
                                {
                                    return;
                                }
                            }
                        }
                        __result = true;
                    }
                }
            }
        }

        static bool raceTableFor(Building_ResearchBench bench, ResearchProjectDef def)
        {
            string tag = "OG";
            List<string> Tags = new List<string>();
            List<string> split = def.defName.Split(new char[] { '_' }).ToList();
            if (split.NullOrEmpty())
            {
                return bench.def.defName.StartsWith("OG") && bench.def.defName.EndsWith("_" + def.requiredResearchBuilding.defName);
            }
            if (split.Count > 1)
            {
                string race = split[1];
                switch (race)
                {
                    case "Imperial":
                        Tags.Add(tag + "I");
                        break;
                    case "Astartes":
                        Tags.Add(tag + "AA");
                        Tags.Add(tag + "I");
                        break;
                    case "Mechanicus":
                        Tags.Add(tag + "AM");
                        break;
                    case "Militarum":
                        Tags.Add(tag + "I");
                        break;
                    case "Sororitas":
                        Tags.Add(tag + "I");
                        Tags.Add(tag + "AS");
                        break;
                    case "Eldar":
                        Tags.Add(tag + "E");
                        break;
                    case "DarkEldar":
                        Tags.Add(tag + "DE");
                        break;
                    case "Aeldari":
                        Tags.Add(tag + "DE");
                        break;
                    case "Tau":
                        Tags.Add(tag + "T");
                        break;
                    case "Kroot":
                        Tags.Add(tag + "T");
                        Tags.Add(tag + "K");
                        break;
                    case "Vespid":
                        Tags.Add(tag + "T");
                        Tags.Add(tag + "V");
                        break;
                    case "Ork":
                        Tags.Add(tag + "O");
                        break;
                    case "Necron":
                        Tags.Add(tag + "N");
                        break;
                    case "Tyranid":
                        Tags.Add(tag + "TY");
                        break;
                    default:
                        return bench.def.defName.StartsWith("OG") && bench.def.defName.EndsWith("_" + def.requiredResearchBuilding.defName);
                }
            }
            else
            {
                Tags.Add(tag);
            }

            if (!Tags.NullOrEmpty())
            {
                for (int i = 0; i < Tags.Count; i++)
                {
                    if (bench.def.defName.StartsWith(Tags[i] + (split.Count > 1 ? "_" : "")))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

}
