using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Verse;
using RimWorld;

namespace AdeptusMechanicus.settings
{
    public class AMAMod : Mod
    {
        public static AMAMod Instance;
        public AMASettings settings;
        public AMAMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<AMASettings>();
            AMASettingsHelper.latest = this.settings;
            AMAMod.Instance = this;
            AMASettings.Instance = base.GetSettings<AMASettings>();
        }
        
        public override string SettingsCategory() => "AM_ModSeries".Translate() + ": " + "AMA_ModName".Translate();

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing_Standard = new Listing_Standard();
            float num = 800f;
            float num2 = 500f + (settings.ShowSpecialRules ? 30 : 0) + (settings.ShowWeaponSpecialRules ? 120 : 0) + (settings.ShowAllowedWeapons ? 120 : 0);
            PreModOptions(listing_Standard, inRect, num, ref num2);
            Rect rect = new Rect(inRect.x, inRect.y - 50, num, num2);
            listing_Standard.BeginScrollView(inRect, ref pos, ref rect);

            listing_Standard.Label("AMA_ModName".Translate()+" Settings");
            ModOptions(ref listing_Standard, rect, inRect, num, num2);

            listing_Standard.EndScrollView(ref rect);

            PostModOptions(listing_Standard, inRect, num, num2);

        }

        public void PreModOptions(Listing_Standard listing_Standard, Rect inRect, float num, ref float num2)
        {
            if (num > num*2)
            {
                Log.Message(string.Format("PreModOptions Listing: {0}, inRect: {1}, num: {2}, num2: {3}", listing_Standard, inRect, num, num2));
            }

        }

        public void ModOptions(ref Listing_Standard listing_Standard, Rect rect, Rect inRect, float num, float num2)
        {
            listing_Standard.CheckboxLabeled("AMA_ShowSpecialRules".Translate(), ref settings.ShowSpecialRules, "AMA_ShowSpecialRulesDesc".Translate());
            Rect rectShowSpecialRules = new Rect(rect.x, rect.y + 10, num, 120f);
            if (settings.ShowSpecialRules)
            {
                listing_Standard.BeginSection(30f * 1f);
                CheckboxLabeled(rectShowSpecialRules.TopHalf().TopHalf().LeftHalf().ContractedBy(4), "AMA_AllowDeepStrike".Translate(), ref settings.AllowDeepStrike, "AMA_AllowDeepStrikeDesc".Translate());
                CheckboxLabeled(rectShowSpecialRules.TopHalf().TopHalf().RightHalf().ContractedBy(4), "AMA_AllowInfiltrate".Translate(), ref settings.AllowInfiltrate, "AMA_AllowInfiltrateDesc".Translate());
                listing_Standard.EndSection(listing_Standard);
            }
            listing_Standard.CheckboxLabeled("AMA_ShowWeaponSpecialRules".Translate(), ref settings.ShowWeaponSpecialRules, "AMA_ShowWeaponSpecialRulesDesc".Translate());
            Rect rectShowWeaponSpecialRules = new Rect(rect.x, rect.y + 10, num, 120f);
            if (settings.ShowWeaponSpecialRules)
            {
                listing_Standard.BeginSection(30f * 4f);
                CheckboxLabeled(rectShowWeaponSpecialRules.TopHalf().TopHalf().LeftHalf().ContractedBy(4), "AMA_AllowRapidFire".Translate(), ref settings.AllowRapidFire, "AMA_AllowRapidFireDesc".Translate());
                CheckboxLabeled(rectShowWeaponSpecialRules.TopHalf().BottomHalf().LeftHalf().ContractedBy(4), "AMA_AllowGetsHot".Translate(), ref settings.AllowGetsHot, "AMA_AllowGetsHotDesc".Translate());
                CheckboxLabeled(rectShowWeaponSpecialRules.TopHalf().TopHalf().RightHalf().ContractedBy(4), "AMA_AllowJams".Translate(), ref settings.AllowJams, "AMA_AllowJamsDesc".Translate());
                CheckboxLabeled(rectShowWeaponSpecialRules.TopHalf().BottomHalf().RightHalf().ContractedBy(4), "AMA_AllowMultiShot".Translate(), ref settings.AllowMultiShot, "AMA_AllowMultiShotDesc".Translate());
                CheckboxLabeled(rectShowWeaponSpecialRules.BottomHalf().TopHalf().LeftHalf().ContractedBy(4), "AMA_AllowUserEffects".Translate(), ref settings.AllowUserEffects, "AMA_AllowUserEffectsDesc".Translate());
                CheckboxLabeled(rectShowWeaponSpecialRules.BottomHalf().BottomHalf().LeftHalf().ContractedBy(4), "AMA_AllowForceWeaponEffect".Translate(), ref settings.AllowForceWeaponEffect, "AMA_AllowForceWeaponEffectDesc".Translate());
                CheckboxLabeled(rectShowWeaponSpecialRules.BottomHalf().TopHalf().RightHalf().ContractedBy(4), "AMA_AllowRendingMeleeEffect".Translate(), ref settings.AllowUserEffects, "AMA_AllowUserEffectsDesc".Translate());
                CheckboxLabeled(rectShowWeaponSpecialRules.BottomHalf().BottomHalf().RightHalf().ContractedBy(4), "AMA_AllowRendingRangedEffect".Translate(), ref settings.AllowForceWeaponEffect, "AMA_AllowForceWeaponEffectDesc".Translate());
                listing_Standard.EndSection(listing_Standard);
            }
            listing_Standard.CheckboxLabeled("AMA_ShowAllowedWeapons".Translate(), ref settings.ShowAllowedWeapons, "AMA_ShowAllowedWeaponsDesc".Translate());
            Rect rectShowAllowedWeapons = new Rect(rect.x, rect.y + 10, num, 120f);
            if (settings.ShowAllowedWeapons)
            {
                listing_Standard.BeginSection(30f * 4f);
                CheckboxLabeled(rectShowWeaponSpecialRules.TopHalf().TopHalf().LeftHalf().LeftHalf().ContractedBy(4), "AMA_AllowImperialWeapons".Translate(), ref settings.AllowImperialWeapons, "AMA_AllowImperialWeaponsDesc".Translate());
                CheckboxLabeled(rectShowWeaponSpecialRules.TopHalf().TopHalf().LeftHalf().RightHalf().ContractedBy(4), "AMA_AllowMechanicusWeapons".Translate(), ref settings.AllowMechanicusWeapons, "AMA_AllowMechanicusWeaponsDesc".Translate());
                CheckboxLabeled(rectShowWeaponSpecialRules.TopHalf().BottomHalf().LeftHalf().ContractedBy(4), "AMA_AllowChaosWeapons".Translate(), ref settings.AllowChaosWeapons, "AMA_AllowChaosWeaponsDesc".Translate());
                CheckboxLabeled(rectShowWeaponSpecialRules.TopHalf().TopHalf().RightHalf().ContractedBy(4), "AMA_AllowEldarWeapons".Translate(), ref settings.AllowEldarWeapons, "AMA_AllowEldarWeaponsDesc".Translate());
                CheckboxLabeled(rectShowWeaponSpecialRules.TopHalf().BottomHalf().RightHalf().ContractedBy(4), "AMA_AllowDarkEldarWeapons".Translate(), ref settings.AllowDarkEldarWeapons, "AMA_AllowDarkEldarWeaponsDesc".Translate());
                CheckboxLabeled(rectShowWeaponSpecialRules.BottomHalf().TopHalf().LeftHalf().ContractedBy(4), "AMA_AllowTauWeapons".Translate(), ref settings.AllowTauWeapons, "AMA_AllowTauWeaponsDesc".Translate());
                CheckboxLabeled(rectShowWeaponSpecialRules.BottomHalf().BottomHalf().LeftHalf().ContractedBy(4), "AMA_AllowOrkWeapons".Translate(), ref settings.AllowOrkWeapons, "AMA_AllowOrkWeaponsDesc".Translate());
                CheckboxLabeled(rectShowWeaponSpecialRules.BottomHalf().TopHalf().RightHalf().ContractedBy(4), "AMA_AllowNecronWeapons".Translate(), ref settings.AllowNecronWeapons, "AMA_AllowNecronWeaponsDesc".Translate());
                CheckboxLabeled(rectShowWeaponSpecialRules.BottomHalf().BottomHalf().RightHalf().ContractedBy(4), "AMA_AllowTyranidWeapons".Translate(), ref settings.AllowTyranidWeapons, "AMA_AllowTyranidWeaponsDesc".Translate());
                listing_Standard.EndSection(listing_Standard);
            }
        }

        public void PostModOptions(Listing_Standard listing_Standard, Rect inRect, float num, float num2)
        {
            this.settings.Write();
        }


        public void CheckboxLabeled(Rect rect, string label, ref bool checkOn, string tooltip = null, bool disabled = false, Texture2D texChechked = null, Texture2D texUnchechked = null, bool placeCheckboxNearText = false)
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
        private Vector2 pos = new Vector2(0f, 0f);
        private Vector2 pos2 = new Vector2(0f, 0f);

        public override void WriteSettings()
        {
            base.WriteSettings();
        }
    }
    
}