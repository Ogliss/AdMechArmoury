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

namespace AdeptusMechanicus.HarmonyInstance
{
    // ResearchProjectDef.PrerequisitesCompleted
    [HarmonyPatch(typeof(ResearchProjectDef), "get_PrerequisitesCompleted")]
    public static class ResearchProjectDef_get_PrerequisitesCompleted_CommonTech_Patch
    {
        public static bool Postfix(bool __result, ResearchProjectDef __instance)
        {
            if (__result)
            {

                if (__instance.HasModExtension<AnyPrerequisiteResearchExtension>())
                {
                    AnyPrerequisiteResearchExtension ext = __instance.GetModExtensionFast<AnyPrerequisiteResearchExtension>();
                    if (!ext.RequiredResearch.NullOrEmpty())
                    {
                        return ext.AnyRequiredResearchCompleted;
                        if (ext.AnyRequiredResearchCompleted)
                        {
                            List<ResearchProjectDef> reqs = new List<ResearchProjectDef>();
                            if (!__instance.prerequisites.NullOrEmpty())
                            {
                                reqs.AddRange(__instance.prerequisites);
                            }
                            if (!__instance.hiddenPrerequisites.NullOrEmpty())
                            {
                                reqs.AddRange(__instance.hiddenPrerequisites);
                            }
                            if (!reqs.NullOrEmpty())
                            {
                                __result = __result || !reqs.Any(x => !x.IsFinished && !ext.RequiredResearch.Contains(x));
                            }
                        }
                        /*
                        else
                        {
                            __result = false;
                        }
                        */
                    }
                }
                /*
                if (__result && AdeptusIntergrationUtility.enabled_AlienRaces)
                {
                    __result = AlienRaceUtility.CanResearch(__instance);
                }
                */
            }
            return __result;
        }

    }
    
}
