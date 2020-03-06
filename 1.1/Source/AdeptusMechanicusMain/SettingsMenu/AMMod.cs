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
        public AMSettings settings;
        public AMMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<AMSettings>();
            SettingsHelper.latest = this.settings;
            AMMod.Instance = this;
            AMSettings.Instance = base.GetSettings<AMSettings>();
        }
        
        public virtual void PreModOptions(Listing_Standard listing_Standard, Rect inRect, float num, ref float num2)
        {

        }

        public virtual void ModOptions(ref Listing_Standard listing_Standard, Rect rect, Rect inRect, float num, float num2)
        {

        }

        public virtual void PostModOptions(Listing_Standard listing_Standard, Rect inRect, float num, float num2)
        {

        }

        public virtual void XenobiologisSettings(ref Listing_Standard listing_Standard, Rect rect, Rect inRect, float num, float num2)
        {

        }
        public virtual void ImperialSettings(ref Listing_Standard listing_Standard, Rect rect, Rect inRect, float num, float num2)
        {
            AstartesSettings(ref listing_Standard, rect, inRect, num, num2);
            MechanicusSettings(ref listing_Standard, rect, inRect, num, num2);
            MilitarumSettings(ref listing_Standard, rect, inRect, num, num2);
        }
        public virtual void AstartesSettings(ref Listing_Standard listing_Standard, Rect rect, Rect inRect, float num, float num2)
        {

        }
        public virtual void MechanicusSettings(ref Listing_Standard listing_Standard, Rect rect, Rect inRect, float num, float num2)
        {

        }
        public virtual void MilitarumSettings(ref Listing_Standard listing_Standard, Rect rect, Rect inRect, float num, float num2)
        {

        }
        public virtual void ChaosSettings(ref Listing_Standard listing_Standard, Rect rect, Rect inRect, float num, float num2)
        {

        }
        public virtual void EldarSettings(ref Listing_Standard listing_Standard, Rect rect, Rect inRect, float num, float num2)
        {

        }
        public virtual void DarkEldarSettings(ref Listing_Standard listing_Standard, Rect rect, Rect inRect, float num, float num2)
        {

        }
        public virtual void OrkSettings(ref Listing_Standard listing_Standard, Rect rect, Rect inRect, float num, float num2)
        {

        }
        public virtual void TauSettings(ref Listing_Standard listing_Standard, Rect rect, Rect inRect, float num, float num2)
        {

        }
        public virtual void NecronSettings(ref Listing_Standard listing_Standard, Rect rect, Rect inRect, float num, float num2)
        {

        }
        public virtual void TyranidSettings(ref Listing_Standard listing_Standard, Rect rect, Rect inRect, float num, float num2)
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
        
        private Vector2 pos = new Vector2(0f, 0f);
        private Vector2 pos2 = new Vector2(0f, 0f);

        public override void WriteSettings()
        {
            base.WriteSettings();
        }
    }
    
}