using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
	// Token: 0x020003B1 RID: 945
	public class Listing_Standardd : Listing
	{
		// Token: 0x06001C19 RID: 7193 RVA: 0x000AC7E5 File Offset: 0x000AA9E5
		public Listing_Standardd(GameFont font)
		{
			this.font = font;
		}

		// Token: 0x06001C1A RID: 7194 RVA: 0x000AC7F4 File Offset: 0x000AA9F4
		public Listing_Standardd()
		{
			this.font = GameFont.Small;
		}

		// Token: 0x06001C1B RID: 7195 RVA: 0x000AC803 File Offset: 0x000AAA03
		public override void Begin(Rect rect)
		{
			base.Begin(rect);
			Text.Font = this.font;
		}



		// Token: 0x06001C1C RID: 7196 RVA: 0x000AC817 File Offset: 0x000AAA17
		public void BeginScrollView(Rect rect, ref Vector2 scrollPosition, ref Rect viewRect)
		{
			Widgets.BeginScrollView(rect, ref scrollPosition, viewRect, true);
			rect.height = 100000f;
			rect.width -= 20f;
			this.Begin(rect.AtZero());
		}

		// Token: 0x06001C1D RID: 7197 RVA: 0x000AC854 File Offset: 0x000AAA54
		public override void End()
		{
			base.End();
			if (this.labelScrollbarPositions != null)
			{
				for (int i = this.labelScrollbarPositions.Count - 1; i >= 0; i--)
				{
					if (!this.labelScrollbarPositionsSetThisFrame.Contains(this.labelScrollbarPositions[i].First))
					{
						this.labelScrollbarPositions.RemoveAt(i);
					}
				}
				this.labelScrollbarPositionsSetThisFrame.Clear();
			}
		}

		// Token: 0x06001C1E RID: 7198 RVA: 0x000AC8BF File Offset: 0x000AAABF
		public void EndScrollView(ref Rect viewRect)
		{
			viewRect = new Rect(0f, 0f, this.listingRect.width, this.curY);
			Widgets.EndScrollView();
			this.End();
		}

		// Token: 0x06001C1F RID: 7199 RVA: 0x000AC8F2 File Offset: 0x000AAAF2
		public Rect Label(TaggedString label, float maxHeight = -1f, string tooltip = null)
		{
			return this.Label(label.Resolve(), maxHeight, tooltip);
		}

		// Token: 0x06001C20 RID: 7200 RVA: 0x000AC904 File Offset: 0x000AAB04
		public Rect Label(string label, float maxHeight = -1f, string tooltip = null)
		{
			float num = Text.CalcHeight(label, base.ColumnWidth);
			bool flag = false;
			if (maxHeight >= 0f && num > maxHeight)
			{
				num = maxHeight;
				flag = true;
			}
			Rect rect = base.GetRect(num);
			if (flag)
			{
				Vector2 labelScrollbarPosition = this.GetLabelScrollbarPosition(this.curX, this.curY);
				Widgets.LabelScrollable(rect, label, ref labelScrollbarPosition, false, true, false);
				this.SetLabelScrollbarPosition(this.curX, this.curY, labelScrollbarPosition);
			}
			else
			{
				Widgets.Label(rect, label);
			}
			if (tooltip != null)
			{
				TooltipHandler.TipRegion(rect, tooltip);
			}
			base.Gap(this.verticalSpacing);
			return rect;
		}

		// Token: 0x06001C21 RID: 7201 RVA: 0x000AC994 File Offset: 0x000AAB94
		public void LabelDouble(string leftLabel, string rightLabel, string tip = null)
		{
			float num = base.ColumnWidth / 2f;
			float width = base.ColumnWidth - num;
			float a = Text.CalcHeight(leftLabel, num);
			float b = Text.CalcHeight(rightLabel, width);
			float height = Mathf.Max(a, b);
			Rect rect = base.GetRect(height);
			if (!tip.NullOrEmpty())
			{
				Widgets.DrawHighlightIfMouseover(rect);
				TooltipHandler.TipRegion(rect, tip);
			}
			Widgets.Label(rect.LeftHalf(), leftLabel);
			Widgets.Label(rect.RightHalf(), rightLabel);
			base.Gap(this.verticalSpacing);
		}

		// Token: 0x06001C22 RID: 7202 RVA: 0x000ACA18 File Offset: 0x000AAC18
		[Obsolete]
		public bool RadioButton(string label, bool active, float tabIn = 0f, string tooltip = null)
		{
			return this.RadioButton_NewTemp(label, active, tabIn, tooltip, null);
		}

		// Token: 0x06001C23 RID: 7203 RVA: 0x000ACA3C File Offset: 0x000AAC3C
		public bool RadioButton_NewTemp(string label, bool active, float tabIn = 0f, string tooltip = null, float? tooltipDelay = null)
		{
			float lineHeight = Text.LineHeight;
			Rect rect = base.GetRect(lineHeight);
			rect.xMin += tabIn;
			if (!tooltip.NullOrEmpty())
			{
				if (Mouse.IsOver(rect))
				{
					Widgets.DrawHighlight(rect);
				}
				TipSignal tip = (tooltipDelay != null) ? new TipSignal(tooltip, tooltipDelay.Value) : new TipSignal(tooltip);
				TooltipHandler.TipRegion(rect, tip);
			}
			bool result = Widgets.RadioButtonLabeled(rect, label, active);
			base.Gap(this.verticalSpacing);
			return result;
		}

		// Token: 0x06001C24 RID: 7204 RVA: 0x000ACABC File Offset: 0x000AACBC
		public void CheckboxLabeled(string label, ref bool checkOn, string tooltip = null)
		{
			float lineHeight = Text.LineHeight;
			Rect rect = base.GetRect(lineHeight);
			if (!tooltip.NullOrEmpty())
			{
				if (Mouse.IsOver(rect))
				{
					Widgets.DrawHighlight(rect);
				}
				TooltipHandler.TipRegion(rect, tooltip);
			}
			Widgets.CheckboxLabeled(rect, label, ref checkOn, false, null, null, false);
			base.Gap(this.verticalSpacing);
		}

		// Token: 0x06001C25 RID: 7205 RVA: 0x000ACB14 File Offset: 0x000AAD14
		public bool CheckboxLabeledSelectable(string label, ref bool selected, ref bool checkOn)
		{
			float lineHeight = Text.LineHeight;
			bool result = Widgets.CheckboxLabeledSelectable(base.GetRect(lineHeight), label, ref selected, ref checkOn);
			base.Gap(this.verticalSpacing);
			return result;
		}

		// Token: 0x06001C26 RID: 7206 RVA: 0x000ACB44 File Offset: 0x000AAD44
		public bool ButtonText(string label, string highlightTag = null)
		{
			Rect rect = base.GetRect(30f);
			bool result = Widgets.ButtonText(rect, label, true, true, true);
			if (highlightTag != null)
			{
				UIHighlighter.HighlightOpportunity(rect, highlightTag);
			}
			base.Gap(this.verticalSpacing);
			return result;
		}

		// Token: 0x06001C27 RID: 7207 RVA: 0x000ACB7D File Offset: 0x000AAD7D
		public bool ButtonTextLabeled(string label, string buttonLabel)
		{
			Rect rect = base.GetRect(30f);
			Widgets.Label(rect.LeftHalf(), label);
			bool result = Widgets.ButtonText(rect.RightHalf(), buttonLabel, true, true, true);
			base.Gap(this.verticalSpacing);
			return result;
		}

		// Token: 0x06001C28 RID: 7208 RVA: 0x000ACBB0 File Offset: 0x000AADB0
		public bool ButtonImage(Texture2D tex, float width, float height)
		{
			base.NewColumnIfNeeded(height);
			bool result = Widgets.ButtonImage(new Rect(this.curX, this.curY, width, height), tex, true);
			base.Gap(height + this.verticalSpacing);
			return result;
		}

		// Token: 0x06001C29 RID: 7209 RVA: 0x000ACBE1 File Offset: 0x000AADE1
		public void None()
		{
			GUI.color = Color.gray;
			Text.Anchor = TextAnchor.UpperCenter;
			this.Label("NoneBrackets".Translate(), -1f, null);
			GenUI.ResetLabelAlign();
			GUI.color = Color.white;
		}

		// Token: 0x06001C2A RID: 7210 RVA: 0x000ACC1C File Offset: 0x000AAE1C
		public string TextEntry(string text, int lineCount = 1)
		{
			Rect rect = base.GetRect(Text.LineHeight * (float)lineCount);
			string result;
			if (lineCount == 1)
			{
				result = Widgets.TextField(rect, text);
			}
			else
			{
				result = Widgets.TextArea(rect, text, false);
			}
			base.Gap(this.verticalSpacing);
			return result;
		}

		// Token: 0x06001C2B RID: 7211 RVA: 0x000ACC5C File Offset: 0x000AAE5C
		public string TextEntryLabeled(string label, string text, int lineCount = 1)
		{
			string result = Widgets.TextEntryLabeled(base.GetRect(Text.LineHeight * (float)lineCount), label, text);
			base.Gap(this.verticalSpacing);
			return result;
		}

		// Token: 0x06001C2C RID: 7212 RVA: 0x000ACC7F File Offset: 0x000AAE7F
		public void TextFieldNumeric<T>(ref T val, ref string buffer, float min = 0f, float max = 1E+09f) where T : struct
		{
			Widgets.TextFieldNumeric<T>(base.GetRect(Text.LineHeight), ref val, ref buffer, min, max);
			base.Gap(this.verticalSpacing);
		}

		// Token: 0x06001C2D RID: 7213 RVA: 0x000ACCA2 File Offset: 0x000AAEA2
		public void TextFieldNumericLabeled<T>(string label, ref T val, ref string buffer, float min = 0f, float max = 1E+09f) where T : struct
		{
			Widgets.TextFieldNumericLabeled<T>(base.GetRect(Text.LineHeight), label, ref val, ref buffer, min, max);
			base.Gap(this.verticalSpacing);
		}

		// Token: 0x06001C2E RID: 7214 RVA: 0x000ACCC7 File Offset: 0x000AAEC7
		public void IntRange(ref IntRange range, int min, int max)
		{
			Widgets.IntRange(base.GetRect(28f), (int)base.CurHeight, ref range, min, max, null, 0);
			base.Gap(this.verticalSpacing);
		}

		// Token: 0x06001C2F RID: 7215 RVA: 0x000ACCF4 File Offset: 0x000AAEF4
		public float Slider(float val, float min, float max)
		{
			float num = Widgets.HorizontalSlider(base.GetRect(22f), val, min, max, false, null, null, null, -1f);
			if (num != val)
			{
				SoundDefOf.DragSlider.PlayOneShotOnCamera(null);
			}
			base.Gap(this.verticalSpacing);
			return num;
		}

		// Token: 0x06001C30 RID: 7216 RVA: 0x000ACD38 File Offset: 0x000AAF38
		public void IntAdjuster(ref int val, int countChange, int min = 0)
		{
			Rect rect = base.GetRect(24f);
			rect.width = 42f;
			if (Widgets.ButtonText(rect, "-" + countChange, true, true, true))
			{
				SoundDefOf.DragSlider.PlayOneShotOnCamera(null);
				val -= countChange * GenUI.CurrentAdjustmentMultiplier();
				if (val < min)
				{
					val = min;
				}
			}
			rect.x += rect.width + 2f;
			if (Widgets.ButtonText(rect, "+" + countChange, true, true, true))
			{
				SoundDefOf.DragSlider.PlayOneShotOnCamera(null);
				val += countChange * GenUI.CurrentAdjustmentMultiplier();
				if (val < min)
				{
					val = min;
				}
			}
			base.Gap(this.verticalSpacing);
		}

		// Token: 0x06001C31 RID: 7217 RVA: 0x000ACDF8 File Offset: 0x000AAFF8
		public void IntSetter(ref int val, int target, string label, float width = 42f)
		{
			if (Widgets.ButtonText(base.GetRect(24f), label, true, true, true))
			{
				SoundDefOf.Tick_Low.PlayOneShotOnCamera(null);
				val = target;
			}
			base.Gap(this.verticalSpacing);
		}

		// Token: 0x06001C32 RID: 7218 RVA: 0x000ACE2A File Offset: 0x000AB02A
		public void IntEntry(ref int val, ref string editBuffer, int multiplier = 1)
		{
			Widgets.IntEntry(base.GetRect(24f), ref val, ref editBuffer, multiplier);
			base.Gap(this.verticalSpacing);
		}

		// Token: 0x06001C33 RID: 7219 RVA: 0x000ACE4B File Offset: 0x000AB04B
		[Obsolete]
		public Listing_Standard BeginSection(float height)
		{
			return this.BeginSection_NewTemp(height, 4f, 4f);
		}

		// Token: 0x06001C34 RID: 7220 RVA: 0x000ACE60 File Offset: 0x000AB060
		public Listing_Standard BeginSection_NewTemp(float height, float sectionBorder = 4f, float bottomBorder = 4f)
		{
			Rect rect = base.GetRect(height + sectionBorder + bottomBorder);
			Widgets.DrawMenuSection(rect);
			Listing_Standard listing_Standard = new Listing_Standard();
			Rect rect2 = new Rect(rect.x + sectionBorder, rect.y + sectionBorder, rect.width - sectionBorder * 2f, rect.height - (sectionBorder + bottomBorder));
			listing_Standard.Begin(rect2);
			return listing_Standard;
		}

		// Token: 0x06001C35 RID: 7221 RVA: 0x000ACEBE File Offset: 0x000AB0BE
		public void EndSection(Listing_Standard listing)
		{
			listing.End();
		}

		// Token: 0x06001C36 RID: 7222 RVA: 0x000ACEC8 File Offset: 0x000AB0C8
		private Vector2 GetLabelScrollbarPosition(float x, float y)
		{
			if (this.labelScrollbarPositions == null)
			{
				return Vector2.zero;
			}
			for (int i = 0; i < this.labelScrollbarPositions.Count; i++)
			{
				Vector2 first = this.labelScrollbarPositions[i].First;
				if (first.x == x && first.y == y)
				{
					return this.labelScrollbarPositions[i].Second;
				}
			}
			return Vector2.zero;
		}

		// Token: 0x06001C37 RID: 7223 RVA: 0x000ACF3C File Offset: 0x000AB13C
		private void SetLabelScrollbarPosition(float x, float y, Vector2 scrollbarPosition)
		{
			if (this.labelScrollbarPositions == null)
			{
				this.labelScrollbarPositions = new List<Pair<Vector2, Vector2>>();
				this.labelScrollbarPositionsSetThisFrame = new List<Vector2>();
			}
			this.labelScrollbarPositionsSetThisFrame.Add(new Vector2(x, y));
			for (int i = 0; i < this.labelScrollbarPositions.Count; i++)
			{
				Vector2 first = this.labelScrollbarPositions[i].First;
				if (first.x == x && first.y == y)
				{
					this.labelScrollbarPositions[i] = new Pair<Vector2, Vector2>(new Vector2(x, y), scrollbarPosition);
					return;
				}
			}
			this.labelScrollbarPositions.Add(new Pair<Vector2, Vector2>(new Vector2(x, y), scrollbarPosition));
		}

		// Token: 0x06001C38 RID: 7224 RVA: 0x000ACFE8 File Offset: 0x000AB1E8
		public bool SelectableDef(string name, bool selected, Action deleteCallback)
		{
			Text.Font = GameFont.Tiny;
			float width = this.listingRect.width - 21f;
			Text.Anchor = TextAnchor.MiddleLeft;
			Rect rect = new Rect(this.curX, this.curY, width, 21f);
			if (selected)
			{
				Widgets.DrawHighlight(rect);
			}
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawBox(rect, 1);
			}
			Text.WordWrap = false;
			Widgets.Label(rect, name);
			Text.WordWrap = true;
			if (deleteCallback != null && Widgets.ButtonImage(new Rect(rect.xMax, rect.y, 21f, 21f), TexButton.DeleteX, Color.white, GenUI.SubtleMouseoverColor, true))
			{
				deleteCallback();
			}
			Text.Anchor = TextAnchor.UpperLeft;
			this.curY += 21f;
			return Widgets.ButtonInvisible(rect, true);
		}

		// Token: 0x06001C39 RID: 7225 RVA: 0x000AD0B3 File Offset: 0x000AB2B3
		[Obsolete("Only used for mod compatibility")]
		public void LabelCheckboxDebug(string label, ref bool checkOn)
		{
			this.LabelCheckboxDebug_NewTmp(label, ref checkOn, false);
		}

		// Token: 0x06001C3A RID: 7226 RVA: 0x000AD0C0 File Offset: 0x000AB2C0
		public void LabelCheckboxDebug_NewTmp(string label, ref bool checkOn, bool highlight)
		{
			Text.Font = GameFont.Tiny;
			base.NewColumnIfNeeded(22f);
			Rect rect = new Rect(this.curX, this.curY, base.ColumnWidth, 22f);
			Widgets.CheckboxLabeled(rect, label, ref checkOn, false, null, null, false);
			if (highlight)
			{
				GUI.color = Color.yellow;
				Widgets.DrawBox(rect, 2);
				GUI.color = Color.white;
			}
			base.Gap(22f + this.verticalSpacing);
		}

		// Token: 0x06001C3B RID: 7227 RVA: 0x000AD138 File Offset: 0x000AB338
		[Obsolete("Only used for mod compatibility")]
		public bool ButtonDebug(string label)
		{
			return this.ButtonDebug_NewTmp(label, false);
		}

		// Token: 0x06001C3C RID: 7228 RVA: 0x000AD144 File Offset: 0x000AB344
		public bool ButtonDebug_NewTmp(string label, bool highlight)
		{
			Text.Font = GameFont.Tiny;
			base.NewColumnIfNeeded(22f);
			bool wordWrap = Text.WordWrap;
			Text.WordWrap = false;
			Rect rect = new Rect(this.curX, this.curY, base.ColumnWidth, 22f);
			bool result = Widgets.ButtonText(rect, label, true, true, true);
			Text.WordWrap = wordWrap;
			if (highlight)
			{
				GUI.color = Color.yellow;
				Widgets.DrawBox(rect, 2);
				GUI.color = Color.white;
			}
			base.Gap(22f + this.verticalSpacing);
			return result;
		}

		// Token: 0x040010AA RID: 4266
		private GameFont font;

		// Token: 0x040010AB RID: 4267
		private List<Pair<Vector2, Vector2>> labelScrollbarPositions;

		// Token: 0x040010AC RID: 4268
		private List<Vector2> labelScrollbarPositionsSetThisFrame;

		// Token: 0x040010AD RID: 4269
		private const float DefSelectionLineHeight = 21f;
	}

	// Token: 0x020003D8 RID: 984
	[StaticConstructorOnStartup]
	internal class TexButton
	{
		// Token: 0x04001159 RID: 4441
		public static readonly Texture2D CloseXBig = ContentFinder<Texture2D>.Get("UI/Widgets/CloseX", true);

		// Token: 0x0400115A RID: 4442
		public static readonly Texture2D CloseXSmall = ContentFinder<Texture2D>.Get("UI/Widgets/CloseXSmall", true);

		// Token: 0x0400115B RID: 4443
		public static readonly Texture2D NextBig = ContentFinder<Texture2D>.Get("UI/Widgets/NextArrow", true);

		// Token: 0x0400115C RID: 4444
		public static readonly Texture2D DeleteX = ContentFinder<Texture2D>.Get("UI/Buttons/Delete", true);

		// Token: 0x0400115D RID: 4445
		public static readonly Texture2D ReorderUp = ContentFinder<Texture2D>.Get("UI/Buttons/ReorderUp", true);

		// Token: 0x0400115E RID: 4446
		public static readonly Texture2D ReorderDown = ContentFinder<Texture2D>.Get("UI/Buttons/ReorderDown", true);

		// Token: 0x0400115F RID: 4447
		public static readonly Texture2D Plus = ContentFinder<Texture2D>.Get("UI/Buttons/Plus", true);

		// Token: 0x04001160 RID: 4448
		public static readonly Texture2D Minus = ContentFinder<Texture2D>.Get("UI/Buttons/Minus", true);

		// Token: 0x04001161 RID: 4449
		public static readonly Texture2D Suspend = ContentFinder<Texture2D>.Get("UI/Buttons/Suspend", true);

		// Token: 0x04001162 RID: 4450
		public static readonly Texture2D SelectOverlappingNext = ContentFinder<Texture2D>.Get("UI/Buttons/SelectNextOverlapping", true);

		// Token: 0x04001163 RID: 4451
		public static readonly Texture2D Info = ContentFinder<Texture2D>.Get("UI/Buttons/InfoButton", true);

		// Token: 0x04001164 RID: 4452
		public static readonly Texture2D Rename = ContentFinder<Texture2D>.Get("UI/Buttons/Rename", true);

		// Token: 0x04001165 RID: 4453
		public static readonly Texture2D Banish = ContentFinder<Texture2D>.Get("UI/Buttons/Banish", true);

		// Token: 0x04001166 RID: 4454
		public static readonly Texture2D OpenStatsReport = ContentFinder<Texture2D>.Get("UI/Buttons/OpenStatsReport", true);

		// Token: 0x04001167 RID: 4455
		public static readonly Texture2D RenounceTitle = ContentFinder<Texture2D>.Get("UI/Buttons/Renounce", true);

		// Token: 0x04001168 RID: 4456
		public static readonly Texture2D Copy = ContentFinder<Texture2D>.Get("UI/Buttons/Copy", true);

		// Token: 0x04001169 RID: 4457
		public static readonly Texture2D Paste = ContentFinder<Texture2D>.Get("UI/Buttons/Paste", true);

		// Token: 0x0400116A RID: 4458
		public static readonly Texture2D Drop = ContentFinder<Texture2D>.Get("UI/Buttons/Drop", true);

		// Token: 0x0400116B RID: 4459
		public static readonly Texture2D Ingest = ContentFinder<Texture2D>.Get("UI/Buttons/Ingest", true);

		// Token: 0x0400116C RID: 4460
		public static readonly Texture2D DragHash = ContentFinder<Texture2D>.Get("UI/Buttons/DragHash", true);

		// Token: 0x0400116D RID: 4461
		public static readonly Texture2D ToggleLog = ContentFinder<Texture2D>.Get("UI/Buttons/DevRoot/ToggleLog", true);

		// Token: 0x0400116E RID: 4462
		public static readonly Texture2D OpenDebugActionsMenu = ContentFinder<Texture2D>.Get("UI/Buttons/DevRoot/OpenDebugActionsMenu", true);

		// Token: 0x0400116F RID: 4463
		public static readonly Texture2D OpenInspector = ContentFinder<Texture2D>.Get("UI/Buttons/DevRoot/OpenInspector", true);

		// Token: 0x04001170 RID: 4464
		public static readonly Texture2D OpenInspectSettings = ContentFinder<Texture2D>.Get("UI/Buttons/DevRoot/OpenInspectSettings", true);

		// Token: 0x04001171 RID: 4465
		public static readonly Texture2D ToggleGodMode = ContentFinder<Texture2D>.Get("UI/Buttons/DevRoot/ToggleGodMode", true);

		// Token: 0x04001172 RID: 4466
		public static readonly Texture2D TogglePauseOnError = ContentFinder<Texture2D>.Get("UI/Buttons/DevRoot/TogglePauseOnError", true);

		// Token: 0x04001173 RID: 4467
		public static readonly Texture2D ToggleTweak = ContentFinder<Texture2D>.Get("UI/Buttons/DevRoot/ToggleTweak", true);

		// Token: 0x04001174 RID: 4468
		public static readonly Texture2D Add = ContentFinder<Texture2D>.Get("UI/Buttons/Dev/Add", true);

		// Token: 0x04001175 RID: 4469
		public static readonly Texture2D NewItem = ContentFinder<Texture2D>.Get("UI/Buttons/Dev/NewItem", true);

		// Token: 0x04001176 RID: 4470
		public static readonly Texture2D Reveal = ContentFinder<Texture2D>.Get("UI/Buttons/Dev/Reveal", true);

		// Token: 0x04001177 RID: 4471
		public static readonly Texture2D Collapse = ContentFinder<Texture2D>.Get("UI/Buttons/Dev/Collapse", true);

		// Token: 0x04001178 RID: 4472
		public static readonly Texture2D Empty = ContentFinder<Texture2D>.Get("UI/Buttons/Dev/Empty", true);

		// Token: 0x04001179 RID: 4473
		public static readonly Texture2D Save = ContentFinder<Texture2D>.Get("UI/Buttons/Dev/Save", true);

		// Token: 0x0400117A RID: 4474
		public static readonly Texture2D NewFile = ContentFinder<Texture2D>.Get("UI/Buttons/Dev/NewFile", true);

		// Token: 0x0400117B RID: 4475
		public static readonly Texture2D RenameDev = ContentFinder<Texture2D>.Get("UI/Buttons/Dev/Rename", true);

		// Token: 0x0400117C RID: 4476
		public static readonly Texture2D Reload = ContentFinder<Texture2D>.Get("UI/Buttons/Dev/Reload", true);

		// Token: 0x0400117D RID: 4477
		public static readonly Texture2D Play = ContentFinder<Texture2D>.Get("UI/Buttons/Dev/Play", true);

		// Token: 0x0400117E RID: 4478
		public static readonly Texture2D Stop = ContentFinder<Texture2D>.Get("UI/Buttons/Dev/Stop", true);

		// Token: 0x0400117F RID: 4479
		public static readonly Texture2D RangeMatch = ContentFinder<Texture2D>.Get("UI/Buttons/Dev/RangeMatch", true);

		// Token: 0x04001180 RID: 4480
		public static readonly Texture2D InspectModeToggle = ContentFinder<Texture2D>.Get("UI/Buttons/Dev/InspectModeToggle", true);

		// Token: 0x04001181 RID: 4481
		public static readonly Texture2D CenterOnPointsTex = ContentFinder<Texture2D>.Get("UI/Buttons/Dev/CenterOnPoints", true);

		// Token: 0x04001182 RID: 4482
		public static readonly Texture2D CurveResetTex = ContentFinder<Texture2D>.Get("UI/Buttons/Dev/CurveReset", true);

		// Token: 0x04001183 RID: 4483
		public static readonly Texture2D QuickZoomHor1Tex = ContentFinder<Texture2D>.Get("UI/Buttons/Dev/QuickZoomHor1", true);

		// Token: 0x04001184 RID: 4484
		public static readonly Texture2D QuickZoomHor100Tex = ContentFinder<Texture2D>.Get("UI/Buttons/Dev/QuickZoomHor100", true);

		// Token: 0x04001185 RID: 4485
		public static readonly Texture2D QuickZoomHor20kTex = ContentFinder<Texture2D>.Get("UI/Buttons/Dev/QuickZoomHor20k", true);

		// Token: 0x04001186 RID: 4486
		public static readonly Texture2D QuickZoomVer1Tex = ContentFinder<Texture2D>.Get("UI/Buttons/Dev/QuickZoomVer1", true);

		// Token: 0x04001187 RID: 4487
		public static readonly Texture2D QuickZoomVer100Tex = ContentFinder<Texture2D>.Get("UI/Buttons/Dev/QuickZoomVer100", true);

		// Token: 0x04001188 RID: 4488
		public static readonly Texture2D QuickZoomVer20kTex = ContentFinder<Texture2D>.Get("UI/Buttons/Dev/QuickZoomVer20k", true);

		// Token: 0x04001189 RID: 4489
		public static readonly Texture2D IconBlog = ContentFinder<Texture2D>.Get("UI/HeroArt/WebIcons/Blog", true);

		// Token: 0x0400118A RID: 4490
		public static readonly Texture2D IconForums = ContentFinder<Texture2D>.Get("UI/HeroArt/WebIcons/Forums", true);

		// Token: 0x0400118B RID: 4491
		public static readonly Texture2D IconTwitter = ContentFinder<Texture2D>.Get("UI/HeroArt/WebIcons/Twitter", true);

		// Token: 0x0400118C RID: 4492
		public static readonly Texture2D IconBook = ContentFinder<Texture2D>.Get("UI/HeroArt/WebIcons/Book", true);

		// Token: 0x0400118D RID: 4493
		public static readonly Texture2D IconSoundtrack = ContentFinder<Texture2D>.Get("UI/HeroArt/WebIcons/Soundtrack", true);

		// Token: 0x0400118E RID: 4494
		public static readonly Texture2D ShowLearningHelper = ContentFinder<Texture2D>.Get("UI/Buttons/ShowLearningHelper", true);

		// Token: 0x0400118F RID: 4495
		public static readonly Texture2D ShowZones = ContentFinder<Texture2D>.Get("UI/Buttons/ShowZones", true);

		// Token: 0x04001190 RID: 4496
		public static readonly Texture2D ShowFertilityOverlay = ContentFinder<Texture2D>.Get("UI/Buttons/ShowFertilityOverlay", true);

		// Token: 0x04001191 RID: 4497
		public static readonly Texture2D ShowTerrainAffordanceOverlay = ContentFinder<Texture2D>.Get("UI/Buttons/ShowTerrainAffordanceOverlay", true);

		// Token: 0x04001192 RID: 4498
		public static readonly Texture2D ShowBeauty = ContentFinder<Texture2D>.Get("UI/Buttons/ShowBeauty", true);

		// Token: 0x04001193 RID: 4499
		public static readonly Texture2D ShowRoomStats = ContentFinder<Texture2D>.Get("UI/Buttons/ShowRoomStats", true);

		// Token: 0x04001194 RID: 4500
		public static readonly Texture2D ShowColonistBar = ContentFinder<Texture2D>.Get("UI/Buttons/ShowColonistBar", true);

		// Token: 0x04001195 RID: 4501
		public static readonly Texture2D ShowRoofOverlay = ContentFinder<Texture2D>.Get("UI/Buttons/ShowRoofOverlay", true);

		// Token: 0x04001196 RID: 4502
		public static readonly Texture2D AutoHomeArea = ContentFinder<Texture2D>.Get("UI/Buttons/AutoHomeArea", true);

		// Token: 0x04001197 RID: 4503
		public static readonly Texture2D AutoRebuild = ContentFinder<Texture2D>.Get("UI/Buttons/AutoRebuild", true);

		// Token: 0x04001198 RID: 4504
		public static readonly Texture2D CategorizedResourceReadout = ContentFinder<Texture2D>.Get("UI/Buttons/ResourceReadoutCategorized", true);

		// Token: 0x04001199 RID: 4505
		public static readonly Texture2D LockNorthUp = ContentFinder<Texture2D>.Get("UI/Buttons/LockNorthUp", true);

		// Token: 0x0400119A RID: 4506
		public static readonly Texture2D UsePlanetDayNightSystem = ContentFinder<Texture2D>.Get("UI/Buttons/UsePlanetDayNightSystem", true);

		// Token: 0x0400119B RID: 4507
		public static readonly Texture2D ShowExpandingIcons = ContentFinder<Texture2D>.Get("UI/Buttons/ShowExpandingIcons", true);

		// Token: 0x0400119C RID: 4508
		public static readonly Texture2D ShowWorldFeatures = ContentFinder<Texture2D>.Get("UI/Buttons/ShowWorldFeatures", true);

		// Token: 0x0400119D RID: 4509
		public static readonly Texture2D[] SpeedButtonTextures = new Texture2D[]
		{
			ContentFinder<Texture2D>.Get("UI/TimeControls/TimeSpeedButton_Pause", true),
			ContentFinder<Texture2D>.Get("UI/TimeControls/TimeSpeedButton_Normal", true),
			ContentFinder<Texture2D>.Get("UI/TimeControls/TimeSpeedButton_Fast", true),
			ContentFinder<Texture2D>.Get("UI/TimeControls/TimeSpeedButton_Superfast", true),
			ContentFinder<Texture2D>.Get("UI/TimeControls/TimeSpeedButton_Superfast", true)
		};
	}
}
