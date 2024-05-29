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
    
    [HarmonyPatch(typeof(PreceptWorker), "CanUse", new Type[] { typeof(ThingDef), typeof(Ideo), typeof(FactionDef) })]
    public static class PreceptWorker_CanUse_FactionRelics_Patch
    {
        [HarmonyPostfix]
        public static AcceptanceReport Postfix(AcceptanceReport __result, PreceptWorker __instance, ThingDef def, Ideo ideo, FactionDef generatingFor)
        {
            if (__instance is PreceptWorker_Relic relic && ideo.culture.defName.StartsWith("OG_"))
            {
                if (def.defName.StartsWith("OGI_"))
                {
                    return ideo.culture.defName.Contains("Imperial");
                }
                if (def.defName.StartsWith("OGAM_"))
                {
                    return ideo.culture.defName.Contains("Mechanicus");
                }
                if (def.defName.StartsWith("OGO_"))
                {
                    return ideo.culture.defName.Contains("Orkoid") || ideo.culture.defName.Contains("Greenskin");
                }
                if (def.defName.StartsWith("OGDE_"))
                {
                    return ideo.culture.defName.Contains("DarkEldar") || ideo.culture.defName.Contains("Drukhari");
                }
                if (def.defName.StartsWith("OGE_"))
                {
                    return ideo.culture.defName.Contains("Eldar") || ideo.culture.defName.Contains("Aeldari");
                }
                if (def.defName.StartsWith("OGT_"))
                {
                    return ideo.culture.defName.Contains("Tau");
                }
                if (def.defName.StartsWith("OGK_"))
                {
                    return ideo.culture.defName.Contains("Kroot");
                }
                if (def.defName.StartsWith("OGC_"))
                {
                    return ideo.culture.defName.Contains("Chaos");
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
