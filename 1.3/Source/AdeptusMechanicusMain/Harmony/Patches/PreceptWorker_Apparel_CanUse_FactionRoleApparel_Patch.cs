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
        static ThingDef def = null;
        [HarmonyPrefix]
        public static void Prefix(AcceptanceReport __result, PreceptWorker_Apparel __instance, ThingDef def, Ideo ideo)
        {
            PreceptWorker_Apparel_CanUse_FactionRoleApparel_Patch.def = def;
        }
        

        [HarmonyPostfix]
        public static AcceptanceReport Postfix(AcceptanceReport __result, PreceptWorker_Apparel __instance, ThingDef def, Ideo ideo, FactionDef generatingFor)
        {
            return __result;
            AcceptanceReport result = __result;
            if (!__result.Accepted && ideo.culture is CultureDef culture)
            {
                if (!culture.preferredApparelTags.NullOrEmpty())
                {
                    foreach (var item in culture.preferredApparelTags)
                    {
                        Log.Message($"Checking {def}");
                        if (def.apparel.tags.Contains(item))
                        {
                            return true;
                        }
                    }

                    if (generatingFor != null)
                    {
                        Log.Message($"{def} not allowed for {generatingFor}");
                        return new AcceptanceReport("RoleApparelRequirementIncompatibleFaction".Translate(generatingFor.LabelCap));
                    }
                    else
                    {
                        if (Find.World != null && Find.FactionManager != null)
                        {
                            foreach (Faction faction in Find.FactionManager.AllFactions)
                            {
                                if (faction.def != generatingFor && faction.ideos != null && faction.ideos.PrimaryIdeo == ideo)
                                {
                                    Log.Message($"{def} not allowed for {faction}");
                                    return new AcceptanceReport("RoleApparelRequirementIncompatibleFaction".Translate(faction.def.LabelCap));
                                }
                            }
                        }
                        Log.Message($"{def} not allowed for {culture}");
                        return new AcceptanceReport("RoleApparelRequirementIncompatibleFaction".Translate(culture.LabelCap));
                    }
                }
            }
            if (ideo.culture.defName.StartsWith("OG_"))
            {
                if (def.defName.StartsWith("OGI_"))
                {
                    /*
                    return new AcceptanceReport("RoleApparelRequirementIncompatibleFaction".Translate(Find.ActiveLanguageWorker.WithIndefiniteArticle((from t in A_1.def.apparel.ideoDesireAllowedFactionCategoryTags
                                                                                                                                                       select ("RoleApparelRequirementIncompatibleFaction_Allowed_" + t).Translate().Resolve()).ToCommaListOr(false), false, false)));
                    */
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

        public static AcceptanceReport CheckFaction(AcceptanceReport __result, FactionDef faction)
        {
            if (def.defName.StartsWith("OG") && def.apparel.canBeDesiredForIdeo && faction.categoryTag != null)
            {
                Log.Message("Testing: {" + def + "} For {" + faction + "}");
                if (def.apparel.ideoDesireAllowedFactionCategoryTags != null && !def.apparel.ideoDesireAllowedFactionCategoryTags.Any(x => x.StartsWith(faction.categoryTag)))
                {
                    Log.Message("Fail: {" + def + "} For {" + faction + "} ideoDesireAllowedFactionCategoryTags");
                    return new AcceptanceReport("RoleApparelRequirementIncompatibleFaction".Translate(Find.ActiveLanguageWorker.WithIndefiniteArticle(def.apparel.ideoDesireAllowedFactionCategoryTags.Where(x => !x.Contains("Refugee")).Select((string t) => ("RoleApparelRequirementIncompatibleFaction_Allowed_" + t).Translate().Resolve()).ToCommaListOr())));
                }
                if (def.apparel.ideoDesireDisallowedFactionCategoryTags != null && def.apparel.ideoDesireDisallowedFactionCategoryTags.Contains(faction.categoryTag))
                {
                    Log.Message("Fail: {" + def + "} For {" + faction + "} ideoDesireDisallowedFactionCategoryTags");
                    return new AcceptanceReport("RoleApparelRequirementIncompatibleFaction".Translate(Find.ActiveLanguageWorker.WithIndefiniteArticle(def.apparel.ideoDesireDisallowedFactionCategoryTags.Select((string t) => ("RoleApparelRequirementIncompatibleFaction_Disallowed_" + t).Translate().Resolve()).ToCommaListOr())));
                }
                Log.Message("Sucess: {" + def + "} For {" + faction + "}");
                return true;
            }
            return __result;
        }
    }
    
    [HarmonyPatch(typeof(PreceptWorker_Apparel), "IsValidApparel", new Type[] { typeof(ThingDef)})]
    public static class PreceptWorker_Apparel_IsValidApparel_FactionPreferredApparel_Patch
    {

        [HarmonyPostfix]
        public static bool IsValidApparel(bool __result, PreceptWorker_Apparel __instance, ThingDef td)
        {
            if (!__result && !td.MadeFromStuff && td.defName.StartsWith("OG"))
            {
                if (!td.IsApparel)
                {
                    return false;
                }
                if (!td.apparel.canBeDesiredForIdeo)
                {
                    return false;
                }
                if (td.thingCategories != null && (td.thingCategories.Any(x=> x.defName.Contains("ArmorHeadgear")) || td.thingCategories.Any(x => x.defName.Contains("ApparelArmor")) || td.thingCategories.Any(x => x.defName.Contains("Wargear"))))
                {
                    return false;
                }
                for (int i = 0; i < __instance.def.comps.Count; i++)
                {
                    PreceptComp_Apparel preceptComp_Apparel;
                    if ((preceptComp_Apparel = (__instance.def.comps[i] as PreceptComp_Apparel)) != null && !preceptComp_Apparel.CanApplyToApparel(td))
                    {
                        return false;
                    }
                }
                return true;
            }
            return __result;
        }
    }
    
}
