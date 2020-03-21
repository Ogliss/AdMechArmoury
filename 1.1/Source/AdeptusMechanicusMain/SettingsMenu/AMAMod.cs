using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Verse;
using RimWorld;
using System;
using AdeptusMechanicus.HarmonyInstance;

namespace AdeptusMechanicus.settings
{
    public class AMAMod : AMMod
    {
        public AMAMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<AMSettings>();
            SettingsHelper.latest = this.settings;
            Instance = this;
            AMSettings.Instance = base.GetSettings<AMSettings>();
            var harmony = new Harmony("com.ogliss.rimworld.mod.AdeptusMechanicus");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            AdeptusMechanicus.HarmonyInstance.Main.PatchPawnsArrivalModeWorker(harmony);
            if (Prefs.DevMode) Log.Message(string.Format("Adeptus Mecanicus: Armoury: successfully completed {0} harmony patches.", harmony.GetPatchedMethods().Select(new Func<MethodBase, Patches>(Harmony.GetPatchInfo)).SelectMany((Patches p) => p.Prefixes.Concat(p.Postfixes).Concat(p.Transpilers)).Count((Patch p) => p.owner.Contains(harmony.Id))), false);
        }

        public override string SettingsCategory() => "AM_ModSeries".Translate() + ": " + "AMA_ModName".Translate();

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing_Standard = new Listing_Standard();
            float num = 800f;
            Rect rect = new Rect(inRect.x, inRect.y - 50, num, MenuLength);
            listing_Standard.BeginScrollView(inRect, ref pos, ref rect);

            listing_Standard.Label("AMA_ModName".Translate() + " Settings");
            ModOptions(ref listing_Standard, rect, inRect, num, MenuLength);


            listing_Standard.EndScrollView(ref rect);
            PostModOptions(listing_Standard, inRect, num, MenuLength);

        }

        public float MenuLength
        {
            get
            {
                float num = 200f + (settings.ShowSpecialRules ? 60 : 0) + (settings.ShowWeaponSpecialRules ? 120 : 0) + (settings.ShowAllowedWeapons ? 120 : 0);
                bool AMXB = AdeptusIntergrationUtil.enabled_MagosXenobiologis;
                if (AMXB)
                {
                    num += 200f;
                    num += SettingsHelper.latest.ShowImperium ? 60f : 0f;
                    num += SettingsHelper.latest.ShowChaos ? 120f : 0f;
                    num += SettingsHelper.latest.ShowEldar ? 60f : 0f;
                    num += SettingsHelper.latest.ShowTau ? 60f : 0f;
                    num += SettingsHelper.latest.ShowOrk ? 60f : 0;
                    num += SettingsHelper.latest.ShowNecron ? 60f : 0;
                    num += SettingsHelper.latest.ShowTyranid ? 60f : 0;
                }
                bool AMXO = AdeptusIntergrationUtil.enabled_XenobiologisOrk;
                if (AMXO)
                {
                    num += (AMSettings.Instance.ShowOrk ? (AMXB ? 60f : 120f) : 0);
                }
                bool AMXE = AdeptusIntergrationUtil.enabled_XenobiologisEldar;
                if (AMXE)
                {
                    num += (AMSettings.Instance.ShowEldar ? (AMXB ? 60f : 120f) : 0);
                }
                bool AMXT = AdeptusIntergrationUtil.enabled_XenobiologisTau;
                if (AMXT)
                {
                    num += (AMSettings.Instance.ShowTau ? (AMXB ? 60f : 120f) : 0);
                }
                bool AMAA = AdeptusIntergrationUtil.enabled_AdeptusAstartes;
                if (AMAA)
                {
                    num += (AMSettings.Instance.ShowImperium ? (AMXB ? 60f : 120f) : 0);
                }
                return num;
            }
        }


        public override void PreModOptions(Listing_Standard listing_Standard, Rect inRect, float num, ref float num2)
        {
            if (num > num * 2)
            {
                Log.Message(string.Format("PreModOptions Listing: {0}, inRect: {1}, num: {2}, num2: {3}", listing_Standard, inRect, num, num2));
            }

        }

        public override void ModOptions(ref Listing_Standard listing_Standard, Rect rect, Rect inRect, float num, float num2)
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
            XenobiologisSettings(ref listing_Standard, rect, inRect, num, num2);
            ImperialSettings(ref listing_Standard, rect, inRect, num, num2);
            ChaosSettings(ref listing_Standard, rect, inRect, num, num2);
            EldarSettings(ref listing_Standard, rect, inRect, num, num2);
            OrkSettings(ref listing_Standard, rect, inRect, num, num2);
            TauSettings(ref listing_Standard, rect, inRect, num, num2);
            NecronSettings(ref listing_Standard, rect, inRect, num, num2);
            TyranidSettings(ref listing_Standard, rect, inRect, num, num2);
        }
        
        public override void PostModOptions(Listing_Standard listing_Standard, Rect inRect, float num, float num2)
        {
            this.settings.Write();
        }

        
        private Vector2 pos = new Vector2(0f, 0f);
        private Vector2 pos2 = new Vector2(0f, 0f);

        public override void WriteSettings()
        {
            base.WriteSettings();
        }
    }
    
}