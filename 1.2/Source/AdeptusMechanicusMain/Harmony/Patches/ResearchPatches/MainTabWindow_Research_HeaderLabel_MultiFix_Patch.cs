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
    [HarmonyPatch(typeof(MainTabWindow_Research), "HeaderLabel")]
    public static class MainTabWindow_Research_HeaderLabel_MultiFix_Patch
	{
		private static readonly Color MissingPrerequisiteColor = ColoredText.RedReadable;
		[HarmonyPostfix]
        public static void Postfix(MainTabWindow_Research __instance, ResearchPrerequisitesUtility.UnlockedHeader headerProject, ref string __result)
		{
            if (headerProject.unlockedBy.NullOrEmpty() || headerProject.unlockedBy.Count == 1)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < headerProject.unlockedBy.Count; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(", ");
				}
				ResearchProjectDef researchProjectDef = headerProject.unlockedBy[i];
				string text = researchProjectDef.LabelCap;
				if (!researchProjectDef.IsFinished)
				{
					text = text.Colorize(MissingPrerequisiteColor);
				}
				stringBuilder.Append(text);
			}
			__result = stringBuilder.ToString();
		}
	}
    
}
