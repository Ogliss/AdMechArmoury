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
using UnityEngine;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(MainTabWindow_Research), "ViewSize")]
    public static class MainTabWindow_Research_ViewSize_SubTabs_Patch
	{
		[HarmonyPrefix]
        public static void Prefix(MainTabWindow_Research __instance, ref ResearchTabDef tab, ref Vector2 __result)
		{
            if (tab == AdeptusResearchTabDefOf.OGAMA_RTab)
            {
                tab = ResearchSubTabUtility.CurSubTab;
            }
		}

	}
    
}
