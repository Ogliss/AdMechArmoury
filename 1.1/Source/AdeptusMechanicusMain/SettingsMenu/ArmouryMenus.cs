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
                if (listing.CheckboxLabeled("AMA_ShowSpecialRules".Translate(), ref settings.ArmouryGeneralSpecialRules, "AMA_ShowSpecialRulesDesc".Translate(), false, false, ArmouryMain.collapseTex, ArmouryMain.expandTex))
                {
                    Listing_StandardExpanding listing_General = listing.BeginSection(Listing_GeneralSpecialRulesContents, true, 0, 4, 0);
                    listing_General.ColumnWidth *= 0.488f;
                    listing_General.CheckboxLabeled("AMA_AllowDeepStrike".Translate(), ref settings.AllowDeepStrike, "AMA_AllowDeepStrikeDesc".Translate());
                    listing_General.NewColumn();
                    listing_General.CheckboxLabeled("AMA_AllowInfiltrate".Translate(), ref settings.AllowInfiltrate, "AMA_AllowInfiltrateDesc".Translate());
                    listing.EndSection(listing_General);
                    listing_General.listingRect.height = listing_General.MaxColumnHeightSeen;
                }
                listing_Menu.EndSection(listing);
                listing.listingRect.height = listing.MaxColumnHeightSeen;
            }
            // Armoury mod Weapon Special rules options menu
            {
                Listing_StandardExpanding listing = listing_Menu.BeginSection(Listing_WeaponSpecialRulesLength, false, 3, 4, 0);
                if (listing.CheckboxLabeled("AMA_ShowWeaponSpecialRules".Translate(), ref settings.ShowAllowedWeaponSpecialRules, "AMA_ShowWeaponSpecialRulesDesc".Translate(), false, false, ArmouryMain.collapseTex, ArmouryMain.expandTex))
                {
                    Listing_StandardExpanding listing_General = listing.BeginSection(Listing_WeaponSpecialRulesContents, true);
                    listing_General.ColumnWidth *= 0.488f;
                    listing_General.CheckboxLabeled("AMA_AllowRapidFire".Translate(), ref settings.AllowRapidFire, "AMA_AllowRapidFireDesc".Translate());
                    listing_General.CheckboxLabeled("AMA_AllowGetsHot".Translate(), ref settings.AllowGetsHot, "AMA_AllowGetsHotDesc".Translate());
                    listing_General.CheckboxLabeled("AMA_AllowJams".Translate(), ref settings.AllowJams, "AMA_AllowJamsDesc".Translate());
                    listing_General.CheckboxLabeled("AMA_AllowMultiShot".Translate(), ref settings.AllowMultiShot, "AMA_AllowMultiShotDesc".Translate());
                    listing_General.NewColumn();
                    listing_General.CheckboxLabeled("AMA_AllowUserEffects".Translate(), ref settings.AllowUserEffects, "AMA_AllowUserEffectsDesc".Translate());
                    listing_General.CheckboxLabeled("AMA_AllowForceWeaponEffect".Translate(), ref settings.AllowForceWeaponEffect, "AMA_AllowForceWeaponEffectDesc".Translate());
                    listing_General.CheckboxLabeled("AMA_AllowRendingMeleeEffect".Translate(), ref settings.AllowRendingMeleeEffect, "AMA_AllowRendingMeleeEffectDesc".Translate());
                    listing_General.CheckboxLabeled("AMA_AllowRendingRangedEffect".Translate(), ref settings.AllowRendingRangedEffect, "AMA_AllowRendingRangedEffectDesc".Translate());
                    listing.EndSection(listing_General);
                    listing_General.listingRect.height = listing_General.MaxColumnHeightSeen;
                }
                listing_Menu.EndSection(listing);
                listing.listingRect.height = listing.MaxColumnHeightSeen;
            }
            // Armoury mod Allowed Weapons options menu
            {
                Listing_StandardExpanding listing = listing_Menu.BeginSection(Listing_AllowedWeaponRulesLength, false, 3, 4, 0);
                if (listing.CheckboxLabeled("AMA_ShowAllowedWeapons".Translate(), ref settings.ShowAllowedWeapons, "AMA_ShowAllowedWeaponsDesc".Translate(), false, false, ArmouryMain.collapseTex, ArmouryMain.expandTex))
                {
                    Listing_StandardExpanding listing_General = listing.BeginSection(Listing_AllowedWeaponRulesContents, true);
                    listing_General.ColumnWidth *= 0.32f;
                    listing_General.CheckboxLabeled("AMA_AllowImperialWeapons".Translate(), ref settings.AllowImperialWeapons, "AMA_AllowImperialWeaponsDesc".Translate());
                    listing_General.CheckboxLabeled("AMA_AllowMechanicusWeapons".Translate(), ref settings.AllowMechanicusWeapons, "AMA_AllowMechanicusWeaponsDesc".Translate());
                    listing_General.CheckboxLabeled("AMA_AllowChaosWeapons".Translate(), ref settings.AllowChaosWeapons, "AMA_AllowChaosWeaponsDesc".Translate());
                    listing_General.NewColumn();
                    listing_General.CheckboxLabeled("AMA_AllowEldarWeapons".Translate(), ref settings.AllowEldarWeapons, "AMA_AllowEldarWeaponsDesc".Translate());
                    listing_General.CheckboxLabeled("AMA_AllowDarkEldarWeapons".Translate(), ref settings.AllowDarkEldarWeapons, "AMA_AllowDarkEldarWeaponsDesc".Translate());
                    listing_General.CheckboxLabeled("AMA_AllowTauWeapons".Translate(), ref settings.AllowTauWeapons, "AMA_AllowTauWeaponsDesc".Translate());
                    listing_General.NewColumn();
                    listing_General.CheckboxLabeled("AMA_AllowOrkWeapons".Translate(), ref settings.AllowOrkWeapons, "AMA_AllowOrkWeaponsDesc".Translate());
                    listing_General.CheckboxLabeled("AMA_AllowNecronWeapons".Translate(), ref settings.AllowNecronWeapons, "AMA_AllowNecronWeaponsDesc".Translate());
                    listing_General.CheckboxLabeled("AMA_AllowTyranidWeapons".Translate(), ref settings.AllowTyranidWeapons, "AMA_AllowTyranidWeaponsDesc".Translate());
                    listing.EndSection(listing_General);
                    listing_General.listingRect.height = listing_General.MaxColumnHeightSeen;
                }
                listing_Menu.EndSection(listing);
                listing.listingRect.height = listing.MaxColumnHeightSeen;

            }
            // Armoury mod Performance options menu
            {
                Listing_StandardExpanding listing = listing_Menu.BeginSection(Listing_PerformanceOptionsLength, false, 3, 4, 0);
                if (listing.CheckboxLabeled("AMA_ShowPerformanceOptions".Translate(), ref settings.ShowPerformanceOptions, "AMA_ShowPerformanceOptionsDesc".Translate(), false, false, ArmouryMain.collapseTex, ArmouryMain.expandTex))
                {
                    Listing_StandardExpanding listing_General = listing.BeginSection(Listing_PerformanceOptionsContents, true);
                    listing_General.ColumnWidth *= 0.32f;
                    listing_General.CheckboxLabeled("AMA_AllowProjectileTrail".Translate(), ref settings.AllowProjectileTrail, "AMAAllowProjectileTrailDesc".Translate());
                    listing_General.CheckboxLabeled("AMA_AllowProjectileGlow".Translate(), ref settings.AllowProjectileGlow, "AMA_AllowProjectileGlowDesc".Translate());
                    listing_General.CheckboxLabeled("AMA_AllowMuzzlePosition".Translate(), ref settings.AllowMuzzlePosition, "AMA_AllowMuzzlePositionDesc".Translate());
                    listing_General.NewColumn();
                    listing_General.CheckboxLabeled("AMA_AllowPauldronDrawer".Translate(), ref settings.AllowPauldronDrawer, "AMA_AllowPauldronDrawerDesc".Translate());
                    listing_General.CheckboxLabeled("AMA_AllowExtraPartDrawer".Translate(), ref settings.AllowExtraPartDrawer, "AMA_AllowExtraPartDrawerDesc".Translate());
                    listing_General.CheckboxLabeled("AMA_AllowHediffPartDrawer".Translate(), ref settings.AllowHediffPartDrawer, "AMA_AllowHediffPartDrawerDesc".Translate());
                    listing_General.NewColumn();
                //    listing_General.CheckboxLabeled("AMA_AllowOrkWeapons".Translate(), ref settings.AllowOrkWeapons, "AMA_AllowOrkWeaponsDesc".Translate());
                //    listing_General.CheckboxLabeled("AMA_AllowNecronWeapons".Translate(), ref settings.AllowNecronWeapons, "AMA_AllowNecronWeaponsDesc".Translate());
                //    listing_General.CheckboxLabeled("AMA_AllowTyranidWeapons".Translate(), ref settings.AllowTyranidWeapons, "AMA_AllowTyranidWeaponsDesc".Translate());
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
