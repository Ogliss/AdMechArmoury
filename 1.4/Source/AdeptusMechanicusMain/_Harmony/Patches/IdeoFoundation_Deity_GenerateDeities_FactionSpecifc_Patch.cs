using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Grammar;

namespace AdeptusMechanicus.HarmonyInstance
{
	[HarmonyPatch(typeof(IdeoFoundation), "CanAddForFaction")]
	public static class IdeoFoundation_CanAddForFaction_FactionSpecifc_Patch
    {
		public static bool Prefix(IdeoFoundation __instance, PreceptDef precept, FactionDef forFaction)
		{
			if (__instance.ideo.culture is CultureDef def && def.deities != null && (!def.deities.requiredDeities.NullOrEmpty() || !def.deities.possibleDeities.NullOrEmpty()))
			{
			//	Log.Message($"Generating Deities for {__instance.ideo.culture.defName}");

			}
			return true;
		}
	}
	[HarmonyPatch(typeof(IdeoFoundation_Deity), "GenerateDeities")]
	public static class IdeoFoundation_Deity_GenerateDeities_FactionSpecifc_Patch
	{
		public static bool Prefix(IdeoFoundation_Deity __instance)
		{
			if (__instance.ideo.culture is CultureDef def && def.deities != null && (!def.deities.requiredDeities.NullOrEmpty() || !def.deities.possibleDeities.NullOrEmpty()))
			{
			//	Log.Message($"Generating Deities for {__instance.ideo.culture.defName}");
				DeityUtility.GenerateSpecificDeities(__instance);
				return false;
			}
			return true;
		}
	}
}
