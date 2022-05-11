using AdeptusMechanicus.settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AdeptusMechanicus
{
    class ArmouryMenus
    {
        private static AMSettings settings = AMAMod.settings;
        private static AMAMod mod = AMAMod.Instance;
        private static bool Dev => AMAMod.Dev;
        private static Listing_StandardExpanding listing_Menu = new Listing_StandardExpanding();
        private static Listing_StandardExpanding listing_Races = new Listing_StandardExpanding();
        private static Listing_StandardExpanding listing_General = new Listing_StandardExpanding();
        private static float Listing_GeneralSpecialRulesLength => AMAMod.Instance.Length(settings.ShowArmourySettings && settings.ArmouryGeneralSpecialRules, 2, AMAMod.lineheight, 8, settings.ShowArmourySettings ? 1 : 0);
        private static float Listing_GeneralSpecialRulesContents => AMAMod.Instance.Length(settings.ArmouryGeneralSpecialRules, 1, AMAMod.lineheight, 0, 0);
        private static float Listing_WeaponSpecialRulesLength => AMAMod.Instance.Length(settings.ShowArmourySettings && settings.ShowAllowedWeaponSpecialRules, 5, AMAMod.lineheight, 8, settings.ShowArmourySettings ? 1 : 0);
        private static float Listing_WeaponSpecialRulesContents => AMAMod.Instance.Length(settings.ShowAllowedWeaponSpecialRules, 4, AMAMod.lineheight, 0, 0);
        private static float Listing_AllowedWeaponRulesLength => AMAMod.Instance.Length(settings.ShowArmourySettings && settings.ShowAllowedWeapons, 4, AMAMod.lineheight, 8, settings.ShowArmourySettings ? 1 : 0);
        private static float Listing_AllowedWeaponRulesContents => AMAMod.Instance.Length(settings.ShowAllowedWeapons, 3, AMAMod.lineheight, 0, 0);
        private static float Listing_MiscOptionsLength => AMAMod.Instance.Length(settings.ShowArmourySettings && settings.ShowPerformanceOptions, 4, AMAMod.lineheight, 8, settings.ShowArmourySettings ? 1 : 0);
        private static float Listing_MiscOptionsContents => AMAMod.Instance.Length(settings.ShowPerformanceOptions, 3, AMAMod.lineheight, 0, 0);
        private static float Listing_PerformanceOptionsLength => AMAMod.Instance.Length(settings.ShowArmourySettings && settings.ShowPerformanceOptions, 4, AMAMod.lineheight, 8, settings.ShowArmourySettings ? 1 : 0);
        private static float Listing_PerformanceOptionsContents => AMAMod.Instance.Length(settings.ShowPerformanceOptions, 3, AMAMod.lineheight, 0, 0);

        private static float Listing_ArmouryLength => (settings.ShowArmourySettings ? 16 : 0) + Listing_ArmouryContents + armouryMenuInc;
        private static float Listing_ArmouryContents => Listing_GeneralSpecialRulesLength + Listing_WeaponSpecialRulesLength + Listing_AllowedWeaponRulesLength + Listing_PerformanceOptionsLength;
        private static float armouryMenuInc = 0f;
        private static float listing_MenuLength = 0f;
        private static float listing_MenuMax = 0f;

        public static void ArmouryModOptionsMenu(Listing_StandardExpanding listing_Main)
        {
            listing_Menu.maxOneColumn = true;
            // Armoury Mod Options
        //    listing_Menu = listing_Main.BeginSection(listing_ArmouryLength, false, 3, 4, 0); 
            listing_Menu = listing_Main.BeginSection(Listing_ArmouryLength, false, 3, 4, 0);


            // Armoury mod General Special rules options menu
            {
                Listing_StandardExpanding listing = listing_Menu.BeginSection(Listing_GeneralSpecialRulesLength, false, 3, 4, 0);
                if (listing.CheckboxLabeled("AdeptusMechanicus.ShowSpecialRules".Translate(), ref settings.ArmouryGeneralSpecialRules, "AdeptusMechanicus.ShowSpecialRulesDesc".Translate(), false, false, ArmouryMain.collapseTex, ArmouryMain.expandTex))
                {
                    Listing_StandardExpanding listing_General = listing.BeginSection(Listing_GeneralSpecialRulesContents, true, 0, 4, 0);
                    listing_General.ColumnWidth *= 0.488f;
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowDeepStrike".Translate(), ref settings.AllowDeepStrike, "AdeptusMechanicus.AllowDeepStrikeDesc".Translate());
                    listing_General.NewColumn();
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowInfiltrate".Translate(), ref settings.AllowInfiltrate, "AdeptusMechanicus.AllowInfiltrateDesc".Translate());
                    listing.EndSection(listing_General);
                    listing_General.listingRect.height = listing_General.MaxColumnHeightSeen;
                }
                listing_Menu.EndSection(listing);
                listing.listingRect.height = listing.MaxColumnHeightSeen;
            }
            // Armoury mod Weapon Special rules options menu
            {
                Listing_StandardExpanding listing = listing_Menu.BeginSection(Listing_WeaponSpecialRulesLength, false, 3, 4, 0);
                if (listing.CheckboxLabeled("AdeptusMechanicus.ShowWeaponSpecialRules".Translate(), ref settings.ShowAllowedWeaponSpecialRules, "AdeptusMechanicus.ShowWeaponSpecialRulesDesc".Translate(), false, false, ArmouryMain.collapseTex, ArmouryMain.expandTex))
                {
                    Listing_StandardExpanding listing_General = listing.BeginSection(Listing_WeaponSpecialRulesContents, true);
                    listing_General.ColumnWidth *= 0.488f;
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowRapidFire".Translate(), ref settings.AllowRapidFire, "AdeptusMechanicus.AllowRapidFireDesc".Translate());
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowGetsHot".Translate(), ref settings.AllowGetsHot, "AdeptusMechanicus.AllowGetsHotDesc".Translate());
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowJams".Translate(), ref settings.AllowJams, "AdeptusMechanicus.AllowJamsDesc".Translate());
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowMultiShot".Translate(), ref settings.AllowMultiShot, "AdeptusMechanicus.AllowMultiShotDesc".Translate());
                    listing_General.NewColumn();
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowUserEffects".Translate(), ref settings.AllowUserEffects, "AdeptusMechanicus.AllowUserEffectsDesc".Translate());
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowForceWeaponEffect".Translate(), ref settings.AllowForceWeaponEffect, "AdeptusMechanicus.AllowForceWeaponEffectDesc".Translate());
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowRendingMeleeEffect".Translate(), ref settings.AllowRendingMeleeEffect, "AdeptusMechanicus.AllowRendingMeleeEffectDesc".Translate());
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowRendingRangedEffect".Translate(), ref settings.AllowRendingRangedEffect, "AdeptusMechanicus.AllowRendingRangedEffectDesc".Translate());
                    listing.EndSection(listing_General);
                    listing_General.listingRect.height = listing_General.MaxColumnHeightSeen;
                }
                listing_Menu.EndSection(listing);
                listing.listingRect.height = listing.MaxColumnHeightSeen;
            }
            // Armoury mod Allowed Weapons options menu
            {
                Listing_StandardExpanding listing = listing_Menu.BeginSection(Listing_AllowedWeaponRulesLength, false, 3, 4, 0);
                if (listing.CheckboxLabeled("AdeptusMechanicus.ShowAllowedWeapons".Translate(), ref settings.ShowAllowedWeapons, "AdeptusMechanicus.ShowAllowedWeaponsDesc".Translate(), false, false, ArmouryMain.collapseTex, ArmouryMain.expandTex))
                {
                    Listing_StandardExpanding listing_General = listing.BeginSection(Listing_AllowedWeaponRulesContents, true);
                    listing_General.ColumnWidth *= 0.32f;
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowImperialWeapons".Translate(), ref settings.AllowImperialWeapons, "AdeptusMechanicus.AllowImperialWeaponsDesc".Translate());
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowMechanicusWeapons".Translate(), ref settings.AllowMechanicusWeapons, "AdeptusMechanicus.AllowMechanicusWeaponsDesc".Translate());
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowChaosWeapons".Translate(), ref settings.AllowChaosWeapons, "AdeptusMechanicus.AllowChaosWeaponsDesc".Translate());
                    listing_General.NewColumn();
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowEldarWeapons".Translate(), ref settings.AllowEldarWeapons, "AdeptusMechanicus.AllowEldarWeaponsDesc".Translate());
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowDarkEldarWeapons".Translate(), ref settings.AllowDarkEldarWeapons, "AdeptusMechanicus.AllowDarkEldarWeaponsDesc".Translate());
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowTauWeapons".Translate(), ref settings.AllowTauWeapons, "AdeptusMechanicus.AllowTauWeaponsDesc".Translate());
                    listing_General.NewColumn();
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowOrkWeapons".Translate(), ref settings.AllowOrkWeapons, "AdeptusMechanicus.AllowOrkWeaponsDesc".Translate());
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowNecronWeapons".Translate(), ref settings.AllowNecronWeapons, "AdeptusMechanicus.AllowNecronWeaponsDesc".Translate());
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowTyranidWeapons".Translate(), ref settings.AllowTyranidWeapons, "AdeptusMechanicus.AllowTyranidWeaponsDesc".Translate());
                    listing.EndSection(listing_General);
                    listing_General.listingRect.height = listing_General.MaxColumnHeightSeen;
                }
                listing_Menu.EndSection(listing);
                listing.listingRect.height = listing.MaxColumnHeightSeen;

            }
            // Armoury mod Misc/Performance options menu
            {
                Listing_StandardExpanding listing = listing_Menu.BeginSection(Listing_PerformanceOptionsLength, false, 3, 4, 0);
                if (listing.CheckboxLabeled("AdeptusMechanicus.ShowPerformanceOptions".Translate(), ref settings.ShowPerformanceOptions, "AdeptusMechanicus.ShowPerformanceOptionsDesc".Translate(), false, false, ArmouryMain.collapseTex, ArmouryMain.expandTex))
                {
                    Listing_StandardExpanding listing_General = listing.BeginSection(Listing_PerformanceOptionsContents, true);
                    listing_General.ColumnWidth *= 0.32f;
                    listing_General.CheckboxLabeled("AdeptusMechanicus.RacialResearchRestriction".Translate(), ref settings.RacialResearchRestriction, "AdeptusMechanicus.RacialResearchRestrictionDesc".Translate());
                    listing_General.CheckboxLabeled("AdeptusMechanicus.RacialConstructionRestriction".Translate(), ref settings.RacialConstructionRestriction, "AdeptusMechanicus.RacialConstructionRestrictionDesc".Translate());
                    listing_General.CheckboxLabeled("AdeptusMechanicus.RacialProductionRestriction".Translate(), ref settings.RacialProductionRestriction, "AdeptusMechanicus.RacialProductionRestrictionDesc".Translate());
                    listing_General.NewColumn();
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowProjectileTrail".Translate(), ref settings.AllowProjectileTrail, "AdeptusMechanicus.AllowProjectileTrailDesc".Translate());
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowProjectileGlow".Translate(), ref settings.AllowProjectileGlow, "AdeptusMechanicus.AllowProjectileGlowDesc".Translate());
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowMuzzlePosition".Translate(), ref settings.AllowMuzzlePosition, "AdeptusMechanicus.AllowMuzzlePositionDesc".Translate());
                    listing_General.NewColumn();
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowPauldronDrawer".Translate(), ref settings.AllowPauldronDrawer, "AdeptusMechanicus.AllowPauldronDrawerDesc".Translate());
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowExtraPartDrawer".Translate(), ref settings.AllowExtraPartDrawer, "AdeptusMechanicus.AllowExtraPartDrawerDesc".Translate());
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowHediffPartDrawer".Translate(), ref settings.AllowHediffPartDrawer, "AdeptusMechanicus.AllowHediffPartDrawerDesc".Translate());
                    listing.EndSection(listing_General);
                    listing_General.listingRect.height = listing_General.MaxColumnHeightSeen;
                }
                listing_Menu.EndSection(listing);
                listing.listingRect.height = listing.MaxColumnHeightSeen;

            }
            listing_Main.EndSection(listing_Menu);
            listing_Menu.listingRect.height = listing_Menu.MaxColumnHeightSeen;
        }
    }
}
