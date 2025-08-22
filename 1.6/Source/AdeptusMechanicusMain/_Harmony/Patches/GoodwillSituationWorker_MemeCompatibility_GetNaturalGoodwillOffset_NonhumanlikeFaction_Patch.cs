using HarmonyLib;
using RimWorld;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(GoodwillSituationWorker_MemeCompatibility), "GetNaturalGoodwillOffset")]
	public static class GoodwillSituationWorker_MemeCompatibility_GetNaturalGoodwillOffset_NonhumanlikeFaction_Patch
	{
		// default 0 value for non-humanlike Factions
		public static bool Prefix(GoodwillSituationWorker_MemeCompatibility __instance, Faction other,ref int __result)
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
