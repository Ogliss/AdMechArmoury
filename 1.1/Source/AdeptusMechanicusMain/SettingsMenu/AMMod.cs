using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Verse;
using RimWorld;

namespace AdeptusMechanicus.settings
{
    public class AMMod : Mod
    {
        public static AMMod Instance;
        public static AMSettings settings;
        public AMMod(ModContentPack content) : base(content)
        {
            AMMod.settings = GetSettings<AMSettings>();
            SettingsHelper.latest = AMMod.settings;
            AMMod.Instance = this;
            AMSettings.Instance = base.GetSettings<AMSettings>();
        }
        
        public virtual void PreModOptions(Listing_Standard listing_Main, Rect inRect, float num, ref float num2, string Label)
        {

        }

        public virtual void ModOptions(ref Listing_Standard listing_Main, Rect rect, Rect inRect, float width, ref float menuLength)
        {

        }

        public virtual void PostModOptions(Listing_Standard listing_Main, Rect inRect, float num, float num2)
        {

        }

        public static void CheckboxLabeled(Rect rect, string label, ref bool checkOn, string tooltip = null, bool disabled = false, Texture2D texChechked = null, Texture2D texUnchechked = null, bool placeCheckboxNearText = false)
        {
            if (!tooltip.NullOrEmpty())
            {
                if (Mouse.IsOver(rect))
                {
                    Widgets.DrawHighlight(rect);
                }
                TooltipHandler.TipRegion(rect, tooltip);
            }
            Widgets.CheckboxLabeled(rect, label, ref checkOn, disabled, texChechked, texUnchechked, placeCheckboxNearText);
           // base.Gap(this.verticalSpacing);
        }

        public static void TextFieldNumericLabeled<T>(Rect rect, string label, ref T val, ref string buffer, float min = 0f, float max = 1E+09f) where T : struct
        {
            Rect rect2 = rect.LeftPart(0.75f).Rounded();
            Rect rect3 = rect.RightPart(0.25f).Rounded();
            TextAnchor anchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleRight;
            Widgets.Label(rect2, label);
            Text.Anchor = anchor;
            Widgets.TextFieldNumeric(rect3, ref val, ref buffer, min, max);
        }


        // create a list of all the patches we want to make controllable.
        /*
        public static List<PatchDescription> Patches = new List<PatchDescription>
        {

            new PatchDescription("AstraMiliatrumMod_ArmourPatch.xml", "AstraMiliatrumMod_ArmourPatch".Translate(), "AstraMiliatrumMod_ArmourPatchTip".Translate()),
            new PatchDescription("AstraMiliatrumMod_WeaponsPatch.xml", "AstraMiliatrumMod_WeaponsPatch".Translate(), "AstraMiliatrumMod_WeaponsPatchTip".Translate())

        };
        */
        private static List<PatchDescription> patches;
        public static List<PatchDescription> Patches
        {
            get
            {
                if (patches.NullOrEmpty())
                {
                    patches = new List<PatchDescription>();
                    if (AdeptusIntergrationUtility.enabled_AstraCore)
                    {
                        patches.Add(new PatchDescription("AstraMiliatrumMod_ArmourPatch.xml", "Astra Miliatrum Armour Patch", "Removes the Astra Militarum versions of dupped Armour when active"));
                        patches.Add(new PatchDescription("AstraMiliatrumMod_WeaponsPatch.xml", "Astra Miliatrum Weapons Patch", "Removes the Astra Militarum versions of dupped Weapons when active"));
                    }
                }
                return patches;
            }
        }

        private Vector2 pos = new Vector2(0f, 0f);
        private Vector2 pos2 = new Vector2(0f, 0f);

        public override void WriteSettings()
        {
            base.WriteSettings();
        }
    }

}