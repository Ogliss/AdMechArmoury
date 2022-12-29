using RimWorld;
using Verse;
using HarmonyLib;
using Verse.Grammar;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Precept), "AddIdeoRulesTo")]
    public static class Precept_AddIdeoRulesTo_CultralRoleNames_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Precept __instance, ref GrammarRequest request)
        {

            if (__instance.ideo.culture is CultureDef def && def.generalRules != null)
            {
            //    Log.Message($"CultureDef: {def} found GrammarRequest: {request.Rules.ToString()} generalRules: {def.generalRules.Rules.Count}");

                request.IncludesBare.Add(def.generalRules);
            }
        }
    }

}
