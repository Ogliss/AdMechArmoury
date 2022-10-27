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

    [HarmonyPatch(typeof(IdeoFoundation), "RandomizeCulture")]
	public static class IdeoFoundation_RandomizeCulture_FactionSpecifc_Patch
	{
		// patch stops 40k cultures being randomly generated on non-40k factions
		public static List<RimWorld.CultureDef> Cultures = DefDatabase<RimWorld.CultureDef>.AllDefsListForReading.FindAll(x => x as CultureDef == null);
		public static void Postfix(IdeoFoundation __instance, IdeoGenerationParms parms)
		{
            if (parms.forFaction != null)
            {
                if (__instance.ideo.culture is CultureDef def && (parms.forFaction.allowedCultures.NullOrEmpty() || !parms.forFaction.allowedCultures.Contains(def)))
				{
					__instance.ideo.culture = Cultures.RandomElement<RimWorld.CultureDef>();
				//	Log.Message($"used {__instance.ideo.culture} culture for {parms.forFaction}");
				}
            }
		}
	}
}
