using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse.AI;
using Verse.AI.Group;
using AdeptusMechanicus;
using AdeptusMechanicus.ExtensionMethods;
using Verse.Sound;
using System;
using RimWorld;
using Verse;
using HarmonyLib;
using AdeptusMechanicus.settings;

namespace AdeptusMechanicus.HarmonyInstance
{
    
    [HarmonyPatch(typeof(PreceptWorker), "CanUse")]
    public static class PreceptWorker_CanUse_FactionRelics_Patch
    {
        [HarmonyPostfix]
        public static AcceptanceReport Postfix(AcceptanceReport __result, PreceptWorker __instance, ThingDef def, Ideo ideo)
        {
            if (__instance is PreceptWorker_Relic relic && ideo.culture.defName.StartsWith("OG_"))
            {
                if (AMAMod.Dev)
                {
                    Log.Message("test");

                }
            }
            return __result;
        }
        static string Tag()
        {
            string tag = string.Empty;

            return tag;
        }
    }
    
}
