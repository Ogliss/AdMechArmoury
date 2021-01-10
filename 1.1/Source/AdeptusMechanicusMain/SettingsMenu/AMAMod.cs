using AdeptusMechanicus.ExtensionMethods;
using AdeptusMechanicus.HarmonyInstance;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
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
            Instance = this;
            AMMod.settings = GetSettings<AMSettings>();
            SettingsHelper.latest = AMMod.settings;
            AMSettings.Instance = AMMod.settings;
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            HarmonyPatches.PatchPawnsArrivalModeWorker(harmony);
            /*
            if (AdeptusIntergrationUtility.enabled_rooloDualWield)
            {
                harmony.Patch(AccessTools.Method(GenTypes.GetTypeInAnyAssembly("CompOversizedWeapon.HarmonyCompOversizedWeapon", "CompOversizedWeapon"), "DrawEquipmentAimingPreFix", null, null), new HarmonyMethod(Main.patchType, "DrawEquipmentAiming_DualWield_OverSized_PreFix", null), new HarmonyMethod(Main.patchType, "DrawEquipmentAiming_DualWield_OverSized_PostFix", null));
                harmony.Patch(AccessTools.Method(GenTypes.GetTypeInAnyAssembly("CompActivatableEffect.HarmonyCompActivatableEffect", "CompActivatableEffect"), "DrawEquipmentAimingPostFix", null, null), new HarmonyMethod(Main.patchType, "DrawEquipmentAiming_DualWield_Activatable_PreFix", null));

                harmony.Patch(AccessTools.Method(GenTypes.GetTypeInAnyAssembly("DualWield.Harmony.PawnRenderer_DrawEquipmentAiming", "DualWield.Harmony"), "DrawEquipmentAimingOverride", null, null), new HarmonyMethod(Main.patchType, "DrawEquipmentAimingOverride_DualWield_compActivatableEffect_PreFix", null));
                harmony.Patch(AccessTools.Method(GenTypes.GetTypeInAnyAssembly("DualWield.Ext_Pawn_EquipmentTracker", "DualWield"), "AddOffHandEquipment", null, null),null , new HarmonyMethod(Main.patchType, "AddOffHandEquipment_PostFix", null));
                harmony.Patch(AccessTools.Method(GenTypes.GetTypeInAnyAssembly("DualWield.Harmony.PawnWeaponGenerator_TryGenerateWeaponFor", "DualWield.Harmony"), "Postfix", null, null), new HarmonyMethod(Main.patchType, "PawnWeaponGenerator_TryGenerateWeaponFor_PostFix", null));
            }
            else
            {

            //    harmony.Patch(AccessTools.Method(GenTypes.GetTypeInAnyAssembly("AdeptusMechanicus.HarmonyCompOversizedWeapon", "AdeptusMechanicus"), "DrawEquipmentAimingPreFix", null, null), new HarmonyMethod(typeof(HarmonyPatch), "DrawEquipmentAiming_ActivatableEffect_OverSized_PreFix", null), new HarmonyMethod(typeof(HarmonyPatch), "DrawEquipmentAiming_ActivatableEffect_OverSized_PostFix", null));
            //    harmony.Patch(AccessTools.Method(GenTypes.GetTypeInAnyAssembly("AdeptusMechanicus.HarmonyCompActivatableEffect", "AdeptusMechanicus"), "DrawEquipmentAimingPostFix", null, null), new HarmonyMethod(typeof(HarmonyPatch), "DrawEquipmentAimingPostFix_OverSized_Activatable_PreFix", null));

            }
            */
            if (AdeptusIntergrationUtility.enabled_ResearchPal)
            {
            //    harmony.Patch(AccessTools.Method(GenTypes.GetTypeInAnyAssembly("ResearchPal.Tree", "ResearchPal"), "DrawEquipmentAimingPostFix", null, null), new HarmonyMethod(typeof(AM_ResearchProjectDef_get_PrerequisitesCompleted_CommonTech_ResearchPal_Patch), "Postfix", null));
            }
            VOPT = AccessTools.GetMethodNames(typeof(Listing_Standard)).Contains("BeginSection_NewTemp");
            showArmouryIntergrationMenu = !Patches.NullOrEmpty();
            if (!Patches.NullOrEmpty())
            {
                IntergrationOptions = (int)Mathf.Round((Patches.Count / 2) + 0.25f);
                var allPatches = content.Patches as List<PatchOperation>;
                foreach (var patch in Patches)
                {
                    if (settings.PatchDisabled[patch] == false)
                    {
                        if (Prefs.DevMode) Log.Message("RemoveAll XML Patch: " + patch.label);
                        allPatches.RemoveAll(p => p.sourceFile.EndsWith(patch.file));
                    }
                    else
                    {
                        if (Prefs.DevMode) Log.Message("Running XML Patch: " + patch.label);
                    }
                }
            }

            listing_Main = new Listing_Standard();
            listing_Armoury = new Listing_Standard();
            listing_Xeno = new Listing_Standard();
            //    listing_Main = new Listing_Standard();
            //    listing_Main = new Listing_Standard();
            listing_Astartes = new Listing_Standard();
            listing_DarkEldar = new Listing_Standard();
            listing_Eldar = new Listing_Standard();
            listing_Tau = new Listing_Standard();
            listing_Ork = new Listing_Standard();
            listing_Necron = new Listing_Standard();
            listing_Tyranid = new Listing_Standard();
            if (Prefs.DevMode) Log.Message(string.Format("Adeptus Mecanicus: Armoury: successfully completed {0} harmony patches.", harmony.GetPatchedMethods().Select(new Func<MethodBase, Patches>(Harmony.GetPatchInfo)).SelectMany((Patches p) => p.Prefixes.Concat(p.Postfixes).Concat(p.Transpilers)).Count((Patch p) => p.owner.Contains(harmony.Id))), false);
        }

        public bool Dev => Prefs.DevMode && SteamUtility.SteamPersonaName.Contains("Ogliss");
        public override string SettingsCategory() => "AM_ModSeries".Translate();

        public string ModLoaded() => "Mods Loaded: " + "AMA_ModName".Translate();

        public override void DoSettingsWindowContents(Rect inRect)
        {
            //    float ml = MainSectionLength != 0 ? MainSectionLength : MenuLengthTotal;
        //    float ml = MenuLengthTotal;
            float ml = ModOptions_MenuLength;
            Rect inRect1 = inRect.TopPart(0.05f);
            Rect inRect2 = inRect.BottomPart(0.95f);
            Rect viewRect = inRect2.ContractedBy(4);
            float width = inRect2.ContractedBy(4).width - 20;

            PreModOptions(listing_Main, inRect1, width, ref ml, ModLoaded());
            Rect rect = new Rect(inRect2.x, inRect2.y, width, ml);
            float height = viewRect.height;
            //  Rect rectHost = rectBottom.LeftHalf().BottomPart(0.9f);
            //  Rect hostRect = new Rect(rectHost.x, rectHost.y, rectHost.width - 20, (potentialhostcount * (Text.LineHeight + listing_Main.verticalSpacing)));
            //    listing_Main.BeginScrollView(viewRect, ref pos, ref rect);
            Widgets.BeginScrollView(viewRect, ref this.pos, rect, true);
            //    ModOptions(ref listing_Main, rect, inRect2, width, ref ml);
            ModOptions_New(ref rect, inRect2, rect.ContractedBy(4).width, ref ml);
            Widgets.EndScrollView();
            //    listing_Main.EndScrollView(ref rect);
            PostModOptions(listing_Main, inRect2, width, ml);

        }

        private static float lineheight = (Text.LineHeight + 2f);
        private float ModOptions_MenuLength
        {
            get
            {
                float num = 32f;
                num += listing_ArmouryLength;
                if (AdeptusIntergrationUtility.enabled_MagosXenobiologis)
                {
                //    modOptions_MenuLength += 32f;
                    num += MenuLengthXenobiologisTotal;
                }
                else
                {
                    if (AdeptusIntergrationUtility.enabled_AdeptusAstartes)
                    {
                        num += +32f;
                        num += MenuLengthXenobiologisRacesAstartes;
                    }
                    if (AdeptusIntergrationUtility.enabled_XenobiologisChaos)
                    {
                        num += +32f;
                        num += XenobiologisChaosMenuLength;
                    }
                    if (AdeptusIntergrationUtility.enabled_XenobiologisEldar)
                    {
                        num += +32f;
                        num += XenobiologisEldarMenuLength;
                    }
                    if (AdeptusIntergrationUtility.enabled_XenobiologisDarkEldar)
                    {
                        num += +32f;
                        num += XenobiologisDarkEldarMenuLength;
                    }
                    if (AdeptusIntergrationUtility.enabled_XenobiologisTau)
                    {
                        num += +32f;
                        num += XenobiologisTauMenuLength;
                    }
                    if (AdeptusIntergrationUtility.enabled_XenobiologisOrk)
                    {
                        num += +32f;
                        num += XenobiologisOrkMenuLength;
                    }
                    if (AdeptusIntergrationUtility.enabled_XenobiologisNecron)
                    {
                        num += +32f;
                        num += XenobiologisNecronMenuLength;
                    }
                    if (AdeptusIntergrationUtility.enabled_XenobiologisTyranid)
                    {
                        num += +32f;
                        num += XenobiologisTyranidMenuLength;
                    }
                }
                if (showArmouryIntergrationMenu)
                {
                    num += 32f;
                    num += listing_ArmouryIntergrationLength;
                    if (settings.ShowArmourySettings)
                    {
                        num += showArmouryIntergrationOptions ? 10f : 8f;
                    }
                    if (AdeptusIntergrationUtility.enabled_MagosXenobiologis)
                    {
                        if (settings.ShowXenobiologisSettings)
                        {
                            num += showArmouryIntergrationOptions ? 14f : 12f;
                        }
                        else
                        {
                            num += 10f;
                        }
                    }
                    else
                    {
                        num += 8f;
                        
                        if (AdeptusIntergrationUtility.enabled_AdeptusAstartes)
                        {
                            num += settings.ShowAstartes ? 12f : 0f;
                        }
                        if (AdeptusIntergrationUtility.enabled_XenobiologisChaos)
                        {
                            num += settings.ShowChaos ? 8f : 0f;
                        }
                        if (AdeptusIntergrationUtility.enabled_XenobiologisEldar)
                        {
                            num += settings.ShowEldar ? 8f : 0f;
                        }
                        if (AdeptusIntergrationUtility.enabled_XenobiologisDarkEldar)
                        {
                            num += settings.ShowDarkEldar ? 8f : 0f;
                        }
                        if (AdeptusIntergrationUtility.enabled_XenobiologisTau)
                        {
                            num += settings.ShowTau ? 8f : 0f;
                        }
                        if (AdeptusIntergrationUtility.enabled_XenobiologisOrk)
                        {
                            num += settings.ShowOrk ? 8f : 0f;
                        }
                        if (AdeptusIntergrationUtility.enabled_XenobiologisNecron)
                        {
                            num += settings.AllowNecron ? 8f : 0f;
                        }
                        if (AdeptusIntergrationUtility.enabled_XenobiologisTyranid)
                        {
                            num += settings.AllowTyranid ? 8f : 0f;
                        }
                        
                    }
                    if (AdeptusIntergrationUtility.enabled_AlienRaces)
                    {

                    }
                }
                return num;
            }
        }
        private float listing_GeneralSpecialRulesLength => Length(settings.ShowArmourySettings && settings.ArmouryGeneralSpecialRules, 2, lineheight, 8, settings.ShowArmourySettings ? 1 : 0);
        private float listing_GeneralSpecialRulesContents => Length(settings.ArmouryGeneralSpecialRules, 1, lineheight, 0, 0);
        private float listing_WeaponSpecialRulesLength => Length(settings.ShowArmourySettings && settings.ShowAllowedWeaponSpecialRules, 5, lineheight, 8, settings.ShowArmourySettings ? 1 : 0);
        private float listing_WeaponSpecialRulesContents => Length(settings.ShowAllowedWeaponSpecialRules, 4, lineheight, 0, 0);
        private float listing_AllowedWeaponRulesLength => Length(settings.ShowArmourySettings && settings.ShowAllowedWeapons, 4, lineheight, 8, settings.ShowArmourySettings ? 1 : 0);
        private float listing_AllowedWeaponRulesContents => Length(settings.ShowAllowedWeapons, 3, lineheight, 0, 0);
        private float listing_ArmouryLength => (settings.ShowArmourySettings ? 16 : 0) + listing_ArmouryContents + armouryMenuInc;
        private float listing_ArmouryContents => listing_GeneralSpecialRulesLength + listing_WeaponSpecialRulesLength + listing_AllowedWeaponRulesLength;
        public void ArmouryMenu()
        {

            // Armoury Mod Options
            listing_Armoury = listing_Main.BeginSection(listing_ArmouryLength, false, 3, 4, 0);
            // Armoury mod General Special rules options menu
            Listing_Standard listing_GeneralSpecialRules = listing_Armoury.BeginSection(listing_GeneralSpecialRulesLength, false, 3, 4, 0);
            listing_GeneralSpecialRules.CheckboxLabeled("AMA_ShowSpecialRules".Translate() + (Dev ? " Menu Length: " + listing_GeneralSpecialRules.CurHeight : ""), ref settings.ArmouryGeneralSpecialRules, "AMA_ShowSpecialRulesDesc".Translate(), false, false, ArmouryMain.collapseTex, ArmouryMain.expandTex);
            if (settings.ArmouryGeneralSpecialRules)
            {
                Listing_Standard listing_General = listing_GeneralSpecialRules.BeginSection(listing_GeneralSpecialRulesContents, true, 0, 4, 0);
                listing_General.ColumnWidth *= 0.488f;
                listing_General.CheckboxLabeled("AMA_AllowDeepStrike".Translate(), ref settings.AllowDeepStrike, "AMA_AllowDeepStrikeDesc".Translate());
                listing_General.NewColumn();
                listing_General.CheckboxLabeled("AMA_AllowInfiltrate".Translate(), ref settings.AllowInfiltrate, "AMA_AllowInfiltrateDesc".Translate());
                listing_GeneralSpecialRules.EndSection(listing_General);
            }
            listing_Armoury.EndSection(listing_GeneralSpecialRules);
            // Armoury mod Weapon Special rules options menu

            Listing_Standard listing_WeaponSpecialRules = listing_Armoury.BeginSection(listing_WeaponSpecialRulesLength, false, 3, 4, 0);
            listing_WeaponSpecialRules.CheckboxLabeled("AMA_ShowWeaponSpecialRules".Translate() + (Dev ? " Menu Length: " + MenuLengthArmouryWeaponSpecialRules + " " + ArmouryWeaponSpecialRules : ""), ref settings.ShowAllowedWeaponSpecialRules, "AMA_ShowWeaponSpecialRulesDesc".Translate(), false, false, ArmouryMain.collapseTex, ArmouryMain.expandTex);
            if (settings.ShowAllowedWeaponSpecialRules)
            {
                Listing_Standard listing_General = listing_WeaponSpecialRules.BeginSection(listing_WeaponSpecialRulesContents, true);
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
            listing_Armoury.EndSection(listing_WeaponSpecialRules);
            // Armoury mod Allowed Weapons options menu
            Listing_Standard listing_AllowedWeapons = listing_Armoury.BeginSection(listing_AllowedWeaponRulesLength, false, 3, 4, 0);
            listing_AllowedWeapons.CheckboxLabeled("AMA_ShowAllowedWeapons".Translate() + (Dev ? " Menu Length: " + MenuLengthArmouryAllowedWeapons + " " + ArmouryAllowedWeapons : ""), ref settings.ShowAllowedWeapons, "AMA_ShowAllowedWeaponsDesc".Translate(), false, false, ArmouryMain.collapseTex, ArmouryMain.expandTex);
            if (settings.ShowAllowedWeapons)
            {
                Listing_Standard listing_General = listing_AllowedWeapons.BeginSection(listing_AllowedWeaponRulesContents, true);
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
            listing_Armoury.EndSection(listing_AllowedWeapons);

            listing_Main.EndSection(listing_Armoury);
        }
        private float armouryMenuInc = 0f;

        public int patchesCount => Patches.Count % 2  == 0 ? Patches.Count / 2 : Patches.Count / 2 + 1;
        private float listing_ArmouryIntergrationLength => Length(this.showArmouryIntergrationOptions, 1, lineheight, 8, 0) + listing_ArmouryIntergrationContents + intergrationMenuInc;
        private float listing_ArmouryIntergrationContents => Length(this.showArmouryIntergrationOptions, patchesCount, lineheight, 0, 0);
        public void IntergrationMenu()
        {

            Listing_Standard listing_ArmouryIntergration = listing_Main.BeginSection(listing_ArmouryIntergrationLength, false, 3);
            //   listing_ArmouryIntergration = listing_Main.BeginSection(MenuLengthIntergration, false, 0);
            listing_ArmouryIntergration.Label("Changes to these settings require a restart to take effect." + (Dev ? " patchesCount: " + patchesCount : ""));
            Listing_Standard listing_General = listing_ArmouryIntergration.BeginSection(listing_ArmouryIntergrationContents, true);
            listing_General.ColumnWidth *= 0.488f;
            bool flag = false;
            for (int i = 0; i < Patches.Count; i++)
            {
                var patch = Patches[i];
                var status = settings.PatchDisabled[patch];
                if (!flag && i+1 > Patches.Count / 2)
                {
                    listing_General.NewColumn();
                    flag = true;
                }
                listing_General.CheckboxLabeled(patch.label, ref status, patch.tooltip);

                settings.PatchDisabled[patch] = status;

            }
            listing_ArmouryIntergration.EndSection(listing_General);
            if (AdeptusIntergrationUtility.enabled_AlienRaces)
            {

            }
            listing_Main.EndSection(listing_ArmouryIntergration);
        }
        private float intergrationMenuInc = 0f;


        private float listing_XenobiologisLength => Length(this.showArmouryIntergrationOptions, 1, lineheight, 8, 0) + listing_ArmouryIntergrationContents + intergrationMenuInc;
        private float listing_XenobiologisContents => Length(this.showArmouryIntergrationOptions, patchesCount, lineheight, 0, 0);

        private float XenobiologisMenuInc = 0f;
        //    private float listing_ArmouryIntergrationContents => Length(this.showArmouryIntergrationOptions, patchesCount, lineheight, 0, 0);
        public void ModOptions_New(ref Rect rect, Rect inRect, float width, ref float ml)
        {
            listing_Main.Begin(rect);

            //    listing_ArmourySettings.CheckboxLabeled("AMA_ModName".Translate() + " Settings" + (Dev ? " Menu Length: " + MenuLengthArmoury : ""), ref settings.ShowArmourySettings, "AMA_ShowSpecialRulesDesc".Translate());

            string labelA = "AMA_ModName".Translate() + " Settings";
            string tooltipA = "AMA_ShowSpecialRulesDesc".Translate();
            if (Dev)
            {
                labelA = "AMA_ModName".Translate() + " Menu Length: " + listing_ArmouryContents + " Total Length: " + listing_ArmouryLength + " " + settings.ShowArmourySettings + " CurInc: " + armouryMenuInc;
                Rect r = listing_Main.GetRect(30f);
                if (listing_Main.ButtonText(r.LeftPart(0.75f).ContractedBy(4), labelA, ref settings.ShowArmourySettings, tooltipA))
                {
                    ArmouryMenu();
                }
                if (Widgets.ButtonText(r.RightPart(0.25f).LeftHalf().ContractedBy(4), "-"))
                {
                    armouryMenuInc--;
                }
                if (Widgets.ButtonText(r.RightPart(0.25f).RightHalf().ContractedBy(4), "+"))
                {
                    armouryMenuInc++;
                }
            }
            else
            {
                if (listing_Main.ButtonText(labelA, ref settings.ShowArmourySettings, tooltipA))
                {
                    ArmouryMenu();
                }
            }
            {
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
            }
            if (AdeptusIntergrationUtility.enabled_MagosXenobiologis)
            {
                string labelXB = "AMXB_ModName".Translate() + " Settings";
                string tooltipXB = "AMA_ShowSpecialRulesDesc".Translate();
                if (Dev)
                {
                    labelXB = "AMXB_ModName".Translate() + " Menu Length: " + MenuLengthXenobiologis + " Total Length: " + MenuLengthXenobiologisTotal  + " Race Menu Length: " + MenuLengthXenobiologisRaces +" " + settings.ShowXenobiologisSettings + " CurInc: " + XenobiologisMenuInc;
                    Rect r = listing_Main.GetRect(30f);
                    if (listing_Main.ButtonText(r.LeftPart(0.75f).ContractedBy(4), labelXB, ref settings.ShowXenobiologisSettings, tooltipXB))
                    {
                        XenobiologisSettings(ref listing_Main, rect, inRect, width, XenobiologisMenuLength);
                    }
                    if (Widgets.ButtonText(r.RightPart(0.25f).LeftHalf().ContractedBy(4), "-"))
                    {
                        XenobiologisMenuInc--;
                    }
                    if (Widgets.ButtonText(r.RightPart(0.25f).RightHalf().ContractedBy(4), "+"))
                    {
                        XenobiologisMenuInc++;
                    }
                }
                else
                {
                    if (listing_Main.ButtonText(labelXB, ref settings.ShowXenobiologisSettings, tooltipXB))
                    {
                        XenobiologisSettings(ref listing_Main, rect, inRect, width, XenobiologisMenuLength);
                    }
                }
            }
            else
            {
                ImperialSettings(ref listing_Main, rect, inRect, width, ml);
                ChaosSettings(ref listing_Main, rect, inRect, width, ml);
                EldarSettings(ref listing_Main, rect, inRect, width, ml);
                DarkEldarSettings(ref listing_Main, rect, inRect, width, ml);
                OrkSettings(ref listing_Main, rect, inRect, width, ml);
                TauSettings(ref listing_Main, rect, inRect, width, ml);
                NecronSettings(ref listing_Main, rect, inRect, width, ml);
                TyranidSettings(ref listing_Main, rect, inRect, width, ml);
            }
            if (showArmouryIntergrationMenu)
            {
                string labelI = "AMA_IntergrationOptions".Translate() + " Settings";
                string tooltipI = "AMA_ShowSpecialRulesDesc".Translate();
                if (Dev)
                {
                    labelI = "AMA_IntergrationOptions".Translate() + " Menu Length: " + listing_ArmouryIntergrationContents + " Total Length: " + listing_ArmouryIntergrationLength + " " + showArmouryIntergrationOptions + " CurInc: " + intergrationMenuInc;
                    Rect r = listing_Main.GetRect(30f);
                    if (listing_Main.ButtonText(r.LeftPart(0.75f).ContractedBy(4), labelI, ref showArmouryIntergrationOptions, tooltipI))
                    {
                        IntergrationMenu();
                    }
                    if (Widgets.ButtonText(r.RightPart(0.25f).LeftHalf().ContractedBy(4), "-"))
                    {
                        intergrationMenuInc--;
                    }
                    if (Widgets.ButtonText(r.RightPart(0.25f).RightHalf().ContractedBy(4), "+"))
                    {
                        intergrationMenuInc++;
                    }
                }
                else
                {
                    if (listing_Main.ButtonText(labelI, ref showArmouryIntergrationOptions, tooltipI))
                    {
                        IntergrationMenu();
                    }
                }
            }
            listing_Main.End();
        }

        public override void PreModOptions(Listing_Standard listing_Main, Rect inRect, float width, ref float menuLength, string label)
        {
            if (Dev)
            {
                if (Widgets.ButtonText(inRect.LeftPart(0.75f).ContractedBy(4), label + " Menulenght CurInc: " + inc))
                {
                    ResetMenu();
                }
                if (Widgets.ButtonText(inRect.RightPart(0.25f).LeftHalf().ContractedBy(4), "-"))
                {
                    inc--;
                }
                if (Widgets.ButtonText(inRect.RightPart(0.25f).RightHalf().ContractedBy(4), "+"))
                {
                    inc++;
                }
                menuLength += inc; 
                string tooltip = "Original menuLength: " + menuLength;
                tooltip += "\nMenuLengthArmoury: " + listing_ArmouryLength + " Total: " + MenuLengthArmouryTotal + " Show Options: " + settings.ShowArmourySettings;
                if (AdeptusIntergrationUtility.enabled_MagosXenobiologis)
                {
                    tooltip += "\nMenuLengthXenobiologis: " + MenuLengthXenobiologis + " Total: " + MenuLengthXenobiologisTotal + " Show Menu: " + AdeptusIntergrationUtility.enabled_MagosXenobiologis + " Show Options: " + settings.ShowXenobiologisSettings;
                }
                tooltip += "\nMenuLengthIntergration: " + listing_ArmouryIntergrationContents + " Total: " + listing_ArmouryIntergrationLength + " Show Menu: "+ showArmouryIntergrationMenu + " Show Options: "+showArmouryIntergrationOptions;
                tooltip += "\nMainSectionLength: " + listing_Main.CurHeight;
                tooltip += "\nMaxColumnHeightSeen: " + listing_Main.MaxColumnHeightSeen;
            //    menuLength = listing_Main.MaxColumnHeightSeen;
                //   menuLength = MainSectionLength;
                TooltipHandler.TipRegion(inRect.ContractedBy(4), tooltip);
            }
            else
            {
                Widgets.Label(inRect.ContractedBy(4), label);
            }

            if (width > width * 2)
            {
            //    log.message(string.Format("PreModOptions Listing: {0}, inRect: {1}, num: {2}, num2: {3}", listing_Main, inRect, width, menuLength));
            }

        }
        private float inc = 0;
        private int MaxColumnHeightSeen = 0;
        public static bool VOPT;
        bool showArmouryIntergrationMenu;
        bool showArmouryIntergrationOptions = false;
        public static int IntergrationOptions;
        public static Listing_Standard listing_Main;
        public static Listing_Standard listing_Armoury;
        public static Listing_Standard listing_Xeno;
        public static Listing_Standard listing_Astartes;
        public static Listing_Standard listing_DarkEldar;
        public static Listing_Standard listing_Eldar;
        public static Listing_Standard listing_Tau;
        public static Listing_Standard listing_Ork;
        public static Listing_Standard listing_Necron;
        public static Listing_Standard listing_Tyranid;
    //    public float TotalMenuLength = 72;
        public float MainSectionLength = 0;
        public float IntergrationSectionLength = 0;

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
        public float XenobiologisAstartesMenuLength = 0;
        public float XenobiologisChaosMenuLength = 0;
        public float XenobiologisEldarMenuLength = 0;
        public float XenobiologisDarkEldarMenuLength = 0;
        public float XenobiologisTauMenuLength = 0;
        public float XenobiologisOrkMenuLength = 0;
        public float XenobiologisTyranidMenuLength = 0;
        public float XenobiologisNecronMenuLength = 0;

        public virtual void ResetMenu()
        {
            settings.ShowArmourySettings = false;
            settings.ArmouryGeneralSpecialRules = false;
            settings.ShowAllowedWeapons = false;
            settings.ShowAllowedWeaponSpecialRules = false;
            settings.ShowXenobiologisSettings = false;
            settings.ShowAllowedRaceSettings = false;
            settings.ShowAstartes = false;
            settings.ShowChaos = false;
            settings.ShowDarkEldar = false;
            settings.ShowEldar = false;
            settings.ShowImperium = false;
            settings.ShowInquisition = false;
            settings.ShowMechanicus = false;
            settings.ShowMilitarum = false;
            settings.ShowNecron = false;
            settings.ShowOrk = false;
            settings.ShowSororitas = false;
            settings.ShowTau = false;
            settings.ShowTyranid = false;
            showArmouryIntergrationOptions = false;
            inc = 0;
        }
        public override void ModOptions(ref Listing_Standard listing_Main, Rect rect, Rect inRect, float width, ref float ml)
        {
            listing_Main.Begin(rect);
            Listing_Standard listing_ArmourySettings = new Listing_Standard();
            Listing_Standard listing_GeneralSpecialRules = new Listing_Standard();
            Listing_Standard listing_WeaponSpecialRules = new Listing_Standard();
            Listing_Standard listing_AllowedWeapons = new Listing_Standard();
            //    listing_Main.CheckboxLabeled("AMA_ModName".Translate() + " Settings", ref settings.ShowArmourySettings, "AMA_ShowSpecialRulesDesc".Translate());
            float lineheight = (Text.LineHeight + listing_Main.verticalSpacing);

            //    listing_ArmourySettings.CheckboxLabeled("AMA_ModName".Translate() + " Settings" + (Dev ? " Menu Length: " + MenuLengthArmoury : ""), ref settings.ShowArmourySettings, "AMA_ShowSpecialRulesDesc".Translate());

            if (listing_Main.ButtonText("AMA_ModName".Translate() + " Settings" + (Dev ? " Menu Length: " + MenuLengthArmoury + " Total Length: " + MenuLengthArmouryTotal + " " + settings.ShowArmourySettings : ""), ref settings.ShowArmourySettings, "AMA_ShowSpecialRulesDesc".Translate()))
            {
                listing_ArmourySettings = listing_Main.BeginSection(MenuLengthArmoury, false, 3);
                // Armoury mod General Special rules options menu
                if (AccessTools.GetMethodNames(typeof(Listing_Standard)).Contains("BeginSection_NewTemp"))
                {
                    listing_GeneralSpecialRules = listing_ArmourySettings.BeginSection_OnePointTwo(MenuLengthArmouryGeneralSpecialRules);
                }
                else
                {
                    listing_GeneralSpecialRules = listing_ArmourySettings.BeginSection_OnePointOne(MenuLengthArmouryGeneralSpecialRules);
                }
                listing_GeneralSpecialRules.CheckboxLabeled("AMA_ShowSpecialRules".Translate() + (Dev ? " Menu Length: " + MenuLengthArmouryGeneralSpecialRules + " " + ArmouryGeneralSpecialRules : ""), ref settings.ArmouryGeneralSpecialRules, "AMA_ShowSpecialRulesDesc".Translate());
                if (settings.ArmouryGeneralSpecialRules)
                {
                    Listing_Standard listing_General = listing_GeneralSpecialRules.BeginSection(Length(settings.ArmouryGeneralSpecialRules, 1, lineheight, 0, 1), true);
                    listing_General.ColumnWidth *= 0.488f;
                    listing_General.CheckboxLabeled("AMA_AllowDeepStrike".Translate(), ref settings.AllowDeepStrike, "AMA_AllowDeepStrikeDesc".Translate());
                    listing_General.NewColumn();
                    listing_General.CheckboxLabeled("AMA_AllowInfiltrate".Translate(), ref settings.AllowInfiltrate, "AMA_AllowInfiltrateDesc".Translate());
                    listing_GeneralSpecialRules.EndSection(listing_General);
                }
                listing_ArmourySettings.EndSection(listing_GeneralSpecialRules);

                // Armoury mod Weapon Special rules options menu
                if (AccessTools.GetMethodNames(typeof(Listing_Standard)).Contains("BeginSection_NewTemp"))
                {
                    listing_WeaponSpecialRules = listing_ArmourySettings.BeginSection_OnePointTwo(MenuLengthArmouryWeaponSpecialRules);
                }
                else
                {
                    listing_WeaponSpecialRules = listing_ArmourySettings.BeginSection_OnePointOne(MenuLengthArmouryWeaponSpecialRules);
                }
                listing_WeaponSpecialRules.CheckboxLabeled("AMA_ShowWeaponSpecialRules".Translate() + (Dev ? " Menu Length: " + MenuLengthArmouryWeaponSpecialRules + " " + ArmouryWeaponSpecialRules : ""), ref settings.ShowAllowedWeaponSpecialRules, "AMA_ShowWeaponSpecialRulesDesc".Translate());
                if (settings.ShowAllowedWeaponSpecialRules)
                {
                    Listing_Standard listing_General = listing_WeaponSpecialRules.BeginSection(Length(settings.ShowAllowedWeaponSpecialRules, 4, lineheight, 0, 1), true);
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
                if (AccessTools.GetMethodNames(typeof(Listing_Standard)).Contains("BeginSection_NewTemp"))
                {
                    listing_AllowedWeapons = listing_ArmourySettings.BeginSection_OnePointTwo(MenuLengthArmouryAllowedWeapons);
                }
                else
                {
                    listing_AllowedWeapons = listing_ArmourySettings.BeginSection_OnePointOne(MenuLengthArmouryAllowedWeapons);
                }
                listing_AllowedWeapons.CheckboxLabeled("AMA_ShowAllowedWeapons".Translate() + (Dev ? " Menu Length: " + MenuLengthArmouryAllowedWeapons + " " + ArmouryAllowedWeapons : ""), ref settings.ShowAllowedWeapons, "AMA_ShowAllowedWeaponsDesc".Translate());
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
                listing_Main.EndSection(listing_ArmourySettings);
            }
            ArmouryGeneralSpecialRules = listing_GeneralSpecialRules.CurHeight;
            ArmouryWeaponSpecialRules = listing_WeaponSpecialRules.CurHeight;
            ArmouryAllowedWeapons = listing_AllowedWeapons.CurHeight;
            ArmourySettings = listing_ArmourySettings.CurHeight;

            {
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
            }
            if (AdeptusIntergrationUtility.enabled_MagosXenobiologis)
            {
                XenobiologisSettings(ref listing_Main, rect, inRect, width, XenobiologisMenuLength);
            }
            else
            {
                ImperialSettings(ref listing_Main, rect, inRect, width, ml);
                ChaosSettings(ref listing_Main, rect, inRect, width, ml);
                EldarSettings(ref listing_Main, rect, inRect, width, ml);
                DarkEldarSettings(ref listing_Main, rect, inRect, width, ml);
                OrkSettings(ref listing_Main, rect, inRect, width, ml);
                TauSettings(ref listing_Main, rect, inRect, width, ml);
                NecronSettings(ref listing_Main, rect, inRect, width, ml);
                TyranidSettings(ref listing_Main, rect, inRect, width, ml);
            }
            if (showArmouryIntergrationMenu)
            {
                string label = "Intergration Options" + (Dev ? " Menu Length: " + MenuLengthIntergration + " Total Length: " + MenuLengthIntergrationTotal + " " + showArmouryIntergrationOptions + " Patches : " + Patches.Count : "");
                string tooltip = "Intergration options for other mods";
                if (listing_Main.ButtonText(label, ref showArmouryIntergrationOptions, tooltip))
                {
                    //   IntergrationSettings(ref listing_Main, rect, inRect, width, ml);
                    Listing_Standard listing_ArmouryIntergration = listing_Main.BeginSection(MenuLengthIntergrationTotal, false, 3, 0, 0);
                    //   listing_ArmouryIntergration = listing_Main.BeginSection(MenuLengthIntergration, false, 0);
                    listing_ArmouryIntergration.Label("Changes to these settings require a restart to take effect");
                    Listing_Standard listing_General = listing_ArmouryIntergration.BeginSection(MenuLengthIntergration, true, 0, 0, 0);
                    listing_General.ColumnWidth *= 0.488f;
                    bool flag = false;
                    for (int i = 0; i < Patches.Count; i++)
                    {
                        var patch = Patches[i];
                        var status = settings.PatchDisabled[patch];
                        if (!flag && i > Patches.Count / 2)
                        {
                            listing_General.NewColumn();
                            flag = true;
                        }
                        listing_General.CheckboxLabeled(patch.label, ref status, patch.tooltip);

                        settings.PatchDisabled[patch] = status;

                    }
                    listing_ArmouryIntergration.EndSection(listing_General);
                    listing_Main.EndSection(listing_ArmouryIntergration);
                    IntergrationSectionLength = listing_ArmouryIntergration.CurHeight;
                }
            }
            MaxColumnHeightSeen = (int)listing_Main.MaxColumnHeightSeen;

            listing_Main.End();
            //    MenuLengthTotal = listing_Main.CurHeight;
            float f = Mathf.Max(listing_Main.MaxColumnHeightSeen, listing_Main.CurHeight) - Mathf.Min(listing_Main.MaxColumnHeightSeen, listing_Main.CurHeight);

            inc = Mathf.Max(0f, listing_Main.MaxColumnHeightSeen - listing_Main.CurHeight);
            MainSectionLength = listing_Main.CurHeight;
        }

        public virtual void IntergrationSettings(ref Listing_Standard listing_Main, Rect rect, Rect inRect, float num, float num2)
        {

            Listing_Standard listing_ArmouryIntergration = listing_Main.BeginSection(MenuLengthIntergration, false, 3);
            //   listing_ArmouryIntergration = listing_Main.BeginSection(MenuLengthIntergration, false, 0);
            listing_ArmouryIntergration.Label("Changes to these settings require a restart to take effect");
            Listing_Standard listing_General = listing_ArmouryIntergration.BeginSection(MenuLengthIntergration, true);
            listing_General.ColumnWidth *= 0.488f;
            bool flag = false;
            for (int i = 0; i < Patches.Count; i++)
            {
                var patch = Patches[i];
                var status = settings.PatchDisabled[patch];
                if (!flag && i > Patches.Count / 2)
                {
                    listing_General.NewColumn();
                    flag = true;
                }
                listing_General.CheckboxLabeled(patch.label, ref status, patch.tooltip);

                settings.PatchDisabled[patch] = status;

            }
            listing_ArmouryIntergration.EndSection(listing_General);
            listing_Main.EndSection(listing_ArmouryIntergration);
            IntergrationSectionLength = listing_ArmouryIntergration.CurHeight;
        }

        public override void PostModOptions(Listing_Standard listing_Main, Rect inRect, float width, float menuLength)
        {
            if (true)
            {

            }

            AMMod.settings.Write();
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
                num += MenuLengthArmouryTotal;
                num += MenuLengthXenobiologisTotal;
                if (!AdeptusIntergrationUtility.enabled_MagosXenobiologis)
                {
                    if (AdeptusIntergrationUtility.enabled_AdeptusAstartes)
                    {
                        num += +30f;
                        num += MenuLengthXenobiologisRacesAstartes;
                    }
                    if (AdeptusIntergrationUtility.enabled_XenobiologisChaos)
                    {
                        num += +30f;
                        num += XenobiologisChaosMenuLength;
                    }
                    if (AdeptusIntergrationUtility.enabled_XenobiologisEldar)
                    {
                        num += +30f;
                        num += XenobiologisEldarMenuLength;
                    }
                    if (AdeptusIntergrationUtility.enabled_XenobiologisDarkEldar)
                    {
                        num += +30f;
                        num += XenobiologisDarkEldarMenuLength;
                    }
                    if (AdeptusIntergrationUtility.enabled_XenobiologisTau)
                    {
                        num += +30f;
                        num += XenobiologisTauMenuLength;
                    }
                    if (AdeptusIntergrationUtility.enabled_XenobiologisOrk)
                    {
                        num += +30f;
                        num += XenobiologisOrkMenuLength;
                    }
                    if (AdeptusIntergrationUtility.enabled_XenobiologisNecron)
                    {
                        num += +30f;
                        num += XenobiologisNecronMenuLength;
                    }
                    if (AdeptusIntergrationUtility.enabled_XenobiologisTyranid)
                    {
                        num += +30f;
                        num += XenobiologisTyranidMenuLength;
                    }
                }
                if (showArmouryIntergrationMenu)
                {
                    num += MenuLengthIntergrationTotal;
                }
                return num;
            }
        }

        public float MenuLengthArmouryTotal
        {
            get
            {
                float num = 30f;
                num += MenuLengthArmoury;
                return num;
            }
        }

        public float MenuLengthArmoury
        {
            get
            {
                float num = MenuLengthArmouryBase;
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
                float num = Length(settings.ShowArmourySettings, 1, lineheight, 0, 0, 0);
                return num;
            }
        }


        public float MenuLengthArmouryGeneralSpecialRules
        {
            get
            {
                float num = Length(settings.ShowArmourySettings &&settings.ArmouryGeneralSpecialRules, 2, lineheight, 8, settings.ShowArmourySettings ? 1 : 0);
                return num;
            }
        }

        public float MenuLengthArmouryWeaponSpecialRules
        {
            get
            {
                float num = Length(settings.ShowArmourySettings && settings.ShowAllowedWeaponSpecialRules, 5, lineheight, 8, settings.ShowArmourySettings ? 1 : 0);
                return num;
            }
        }

        public float MenuLengthArmouryAllowedWeapons
        {
            get
            {
                float num = Length(settings.ShowArmourySettings && settings.ShowAllowedWeapons, 4, lineheight, 8, settings.ShowArmourySettings ? 1: 0);
                return num;
            }
        }

        public float MenuLengthXenobiologisTotal
        {
            get
            {
                float num = 0;
                if (AdeptusIntergrationUtility.enabled_MagosXenobiologis)
                {
                    num += 30;
                    num += MenuLengthXenobiologis;
                }
                return num;
            }
        }

        public float MenuLengthXenobiologis
        {
            get
            {
                float num = MenuLengthXenobiologisBase;
                if (AdeptusIntergrationUtility.enabled_MagosXenobiologis)
                {
                    num += MenuLengthXenobiologisBaseRace;
                    num += MenuLengthXenobiologisRaceContents;
                    num += XenobiologisMenuInc;
                }
                return num;
            }
        }

        public float MenuLengthXenobiologisBase
        {
            get
            {
                bool XBOptions = settings.ShowXenobiologisSettings;
                bool XBRaceOptions = settings.ShowAllowedRaceSettings && XBOptions;
                int Options = 2;
                float num = Length(XBOptions, Options, lineheight, 0, 0);
                return num;
            }
        }
        public float MenuLengthXenobiologisBaseOptions
        {
            get
            {
                bool XBOptions = settings.ShowXenobiologisSettings;
                bool XBRaceOptions = settings.ShowAllowedRaceSettings && XBOptions;
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
                float num = Length(XBRaceOptions, 1, lineheight, 64, XBOptions ? 1 : 0);
                return num;
            }
        }

        public float MenuLengthXenobiologisRacesImperial
        {
            get
            {
                bool showXB = settings.ShowXenobiologisSettings;
                bool showRaces = settings.ShowAllowedRaceSettings && showXB;
                bool showImperial = settings.ShowImperium;
                bool Options = showXB && showRaces && showImperial;
                float num = 0;
                num += Length(Options, 1, lineheight, 8, showRaces ? 1 : 0);
                num += MenuLengthXenobiologisRacesImperialOptions;
                if (AdeptusIntergrationUtility.enabled_AdeptusAstartes && Options && settings.AllowAdeptusAstartes)
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
                if (AdeptusIntergrationUtility.enabled_AdeptusAstartes)
                {
                    bool showXB = settings.ShowXenobiologisSettings;
                    bool showRaces = settings.ShowAllowedRaceSettings;
                    bool showImperial = settings.ShowImperium;
                    bool allowAstartes = settings.AllowAdeptusAstartes;// && settings.ShowAstartes;
                    bool showAstartes = settings.ShowAstartes;
                    bool options = (showXB && showRaces && showImperial) || !AdeptusIntergrationUtility.enabled_MagosXenobiologis;
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
                if (AdeptusIntergrationUtility.enabled_AdeptusAstartes)
                {
                    bool showXB = settings.ShowXenobiologisSettings;
                    bool showRaces = settings.ShowAllowedRaceSettings;
                    bool showImperial = settings.ShowImperium;
                    bool allowAstartes = settings.AllowAdeptusAstartes;// && settings.ShowAstartes;
                    bool showAstartes = settings.ShowAstartes;
                    bool options = ((showXB && showRaces && showImperial) || !AdeptusIntergrationUtility.enabled_MagosXenobiologis) && allowAstartes && showAstartes;
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

        public float MenuLengthIntergrationTotal
        {
            get
            {
                float num = 30;
                if (this.showArmouryIntergrationOptions)
                {
                    num += MenuLengthIntergration;
                }
                return num;
            }
        }

        public float MenuLengthIntergration
        {
            get
            {
                float num = MenuLengthIntergrationBase;
                return num;
            }
        }
        public float MenuLengthIntergrationBase
        {
            get
            {
                float num = Length(this.showArmouryIntergrationOptions, (int)Mathf.Round((Patches.Count / 2f) + 0.5f), lineheight, 0, 0);
                return num;
            }
        }

        public virtual void XenobiologisSettings(ref Listing_Standard listing_Main, Rect rect, Rect inRect, float num, float xenobiologisMenuLenght)
        {

        }
        public virtual void ImperialSettings(ref Listing_Standard listing_Main, Rect rect, Rect inRect, float num, float num2)
        {
            if (!AdeptusIntergrationUtility.enabled_MagosXenobiologis)
            {
                AstartesSettings(ref listing_Main, rect, inRect, num, MenuLengthXenobiologisRacesAstartes);
                MechanicusSettings(ref listing_Main, rect, inRect, num, num2);
                MilitarumSettings(ref listing_Main, rect, inRect, num, num2);
            }
            
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