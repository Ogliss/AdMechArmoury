using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
	// Token: AdeptusMechanicus.Page_ConfigureStartingPawnsSpecific
	public class Page_ConfigureStartingPawnsSpecific : Page
	{
		// Token: 0x170010F4 RID: 4340
		// (get) Token: 0x06005EC9 RID: 24265 RVA: 0x0020411E File Offset: 0x0020231E
		public override string PageTitle
		{
			get
			{
				return "CreateCharacters".Translate();
			}
		}

		// Token: 0x06005ECA RID: 24266 RVA: 0x0020412F File Offset: 0x0020232F
		public override void PreOpen()
		{
			base.PreOpen();
			if (Find.GameInitData.startingAndOptionalPawns.Count > 0)
			{
				this.curPawn = Find.GameInitData.startingAndOptionalPawns[0];
			}
		}

		// Token: 0x06005ECB RID: 24267 RVA: 0x0020415F File Offset: 0x0020235F
		public override void PostOpen()
		{
			base.PostOpen();
			TutorSystem.Notify_Event("PageStart-ConfigureStartingPawns");
		}

		// Token: 0x06005ECC RID: 24268 RVA: 0x00204178 File Offset: 0x00202378
		public override void DoWindowContents(Rect rect)
		{
			base.DrawPageTitle(rect);
			rect.yMin += 45f;
			base.DoBottomButtons(rect, "Start".Translate(), null, null, true, false);
			rect.yMax -= 38f;
			Rect rect2 = rect;
			rect2.width = 140f;
			this.DrawPawnList(rect2);
			UIHighlighter.HighlightOpportunity(rect2, "ReorderPawn");
			Rect rect3 = rect;
			rect3.xMin += 140f;
			Rect rect4 = rect3.BottomPartPixels(141f);
			rect3.yMax = rect4.yMin;
			rect3 = rect3.ContractedBy(4f);
			rect4 = rect4.ContractedBy(4f);
			this.DrawPortraitArea(rect3);
			this.DrawSkillSummaries(rect4);
		}

		// Token: 0x06005ECD RID: 24269 RVA: 0x00204244 File Offset: 0x00202444
		private void DrawPawnList(Rect rect)
		{
			Rect rect2 = rect;
			rect2.height = 60f;
			rect2 = rect2.ContractedBy(4f);
			int groupID = ReorderableWidget.NewGroup(delegate (int from, int to)
			{
				if (!TutorSystem.AllowAction("ReorderPawn"))
				{
					return;
				}
				Pawn item = Find.GameInitData.startingAndOptionalPawns[from];
				Find.GameInitData.startingAndOptionalPawns.Insert(to, item);
				Find.GameInitData.startingAndOptionalPawns.RemoveAt((from < to) ? from : (from + 1));
				TutorSystem.Notify_Event("ReorderPawn");
				if (to < Find.GameInitData.startingPawnCount && from >= Find.GameInitData.startingPawnCount)
				{
					TutorSystem.Notify_Event("ReorderPawnOptionalToStarting");
				}
			}, ReorderableDirection.Vertical, -1f, null);
			rect2.y += 15f;
			this.DrawPawnListLabelAbove(rect2, "StartingPawnsSelected".Translate());
			for (int i = 0; i < Find.GameInitData.startingAndOptionalPawns.Count; i++)
			{
				if (i == Find.GameInitData.startingPawnCount)
				{
					rect2.y += 30f;
					this.DrawPawnListLabelAbove(rect2, "StartingPawnsLeftBehind".Translate());
				}
				Pawn pawn = Find.GameInitData.startingAndOptionalPawns[i];
				GUI.BeginGroup(rect2);
				Rect rect3 = new Rect(Vector2.zero, rect2.size);
				Widgets.DrawOptionBackground(rect3, this.curPawn == pawn);
				MouseoverSounds.DoRegion(rect3);
				GUI.color = new Color(1f, 1f, 1f, 0.2f);
				GUI.DrawTexture(new Rect(110f - Page_ConfigureStartingPawnsSpecific.PawnSelectorPortraitSize.x / 2f, 40f - Page_ConfigureStartingPawnsSpecific.PawnSelectorPortraitSize.y / 2f, Page_ConfigureStartingPawnsSpecific.PawnSelectorPortraitSize.x, Page_ConfigureStartingPawnsSpecific.PawnSelectorPortraitSize.y), PortraitsCache.Get(pawn, Page_ConfigureStartingPawnsSpecific.PawnSelectorPortraitSize, default(Vector3), 1f, true, true));
				GUI.color = Color.white;
				Rect rect4 = rect3.ContractedBy(4f).Rounded();
				NameTriple nameTriple = pawn.Name as NameTriple;
				string label;
				if (nameTriple != null)
				{
					label = (string.IsNullOrEmpty(nameTriple.Nick) ? nameTriple.First : nameTriple.Nick);
				}
				else
				{
					label = pawn.LabelShort;
				}
				Widgets.Label(rect4.TopPart(0.5f).Rounded(), label);
				if (Text.CalcSize(pawn.story.TitleCap).x > rect4.width)
				{
					Widgets.Label(rect4.BottomPart(0.5f).Rounded(), pawn.story.TitleShortCap);
				}
				else
				{
					Widgets.Label(rect4.BottomPart(0.5f).Rounded(), pawn.story.TitleCap);
				}
				if (Event.current.type == EventType.MouseDown && Mouse.IsOver(rect3))
				{
					this.curPawn = pawn;
					SoundDefOf.Tick_Tiny.PlayOneShotOnCamera(null);
				}
				GUI.EndGroup();
				if (ReorderableWidget.Reorderable(groupID, rect2.ExpandedBy(4f), false))
				{
					Widgets.DrawRectFast(rect2, Widgets.WindowBGFillColor * new Color(1f, 1f, 1f, 0.5f), null);
				}
				if (Mouse.IsOver(rect2))
				{
					TooltipHandler.TipRegion(rect2, new TipSignal("DragToReorder".Translate(), pawn.GetHashCode() * 3499));
				}
				rect2.y += 60f;
			}
		}

		// Token: 0x06005ECE RID: 24270 RVA: 0x00204558 File Offset: 0x00202758
		private void DrawPawnListLabelAbove(Rect rect, string label)
		{
			rect.yMax = rect.yMin;
			rect.yMin -= 30f;
			rect.xMin -= 4f;
			Text.Font = GameFont.Tiny;
			Text.Anchor = TextAnchor.LowerLeft;
			Widgets.Label(rect, label);
			Text.Anchor = TextAnchor.UpperLeft;
			Text.Font = GameFont.Small;
		}

		// Token: 0x06005ECF RID: 24271 RVA: 0x002045B8 File Offset: 0x002027B8
		private void DrawPortraitArea(Rect rect)
		{
			Widgets.DrawMenuSection(rect);
			rect = rect.ContractedBy(17f);
			GUI.DrawTexture(new Rect(rect.center.x - Page_ConfigureStartingPawnsSpecific.PawnPortraitSize.x / 2f, rect.yMin - 24f, Page_ConfigureStartingPawnsSpecific.PawnPortraitSize.x, Page_ConfigureStartingPawnsSpecific.PawnPortraitSize.y), PortraitsCache.Get(this.curPawn, Page_ConfigureStartingPawnsSpecific.PawnPortraitSize, default(Vector3), 1f, true, true));
			Rect rect2 = rect;
			rect2.width = 500f;
			CharacterCardUtility.DrawCharacterCard(rect2, this.curPawn, new Action(this.RandomizeCurPawn), rect);
			Rect rect3 = rect;
			rect3.yMin += 100f;
			rect3.xMin = rect2.xMax + 5f;
			rect3.height = 200f;
			Text.Font = GameFont.Medium;
			Widgets.Label(rect3, "Health".Translate());
			Text.Font = GameFont.Small;
			rect3.yMin += 35f;
			HealthCardUtility.DrawHediffListing(rect3, this.curPawn, true);
			Rect rect4 = new Rect(rect3.x, rect3.yMax, rect3.width, 200f);
			Text.Font = GameFont.Medium;
			Widgets.Label(rect4, "Relations".Translate());
			Text.Font = GameFont.Small;
			rect4.yMin += 35f;
			SocialCardUtility.DrawRelationsAndOpinions(rect4, this.curPawn);
		}

		// Token: 0x06005ED0 RID: 24272 RVA: 0x00204734 File Offset: 0x00202934
		private void DrawSkillSummaries(Rect rect)
		{
			rect.xMin += 10f;
			rect.xMax -= 10f;
			Widgets.DrawMenuSection(rect);
			rect = rect.ContractedBy(17f);
			Text.Font = GameFont.Medium;
			Widgets.Label(new Rect(rect.min, new Vector2(rect.width, 45f)), "TeamSkills".Translate());
			Text.Font = GameFont.Small;
			rect.yMin += 45f;
			rect = rect.LeftPart(0.25f);
			rect.height = 27f;
			List<SkillDef> allDefsListForReading = DefDatabase<SkillDef>.AllDefsListForReading;
			if (this.SkillsPerColumn < 0)
			{
				this.SkillsPerColumn = Mathf.CeilToInt((float)(from sd in allDefsListForReading
															   where sd.pawnCreatorSummaryVisible
															   select sd).Count<SkillDef>() / 4f);
			}
			int num = 0;
			for (int i = 0; i < allDefsListForReading.Count; i++)
			{
				SkillDef skillDef = allDefsListForReading[i];
				if (skillDef.pawnCreatorSummaryVisible)
				{
					Rect r = rect;
					r.x = rect.x + rect.width * (float)(num / this.SkillsPerColumn);
					r.y = rect.y + rect.height * (float)(num % this.SkillsPerColumn);
					r.height = 24f;
					r.width -= 4f;
					Pawn pawn = this.FindBestSkillOwner(skillDef);
					SkillUI.DrawSkill(pawn.skills.GetSkill(skillDef), r.Rounded(), SkillUI.SkillDrawMode.Menu, pawn.Name.ToString());
					num++;
				}
			}
		}

		// Token: 0x06005ED1 RID: 24273 RVA: 0x002048EC File Offset: 0x00202AEC
		private Pawn FindBestSkillOwner(SkillDef skill)
		{
			Pawn pawn = Find.GameInitData.startingAndOptionalPawns[0];
			SkillRecord skillRecord = pawn.skills.GetSkill(skill);
			for (int i = 1; i < Find.GameInitData.startingPawnCount; i++)
			{
				SkillRecord skill2 = Find.GameInitData.startingAndOptionalPawns[i].skills.GetSkill(skill);
				if (skillRecord.TotallyDisabled || skill2.Level > skillRecord.Level || (skill2.Level == skillRecord.Level && skill2.passion > skillRecord.passion))
				{
					pawn = Find.GameInitData.startingAndOptionalPawns[i];
					skillRecord = skill2;
				}
			}
			return pawn;
		}

		// Token: 0x06005ED2 RID: 24274 RVA: 0x00204990 File Offset: 0x00202B90
		private void RandomizeCurPawn()
		{
			if (!TutorSystem.AllowAction("RandomizePawn"))
			{
				return;
			}
			int num = 0;
			do
			{
				SpouseRelationUtility.Notify_PawnRegenerated(this.curPawn);
				this.curPawn = StartingPawnUtility.RandomizeInPlace(this.curPawn);
				num++;
			}
			while (num <= 20 && !StartingPawnUtility.WorkTypeRequirementsSatisfied());
			TutorSystem.Notify_Event("RandomizePawn");
		}

		// Token: 0x06005ED3 RID: 24275 RVA: 0x002049EC File Offset: 0x00202BEC
		protected override bool CanDoNext()
		{
			if (!base.CanDoNext())
			{
				return false;
			}
			if (TutorSystem.TutorialMode)
			{
				WorkTypeDef workTypeDef = StartingPawnUtility.RequiredWorkTypesDisabledForEveryone().FirstOrDefault<WorkTypeDef>();
				if (workTypeDef != null)
				{
					Messages.Message("RequiredWorkTypeDisabledForEveryone".Translate() + ": " + workTypeDef.gerundLabel.CapitalizeFirst() + ".", MessageTypeDefOf.RejectInput, false);
					return false;
				}
			}
			using (List<Pawn>.Enumerator enumerator = Find.GameInitData.startingAndOptionalPawns.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.Name.IsValid)
					{
						Messages.Message("EveryoneNeedsValidName".Translate(), MessageTypeDefOf.RejectInput, false);
						return false;
					}
				}
			}
			PortraitsCache.Clear();
			return true;
		}

		// Token: 0x06005ED4 RID: 24276 RVA: 0x00204ACC File Offset: 0x00202CCC
		protected override void DoNext()
		{
			this.CheckWarnRequiredWorkTypesDisabledForEveryone(delegate
			{
				foreach (Pawn pawn in Find.GameInitData.startingAndOptionalPawns)
				{
					NameTriple nameTriple = pawn.Name as NameTriple;
					if (nameTriple != null && string.IsNullOrEmpty(nameTriple.Nick))
					{
						pawn.Name = new NameTriple(nameTriple.First, nameTriple.First, nameTriple.Last);
					}
				}
				base.DoNext();
			});
		}

		// Token: 0x06005ED5 RID: 24277 RVA: 0x00204AE0 File Offset: 0x00202CE0
		private void CheckWarnRequiredWorkTypesDisabledForEveryone(Action nextAction)
		{
			IEnumerable<WorkTypeDef> enumerable = StartingPawnUtility.RequiredWorkTypesDisabledForEveryone();
			if (enumerable.Any<WorkTypeDef>())
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (WorkTypeDef workTypeDef in enumerable)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.AppendLine();
					}
					stringBuilder.Append("  - " + workTypeDef.gerundLabel.CapitalizeFirst());
				}
				TaggedString text = "ConfirmRequiredWorkTypeDisabledForEveryone".Translate(stringBuilder.ToString());
				Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(text, nextAction, false, null));
				return;
			}
			nextAction();
		}

		// Token: 0x06005ED6 RID: 24278 RVA: 0x00204B98 File Offset: 0x00202D98
		public void SelectPawn(Pawn c)
		{
			if (c != this.curPawn)
			{
				this.curPawn = c;
			}
		}

		// Token: 0x040033F4 RID: 13300
		private Pawn curPawn;

		// Token: 0x040033F5 RID: 13301
		private const float TabAreaWidth = 140f;

		// Token: 0x040033F6 RID: 13302
		private const float RightRectLeftPadding = 5f;

		// Token: 0x040033F7 RID: 13303
		private const float PawnEntryHeight = 60f;

		// Token: 0x040033F8 RID: 13304
		private const float SkillSummaryHeight = 141f;

		// Token: 0x040033F9 RID: 13305
		private const int SkillSummaryColumns = 4;

		// Token: 0x040033FA RID: 13306
		private const int TeamSkillExtraInset = 10;

		// Token: 0x040033FB RID: 13307
		private static readonly Vector2 PawnPortraitSize = new Vector2(92f, 128f);

		// Token: 0x040033FC RID: 13308
		private static readonly Vector2 PawnSelectorPortraitSize = new Vector2(70f, 110f);

		// Token: 0x040033FD RID: 13309
		private int SkillsPerColumn = -1;
	}
}
