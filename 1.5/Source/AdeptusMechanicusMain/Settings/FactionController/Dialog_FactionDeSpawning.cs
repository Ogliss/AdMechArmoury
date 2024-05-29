using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
	public class Dialog_FactionDeSpawning : Window
	{
		private Faction faction;
		private IEnumerator<Faction> factionEnumerator;
		private static Color colorCoreMod = new Color(125 / 255f, 97 / 255f, 51 / 255f);
		private static Color colorMod = new Color(115 / 255f, 162 / 255f, 47 / 255f);

		public static void OpenDialog(IEnumerator<Faction> enumerator)
		{
			Find.WindowStack.Add(new Dialog_FactionDeSpawning(enumerator));
		}

		private Dialog_FactionDeSpawning(IEnumerator<Faction> enumerator)
		{
			doCloseButton = false;
			forcePause = true;
			absorbInputAroundWindow = true;
			factionEnumerator = enumerator;
			faction = enumerator.Current;
		}

		public override void DoWindowContents(Rect inRect)
		{
			Listing_Standard listing_Standard = new Listing_Standard();
			listing_Standard.Begin(inRect.AtZero());

			// Icon
			if (faction.def.FactionIcon)
			{
				var rectIcon = listing_Standard.GetRect(64);
				var center = rectIcon.center.x;
				rectIcon.xMin = center - 32;
				rectIcon.xMax = center + 32;
				GUI.DrawTexture(rectIcon, faction.def.FactionIcon);
			}

			// Title
			Text.Font = GameFont.Medium;
			Text.Anchor = TextAnchor.MiddleCenter;
			listing_Standard.Label("AdeptusMechanicus.FactionTitle".Translate(new NamedArgument(faction.def.LabelCap, "FactionName")));
			listing_Standard.GapLine();

			// Description
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperLeft;
			var modName = GetModName();
			listing_Standard.Label("AdeptusMechanicus.ModInfo".Translate(new NamedArgument(modName, "ModName")));
			if (faction.def.hidden)
			{
				GUI.color = colorCoreMod;
				listing_Standard.Label("AdeptusMechanicus.HiddenFactionInfo".Translate());

				if (faction.def.requiredCountAtGameStart > 0)
				{
					listing_Standard.Label("AdeptusMechanicus.RequiredFactionInfo".Translate(new NamedArgument(modName, "ModName")));
				}
			}
			GUI.color = new Color(1f, 0.3f, 0.35f);
			if (!faction.def.canMakeRandomly && faction.def.requiredCountAtGameStart <= 0)
			{
				listing_Standard.Label("AdeptusMechanicus.NonSpawningFactionInfo".Translate());
			}
			GUI.color = Color.white;

			listing_Standard.Gap(40);
			listing_Standard.Label("AdeptusMechanicus.FactionSelectOption".Translate());
			listing_Standard.Gap(60);

			// Options
			if (!faction.def.hidden)
			{
				if (listing_Standard.ButtonText("AdeptusMechanicus.FactionButtonRemoveFull".Translate())) DeSpawnWithBases();
			}
			if (listing_Standard.ButtonText("AdeptusMechanicus.FactionButtonRemove".Translate())) DeSpawnWithoutBases();

			if (listing_Standard.ButtonText("AdeptusMechanicus.FactionButtonSkip".Translate())) Skip();
			GUI.color = new Color(1f, 0.3f, 0.35f);
			if (listing_Standard.ButtonText("AdeptusMechanicus.FactionButtonIgnore".Translate())) Ignore();
			GUI.color = Color.white;

			listing_Standard.End();
		}

		private void DeSpawnWithBases()
		{
			Dialog_FactionSpawningSettlements.OpenDialog(SpawnCallback);

			void SpawnCallback(int amount, int minDistance)
			{
				try
				{
					var faction = FactionDeSpawningUtility.DeSpawnWithSettlements(this.faction);
					if (faction == null)
						Messages.Message("AdeptusMechanicus.FactionMessageFailedFull".Translate(), MessageTypeDefOf.RejectInput, false);
					else
					{
						Messages.Message("AdeptusMechanicus.FactionMessageSuccessFull".Translate(new NamedArgument(faction.GetCallLabel(), "FactionName")), MessageTypeDefOf.TaskCompletion);
						Close();
					}
				}
				catch (Exception e)
				{
					Log.Error($"An error occurred when trying to spawn faction {faction?.def.defName}:\n{e.Message}\n{e.StackTrace}");
					Messages.Message("AdeptusMechanicus.FactionMessageFailedFull".Translate(), MessageTypeDefOf.RejectInput, false);
				}
			}
		}

		private void DeSpawnWithoutBases()
		{
			try
			{
				var faction = FactionDeSpawningUtility.DeSpawnWithoutSettlements(this.faction);
				Messages.Message("AdeptusMechanicus.FactionMessageSuccess".Translate(new NamedArgument(faction.GetCallLabel(), "FactionName")), MessageTypeDefOf.TaskCompletion);
				Close();
			}
			catch (Exception e)
			{
				Log.Error($"An error occurred when trying to remove faction {faction?.def.defName}:\n{e.Message}\n{e.StackTrace}");
				Messages.Message("AdeptusMechanicus.FactionMessageFailed".Translate(), MessageTypeDefOf.RejectInput, false);
			}
		}

		private void Skip()
		{
			Close();
		}

		private void Ignore()
		{
			Find.World.GetComponent<FactionSpawningState>().Ignore(faction.def);
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
			if (faction?.def.modContentPack == null) return "AdeptusMechanicus.AnUnknownMod".Translate();
			if (faction.def.modContentPack.IsCoreMod) return faction.def.modContentPack.Name.Colorize(colorCoreMod);
			return faction.def.modContentPack.Name.Colorize(colorMod);
		}
	}
}