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
using System.Text.RegularExpressions;

namespace AdeptusMechanicus.HarmonyInstance
{
	[HarmonyPatch(typeof(BackstoryDatabase), "GetIdentifierClosestMatch")]
    public static class BackCompatibility_Backstory_GetIdentifierClosestMatch_Patch
	{
		public static void Prefix(ref string identifier)
		{
			if (BackstoryDatabase.allBackstories.ContainsKey(identifier))
			{
				return;
			}
            if (identifier == "OG_Ork_Smart_Child")
            {
				identifier = "OG_Ork_Odd_Child";
				return;
			}
		//	Log.Message("Find matching backstory for: " + identifier);
			if (!identifier.StartsWith("OG_Imperial_") && BackstoryDatabase.allBackstories.ContainsKey("OG_Imperial_" + identifier))
			{
				identifier = "OG_Imperial_" + identifier;
			//	Log.Message("Final matching found: " + identifier + " Using: " + "OG_Imperial_");
				return;
			}
			if (!identifier.StartsWith("OG_") && BackstoryDatabase.allBackstories.ContainsKey("OG_" + identifier))
			{
				identifier = "OG_" + identifier;
			//	Log.Message("Final matching found: " + identifier + " Using: " + "OG_");
				return;
			}
			string newIdentifier = string.Empty;
			char[] charSeparators = new char[] { '_' };
			List<string> split = identifier.Split(charSeparators).ToList();
			List<string> factions = new List<string>() {"Imperial", "Chaos", "Eldar", "DarkEldar", "Tau", "Kroot", "Vespid", "Kroot", "Necron", "Tyranid"};
			string faction = string.Empty;
			List<string> subfactions = new List<string>() {"Craftworld", "Outcast", "Harlequin", "Outcast", "Tek", "Feral", "Mechanicus", "Militarum", "Astartes", "Ogryn", "Ratlin", "Beastmen" };
			string subfaction = string.Empty;
			List<KeyValuePair<string, Backstory>> found = new List<KeyValuePair<string, Backstory>>();

            foreach (var item in split)
			{
				if (item == "OG" || subfactions.Contains(item))
				{
					if (faction != subfaction && item != "OG") subfaction = item;
					continue;
				}
				if (factions.Contains(item))
				{
					faction = item;
					found = BackstoryDatabase.allBackstories.Where(x => x.Key.Contains(item)).ToList();
				}
                else
                {
					if (!found.NullOrEmpty())
					{
						found = found.Where(x => x.Key.Contains(item)).ToList();
					}
                    else
					{
						found = BackstoryDatabase.allBackstories.Where(x => x.Key.Contains(item)).ToList();
					}
				}
			//	if (!found.NullOrEmpty()) Log.Message(item+" found "+ found.Count+" matching");
			}
			if (!found.NullOrEmpty())
			{
				identifier = found.RandomElement().Key;
			//	Log.Message("Final matching found: " + found.Count+" Using: "+ identifier);
				return;
			}
		}
	}
}
