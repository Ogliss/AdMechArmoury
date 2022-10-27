using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus.settings
{
    public class FactionSettingHandle : SettingHandle
    {
        public FactionSettingHandle() { }
        public FactionSettingHandle(FactionDef def, ModContentPack pack)
        {
            contentPack = pack.PackageId;
            factionDefNane = def.defName;
            hidden = def.hidden;
            hidden_default = def.hidden;
            startingCountAtWorldCreation = def.startingCountAtWorldCreation;
        }

        public FactionDef FactionDef
        {
            get
            {
                if (factionDef == null && loaded == null)
                {
                    factionDef = DefDatabase<FactionDef>.GetNamedSilentFail(factionDefNane);
                }
                return factionDef;
            }
        }

        public string FactionCatergoryTag => FactionDef?.categoryTag ?? "";
        public string FactionCatergory
        {
            get
            {
                string cat = "None";
                if (FactionCatergoryTag.NullOrEmpty())
                {
                    return cat;
                }
                if (FactionCatergoryTag.Contains("Imperial"))
                {
                    return "Imperial";
                }
                if (FactionCatergoryTag.Contains("Chaos"))
                {
                    return "Chaos";
                }
                if (FactionCatergoryTag.Contains("Aeldari"))
                {
                    return "Aeldari";
                }
                if (FactionCatergoryTag.Contains("Tau"))
                {
                    return "Tau";
                }
                if (FactionCatergoryTag.Contains("Orkoid"))
                {
                    return "Ork";
                }
                if (FactionCatergoryTag.Contains("Necron"))
                {
                    return "Necron";
                }
                if (FactionCatergoryTag.Contains("Tyranid"))
                {
                    return "Tyraid";
                }
                return cat;
            }
        }

        public bool Loaded => loaded ??= FactionDef != null;
        private bool? loaded;
        public FactionDef factionDef; 
        public string contentPack;
        public override void ExposeData()
        {
            Scribe_Values.Look(ref this.hidden, "hidden", false);
            Scribe_Values.Look(ref this.hidden_default, "hidden_default", true);
            Scribe_Values.Look(ref this.enabled, "enabled", true);
            Scribe_Values.Look(ref this.startingCountAtWorldCreation, "startingCountAtWorldCreation", 1);
            Scribe_Values.Look(ref this.factionDefNane, "factionDefNane");
            Scribe_Values.Look(ref this.contentPack, "contentPack");
        }
        public string factionDefNane;
        public bool enabled;
        public bool hidden_default;
        public bool hidden;
        public int startingCountAtWorldCreation;

    }

    public static class FactionSettingListingExt
    {
        public static void FactionSettingFor(this Listing_StandardExpanding parent, string label, FactionSettingHandle raceSetting, string tooltip = null, bool disabled = false, bool highlight = false, Texture2D texChecked = null, Texture2D texUnchecked = null)
        {
            float lineHeight = Text.LineHeight;
            Rect rect = parent.GetRect(lineHeight);
            if (!tooltip.NullOrEmpty() || highlight)
            {
                if (Mouse.IsOver(rect))
                {
                    Widgets.DrawHighlight(rect);
                }
                if (!tooltip.NullOrEmpty()) TooltipHandler.TipRegion(rect, tooltip);
            }
            FactionSettingLabeled(rect, raceSetting, disabled, texChecked, texUnchecked);
            parent.Gap(parent.verticalSpacing);
        }
        public static void FactionSettingLabeled(Rect rect, FactionSettingHandle factionSetting, bool disabled = false, Texture2D texChecked = null, Texture2D texUnchecked = null)
        {
            TextAnchor anchor = Text.Anchor;
            Verse.Text.Anchor = TextAnchor.MiddleLeft;
            Rect labelRect = rect.LeftPart(0.25f);
            Rect boxesRect = rect.RightPart(0.75f);
            float x = boxesRect.width / 4;
            Widgets.Label(labelRect, factionSetting.FactionDef.LabelCap);
            float num = x;
            AdeptusWidgets.CheckboxSmallDraw(boxesRect.x + num, rect.y, ref factionSetting.enabled, disabled, 24f, texChecked, texUnchecked);
            num += x;
            AdeptusWidgets.CheckboxSmallDraw(boxesRect.x + num, rect.y, ref factionSetting.hidden, disabled, 24f, texChecked, texUnchecked);
            if (AdeptusIntergrationUtility.enabled_AdeptusAstartes)
            {
                num += x;
                //        AdeptusWidgets.CheckboxSmallDraw(boxesRect.x + num, rect.y, ref factionSetting.Astartes, disabled, 24f, texChecked, texUnchecked);
            }
            Text.Anchor = anchor;
        }
    }
}