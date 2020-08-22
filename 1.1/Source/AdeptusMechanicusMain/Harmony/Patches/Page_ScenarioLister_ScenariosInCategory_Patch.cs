using RimWorld;
using Verse;
using HarmonyLib;
using System.Collections.Generic;
using AdeptusMechanicus.settings;

namespace AdeptusMechanicus.HarmonyInstance
{
    // ScenarioLister.ScenariosInCategory
    [HarmonyPatch(typeof(ScenarioLister), "ScenariosInCategory")]
    public static class Page_ScenarioLister_ScenariosInCategory_Patch
    {
        [HarmonyPostfix]
        public static IEnumerable<Scenario> ScenariosInCategoryPostfix(IEnumerable<Scenario> __result, ScenarioCategory cat)
        {
            foreach (Scenario scen in __result)
            {
                if (!DefDatabase<ScenarioDef>.AllDefsListForReading.Any(x => x.scenario.name == scen.name))
                {
                    yield return scen;
                }
                if (cat == ScenarioCategory.FromDef)
                {
                    ScenarioDef scenDef = DefDatabase<ScenarioDef>.AllDefsListForReading.Find(x => x.scenario.name == scen.name);
                    //   Log.Message("checking name: " + scen.name+ " def: "+ scenDef.defName);
                    if (scenDef.defName.Contains("OGAM_TestScenario_") && SteamUtility.SteamPersonaName != "Ogliss")
                    {
                        //    Log.Message("skipping "+ scen.name);
                        continue;
                    }
                    if (scenDef.defName.Contains("OG_Astartes_"))
                    {
                        if (!SettingsHelper.latest.AllowAdeptusAstartes)
                        {
                            continue;
                        }
                    }

                    if (scenDef.defName.Contains("OG_Mechanicus_"))
                    {
                        if (!SettingsHelper.latest.AllowAdeptusMechanicus)
                        {
                            continue;
                        }
                    }

                    if (scenDef.defName.Contains("OG_Militarum_"))
                    {
                        if (!SettingsHelper.latest.AllowAdeptusMilitarum)
                        {
                            continue;
                        }
                    }

                    if (scenDef.defName.Contains("OG_Sororitas_"))
                    {
                        if (!SettingsHelper.latest.AllowAdeptusSororitas)
                        {
                            continue;
                        }
                    }

                    if (scenDef.defName.Contains("OG_Choas_Deamons_"))
                    {
                        if (!SettingsHelper.latest.AllowChaosDeamons)
                        {
                            continue;
                        }
                    }

                    if (scenDef.defName.Contains("OG_Choas_Guard_"))
                    {
                        if (!SettingsHelper.latest.AllowChaosGuard)
                        {
                            continue;
                        }
                    }

                    if (scenDef.defName.Contains("OG_Choas_Marine_"))
                    {
                        if (!SettingsHelper.latest.AllowChaosMarine)
                        {
                            continue;
                        }
                    }

                    if (scenDef.defName.Contains("OG_Choas_Mechanicus_"))
                    {
                        if (!SettingsHelper.latest.AllowChaosMechanicus)
                        {
                            continue;
                        }
                    }

                    if (scenDef.defName.Contains("OG_Dark_Eldar_"))
                    {
                        if (!SettingsHelper.latest.AllowDarkEldar)
                        {
                            continue;
                        }
                    }

                    if (scenDef.defName.Contains("OG_Eldar_"))
                    {
                        if (scenDef.defName.Contains("Craftworld"))
                        {
                            if (!SettingsHelper.latest.AllowEldarCraftworld)
                            {
                                continue;
                            }
                        }
                        if (scenDef.defName.Contains("Exodite"))
                        {
                            if (!SettingsHelper.latest.AllowEldarExodite)
                            {
                                continue;
                            }
                        }
                        if (scenDef.defName.Contains("Harlequinn"))
                        {
                            if (!SettingsHelper.latest.AllowEldarHarlequinn)
                            {
                                continue;
                            }
                        }
                    }

                    if (scenDef.defName.Contains("OG_Tau_"))
                    {
                        if (!SettingsHelper.latest.AllowTau)
                        {
                            continue;
                        }
                    }

                    if (scenDef.defName.Contains("OG_Kroot_"))
                    {
                        if (!SettingsHelper.latest.AllowKroot)
                        {
                            continue;
                        }
                    }

                    if (scenDef.defName.Contains("OG_Vespid_"))
                    {
                        if (!SettingsHelper.latest.AllowVespidAuxiliaries)
                        {
                            continue;
                        }
                    }

                    if (scenDef.defName.Contains("OG_Ork_") || scenDef.defName.Contains("OG_Grot_"))
                    {
                        if (scenDef.defName.Contains("Tek"))
                        {
                            if (!SettingsHelper.latest.AllowOrkTek)
                            {
                                continue;
                            }
                        }
                        if (scenDef.defName.Contains("Feral"))
                        {
                            if (!SettingsHelper.latest.AllowOrkFeral)
                            {
                                continue;
                            }
                        }
                    }
                }
                yield return scen;
            }
            yield break;

        }
    }
}