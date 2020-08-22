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
using AdeptusMechanicus.ExtensionMethods;
using AdeptusMechanicus.settings;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(IncidentWorker), "get_BaseChanceThisGame")]
    public static class IncidentWorker_BaseChanceThisGame_ModOptions_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(IncidentWorker __instance, ref float __result)
        {
            IncidentDef incidentDef = __instance.def;
            if (incidentDef.defName.Contains("OGN_MonolithAppears"))
            {
                if (!SettingsHelper.latest.AllowNecronMonolith)
                {
                    __result = 0f;
                }
            }
            if (incidentDef.defName.Contains("OG_Chaos_Deamon"))
            {
                if (incidentDef.defName.Contains("Warpstorm_Deamonic"))
                {
                    if (!SettingsHelper.latest.AllowWarpstorm)
                    {
                        __result = 0f;
                    }
                }
                if (incidentDef.defName.Contains("Deamonic_Incursion"))
                {
                    if (!SettingsHelper.latest.AllowChaosDeamonicIncursion)
                    {
                        __result = 0f;
                    }
                }
                if (incidentDef.defName.Contains("Daemonic_Infestation"))
                {
                    if (!SettingsHelper.latest.AllowChaosDeamonicInfestation)
                    {
                        __result = 0f;
                    }
                }
            }
            if (incidentDef.defName.Contains("OG_Ork_Rok_Crash"))
            {
                if (!SettingsHelper.latest.AllowOrkRok)
                {
                    __result = 0f;
                }
            }
        }
    }
    
}
