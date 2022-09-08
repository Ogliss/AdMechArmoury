using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
	public class Listing_StandardExpanding_Fucked : Listing_Standard
	{
		public Listing_StandardExpanding_Fucked(GameFont font)
		{
			this.font = font;
		}

		public Listing_StandardExpanding_Fucked()
		{
			this.font = GameFont.Small;
		}

		public override void Begin(Rect rect)
		{
			base.Begin(rect);
			Text.Font = this.font;
		}

		public bool ButtonTextLine(string label, string highlightTag = null, bool extend = false)
		{
			Rect rect = this.GetRect(Text.LineHeight);
			bool result = Widgets.ButtonText(rect, label, true, true, true);
			if (highlightTag != null)
			{
				UIHighlighter.HighlightOpportunity(rect, highlightTag);
			}
			this.Gap(this.verticalSpacing);
			return result;
		}

		public void TextFieldNumericLabeled<T>(string label, ref T val, ref string buffer, float min = 0f, float max = 1E+09f, string tooltip = null, float textpart = 0.75f, float boxpart = 0.25f) where T : struct
		{
			AdeptusWidgets.TextFieldNumericLabeled<T>(this.GetRect(Text.LineHeight), label, ref val, ref buffer, min, max, tooltip, textpart, boxpart);
			this.Gap(this.verticalSpacing);
		}

		public Listing_StandardExpanding_Fucked BeginSection(float height, bool hidesection = false, int type = 0, int sectionBorder = 4, int bottomBorder = 4, bool extend = true)
		{
			this.bottomBorder = bottomBorder;
			this.sectionBorder = sectionBorder;
			Rect rect = this.GetRect(height + sectionBorder + bottomBorder);
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
			Listing_StandardExpanding_Fucked listing_Standard = new Listing_StandardExpanding_Fucked();
			Rect rect2 = new Rect(rect.x + sectionBorder, rect.y + sectionBorder, rect.width - sectionBorder * 2f, rect.height - (sectionBorder + bottomBorder));
			listing_Standard.frameRect = rect;
			listing_Standard.parent = this;
			listing_Standard.Begin(rect2);
			return listing_Standard;
		}

		public new Rect GetRect(float height)
		{
			this.NewColumnIfNeeded(height);
			Rect result = new Rect(this.curX, this.curY, this.ColumnWidth, height);
			this.ExtendRec(height);
			return result;
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

		public new void NewColumn()
		{
			this.maxHeightColumnSeen = Math.Max(this.curY, this.maxHeightColumnSeen);
			this.curY = 0f;
			this.curX += this.ColumnWidth + 17f;
		}
		public Listing_StandardExpanding_Fucked BeginSection(float height, out Rect frame, out Rect contents, bool hidesection = false, int type = 0, int sectionBorder = 4, int bottomBorder = 4, bool extend = true)
		{
			this.bottomBorder = bottomBorder;
			this.sectionBorder = sectionBorder;
			frame = this.GetRect(height + sectionBorder + bottomBorder);
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
			Listing_StandardExpanding_Fucked listing_Standard = new Listing_StandardExpanding_Fucked();
			contents = new Rect(frame.x + sectionBorder, frame.y + sectionBorder, frame.width - sectionBorder * 2f, frame.height - (sectionBorder + bottomBorder));
			listing_Standard.frameRect = frame;
			listing_Standard.parent = this;
			listing_Standard.Begin(contents);
			return listing_Standard;
		}

		public bool ButtonText(string label, ref bool setting, string highlightTag = null, bool extend = false)
		{
			Rect rect = this.GetRect(30);
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
			Rect rect = this.GetRect(30);
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
			return checkOn;
		}

		public bool CheckboxLabeled(string label, ref bool checkOn, bool dev, ref float modifier, string tooltip = null, bool disabled = false, bool highlight = false, Texture2D texChecked = null, Texture2D texUnchecked = null, bool placeCheckboxNearText = false, bool extend = false)
		{
			float lineHeight = Text.LineHeight;
			Rect rect = this.GetRect(lineHeight);
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

		public new void EndSection(Listing_Standard listing)
		{
			listing.End();
		}
		public override void End()
		{
			ExtendRec();
			base.End();
			if (this.parent?.curY != null && this.parent.curY < this.curY)
			{
				if (logging) Log.Message($"Parent curY needs to extend {this.parent.listingRect.height} < {this.curY}");
			}
		}

		public void ExtendRec()
		{
			if (this.listingRect != null && this.listingRect.height < this.curY)
			{
				if (logging) Log.Message($"This listingRect.height needs to extend {this.listingRect.height} < {this.curY}");
				this.listingRect.height += this.curY;
			}
			if (this.frameRect != null && this.frameRect.height < sectionBorder + this.curY + bottomBorder)
			{
				this.frameRect.height += sectionBorder + this.curY + bottomBorder;
				if (logging) Log.Message($"This listingRect.height needs to extend {this.frameRect.height} < {sectionBorder + this.curY + bottomBorder}");
			}
			if (parent != null)
			{
				parent.curY += curY;
				parent.ExtendRec();
				if (this.parent?.listingRect != null && this.parent.listingRect.height < sectionBorder + this.curY + bottomBorder)
				{
					if (logging) Log.Message($"Parent listingRect.height needs to extend {this.parent.listingRect.height} < {this.curY}");
					this.parent.listingRect.height += sectionBorder + this.curY + bottomBorder;
				}
				if (this.parent?.frameRect != null && this.parent.frameRect.height < sectionBorder + this.curY + bottomBorder)
				{
					if (logging) Log.Message($"Parent frameRect.height needs to extend {this.parent.frameRect.height} < {sectionBorder + this.curY + bottomBorder}");
					this.parent.frameRect.height += sectionBorder + this.curY + bottomBorder;
				}
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

		public bool logging = false;
		public new bool maxOneColumn = true;
		private int sectionBorder = 4;
		private int bottomBorder = 4;
		public Rect frameRect;
		public Listing_StandardExpanding_Fucked parent;
	}
}
