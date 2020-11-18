using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
	public class Dialog_FactionSpawning : Window
	{
		private FactionDef factionDef;
		private IEnumerator<FactionDef> factionEnumerator;
		private static Color colorCoreMod = new Color(125 / 255f, 97 / 255f, 51 / 255f);
		private static Color colorMod = new Color(115 / 255f, 162 / 255f, 47 / 255f);

		public static void OpenDialog(IEnumerator<FactionDef> enumerator)
		{
			Find.WindowStack.Add(new Dialog_FactionSpawning(enumerator));
		}

		private Dialog_FactionSpawning(IEnumerator<FactionDef> enumerator)
		{
			doCloseButton = false;
			forcePause = true;
			absorbInputAroundWindow = true;
			factionEnumerator = enumerator;
			factionDef = enumerator.Current;
		}

		public override void DoWindowContents(Rect inRect)
		{
			Listing_Standard listing_Standard = new Listing_Standard();
			listing_Standard.Begin(inRect.AtZero());

			// Icon
			if (factionDef.FactionIcon)
			{
				var rectIcon = listing_Standard.GetRect(64);
				var center = rectIcon.center.x;
				rectIcon.xMin = center - 32;
				rectIcon.xMax = center + 32;
				GUI.DrawTexture(rectIcon, factionDef.FactionIcon);
			}

			// Title
			Text.Font = GameFont.Medium;
			Text.Anchor = TextAnchor.MiddleCenter;
			listing_Standard.Label("AdeptusMechanicus.FactionTitle".Translate(new NamedArgument(factionDef.LabelCap, "FactionName")));
			listing_Standard.GapLine();

			// Description
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperLeft;
			var modName = GetModName();
			listing_Standard.Label("AdeptusMechanicus.ModInfo".Translate(new NamedArgument(modName, "ModName")));
			if (factionDef.hidden)
			{
				GUI.color = colorCoreMod;
				listing_Standard.Label("AdeptusMechanicus.HiddenFactionInfo".Translate());

				if (factionDef.requiredCountAtGameStart > 0)
				{
					listing_Standard.Label("AdeptusMechanicus.RequiredFactionInfo".Translate(new NamedArgument(modName, "ModName")));
				}
			}
			GUI.color = new Color(1f, 0.3f, 0.35f);
			if (!factionDef.canMakeRandomly && factionDef.requiredCountAtGameStart <= 0)
			{
				listing_Standard.Label("AdeptusMechanicus.NonSpawningFactionInfo".Translate());
			}
			GUI.color = Color.white;

			listing_Standard.Gap(40);
			listing_Standard.Label("AdeptusMechanicus.FactionSelectOption".Translate());
			listing_Standard.Gap(60);

			// Options
			if (!factionDef.hidden)
			{
				if (listing_Standard.ButtonText("AdeptusMechanicus.FactionButtonAddFull".Translate())) SpawnWithBases();
			}
			if (listing_Standard.ButtonText("AdeptusMechanicus.FactionButtonAdd".Translate())) SpawnWithoutBases();

			if (listing_Standard.ButtonText("AdeptusMechanicus.FactionButtonSkip".Translate())) Skip();
			GUI.color = new Color(1f, 0.3f, 0.35f);
			if (listing_Standard.ButtonText("AdeptusMechanicus.FactionButtonIgnore".Translate())) Ignore();
			GUI.color = Color.white;

			listing_Standard.End();
		}

		private void SpawnWithBases()
		{
			Dialog_FactionSpawningSettlements.OpenDialog(SpawnCallback);

			void SpawnCallback(int amount, int minDistance)
			{
				try
				{
					var faction = FactionSpawningUtility.SpawnWithSettlements(factionDef, amount, minDistance, out var spawned);
					if (faction == null || spawned == 0)
						Messages.Message("AdeptusMechanicus.FactionMessageFailedFull".Translate(), MessageTypeDefOf.RejectInput, false);
					else
					{
						Messages.Message("AdeptusMechanicus.FactionMessageSuccessFull".Translate(new NamedArgument(faction.GetCallLabel(), "FactionName"), new NamedArgument(spawned, "Amount")), MessageTypeDefOf.TaskCompletion);
						Close();
					}
				}
				catch (Exception e)
				{
					Log.Error($"An error occurred when trying to spawn faction {factionDef?.defName}:\n{e.Message}\n{e.StackTrace}");
					Messages.Message("AdeptusMechanicus.FactionMessageFailedFull".Translate(), MessageTypeDefOf.RejectInput, false);
				}
			}
		}

		private void SpawnWithoutBases()
		{
			try
			{
				var faction = FactionSpawningUtility.SpawnWithoutSettlements(factionDef);
				Messages.Message("AdeptusMechanicus.FactionMessageSuccess".Translate(new NamedArgument(faction.GetCallLabel(), "FactionName")), MessageTypeDefOf.TaskCompletion);
				Close();
			}
			catch (Exception e)
			{
				Log.Error($"An error occurred when trying to spawn faction {factionDef?.defName}:\n{e.Message}\n{e.StackTrace}");
				Messages.Message("AdeptusMechanicus.FactionMessageFailed".Translate(), MessageTypeDefOf.RejectInput, false);
			}
		}

		private void Skip()
		{
			Close();
		}

		private void Ignore()
		{
			Find.World.GetComponent<FactionSpawningState>().Ignore(factionDef);
			Close();
		}

		public override void PostClose()
		{
			if (factionEnumerator.MoveNext())
			{
				OpenDialog(factionEnumerator);
			}
		}

		private string GetModName()
		{
			if (factionDef?.modContentPack == null) return "AdeptusMechanicus.AnUnknownMod".Translate();
			if (factionDef.modContentPack.IsCoreMod) return factionDef.modContentPack.Name.Colorize(colorCoreMod);
			return factionDef.modContentPack.Name.Colorize(colorMod);
		}
	}
}