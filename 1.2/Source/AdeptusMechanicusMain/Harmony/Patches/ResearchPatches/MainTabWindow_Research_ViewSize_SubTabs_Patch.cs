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
            if (ResearchSubTabUtility.CurSubTab.IsSubTabOf(tab))
            {
                tab = ResearchSubTabUtility.CurSubTab;
            }
		}
        /*
		[HarmonyPostfix]
        public static void Postfix(MainTabWindow_Research __instance, ref ResearchTabDef tab, ref Vector2 __result)
		{
            if (tab == ResearchSubTabUtility.CurSubTab)
            {
                Vector2 result = __result;
                result.y -= 6;
                __result = result;
            }
		}
        */

	}
    
}
