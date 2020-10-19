using AdeptusMechanicus.ExtensionMethods;
using AdeptusMechanicus.HarmonyInstance;
using HarmonyLib;
using RimWorld;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus.settings
{
    public class AMAMod : AMMod
    {
        public static Harmony harmony = new Harmony("com.ogliss.rimworld.mod.AdeptusMechanicus");
        public AMAMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<AMSettings>();
            SettingsHelper.latest = this.settings;
            Instance = this;
            AMSettings.Instance = base.GetSettings<AMSettings>();
        //    var harmony = new Harmony("com.ogliss.rimworld.mod.AdeptusMechanicus");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            AdeptusMechanicus.HarmonyInstance.HarmonyPatches.PatchPawnsArrivalModeWorker(harmony);

            if (AdeptusIntergrationUtil.enabled_rooloDualWield)
            {
                /*
                harmony.Patch(AccessTools.Method(GenTypes.GetTypeInAnyAssembly("CompOversizedWeapon.HarmonyCompOversizedWeapon", "CompOversizedWeapon"), "DrawEquipmentAimingPreFix", null, null), new HarmonyMethod(Main.patchType, "DrawEquipmentAiming_DualWield_OverSized_PreFix", null), new HarmonyMethod(Main.patchType, "DrawEquipmentAiming_DualWield_OverSized_PostFix", null));
                harmony.Patch(AccessTools.Method(GenTypes.GetTypeInAnyAssembly("CompActivatableEffect.HarmonyCompActivatableEffect", "CompActivatableEffect"), "DrawEquipmentAimingPostFix", null, null), new HarmonyMethod(Main.patchType, "DrawEquipmentAiming_DualWield_Activatable_PreFix", null));

                harmony.Patch(AccessTools.Method(GenTypes.GetTypeInAnyAssembly("DualWield.Harmony.PawnRenderer_DrawEquipmentAiming", "DualWield.Harmony"), "DrawEquipmentAimingOverride", null, null), new HarmonyMethod(Main.patchType, "DrawEquipmentAimingOverride_DualWield_compActivatableEffect_PreFix", null));
                harmony.Patch(AccessTools.Method(GenTypes.GetTypeInAnyAssembly("DualWield.Ext_Pawn_EquipmentTracker", "DualWield"), "AddOffHandEquipment", null, null),null , new HarmonyMethod(Main.patchType, "AddOffHandEquipment_PostFix", null));
                harmony.Patch(AccessTools.Method(GenTypes.GetTypeInAnyAssembly("DualWield.Harmony.PawnWeaponGenerator_TryGenerateWeaponFor", "DualWield.Harmony"), "Postfix", null, null), new HarmonyMethod(Main.patchType, "PawnWeaponGenerator_TryGenerateWeaponFor_PostFix", null));
                */
            }
            else
            {

                //    harmony.Patch(AccessTools.Method(GenTypes.GetTypeInAnyAssembly("AdeptusMechanicus.HarmonyCompOversizedWeapon", "AdeptusMechanicus"), "DrawEquipmentAimingPreFix", null, null), new HarmonyMethod(typeof(HarmonyPatch), "DrawEquipmentAiming_ActivatableEffect_OverSized_PreFix", null), new HarmonyMethod(typeof(HarmonyPatch), "DrawEquipmentAiming_ActivatableEffect_OverSized_PostFix", null));
                //    harmony.Patch(AccessTools.Method(GenTypes.GetTypeInAnyAssembly("AdeptusMechanicus.HarmonyCompActivatableEffect", "AdeptusMechanicus"), "DrawEquipmentAimingPostFix", null, null), new HarmonyMethod(typeof(HarmonyPatch), "DrawEquipmentAimingPostFix_OverSized_Activatable_PreFix", null));

            }
            if (AdeptusIntergrationUtil.enabled_ResearchPal)
            {
            //    harmony.Patch(AccessTools.Method(GenTypes.GetTypeInAnyAssembly("ResearchPal.Tree", "ResearchPal"), "DrawEquipmentAimingPostFix", null, null), new HarmonyMethod(typeof(AM_ResearchProjectDef_get_PrerequisitesCompleted_CommonTech_ResearchPal_Patch), "Postfix", null));
            }

            if (Prefs.DevMode) Log.Message(string.Format("Adeptus Mecanicus: Armoury: successfully completed {0} harmony patches.", harmony.GetPatchedMethods().Select(new Func<MethodBase, Patches>(Harmony.GetPatchInfo)).SelectMany((Patches p) => p.Prefixes.Concat(p.Postfixes).Concat(p.Transpilers)).Count((Patch p) => p.owner.Contains(harmony.Id))), false);
        }

        public override string SettingsCategory() => "AM_ModSeries".Translate();

        public string ModLoaded() => "Mods Loaded: " + "AMA_ModName".Translate();

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing_Main = new Listing_Standard();
            Rect inRect1 = inRect.TopPart(0.05f);
            Rect inRect2 = inRect.BottomPart(0.95f);
            Rect viewRect = inRect2.ContractedBy(4);
            Rect rect = new Rect(inRect2.x, inRect2.y, inRect2.ContractedBy(4).width-20, MenuLengthTotal);
            float width = rect.width;
            float height = viewRect.height;

            PreModOptions(listing_Main, inRect1, width, ref height, ModLoaded());
            //  Rect rectHost = rectBottom.LeftHalf().BottomPart(0.9f);
            //  Rect hostRect = new Rect(rectHost.x, rectHost.y, rectHost.width - 20, (potentialhostcount * (Text.LineHeight + listing_Main.verticalSpacing)));
            //    listing_Main.BeginScrollView(viewRect, ref pos, ref rect);
            Widgets.BeginScrollView(viewRect, ref this.pos, rect, true);
            ModOptions(ref listing_Main, rect, viewRect, width, MenuLengthTotal);
            Widgets.EndScrollView();
            //    listing_Main.EndScrollView(ref rect);
            PostModOptions(listing_Main, inRect2, width, MenuLengthTotal);

        }

        public override void PreModOptions(Listing_Standard listing_Main, Rect inRect, float width, ref float menuLength, string label)
        {
            Widgets.Label(inRect.ContractedBy(4), label);
            string tooltip = "Total Menu Length: " + MenuLengthTotal;
            tooltip += " Armoury Listing length: " + MenuLengthArmoury;
            tooltip += " Xenobiologis Listing length: " + MenuLengthXenobiologis;
            tooltip += " Main Listing current length: " + MainSectionLength;
            if (Prefs.DevMode && SteamUtility.SteamPersonaName.Contains("Ogliss"))
            {
                TooltipHandler.TipRegion(inRect.ContractedBy(4), tooltip);
            }
            if (width > width * 2)
            {
            //    log.message(string.Format("PreModOptions Listing: {0}, inRect: {1}, num: {2}, num2: {3}", listing_Main, inRect, width, menuLength));
            }

        }

        public Listing_Standard listing_Main = new Listing_Standard();
        public float TotalMenuLength = 24;
        public float MainSectionLength = 0;
        public float ArmouryGeneralSpecialRules = 0;
        public float ArmouryWeaponSpecialRules = 0;
        public float ArmouryAllowedWeapons = 0;
        public float ArmourySettings = 0;
        public float ArmouryMenuLength = 0;

        public float XenobiologisGeneral = 0;
        public float XenoSettings = 0;

        public float XenobiologisMenuLength = 0;

        public float XenobiologisRaceMenuLength = 0;
        public float XenobiologisImperialMenuLength = 0;
    //    public float XenobiologisAstartesMenuLength = 0;
        public float XenobiologisChaosMenuLength = 0;
        public float XenobiologisEldarMenuLength = 0;
        public float XenobiologisDarkEldarMenuLength = 0;
        public float XenobiologisTauMenuLength = 0;
        public float XenobiologisOrkMenuLength = 0;
        public float XenobiologisTyranidMenuLength = 0;
        public float XenobiologisNecronMenuLength = 0;


        public float ImperialMenuLength = 0;
        public float EldarMenuLength = 0;
        public float DarkEldarMenuLength = 0;
        public float TauMenuLength = 0;
        public float OrkMenuLength = 0;
        public float TyranidMenuLength = 0;
        public float NecronMenuLength = 0; 
        public override void ModOptions(ref Listing_Standard listing_Main, Rect rect, Rect inRect, float width, float menuLength)
        {
            listing_Main.Begin(rect);
            Listing_Standard listing_ArmourySettings = new Listing_Standard();
            Listing_Standard listing_XenobiologisSettings = new Listing_Standard();
            Listing_Standard listing_GeneralSpecialRules = new Listing_Standard();
            Listing_Standard listing_WeaponSpecialRules = new Listing_Standard();
            Listing_Standard listing_AllowedWeapons = new Listing_Standard();
            //    listing_Main.CheckboxLabeled("AMA_ModName".Translate() + " Settings", ref settings.ShowArmourySettings, "AMA_ShowSpecialRulesDesc".Translate());
            float lineheight = (Text.LineHeight + listing_Main.verticalSpacing);

            listing_ArmourySettings = listing_Main.BeginSection(MenuLengthArmoury, false,3);
            listing_ArmourySettings.CheckboxLabeled("AMA_ModName".Translate() + " Settings" + (Prefs.DevMode && SteamUtility.SteamPersonaName.Contains("Ogliss") ? " Menu Length: " + MenuLengthArmoury : ""), ref settings.ShowArmourySettings, "AMA_ShowSpecialRulesDesc".Translate());
            if (settings.ShowArmourySettings)
            {
                // Armoury mod General Special rules options menu
                listing_GeneralSpecialRules = listing_ArmourySettings.BeginSection(MenuLengthArmouryGeneralSpecialRules);
                listing_GeneralSpecialRules.CheckboxLabeled("AMA_ShowSpecialRules".Translate() + (Prefs.DevMode && SteamUtility.SteamPersonaName.Contains("Ogliss") ? " Menu Length: " + ArmouryGeneralSpecialRules : ""), ref settings.ArmouryGeneralSpecialRules, "AMA_ShowSpecialRulesDesc".Translate());
                if (settings.ArmouryGeneralSpecialRules)
                {
                    Listing_Standard listing_General = listing_GeneralSpecialRules.BeginSection(Length(settings.ArmouryGeneralSpecialRules, 1, lineheight,0,1),true);
                    listing_General.ColumnWidth *= 0.488f;
                    listing_General.CheckboxLabeled("AMA_AllowDeepStrike".Translate(), ref settings.AllowDeepStrike, "AMA_AllowDeepStrikeDesc".Translate());
                    listing_General.NewColumn();
                    listing_General.CheckboxLabeled("AMA_AllowInfiltrate".Translate(), ref settings.AllowInfiltrate, "AMA_AllowInfiltrateDesc".Translate());
                    listing_GeneralSpecialRules.EndSection(listing_General);
                }
                listing_ArmourySettings.EndSection(listing_GeneralSpecialRules);

                // Armoury mod Weapon Special rules options menu
                listing_WeaponSpecialRules = listing_ArmourySettings.BeginSection(MenuLengthArmouryWeaponSpecialRules);
                listing_WeaponSpecialRules.CheckboxLabeled("AMA_ShowWeaponSpecialRules".Translate() + (Prefs.DevMode && SteamUtility.SteamPersonaName.Contains("Ogliss") ? " Menu Length: " + ArmouryWeaponSpecialRules : ""), ref settings.ShowWeaponSpecialRules, "AMA_ShowWeaponSpecialRulesDesc".Translate());
                if (settings.ShowWeaponSpecialRules)
                {
                    Listing_Standard listing_General = listing_WeaponSpecialRules.BeginSection(Length(settings.ShowWeaponSpecialRules, 4, lineheight, 0, 1), true);
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
                    listing_WeaponSpecialRules.EndSection(listing_General);
                }
                listing_ArmourySettings.EndSection(listing_WeaponSpecialRules);

                // Armoury mod Allowed Weapons options menu
                listing_AllowedWeapons = listing_ArmourySettings.BeginSection(MenuLengthArmouryAllowedWeapons);
                listing_AllowedWeapons.CheckboxLabeled("AMA_ShowAllowedWeapons".Translate() + (Prefs.DevMode && SteamUtility.SteamPersonaName.Contains("Ogliss") ? " Menu Length: " + ArmouryAllowedWeapons : ""), ref settings.ShowAllowedWeapons, "AMA_ShowAllowedWeaponsDesc".Translate());
                if (settings.ShowAllowedWeapons)
                {
                    Listing_Standard listing_General = listing_AllowedWeapons.BeginSection(Length(settings.ShowAllowedWeapons, 3, lineheight, 0, 1), true);
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
                    listing_AllowedWeapons.EndSection(listing_General);
                }
                listing_ArmourySettings.EndSection(listing_AllowedWeapons);

            }
            listing_Main.EndSection(listing_ArmourySettings);
            ArmouryGeneralSpecialRules = listing_GeneralSpecialRules.CurHeight;
            ArmouryWeaponSpecialRules = listing_WeaponSpecialRules.CurHeight;
            ArmouryAllowedWeapons = listing_AllowedWeapons.CurHeight;
            ArmourySettings = ArmouryGeneralSpecialRules + ArmouryWeaponSpecialRules + ArmouryAllowedWeapons;
            if (!settings.AllowImperialWeapons)
            {
                settings.AllowAdeptusAstartes = false;
                settings.AllowAdeptusMilitarum = false;
                settings.AllowAdeptusSororitas = false;
            }
            if (!settings.AllowMechanicusWeapons)
            {
                settings.AllowAdeptusMechanicus = false;
            }
            if (!settings.AllowChaosWeapons)
            {
                settings.AllowChaosMarine = false;
                settings.AllowChaosMechanicus = false;
                settings.AllowChaosGuard = false;
            }
            if (!settings.AllowEldarWeapons)
            {
                settings.AllowEldarCraftworld = false;
                settings.AllowEldarExodite = false;
                settings.AllowEldarHarlequinn = false;
                settings.AllowEldarWraithguard = false;
            }
            if (!settings.AllowDarkEldarWeapons)
            {
                settings.AllowDarkEldar = false;
            }
            if (!settings.AllowOrkWeapons)
            {
                settings.AllowOrkFeral = false;
                settings.AllowOrkTek = false;
                settings.AllowOrkRok = false;
            }
            if (!settings.AllowTauWeapons)
            {
                settings.AllowTau = false;
                settings.AllowGueVesaAuxiliaries = false;
                settings.AllowKroot = false;
                settings.AllowKrootAuxiliaries = false;
                settings.AllowVespid = false;
                settings.AllowVespidAuxiliaries = false;
            }
            if (!settings.AllowNecronWeapons)
            {
                settings.AllowNecron = false;
            }
            if (!settings.AllowTyranidWeapons)
            {
                settings.AllowTyranid = false;
            }
            if (AdeptusIntergrationUtil.enabled_MagosXenobiologis)
            {
                bool XBOptions = settings.ShowXenobiologisSettings;
                bool XBRaceOptions = settings.ShowAllowedRaceSettings;
                float menuLengthXBMain = Length(XBOptions, 1, lineheight, XBOptions ? 8 : 0);
                float menuLengthXBMainOptions = Length(XBOptions, 1, lineheight, 8, 0);
                menuLengthXBMain += XBOptions ? menuLengthXBMainOptions : 0;
                float menuLengthXBRace = Length(XBOptions, 1, lineheight, 8, XBOptions ? 1 : 0);
                XenobiologisSettings(ref listing_Main, rect, inRect, width, XenobiologisMenuLength);
            }
            else
            {
                ImperialSettings(ref listing_Main, rect, inRect, width, menuLength);
                ChaosSettings(ref listing_Main, rect, inRect, width, menuLength);
                EldarSettings(ref listing_Main, rect, inRect, width, menuLength);
                OrkSettings(ref listing_Main, rect, inRect, width, menuLength);
                TauSettings(ref listing_Main, rect, inRect, width, menuLength);
                NecronSettings(ref listing_Main, rect, inRect, width, menuLength);
                TyranidSettings(ref listing_Main, rect, inRect, width, menuLength);
            }
            listing_Main.End();
            MainSectionLength = listing_Main.CurHeight;
        }

        public override void PostModOptions(Listing_Standard listing_Main, Rect inRect, float width, float menuLength)
        {
            this.settings.Write();
        }

        public float Length(bool setting, float lines, float lineheight, float offset = 8, float linesfallback = 1, float offsetfallback = 0)
        {
            return ((lineheight * (setting ? lines : linesfallback)) + (setting ? offset : offsetfallback));
        }


        public float MenuLengthTotal
        {
            get
            {
                float num = 0;
                num += 16;
                num += MenuLengthMain;
                return num;
            }
        }

        public float MenuLengthMain
        {
            get
            {
                float num = 0;
                num += MenuLengthArmouryTotal;
                num += MenuLengthXenobiologisTotal;
                return num;
            }
        }
        public float MenuLengthArmoury
        {
            get
            {
                return MenuLengthArmouryTotal;
                return ArmouryMenuLength;
            }
        }

        public float MenuLengthArmouryTotal
        {
            get
            {
                float num = 0;
                num += 0;
                num += MenuLengthArmouryBase;
                num += MenuLengthArmouryGeneralSpecialRules;
                num += MenuLengthArmouryWeaponSpecialRules;
                num += MenuLengthArmouryAllowedWeapons;
                return num;
            }
        }

        public float MenuLengthArmouryBase
        {
            get
            {
                float lineheight = (Text.LineHeight + listing_Main.verticalSpacing);
                float num = Length(settings.ShowArmourySettings, 2, lineheight, 0, 1, 0);
                return num;
            }
        }

        public float MenuLengthArmouryGeneralSpecialRules
        {
            get
            {
                float lineheight = (Text.LineHeight + listing_Main.verticalSpacing);
                float num = Length(settings.ShowArmourySettings &&settings.ArmouryGeneralSpecialRules, 2, lineheight, 8, settings.ShowArmourySettings ? 1 : 0);
                return num;
            }
        }

        public float MenuLengthArmouryWeaponSpecialRules
        {
            get
            {
                float lineheight = (Text.LineHeight + listing_Main.verticalSpacing);
                float num = Length(settings.ShowArmourySettings && settings.ShowWeaponSpecialRules, 5, lineheight, 8, settings.ShowArmourySettings ? 1 : 0);
                return num;
            }
        }

        public float MenuLengthArmouryAllowedWeapons
        {
            get
            {
                float lineheight = (Text.LineHeight + listing_Main.verticalSpacing);
                float num = Length(settings.ShowArmourySettings && settings.ShowAllowedWeapons, 4, lineheight, 8, settings.ShowArmourySettings ? 1: 0);
                return num;
            }
        }

        public float MenuLengthXenobiologis
        {
            get
            {
                float num = 0;
                //    num += XenobiologisMenuLength;
                num += MenuLengthXenobiologisTotal;

                return num;
            }
        }

        public float MenuLengthXenobiologisTotal
        {
            get
            {
                float num = 0;
                num += 0;
                num += MenuLengthXenobiologisBase;
                num += MenuLengthXenobiologisBaseRace;
                num += MenuLengthXenobiologisRaceContents;
                return num;
            }
        }

        public float MenuLengthXenobiologisBase
        {
            get
            {
                float lineheight = (Text.LineHeight + listing_Main.verticalSpacing);
                bool XBOptions = settings.ShowXenobiologisSettings;
                bool XBRaceOptions = settings.ShowAllowedRaceSettings && XBOptions;
                int Options = 3;
                float num = Length(XBOptions, Options, lineheight, 0);
                return num;
            }
        }
        public float MenuLengthXenobiologisBaseOptions
        {
            get
            {
                bool XBOptions = settings.ShowXenobiologisSettings;
                bool XBRaceOptions = settings.ShowAllowedRaceSettings && XBOptions;
                float lineheight = (Text.LineHeight + listing_Main.verticalSpacing);
                float num = Length(XBOptions, 1, lineheight, 0, 0);
                return num;
            }
        }
        public float MenuLengthXenobiologisBaseRace
        {
            get
            {
                bool XBOptions = settings.ShowXenobiologisSettings;
                bool XBRaceOptions = settings.ShowAllowedRaceSettings && XBOptions;
                float lineheight = (Text.LineHeight + listing_Main.verticalSpacing);
                float num = Length(XBRaceOptions, 1, lineheight, 64, XBOptions ? 1 : 0);
                return num;
            }
        }

        public float MenuLengthXenobiologisRacesImperial
        {
            get
            {
                float lineheight = (Text.LineHeight + listing_Main.verticalSpacing);
                bool showXB = settings.ShowXenobiologisSettings;
                bool showRaces = settings.ShowAllowedRaceSettings && showXB;
                bool showImperial = settings.ShowImperium;
                bool Options = showXB && showRaces && showImperial;
                float num = 0;
                num += Length(Options, 1, lineheight, 8, showRaces ? 1 : 0);
                num += MenuLengthXenobiologisRacesImperialOptions;
                if (AdeptusIntergrationUtil.enabled_AdeptusAstartes && Options && settings.AllowAdeptusAstartes)
                {
                    num += MenuLengthXenobiologisRacesAstartes + 8;
                }
                return num;
            }
        }
        public float MenuLengthXenobiologisRacesImperialOptions
        {
            get
            {
                float lineheight = (Text.LineHeight + listing_Main.verticalSpacing);
                bool showXB = settings.ShowXenobiologisSettings;
                bool showRaces = settings.ShowAllowedRaceSettings;
                bool showImperial = settings.ShowImperium;
                bool Options = showXB && showRaces && showImperial;
                float num = 0;
                num += Length(Options, 2, lineheight, 0, 0);
                return num;
            }
        }

        public float MenuLengthXenobiologisRacesAstartes
        {
            get
            {
                if (AdeptusIntergrationUtil.enabled_AdeptusAstartes)
                {
                    bool showXB = settings.ShowXenobiologisSettings;
                    bool showRaces = settings.ShowAllowedRaceSettings;
                    bool showImperial = settings.ShowImperium;
                    bool allowAstartes = settings.AllowAdeptusAstartes;// && settings.ShowAstartes;
                    bool showAstartes = settings.ShowAstartes;
                    float lineheight = (Text.LineHeight + listing_Main.verticalSpacing);
                    bool options = showXB && showRaces && showImperial;
                    float num = Length(options && allowAstartes && showAstartes, 1, lineheight, 8, options && allowAstartes ? 1 : 0);
                    num += MenuLengthXenobiologisRacesAstartesOptions;
                    return num;
                }
                else
                {
                    return 0f;
                }
            }
        }

        public float MenuLengthXenobiologisRacesAstartesOptions
        {
            get
            {
                if (AdeptusIntergrationUtil.enabled_AdeptusAstartes)
                {
                    bool showXB = settings.ShowXenobiologisSettings;
                    bool showRaces = settings.ShowAllowedRaceSettings;
                    bool showImperial = settings.ShowImperium;
                    bool allowAstartes = settings.AllowAdeptusAstartes;// && settings.ShowAstartes;
                    bool showAstartes = settings.ShowAstartes;
                    float lineheight = (Text.LineHeight + listing_Main.verticalSpacing);
                    bool options = showXB && showRaces && showImperial && allowAstartes && showAstartes;
                    float num = Length(options, 2, lineheight, 42, 0);
                    return num;
                }
                else
                {
                    return 0f;
                }
            }
        }

        public float MenuLengthXenobiologisRaces
        {
            get
            {
                float num = 0;
                if (settings.ShowXenobiologisSettings)
                {
                    num += XenobiologisRaceMenuLength;
                    if (settings.ShowAllowedRaceSettings)
                    {
                        num += MenuLengthXenobiologisRaceContents;
                    }
                }
                return num;
            }
        }
        public float MenuLengthXenobiologisRaceContents
        {
            get
            {
                float num = 0;
                if (settings.ShowXenobiologisSettings && settings.ShowAllowedRaceSettings)
                {
                    num += MenuLengthXenobiologisRacesImperial;
                    num += XenobiologisChaosMenuLength;
                    num += XenobiologisEldarMenuLength;
                    num += XenobiologisDarkEldarMenuLength;
                    num += XenobiologisTauMenuLength;
                    num += XenobiologisOrkMenuLength;
                    num += XenobiologisNecronMenuLength;
                    num += XenobiologisTyranidMenuLength;
                }
                /*
                if (settings.ShowXenobiologisSettings && settings.ShowAllowedRaceSettings)
                {
                    num += settings.ShowImperium ? XenobiologisImperialMenuLength : 0;
                    num += settings.ShowChaos ? XenobiologisChaosMenuLength : 0;
                    num += settings.ShowEldar ? XenobiologisEldarMenuLength : 0;
                    num += settings.ShowDarkEldar ? XenobiologisDarkEldarMenuLength : 0;
                    num += settings.ShowTau ? XenobiologisTauMenuLength : 0;
                    num += settings.ShowOrk ? XenobiologisOrkMenuLength : 0;
                    num += settings.ShowNecron ? XenobiologisNecronMenuLength : 0;
                    num += settings.ShowTyranid ? XenobiologisTyranidMenuLength : 0;
                }
                */
                return num;
            }
        }

        public virtual void XenobiologisSettings(ref Listing_Standard listing_Main, Rect rect, Rect inRect, float num, float xenobiologisMenuLenght)
        {

        }

        public virtual void ImperialSettings(ref Listing_Standard listing_Main, Rect rect, Rect inRect, float num, float num2)
        {
            /*
            AstartesSettings(ref listing_Main, rect, inRect, num, num2);
            MechanicusSettings(ref listing_Main, rect, inRect, num, num2);
            MilitarumSettings(ref listing_Main, rect, inRect, num, num2);
            */
        }
        public virtual void AstartesSettings(ref Listing_Standard listing_Main, Rect rect, Rect inRect, float num, float num2)
        {

        }
        public virtual void MechanicusSettings(ref Listing_Standard listing_Main, Rect rect, Rect inRect, float num, float num2)
        {

        }
        public virtual void MilitarumSettings(ref Listing_Standard listing_Main, Rect rect, Rect inRect, float num, float num2)
        {

        }
        public virtual void ChaosSettings(ref Listing_Standard listing_Main, Rect rect, Rect inRect, float num, float num2)
        {

        }
        public virtual void EldarSettings(ref Listing_Standard listing_Main, Rect rect, Rect inRect, float num, float num2)
        {

        }
        public virtual void DarkEldarSettings(ref Listing_Standard listing_Main, Rect rect, Rect inRect, float num, float num2)
        {

        }
        public virtual void OrkSettings(ref Listing_Standard listing_Main, Rect rect, Rect inRect, float num, float num2)
        {

        }
        public virtual void TauSettings(ref Listing_Standard listing_Main, Rect rect, Rect inRect, float num, float num2)
        {

        }
        public virtual void NecronSettings(ref Listing_Standard listing_Main, Rect rect, Rect inRect, float num, float num2)
        {

        }
        public virtual void TyranidSettings(ref Listing_Standard listing_Main, Rect rect, Rect inRect, float num, float num2)
        {

        }

        private Vector2 pos = new Vector2(0f, 0f);
        private Vector2 pos2 = new Vector2(0f, 0f);

        public override void WriteSettings()
        {
            base.WriteSettings();
        }
    }
    
}