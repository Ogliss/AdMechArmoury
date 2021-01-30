using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
    [StaticConstructorOnStartup]
    public class AdeptusWidgets
    {

        public static void CheckboxLabeled(Rect rect, string label, ref bool checkOn, bool disabled = false, Texture2D texChecked = null, Texture2D texUnchecked = null, bool placeCheckboxNearText = false)
        {
            TextAnchor anchor = Text.Anchor;
            Verse.Text.Anchor = TextAnchor.MiddleLeft;
            if (placeCheckboxNearText)
            {
                rect.width = Mathf.Min(rect.width, Text.CalcSize(label).x + 24f + 10f);
            }
            Widgets.Label(rect, label);
            if (!disabled && Widgets.ButtonInvisible(rect, true))
            {
                checkOn = !checkOn;
                if (checkOn)
                {
                    SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera(null);
                }
                else
                {
                    SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera(null);
                }
            }
            CheckboxDraw(rect.x + rect.width - 24f, rect.y, checkOn, disabled, 24f, texChecked, texUnchecked);
            Text.Anchor = anchor;
        }
        private static void CheckboxDraw(float x, float y, bool active, bool disabled, float size = 24f, Texture2D texChecked = null, Texture2D texUnchecked = null)
        {
            Color color = GUI.color;
            if (disabled)
            {
                GUI.color = InactiveColor;
            }
            Texture2D image;
            if (active)
            {
                image = ((texChecked != null) ? texChecked : Widgets.CheckboxOnTex);
            }
            else
            {
                image = ((texUnchecked != null) ? texUnchecked : Widgets.CheckboxOffTex);
            }
            GUI.DrawTexture(new Rect(x, y, size, size), image);
            if (disabled)
            {
                GUI.color = color;
            }
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

        public static void FloatRangeWithTypeIn(Rect rect, int id, ref IntRange fRange, int sliderMin = 0, int sliderMax = 1, string labelKey = null)
        {
            Rect rect2 = new Rect(rect);
            rect2.width = rect.width / 5f;
            Rect rect3 = new Rect(rect);
            rect3.width = rect.width / 2f;
            rect3.x = rect.x + rect.width / 5f;
            rect3.height = rect.height / 2f;
            rect3.width -= rect.height;
            Rect butRect = new Rect(rect3);
            butRect.x = rect3.xMax;
            butRect.height = rect.height;
            butRect.width = rect.height;
            Rect rect4 = new Rect(rect);
            rect4.x = rect.x + rect.width * 0.75f;
            rect4.width = rect.width / 4f;
            rect3.y += 4f;
            rect3.height += 4f;
            Widgets.IntRange(rect3, id, ref fRange, sliderMin, sliderMax, labelKey);
            if (Widgets.ButtonImage(butRect, RangeMatch, true))
            {
                fRange.max = fRange.min;
            }
            int.TryParse(Widgets.TextField(rect2, fRange.min.ToString()), out fRange.min);
            int.TryParse(Widgets.TextField(rect4, fRange.max.ToString()), out fRange.max);
        }

        public static Rect VertFillableBar(Rect rect, float fillPercent, Texture2D fillTex, Texture2D bgTex, bool doBorder)
        {
            if (doBorder)
            {
                GUI.DrawTexture(rect, BaseContent.BlackTex);
                rect = rect.ContractedBy(3f);
            }
            if (bgTex != null)
            {
                GUI.DrawTexture(rect, bgTex);
            }
            Rect result = rect;
            rect.height *= fillPercent;
            GUI.DrawTexture(rect, fillTex);
            return result;
        }

        public static readonly Texture2D RangeMatch = ContentFinder<Texture2D>.Get("UI/Buttons/Dev/RangeMatch", true);
        private static readonly Color InactiveColor = new Color(0.37f, 0.37f, 0.37f, 0.8f);
    }
}
