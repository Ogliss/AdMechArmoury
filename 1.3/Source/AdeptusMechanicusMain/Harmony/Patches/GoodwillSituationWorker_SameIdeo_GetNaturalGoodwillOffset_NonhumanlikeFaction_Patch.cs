using HarmonyLib;
using RimWorld;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(GoodwillSituationWorker_SameIdeo), "GetNaturalGoodwillOffset")]
	public static class GoodwillSituationWorker_SameIdeo_GetNaturalGoodwillOffset_NonhumanlikeFaction_Patch
	{
		// default 0 value for non-humanlike Factions
		public static bool Prefix(GoodwillSituationWorker_SameIdeo __instance, Faction other,ref int __result)
		{
            if (__instance.def.defName.StartsWith("OG_") && !other.def.humanlikeFaction)
            {
				__result = 0;
				return false;
			}
			return false;
		}
	}
}
