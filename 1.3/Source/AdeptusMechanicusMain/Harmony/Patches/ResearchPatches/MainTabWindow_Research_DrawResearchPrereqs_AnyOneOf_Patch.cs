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
    [HarmonyPatch(typeof(MainTabWindow_Research), "DrawResearchPrereqs")]
	public static class MainTabWindow_Research_DrawResearchPrereqs_AnyOneOf_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(MainTabWindow_Research __instance, ResearchProjectDef project, Rect rect, ref float __result)
		{
			if (project.HasModExtension<AnyPrerequisiteResearchExtension>())
			{
				AnyPrerequisiteResearchExtension ext = project.GetModExtensionFast<AnyPrerequisiteResearchExtension>();
				if (project.prerequisites.NullOrEmpty<ResearchProjectDef>() && ext.RequiredResearch.NullOrEmpty())
				{
					return true;
				}
				float xMin = rect.xMin;
				float yMin = rect.yMin;
				Widgets.LabelCacheHeight(ref rect, "ResearchPrerequisites".Translate() + ":", true, false);
				rect.yMin += rect.height;
				rect.xMin += 6f;
				for (int i = 0; i < project.prerequisites.Count; i++)
				{
					SetPrerequisiteStatusColor(project.prerequisites[i].IsFinished, project);
					Widgets.LabelCacheHeight(ref rect, project.prerequisites[i].LabelCap, true, false);
					rect.yMin += rect.height;
				}
				GUI.color = Color.white;
				Widgets.LabelCacheHeight(ref rect, "and ONE of the following" + ":", true, false);
				rect.yMin += rect.height;
				rect.xMin += 6f;
				if (ext.RequiredResearch != null)
				{
					for (int j = 0; j < ext.RequiredResearch.Count; j++)
					{
						SetPrerequisiteStatusColor(ext.RequiredResearch[j].IsFinished, project);
						Widgets.LabelCacheHeight(ref rect, ext.RequiredResearch[j].LabelCap, true, false);
						rect.yMin += rect.height;
					}
				}
				GUI.color = Color.white;
				rect.xMin = xMin;
				__result = rect.yMin - yMin;
				return false;
			}
			return true;
		}
		private static void SetPrerequisiteStatusColor(bool present, ResearchProjectDef project)
		{
			if (project.IsFinished)
			{
				return;
			}
			if (present)
			{
				GUI.color = FulfilledPrerequisiteColor;
				return;
			}
			GUI.color = MissingPrerequisiteColor;
		}

		[HarmonyPostfix]
        public static void Postfix(ResearchProjectDef project, Rect rect, ref float __result)
        {

		}
		private static readonly Color FulfilledPrerequisiteColor = Color.green;

		private static readonly Color MissingPrerequisiteColor = ColoredText.ThreatColor;
	}
    
}
