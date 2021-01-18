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
                    if (scenDef.defName.Contains("OG_Ork_Tek_Scenario_Test") || scenDef.defName.Contains("OG_Ork_Feral_Tribe") || scenDef.defName.Contains("OG_Eldar_Craftworld_Scenario_Test"))
                    {
                        //    Log.Message("skipping "+ scen.name);
                        continue;
                    }
                    if (scenDef.defName.Contains("OG_Astartes_"))
                    {
                        if (!AMAMod.settings.AllowAdeptusAstartes)
                        {
                            continue;
                        }
                    }

                    if (scenDef.defName.Contains("OG_Mechanicus_"))
                    {
                        if (!AMAMod.settings.AllowAdeptusMechanicus)
                        {
                            continue;
                        }
                    }

                    if (scenDef.defName.Contains("OG_Militarum_"))
                    {
                        if (!AMAMod.settings.AllowAdeptusMilitarum)
                        {
                            continue;
                        }
                    }

                    if (scenDef.defName.Contains("OG_Sororitas_"))
                    {
                        if (!AMAMod.settings.AllowAdeptusSororitas)
                        {
                            continue;
                        }
                    }

                    if (scenDef.defName.Contains("OG_Choas_Deamons_"))
                    {
                        if (!AMAMod.settings.AllowChaosDeamons)
                        {
                            continue;
                        }
                    }

                    if (scenDef.defName.Contains("OG_Choas_Guard_"))
                    {
                        if (!AMAMod.settings.AllowChaosGuard)
                        {
                            continue;
                        }
                    }

                    if (scenDef.defName.Contains("OG_Choas_Marine_"))
                    {
                        if (!AMAMod.settings.AllowChaosMarine)
                        {
                            continue;
                        }
                    }

                    if (scenDef.defName.Contains("OG_Choas_Mechanicus_"))
                    {
                        if (!AMAMod.settings.AllowChaosMechanicus)
                        {
                            continue;
                        }
                    }

                    if (scenDef.defName.Contains("OG_DarkEldar_"))
                    {
                        if (!AMAMod.settings.AllowDarkEldar)
                        {
                            continue;
                        }
                    }

                    if (scenDef.defName.Contains("OG_Eldar_"))
                    {
                        if (scenDef.defName.Contains("Craftworld"))
                        {
                            if (!AMAMod.settings.AllowEldarCraftworld)
                            {
                                continue;
                            }
                        }
                        if (scenDef.defName.Contains("Exodite"))
                        {
                            if (!AMAMod.settings.AllowEldarExodite)
                            {
                                continue;
                            }
                        }
                        if (scenDef.defName.Contains("Harlequinn"))
                        {
                            if (!AMAMod.settings.AllowEldarHarlequinn)
                            {
                                continue;
                            }
                        }
                    }

                    if (scenDef.defName.Contains("OG_Tau_"))
                    {
                        if (!AMAMod.settings.AllowTau)
                        {
                            continue;
                        }
                    }

                    if (scenDef.defName.Contains("OG_Kroot_"))
                    {
                        if (!AMAMod.settings.AllowKroot)
                        {
                            continue;
                        }
                    }

                    if (scenDef.defName.Contains("OG_Vespid_"))
                    {
                        if (!AMAMod.settings.AllowVespidAuxiliaries)
                        {
                            continue;
                        }
                    }

                    if (scenDef.defName.Contains("OG_Ork_") || scenDef.defName.Contains("OG_Grot_"))
                    {
                        if (scenDef.defName.Contains("Tek"))
                        {
                            if (!AMAMod.settings.AllowOrkTek)
                            {
                                continue;
                            }
                        }
                        if (scenDef.defName.Contains("Feral"))
                        {
                            if (!AMAMod.settings.AllowOrkFeral)
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