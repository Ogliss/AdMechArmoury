using System;
using System.Collections.Generic;
using AdeptusMechanicus.settings;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
    public class Listing_StandardExpanding : Listing_Standard
	{
		public Listing_StandardExpanding(GameFont font)
		{
			this.font = font;
		}

		public Listing_StandardExpanding()
		{
			this.font = GameFont.Small;
		}

		protected new void NewColumnIfNeeded(float neededHeight)
		{
			if (this.maxOneColumn)
			{
				return;
			}
			if (this.curY + neededHeight > this.listingRect.height)
			{
				this.NewColumn();
			}
		}
		public override void Begin(Rect rect)
		{
			base.Begin(rect);
			Text.Font = this.font;
		}

		public Rect GetRect(float height, Listing_StandardExpanding parent = null, bool extend = false)
		{
			this.NewColumnIfNeeded(height);
			Rect result = new Rect(this.curX, this.curY, this.ColumnWidth, height);
			this.curY += height;
			return result;
		}

		public new bool maxOneColumn = true;
		public bool ButtonTextLine(string label, string highlightTag = null, bool extend = false)
		{
			Rect rect = this.GetRect(Text.LineHeight, null, extend);
			bool result = Widgets.ButtonText(rect, label, true, true, true);
			if (highlightTag != null)
			{
				UIHighlighter.HighlightOpportunity(rect, highlightTag);
			}
			this.Gap(this.verticalSpacing);
			return result;
		}

		// Token: 0x06001B7B RID: 7035 RVA: 0x000A80E6 File Offset: 0x000A62E6
		public void TextFieldNumericLabeled<T>(string label, ref T val, ref string buffer, float min = 0f, float max = 1E+09f, string tooltip = null, float textpart = 0.75f, float boxpart = 0.25f) where T : struct
		{
			AdeptusWidgets.TextFieldNumericLabeled<T>(this.GetRect(Text.LineHeight), label, ref val, ref buffer, min, max, tooltip, textpart, boxpart);
			this.Gap(this.verticalSpacing);
		}

		public Listing_StandardExpanding BeginSection(float height, bool hidesection = false, int type = 0, int sectionBorder = 4, int bottomBorder = 4, Listing_StandardExpanding parent = null)
		{
			return BeginSection(height, out Rect out1, out Rect out2, hidesection, type, sectionBorder, bottomBorder, parent, false);
		}

		public Listing_StandardExpanding BeginSection(float height, out Rect frame, out Rect contents, bool hidesection = false, int type = 0, int sectionBorder = 4, int bottomBorder = 4, Listing_StandardExpanding parent = null, bool extend = false)
		{
			frame = this.GetRect(height + sectionBorder + bottomBorder, null, extend);
			if (!hidesection)
			{
				switch (type)
				{
					case 1:
						Widgets.DrawWindowBackground(frame);
						break;
					case 2:
						Widgets.DrawWindowBackgroundTutor(frame);
						break;
					case 3:
						Widgets.DrawOptionUnselected(frame);
						break;
					case 4:
						Widgets.DrawOptionSelected(frame);
						break;
					default:
						Widgets.DrawMenuSection(frame);
						break;
				}
			}
			Listing_StandardExpanding listing_Standard = new Listing_StandardExpanding();
			contents = new Rect(frame.x + sectionBorder, frame.y + sectionBorder, frame.width - sectionBorder * 2f, frame.height - (sectionBorder + bottomBorder));
			listing_Standard.frameRect = frame;
			listing_Standard.parent = this;
			listing_Standard.Begin(contents);
			return listing_Standard;
		}

		public bool ButtonText(string label, ref bool setting, string highlightTag = null, bool extend = false)
		{
			Rect rect = this.GetRect(30, null, extend);
			if (Widgets.ButtonText(rect, label, true, true, true))
			{
				setting = !setting;
			}
			if (highlightTag != null)
			{
				UIHighlighter.HighlightOpportunity(rect, highlightTag);
			}
			if (!highlightTag.NullOrEmpty()) TooltipHandler.TipRegion(rect, highlightTag);
			this.Gap(this.verticalSpacing);
			return setting;
		}
		public bool ButtonText(string label, ref bool setting, bool dev, ref float modifier, string highlightTag = null, bool extend = false)
		{
			Rect rect = this.GetRect(30, null, extend);
            if (dev)
			{
				Rect r = rect.RightPart(0.25f);
				rect = rect.LeftPart(0.75f);
				if (Widgets.ButtonText(r.LeftHalf(), "-"))
				{
					modifier--;
				}
				if (Widgets.ButtonText(r.RightHalf(), "+"))
				{
					modifier++;
				}
			}
			if (Widgets.ButtonText(rect, label, true, true, true))
			{
				setting = !setting;
			}
			if (highlightTag != null)
			{
				UIHighlighter.HighlightOpportunity(rect, highlightTag);
			}
			if (!highlightTag.NullOrEmpty()) TooltipHandler.TipRegion(rect, highlightTag);
			this.Gap(this.verticalSpacing);
			return setting;
		}
		/*
		public void CheckboxLabeled(string label, ref bool checkOn, string tooltip = null, bool disabled = false, bool highlight = false, Texture2D texChecked = null, Texture2D texUnchecked = null, bool placeCheckboxNearText = false)
		{
			float lineHeight = Text.LineHeight;
			Rect rect = this.GetRect(lineHeight);
			if (!tooltip.NullOrEmpty() || highlight)
			{
				if (Mouse.IsOver(rect))
				{
					Widgets.DrawHighlight(rect);
				}
				if (!tooltip.NullOrEmpty()) TooltipHandler.TipRegion(rect, tooltip);
			}
			AdeptusWidgets.CheckboxLabeled(rect, label, ref checkOn, disabled, texChecked, texUnchecked, placeCheckboxNearText);
			this.Gap(this.verticalSpacing);
		}
		*/
		public bool CheckboxLabeledSection(string label, ref bool checkOn, string tooltip = null, bool disabled = false, bool highlight = false, Texture2D texChecked = null, Texture2D texUnchecked = null, bool placeCheckboxNearText = false, bool extend = false)
        {

			return checkOn;
		}
		public void RaceSettingLabeled(string label, RaceSettingHandle raceSetting, string tooltip = null, bool disabled = false, bool highlight = false, Texture2D texChecked = null, Texture2D texUnchecked = null)
		{
			float lineHeight = Text.LineHeight;
			Rect rect = this.GetRect(lineHeight);
			if (!tooltip.NullOrEmpty() || highlight)
			{
				if (Mouse.IsOver(rect))
				{
					Widgets.DrawHighlight(rect);
				}
				if (!tooltip.NullOrEmpty()) TooltipHandler.TipRegion(rect, tooltip);
			}
			AdeptusWidgets.RaceSettingLabeled(rect, raceSetting, disabled, texChecked, texUnchecked);
			this.Gap(this.verticalSpacing);
		}
		public bool CheckboxLabeled(string label, ref bool checkOn, string tooltip = null, bool disabled = false, bool highlight = false, Texture2D texChecked = null, Texture2D texUnchecked = null, bool placeCheckboxNearText = false, bool extend = false)
		{
			float lineHeight = Text.LineHeight;
			Rect rect = this.GetRect(lineHeight, null, extend);
			if (!tooltip.NullOrEmpty() || highlight)
			{
				if (Mouse.IsOver(rect))
				{
					Widgets.DrawHighlight(rect);
				}
				if (!tooltip.NullOrEmpty()) TooltipHandler.TipRegion(rect, tooltip);
			}
			AdeptusWidgets.CheckboxLabeled(rect, label, ref checkOn, disabled, texChecked, texUnchecked, placeCheckboxNearText);
			this.Gap(this.verticalSpacing);
			return checkOn;
		}
		public bool CheckboxLabeled(string label, ref bool checkOn, bool dev, ref float modifier, string tooltip = null, bool disabled = false, bool highlight = false, Texture2D texChecked = null, Texture2D texUnchecked = null, bool placeCheckboxNearText = false, bool extend = false)
		{
			float lineHeight = Text.LineHeight;
			Rect rect = this.GetRect(lineHeight, null, extend);
			if (dev)
			{
				Rect r = rect.RightPart(0.25f);
				rect = rect.LeftPart(0.75f);
				if (Widgets.ButtonText(r.LeftHalf(), "-"))
				{
					modifier--;
				}
				if (Widgets.ButtonText(r.RightHalf(), "+"))
				{
					modifier++;
				}
			}
			if (!tooltip.NullOrEmpty() || highlight)
			{
				if (Mouse.IsOver(rect))
				{
					Widgets.DrawHighlight(rect);
				}
				if (!tooltip.NullOrEmpty()) TooltipHandler.TipRegion(rect, tooltip);
			}
			AdeptusWidgets.CheckboxLabeled(rect, label, ref checkOn, disabled, texChecked, texUnchecked, placeCheckboxNearText);
			this.Gap(this.verticalSpacing);
			return checkOn;
		}

		public void EndSection(Listing_StandardExpanding listing)
		{
			listing.End();
		}

		public override void End()
		{
			base.End();
		}

        public void ExtendRec()
		{
            if (this.curY > this.listingRect.height)
			{

				listingRect.height = this.curY;
			}
		}

		public void ExtendRec(float f)
		{
			curY += f;
			if (this.listingRect != null)
			{
				this.listingRect.height += f;
			}
			if (this.frameRect != null)
			{
				this.frameRect.height += f;
			}
		}
		public Listing_StandardExpanding parent;
		public Rect frameRect;

	}

}
