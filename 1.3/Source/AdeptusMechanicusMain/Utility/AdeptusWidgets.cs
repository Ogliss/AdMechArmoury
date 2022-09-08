using AdeptusMechanicus.settings;
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
        public static Vector2 colorSelectorScrollPos = new Vector2(0f, 0f);
        public static bool ColorSelector(Rect rect, ref Color color, List<Color> colors, out float height, Texture icon = null, int colorSize = 22, int colorPadding = 2, ApparelComposite composite = null)
        {
            height = 0f;
            bool result = false;
            int num = colorSize + colorPadding * 2;
            float num2 = (icon != null) ? ((float)(colorSize * 4) + 10f) : 0f;
            int num3 = Mathf.FloorToInt((rect.width - num2) / (float)(num + colorPadding));
            int num4 = Mathf.CeilToInt((float)colors.Count / (float)num3);
            Widgets.BeginGroup(rect);
            if (icon != null)
            {
                Widgets.ColorSelectorIcon(new Rect(5f, 5f, (float)(colorSize * 4), (float)(colorSize * 4)), icon, color, colorSize);
            }
            int num5 = 0;
            foreach (Color color2 in colors)
            {
                int num6 = num5 / num3;
                int num7 = num5 % num3;
                float num8 = (icon != null) ? ((num2 - (float)(num * num4) - (float)colorPadding) / 2f) : 0f;
                Rect rect2 = new Rect(num2 + (float)(num7 * num) + (float)(num7 * colorPadding), num8 + (float)(num6 * num) + (float)(num6 * colorPadding), (float)num, (float)num);
                Widgets.DrawLightHighlight(rect2);
                Widgets.DrawHighlightIfMouseover(rect2);
                if (color.IndistinguishableFrom(color2))
                {
                    Widgets.DrawBox(rect2, 1, null);
                }
                Widgets.DrawBoxSolid(new Rect(rect2.x + (float)colorPadding, rect2.y + (float)colorPadding, (float)colorSize, (float)colorSize), color2);
                if (Widgets.ButtonInvisible(rect2, true))
                {
                    result = true;
                    color = color2;
                    SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
                    if (composite?.Wearer != null) AdeptusApparelUtility.UpdateApparelGraphicsFor(composite?.Wearer);
                }
                height = rect2.yMax + (float)colorPadding;
                num5++;
            }
            Widgets.EndGroup();
            return result;
        }
        public static void ScrollHorizontalAndVert(Rect outRect, ref Vector2 scrollPosition, Rect viewRect, float ScrollWheelSpeed = 20f)
        {
            if (Event.current.type == EventType.ScrollWheel && !KeyBindingDefOf.ModifierIncrement_10x.IsDown && Mouse.IsOver(outRect))
            {
                scrollPosition.x += Event.current.delta.y * ScrollWheelSpeed;
                float num = 0f;
                float num2 = viewRect.width - outRect.width + 16f;
                if (scrollPosition.x < num)
                {
                    scrollPosition.x = num;
                }
                if (scrollPosition.x > num2)
                {
                    scrollPosition.x = num2;
                }
                Event.current.Use();
            }
        }
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
        public static void RaceSettingLabeled(Rect rect, RaceSettingHandle raceSetting, bool disabled = false, Texture2D texChecked = null, Texture2D texUnchecked = null)
        {
            TextAnchor anchor = Text.Anchor;
            Verse.Text.Anchor = TextAnchor.MiddleLeft;
            Rect labelRect = rect.LeftPart(0.3f);
            Rect boxesRect = rect.RightPart(0.7f);
            float x = boxesRect.width / 14f;
            Widgets.Label(labelRect, raceSetting.Race.LabelCap);
            float num = x;
            CheckboxSmallDraw(boxesRect.x + num, rect.y, ref raceSetting.Imperial, disabled, 24f, texChecked, texUnchecked);
            if (AdeptusIntergrationUtility.enabled_AdeptusMechanicus)
            {
                num += x;
                CheckboxSmallDraw(boxesRect.x + num, rect.y, ref raceSetting.Mechanicus, disabled, 24f, texChecked, texUnchecked);
            }
            if (AdeptusIntergrationUtility.enabled_AdeptusAstartes)
            {
                num += x;
                CheckboxSmallDraw(boxesRect.x + num, rect.y, ref raceSetting.Astartes, disabled, 24f, texChecked, texUnchecked);
            }
            if (AdeptusIntergrationUtility.enabled_XenobiologisChaos)
            {
                num += x;
                CheckboxSmallDraw(boxesRect.x + num, rect.y, ref raceSetting.Chaos, disabled, 24f, texChecked, texUnchecked);
            }
            if (AdeptusIntergrationUtility.enabled_XenobiologisEldar)
            {
                num += x;
                CheckboxSmallDraw(boxesRect.x + num, rect.y, ref raceSetting.Aeldari, disabled, 24f, texChecked, texUnchecked);
            }
            if (AdeptusIntergrationUtility.enabled_XenobiologisOrk)
            {
                num += x;
                CheckboxSmallDraw(boxesRect.x + num, rect.y, ref raceSetting.Orkoid, disabled, 24f, texChecked, texUnchecked);
            }
            if (AdeptusIntergrationUtility.enabled_XenobiologisTau)
            {
                num += x;
                CheckboxSmallDraw(boxesRect.x + num, rect.y, ref raceSetting.Tau, disabled, 24f, texChecked, texUnchecked);
                num += x;
                CheckboxSmallDraw(boxesRect.x + num, rect.y, ref raceSetting.Kroot, disabled, 24f, texChecked, texUnchecked);
                num += x;
                CheckboxSmallDraw(boxesRect.x + num, rect.y, ref raceSetting.Vespid, disabled, 24f, texChecked, texUnchecked);
            }
            if (AdeptusIntergrationUtility.enabled_XenobiologisNecron)
            {
                num += x;
                CheckboxSmallDraw(boxesRect.x + num, rect.y, ref raceSetting.Necron, disabled, 24f, texChecked, texUnchecked);
            }
            if (AdeptusIntergrationUtility.enabled_XenobiologisTyranid)
            {
                num += x;
                CheckboxSmallDraw(boxesRect.x + num, rect.y, ref raceSetting.Tyranid, disabled, 24f, texChecked, texUnchecked);
            }
            Text.Anchor = anchor;
        }
        public static void CheckboxSmallDraw(float x, float y, ref bool active, bool disabled = false, float size = 24f, Texture2D texChecked = null, Texture2D texUnchecked = null)
        {
            Rect rect = new Rect(x, y, size, size);
            if (!disabled && Widgets.ButtonInvisible(rect, true))
            {
                active = !active;
                if (active)
                {
                    SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera(null);
                }
                else
                {
                    SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera(null);
                }
            }
            Color color = GUI.color;
            if (disabled)
            {
                GUI.color = InactiveColor;
            }
            Texture2D image;
            Texture2D imageActive = ((texChecked != null) ? texChecked : Widgets.CheckboxOnTex);
            Texture2D imageInactive = ((texUnchecked != null) ? texUnchecked : Widgets.CheckboxOffTex);
            if (active)
            {
                image = disabled ? imageInactive : imageActive;
            }
            else
            {
                image = imageInactive;
            }
            GUI.DrawTexture(rect, image);
            if (disabled)
            {
                GUI.color = color;
            }
        }
        private static void CheckboxDraw(float x, float y, bool active, bool disabled, float size = 24f, Texture2D texChecked = null, Texture2D texUnchecked = null)
        {
            Color color = GUI.color;
            if (disabled)
            {
                GUI.color = InactiveColor;
            }
            Texture2D image;
            Texture2D imageActive = ((texChecked != null) ? texChecked : Widgets.CheckboxOnTex);
            Texture2D imageInactive = ((texUnchecked != null) ? texUnchecked : Widgets.CheckboxOffTex);
            if (active)
            {
                image = disabled ? imageInactive : imageActive;
            }
            else
            {
                image = imageInactive;
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
        public static readonly Color InactiveColor = new Color(0.37f, 0.37f, 0.37f, 0.8f);
    }
}
