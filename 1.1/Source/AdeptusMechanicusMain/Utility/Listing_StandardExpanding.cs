using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
    // Token: 0x020003B1 RID: 945
    public class Listing_StandardExpanding : Listing_Standard
	{
		// Token: 0x06001C19 RID: 7193 RVA: 0x000AC7E5 File Offset: 0x000AA9E5
		public Listing_StandardExpanding(GameFont font)
		{
			this.font = font;
		}

		// Token: 0x06001C1A RID: 7194 RVA: 0x000AC7F4 File Offset: 0x000AA9F4
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
		// Token: 0x06001C1B RID: 7195 RVA: 0x000AC803 File Offset: 0x000AAA03
		public override void Begin(Rect rect)
		{
			base.Begin(rect);
			Text.Font = this.font;
		}

		public Rect GetRect(float height, Listing_StandardExpanding parent = null, bool extend = false)
		{
            if (extend)
			{
				if (this.curY + height > this.listingRect.height)
				{
					this.listingRect.height = this.curY + height;
				}
			}
			this.NewColumnIfNeeded(height);
			Rect result = new Rect(this.curX, this.curY, this.ColumnWidth, height);
			this.curY += height;
            if (parent != null)
            {
				parent.curY += height;
			}
            if (listingRect.height < this.curY + height)
			{
				listingRect.height += height;
			}
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
		public void TextFieldNumericLabeled<T>(string label, ref T val, ref string buffer, float min = 0f, float max = 1E+09f, string tooltip = null, float textpart = 0.75f, float boxpart = 0.25f, bool extend = false) where T : struct
		{
			AdeptusWidgets.TextFieldNumericLabeled<T>(this.GetRect(Text.LineHeight), label, ref val, ref buffer, min, max, tooltip, textpart, boxpart);
			this.Gap(this.verticalSpacing);
		}

		public Listing_StandardExpanding BeginSection(float height, bool hidesection = false, int type = 0, int sectionBorder = 4, int bottomBorder = 4, Listing_StandardExpanding parent = null)
		{
			Rect rect = this.GetRect(height + sectionBorder + bottomBorder);
			this.frameRect = rect;
			if (parent != null)
			{
				parent.listingRect.height += rect.height;

			}
			if (!hidesection)
			{
				switch (type)
				{
					case 1:
						Widgets.DrawWindowBackground(rect);
						break;
					case 2:
						Widgets.DrawWindowBackgroundTutor(rect);
						break;
					case 3:
						Widgets.DrawOptionUnselected(rect);
						break;
					case 4:
						Widgets.DrawOptionSelected(rect);
						break;
					default:
						Widgets.DrawMenuSection(rect);
						break;
				}
			}
			Listing_StandardExpanding listing_Standard = new Listing_StandardExpanding();
			Rect rect2 = new Rect(rect.x + sectionBorder, rect.y + sectionBorder, rect.width - sectionBorder * 2f, rect.height - (sectionBorder + bottomBorder));
			listing_Standard.Begin(rect2);
			return listing_Standard;
		}

		public Listing_StandardExpanding BeginSection(float height, out Rect frane, bool hidesection = false, int type = 0, int sectionBorder = 4, int bottomBorder = 4, Listing_StandardExpanding parent = null, bool extend = false)
		{
			frane = this.GetRect(height + sectionBorder + bottomBorder, null, extend);
			this.frameRect = frane;
			if (parent != null)
			{
				parent.curY += frane.height;

			}
			if (!hidesection)
			{
				switch (type)
				{
					case 1:
						Widgets.DrawWindowBackground(frane);
						break;
					case 2:
						Widgets.DrawWindowBackgroundTutor(frane);
						break;
					case 3:
						Widgets.DrawOptionUnselected(frane);
						break;
					case 4:
						Widgets.DrawOptionSelected(frane);
						break;
					default:
						Widgets.DrawMenuSection(frane);
						break;
				}
			}
			Listing_StandardExpanding listing_Standard = new Listing_StandardExpanding();
			Rect rect2 = new Rect(frane.x + sectionBorder, frane.y + sectionBorder, frane.width - sectionBorder * 2f, frane.height - (sectionBorder + bottomBorder));

			listing_Standard.Begin(rect2);
			return listing_Standard;
		}

		public Listing_StandardExpanding BeginSection(float height, out Rect frane, out Rect contents, bool hidesection = false, int type = 0, int sectionBorder = 4, int bottomBorder = 4, Listing_StandardExpanding parent = null, bool extend = false)
		{
			frane = this.GetRect(height + sectionBorder + bottomBorder, null, extend);
			this.frameRect = frane;
			if (parent != null)
			{
				parent.curY += frane.height;

			}
			if (!hidesection)
			{
				switch (type)
				{
					case 1:
						Widgets.DrawWindowBackground(frane);
						break;
					case 2:
						Widgets.DrawWindowBackgroundTutor(frane);
						break;
					case 3:
						Widgets.DrawOptionUnselected(frane);
						break;
					case 4:
						Widgets.DrawOptionSelected(frane);
						break;
					default:
						Widgets.DrawMenuSection(frane);
						break;
				}
			}
			Listing_StandardExpanding listing_Standard = new Listing_StandardExpanding();
			contents = new Rect(frane.x + sectionBorder, frane.y + sectionBorder, frane.width - sectionBorder * 2f, frane.height - (sectionBorder + bottomBorder));

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
			this.listingRect.height += listing.listingRect.height;
		}

		public override void End()
		{
			extendRec();
			base.End();
		}

        public void extendRec()
		{
            if (this.curY > this.listingRect.height)
			{

				listingRect.height = this.curY;
			}
		}
		
        public void ExtendRec(float f)
		{
			listingRect.height += f;
			frameRect.height += f;
		}

		public new Rect listingRect;
		public Rect frameRect;

		// Token: 0x040010AA RID: 4266
		private GameFont font;

		// Token: 0x040010AB RID: 4267
		private List<Pair<Vector2, Vector2>> labelScrollbarPositions;

		// Token: 0x040010AC RID: 4268
		private List<Vector2> labelScrollbarPositionsSetThisFrame;

		// Token: 0x040010AD RID: 4269
		private const float DefSelectionLineHeight = 21f;
	}
}
