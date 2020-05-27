using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Harmony;
using Verse.Sound;

namespace AdeptusMechanicus.Harmony
{
    [HarmonyPatch(typeof(RecipeDef), "get_AvailableNow")]
    public static class AM_RecipeDef_AvailableNow_ExtraPrerequisite_Patch
    {
        [HarmonyPostfix]
        public static void AvailableNowPostfix(RecipeDef __instance, ref bool __result)
        {
            if (__result)
            {
                if (__instance!=null)
                {
                    if (__instance.ProducedThingDef!=null)
                    {
                        if (__instance.ProducedThingDef.comps!=null)
                        {
                            if (__instance.ProducedThingDef.GetCompProperties<CompProperties_ExtraPrerequisite>() != null)
                            {
                                if (__instance.ProducedThingDef.GetCompProperties<CompProperties_ExtraPrerequisite>() is CompProperties_ExtraPrerequisite ExtraPrerequisites)
                                {
                                    if (!ExtraPrerequisites.ExtraResarchPrerequisites.NullOrEmpty())
                                    {
                                        List<ResearchProjectDef> extras = ExtraResarchPrerequisites(ExtraPrerequisites);
                                        if (!extras.NullOrEmpty())
                                        {
                                            foreach (ResearchProjectDef research in extras)
                                            {
                                                __result = research.IsFinished;
                                                if (!__result)
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public static List<ResearchProjectDef> ExtraResarchPrerequisites(CompProperties_ExtraPrerequisite Props)
        {
            List<ResearchProjectDef> list = new List<ResearchProjectDef>();
            foreach (var item in Props.ExtraResarchPrerequisites)
            {
                ResearchProjectDef Def = DefDatabase<ResearchProjectDef>.GetNamedSilentFail(item);
                if (Def != null)
                {
                    list.Add(Def);
                }
            }
            return list;
        }
    }
}
