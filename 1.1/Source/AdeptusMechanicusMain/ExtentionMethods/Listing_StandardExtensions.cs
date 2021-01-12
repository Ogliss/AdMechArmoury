using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus.ExtensionMethods
{
    public static class Listing_StandardExtensions
    {
        public static Listing_Standard BeginSection_OnePointTwo(this Listing_Standard listing_Main, float f, float sectionBorder = 4f, float bottomBorder = 4f)
        {
            return listing_Main.BeginSection_NewTemp(f, sectionBorder, bottomBorder);
        }

        public static Listing_Standard BeginSection_OnePointOne(this Listing_Standard listing_Main, float f)
        {
            #pragma warning disable CS0612 // Type or member is obsolete
            return listing_Main.BeginSection(f);
            #pragma warning restore CS0612 // Type or member is obsolete
        }

        public static bool ButtonTextLine(this Listing_Standard L, string label, string highlightTag = null)
        {
            Rect rect = L.GetRect(Text.LineHeight);
            bool result = Widgets.ButtonText(rect, label, true, true, true);
            if (highlightTag != null)
            {
                UIHighlighter.HighlightOpportunity(rect, highlightTag);
            }
            L.Gap(L.verticalSpacing);
            return result;
        }

        // Token: 0x06001B7B RID: 7035 RVA: 0x000A80E6 File Offset: 0x000A62E6
        public static void TextFieldNumericLabeled<T>(this Listing_Standard L, string label, ref T val, ref string buffer, float min = 0f, float max = 1E+09f, string tooltip = null, float textpart = 0.75f, float boxpart = 0.25f) where T : struct
        {
            AdeptusWidgets.TextFieldNumericLabeled<T>(L.GetRect(Text.LineHeight), label, ref val, ref buffer, min, max, tooltip, textpart, boxpart);
            L.Gap(L.verticalSpacing);
        }

        // Token: 0x06001B47 RID: 6983 RVA: 0x000A6A5C File Offset: 0x000A4C5C
        public static Listing_Standard BeginSection(this Listing_Standard L, Listing_Standard listing, bool hidesection = false, int type = 0, int sectionBorder = 4, int bottomBorder = 4)
        {

            Rect rect = L.GetRect(listing.MaxColumnHeightSeen + sectionBorder + bottomBorder);
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
            Listing_Standard listing_Standard = new Listing_Standard();
            Rect rect2 = new Rect(rect.x + sectionBorder, rect.y + sectionBorder, rect.width - sectionBorder * 2f, rect.height - (sectionBorder + bottomBorder));

            listing_Standard.Begin(rect2);
            return listing_Standard;
        }
        
        // Token: 0x06001B47 RID: 6983 RVA: 0x000A6A5C File Offset: 0x000A4C5C
        public static Listing_Standard BeginSection(this Listing_Standard L, float height, bool hidesection = false, int type = 0, int sectionBorder = 4, int bottomBorder = 4)
        {
            Rect rect = L.GetRect(height + sectionBorder + bottomBorder);
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
            Listing_Standard listing_Standard = new Listing_Standard();
            Rect rect2 = new Rect(rect.x + sectionBorder, rect.y + sectionBorder, rect.width - sectionBorder * 2f, rect.height - (sectionBorder + bottomBorder));

            listing_Standard.Begin(rect2);
            return listing_Standard;
        }

        public static bool ButtonText(this Listing_Standard listing_Standard, string label, ref bool setting, string highlightTag = null)
        {
            Rect rect = listing_Standard.GetRect(30);
            if (Widgets.ButtonText(rect, label, true, true, true))
            {
                setting = !setting;
            }
            if (highlightTag != null)
            {
                UIHighlighter.HighlightOpportunity(rect, highlightTag);
            }
            if (!highlightTag.NullOrEmpty()) TooltipHandler.TipRegion(rect, highlightTag);
            listing_Standard.Gap(listing_Standard.verticalSpacing);
            return setting;
        }
        public static bool ButtonText(this Listing_Standard listing_Standard, Rect rect, string label, ref bool setting, string highlightTag = null)
        {
            if (Widgets.ButtonText(rect, label, true, true, true))
            {
                setting = !setting;
            }
            if (highlightTag != null)
            {
                UIHighlighter.HighlightOpportunity(rect, highlightTag);
            }
            listing_Standard.Gap(listing_Standard.verticalSpacing);
            return setting;
        }
        public static void CheckboxLabeled(this Listing_Standard listing_Standard, string label, ref bool checkOn, string tooltip = null, bool disabled = false, bool highlight = false, Texture2D texChecked = null, Texture2D texUnchecked = null, bool placeCheckboxNearText = false)
        {
            float lineHeight = Text.LineHeight;
            Rect rect = listing_Standard.GetRect(lineHeight);
            if (!tooltip.NullOrEmpty() || highlight)
            {
                if (Mouse.IsOver(rect))
                {
                    Widgets.DrawHighlight(rect);
                }
                if (!tooltip.NullOrEmpty()) TooltipHandler.TipRegion(rect, tooltip);
            }
            AdeptusWidgets.CheckboxLabeled(rect, label, ref checkOn, disabled, texChecked, texUnchecked, placeCheckboxNearText);
            listing_Standard.Gap(listing_Standard.verticalSpacing);
        }

    }
}
