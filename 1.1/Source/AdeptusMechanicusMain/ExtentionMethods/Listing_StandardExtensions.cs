using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus.ExtensionMethods
{
    public static class Listing_StandardExtensions
    {
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
            TextFieldNumericLabeled<T>(L.GetRect(Text.LineHeight), label, ref val, ref buffer, min, max, tooltip, textpart, boxpart);
            L.Gap(L.verticalSpacing);
        }
        public static void TextFieldNumericLabeled<T>(Rect rect, string label, ref T val, ref string buffer, float min = 0f, float max = 1E+09f, string tooltip = null, float textpart = 0.75f, float boxpart = 0.25f) where T : struct
        {
            Rect rect2 = rect.LeftPart(textpart).Rounded();
            Rect rect3 = rect.RightPart(boxpart).Rounded();
            TextAnchor anchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect2, label);
            if (tooltip != null)
            {
                TooltipHandler.TipRegion(rect2, tooltip);
            }
            Text.Anchor = anchor;
            Widgets.TextFieldNumeric(rect3, ref val, ref buffer, min, max);
        }

        // Token: 0x06001B47 RID: 6983 RVA: 0x000A6A5C File Offset: 0x000A4C5C
        public static Listing_Standard BeginSection(this Listing_Standard L, float height, bool hidesection = false, int type = 0)
        {
            Rect rect = L.GetRect(height + 8f);
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
            listing_Standard.Begin(rect.ContractedBy(4f));
            return listing_Standard;
        }

        public static void CheckboxLabeled(this Listing_Standard listing_Standard, string label, ref bool checkOn, string tooltip = null, bool disabled = false, bool highlight = false)
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
            Widgets.CheckboxLabeled(rect, label, ref checkOn, disabled, null, null, false);
            listing_Standard.Gap(listing_Standard.verticalSpacing);
        }

    }
}
