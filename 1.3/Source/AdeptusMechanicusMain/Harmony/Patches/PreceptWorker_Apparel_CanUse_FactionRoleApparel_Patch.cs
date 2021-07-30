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

namespace AdeptusMechanicus.HarmonyInstance
{
    //  if (__instance is PreceptWorker_Apparel relic && ideo.culture.defName.StartsWith("OG_")) { }
    // PreceptWorker.ThingDefsForIdeo
    // PreceptWorker_Apparel.CanUse
    [HarmonyPatch(typeof(PreceptWorker_Apparel), "CanUse", new Type[] { typeof(ThingDef), typeof(Ideo), typeof(FactionDef) })]
    public static class PreceptWorker_Apparel_CanUse_FactionRoleApparel_Patch
    {
        /*
        [HarmonyPrefix]
        public static void Prefix(AcceptanceReport __result, PreceptWorker_Apparel __instance, ThingDef def, Ideo ideo)
        {

        }
        */

        [HarmonyPostfix]
        public static AcceptanceReport Postfix(AcceptanceReport __result, PreceptWorker_Apparel __instance, ThingDef def, Ideo ideo)
        {
            if (ideo.culture.defName.StartsWith("OG_"))
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
    }
    
}
