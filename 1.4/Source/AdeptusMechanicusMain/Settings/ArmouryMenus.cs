using AdeptusMechanicus.settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AdeptusMechanicus
{
    public static class ArmouryMenus
    {
        private static AMSettings settings = AMAMod.settings;
        private static AMAMod mod = AMAMod.Instance;
        private static bool Dev => AMAMod.Dev;

        private static float length_Menu = 0;
        private static float length_GSRules = 0;
        private static float length_GSRulesInc = 0;
        private static float length_GSRulesContent = 0;
        private static float length_WSRules = 0;
        private static float length_WSRulesInc = 0;
        private static float length_WSRulesContent = 0;
        private static float length_AWRules = 0;
        private static float length_AWRulesInc = 0;
        private static float length_AWRulesContent = 0;
        private static float length_Perfomance = 0;
        private static float length_PerfomanceInc = 0;
        private static float length_PerfomanceContent = 0;
        public static void ArmouryModOptionsMenu(Listing_StandardExpanding listing_Main)
        {
            //    listing_Menu.maxOneColumn = true;
            // Armoury Mod Options
            //    listing_Menu = listing_Main.BeginSection(listing_ArmouryLength, false, 3, 4, 0); 
            Listing_StandardExpanding listing_Menu = listing_Main.BeginSection(length_Menu + mod.armouryMenuInc, false, 3, 4, 0);
            // Armoury mod General Special rules options menu
            {
                Listing_StandardExpanding listing = listing_Menu.BeginSection(length_GSRules + length_GSRulesInc, false, 3, 4, 0);
                if (listing.CheckboxLabeled("AdeptusMechanicus.ShowSpecialRules".Translate(), ref settings.ArmouryGeneralSpecialRules, Dev, ref length_GSRulesInc, "AdeptusMechanicus.ShowSpecialRulesDesc".Translate(), false, false, ArmouryMain.collapseTex, ArmouryMain.expandTex, extend: true))
                {
                    Listing_StandardExpanding listing_General = listing.BeginSection(length_GSRulesContent, true, 0, 4, 0);
                    listing_General.maxOneColumn = false;
                    listing_General.ColumnWidth *= 0.488f;
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowDeepStrike".Translate(), ref settings.AllowDeepStrike, "AdeptusMechanicus.AllowDeepStrikeDesc".Translate(), extend: true);
                    listing_General.NewColumn();
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowInfiltrate".Translate(), ref settings.AllowInfiltrate, "AdeptusMechanicus.AllowInfiltrateDesc".Translate());
                    listing.EndSection(listing_General);
                    length_GSRulesContent = listing_General.curY;
                }
                listing_Menu.EndSection(listing);
                length_GSRules = listing.curY - length_GSRulesInc; 
            }
            // Armoury mod Weapon Special rules options menu
            {
                Listing_StandardExpanding listing = listing_Menu.BeginSection(length_WSRules + length_WSRulesInc, false, 3, 4, 0);
                if (listing.CheckboxLabeled("AdeptusMechanicus.ShowWeaponSpecialRules".Translate(), ref settings.ShowAllowedWeaponSpecialRules, Dev, ref length_WSRulesInc, "AdeptusMechanicus.ShowWeaponSpecialRulesDesc".Translate(), false, false, ArmouryMain.collapseTex, ArmouryMain.expandTex, extend: true))
                {
                    Listing_StandardExpanding listing_General = listing.BeginSection(length_WSRulesContent + length_WSRulesInc, true);
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
                    length_WSRulesContent = listing_General.curY;
                }
                listing_Menu.EndSection(listing);
                length_WSRules = listing.curY - length_WSRulesInc;
            }
            // Armoury mod Allowed Weapons options menu
            {
                Listing_StandardExpanding listing = listing_Menu.BeginSection(length_AWRules + length_AWRulesInc, false, 3, 4, 0);
                if (listing.CheckboxLabeled("AdeptusMechanicus.ShowAllowedWeapons".Translate(), ref settings.ShowAllowedWeapons, "AdeptusMechanicus.ShowAllowedWeaponsDesc".Translate(), false, false, ArmouryMain.collapseTex, ArmouryMain.expandTex, extend: true))
                {
                    Listing_StandardExpanding listing_General = listing.BeginSection(length_AWRulesContent, true);
                    listing_General.ColumnWidth *= 0.32f;
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowImperialWeapons".Translate(), ref settings.AllowImperialWeapons, "AdeptusMechanicus.AllowImperialWeaponsDesc".Translate(), extend: true);
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowMechanicusWeapons".Translate(), ref settings.AllowMechanicusWeapons, "AdeptusMechanicus.AllowMechanicusWeaponsDesc".Translate(), extend: true);
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowChaosWeapons".Translate(), ref settings.AllowChaosWeapons, "AdeptusMechanicus.AllowChaosWeaponsDesc".Translate(), extend: true);
                    listing_General.NewColumn();
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowEldarWeapons".Translate(), ref settings.AllowEldarWeapons, "AdeptusMechanicus.AllowEldarWeaponsDesc".Translate());
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowDarkEldarWeapons".Translate(), ref settings.AllowDarkEldarWeapons, "AdeptusMechanicus.AllowDarkEldarWeaponsDesc".Translate());
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowTauWeapons".Translate(), ref settings.AllowTauWeapons, "AdeptusMechanicus.AllowTauWeaponsDesc".Translate());
                    listing_General.NewColumn();
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowOrkWeapons".Translate(), ref settings.AllowOrkWeapons, "AdeptusMechanicus.AllowOrkWeaponsDesc".Translate());
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowNecronWeapons".Translate(), ref settings.AllowNecronWeapons, "AdeptusMechanicus.AllowNecronWeaponsDesc".Translate());
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowTyranidWeapons".Translate(), ref settings.AllowTyranidWeapons, "AdeptusMechanicus.AllowTyranidWeaponsDesc".Translate());
                    listing.EndSection(listing_General);
                    length_AWRulesContent = listing_General.curY;
                }
                listing_Menu.EndSection(listing);
                length_AWRules = listing.curY - length_AWRulesInc;
            }
            // Armoury mod Misc/Performance options menu
            {
                Listing_StandardExpanding listing = listing_Menu.BeginSection(length_Perfomance + length_PerfomanceInc, false, 3, 4, 0);
                if (listing.CheckboxLabeled("AdeptusMechanicus.ShowPerformanceOptions".Translate(), ref settings.ShowPerformanceOptions, "AdeptusMechanicus.ShowPerformanceOptionsDesc".Translate(), false, false, ArmouryMain.collapseTex, ArmouryMain.expandTex, extend: true))
                {
                    Listing_StandardExpanding listing_General = listing.BeginSection(length_PerfomanceContent, true);
                    listing_General.ColumnWidth *= 0.32f;
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowProjectileGlow".Translate(), ref settings.AllowProjectileGlow, "AdeptusMechanicus.AllowProjectileGlowDesc".Translate());
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowExtraPartDrawer".Translate(), ref settings.AllowExtraPartDrawer, "AdeptusMechanicus.AllowExtraPartDrawerDesc".Translate());
                    listing_General.NewColumn();
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowMuzzlePosition".Translate(), ref settings.AllowMuzzlePosition, "AdeptusMechanicus.AllowMuzzlePositionDesc".Translate());
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowHediffPartDrawer".Translate(), ref settings.AllowHediffPartDrawer, "AdeptusMechanicus.AllowHediffPartDrawerDesc".Translate());
                    listing_General.NewColumn();
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowProjectileTrail".Translate(), ref settings.AllowProjectileTrail, "AdeptusMechanicus.AllowProjectileTrailDesc".Translate());
                    listing_General.CheckboxLabeled("AdeptusMechanicus.AllowPauldronDrawer".Translate(), ref settings.AllowPauldronDrawer, "AdeptusMechanicus.AllowPauldronDrawerDesc".Translate());
                    listing.EndSection(listing_General);
                    length_PerfomanceContent = listing_General.curY;
                }
                listing_Menu.EndSection(listing);
                length_Perfomance = listing.curY - length_PerfomanceInc;
            }
            listing_Main.EndSection(listing_Menu);
            length_Menu = (listing_Menu.curY) - mod.armouryMenuInc;
        }
    }
}
