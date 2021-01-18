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
        private static float listing_GeneralSpecialRulesLength => AMAMod.Instance.Length(settings.ShowArmourySettings && settings.ArmouryGeneralSpecialRules, 2, AMAMod.lineheight, 8, settings.ShowArmourySettings ? 1 : 0);
        private static float listing_GeneralSpecialRulesContents => AMAMod.Instance.Length(settings.ArmouryGeneralSpecialRules, 1, AMAMod.lineheight, 0, 0);
        private static float listing_WeaponSpecialRulesLength => AMAMod.Instance.Length(settings.ShowArmourySettings && settings.ShowAllowedWeaponSpecialRules, 5, AMAMod.lineheight, 8, settings.ShowArmourySettings ? 1 : 0);
        private static float listing_WeaponSpecialRulesContents => AMAMod.Instance.Length(settings.ShowAllowedWeaponSpecialRules, 4, AMAMod.lineheight, 0, 0);
        private static float listing_AllowedWeaponRulesLength => AMAMod.Instance.Length(settings.ShowArmourySettings && settings.ShowAllowedWeapons, 4, AMAMod.lineheight, 8, settings.ShowArmourySettings ? 1 : 0);
        private static float listing_AllowedWeaponRulesContents => AMAMod.Instance.Length(settings.ShowAllowedWeapons, 3, AMAMod.lineheight, 0, 0);

        private static float listing_ArmouryLength => (settings.ShowArmourySettings ? 16 : 0) + listing_ArmouryContents + armouryMenuInc;
        private static float listing_ArmouryContents => listing_GeneralSpecialRulesLength + listing_WeaponSpecialRulesLength + listing_AllowedWeaponRulesLength;
        private static float armouryMenuInc = 0f;
        private static float listing_MenuLength = 0f;
        private static float listing_MenuMax = 0f;

        public static void ArmouryModOptionsMenu(Listing_StandardExpanding listing_Main)
        {
            listing_Menu.maxOneColumn = true;
            // Armoury Mod Options
        //    listing_Menu = listing_Main.BeginSection(listing_ArmouryLength, false, 3, 4, 0); 
            listing_Menu = listing_Main.BeginSection(listing_ArmouryLength, false, 3, 4, 0);


            // Armoury mod General Special rules options menu
            {
                Listing_StandardExpanding listing = listing_Menu.BeginSection(listing_GeneralSpecialRulesLength, false, 3, 4, 0);
                if (listing.CheckboxLabeled("AMA_ShowSpecialRules".Translate(), ref settings.ArmouryGeneralSpecialRules, "AMA_ShowSpecialRulesDesc".Translate(), false, false, ArmouryMain.collapseTex, ArmouryMain.expandTex))
                {
                    Listing_StandardExpanding listing_General = listing.BeginSection(listing_GeneralSpecialRulesContents, true, 0, 4, 0);
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
                Listing_StandardExpanding listing = listing_Menu.BeginSection(listing_WeaponSpecialRulesLength, false, 3, 4, 0);
                if (listing.CheckboxLabeled("AMA_ShowWeaponSpecialRules".Translate(), ref settings.ShowAllowedWeaponSpecialRules, "AMA_ShowWeaponSpecialRulesDesc".Translate(), false, false, ArmouryMain.collapseTex, ArmouryMain.expandTex))
                {
                    Listing_StandardExpanding listing_General = listing.BeginSection(listing_WeaponSpecialRulesContents, true);
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
                Listing_StandardExpanding listing = listing_Menu.BeginSection(listing_AllowedWeaponRulesLength, false, 3, 4, 0);
                if (listing.CheckboxLabeled("AMA_ShowAllowedWeapons".Translate(), ref settings.ShowAllowedWeapons, "AMA_ShowAllowedWeaponsDesc".Translate(), false, false, ArmouryMain.collapseTex, ArmouryMain.expandTex))
                {
                    Listing_StandardExpanding listing_General = listing.BeginSection(listing_AllowedWeaponRulesContents, true);
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
            listing_Main.EndSection(listing_Menu);
            listing_Menu.listingRect.height = listing_Menu.MaxColumnHeightSeen;
        }
    }
}
