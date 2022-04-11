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
    public class AMAMod : Mod
    {

        #region overrides
        public AMAMod(ModContentPack content) : base(content)
        {
            Instance = this;
            settings = GetSettings<AMSettings>();
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            HarmonyPatches.PatchPawnsArrivalModeWorker(harmony);
            if (AdeptusIntergrationUtility.enabled_ResearchPal)
            {
            //    harmony.Patch(AccessTools.Method(GenTypes.GetTypeInAnyAssembly("ResearchPal.Tree", "ResearchPal"), "DrawEquipmentAimingPostFix", null, null), new HarmonyMethod(typeof(AM_ResearchProjectDef_get_PrerequisitesCompleted_CommonTech_ResearchPal_Patch), "Postfix", null));
            }
            showArmouryIntergrationMenu = !Patches.NullOrEmpty() || (AdeptusIntergrationUtility.enabled_AlienRaces && Dev);
            if (!Patches.NullOrEmpty())
            {
                IntergrationOptions = (int)Mathf.Round((Patches.Count / 2) + 0.25f);
                var allPatches = content.Patches as List<PatchOperation>;
                foreach (var patch in Patches)
                {
                    if (patch.optional)
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
            }

            if (AdeptusIntergrationUtility.enabled_rooloDualWield)
            {
                string tname = "DualWield.Ext_Pawn";
                string nspace = "DualWield";
                string mname = "TryStartOffHandAttack";
                MethodInfo target = AccessTools.Method(GenTypes.GetTypeInAnyAssembly(tname, nspace), mname, null, null);
                if (target == null)
                {
                    Log.Warning("Target: " + tname + "." + mname + " Not found");
                }
                else
                {
                    Type t = typeof(Ext_Pawn_TryStartOffHandAttack_DualWield_Patch);
                    string pmname = "Prefix";
                    MethodInfo patch = t.GetMethod(pmname);
                    if (patch == null)
                    {
                        Log.Warning("Patch is null " + t.Name.ToString() + "." + pmname);
                    }
                    else
                    {
                        // JobDriver_AttackStatic
                        if (harmony.Patch(target, new HarmonyMethod(patch)) == null)
                        {
                            Log.Warning("AdeptusMechanicus.ModName".Translate() + ": " + tname + " Patch Failed to apply");
                        }
                    }
                }

            }

            listing_Main = new Listing_StandardExpanding();
            if (Prefs.DevMode) Log.Message(string.Format("Adeptus Mecanicus: Armoury: successfully completed {0} harmony patches.", harmony.GetPatchedMethods().Select(new Func<MethodBase, Patches>(Harmony.GetPatchInfo)).SelectMany((Patches p) => p.Prefixes.Concat(p.Postfixes).Concat(p.Transpilers)).Count((Patch p) => p.owner.Contains(harmony.Id))));
        }
        public override string SettingsCategory() => "AdeptusMechanicus.ModSeries".Translate();
        public override void DoSettingsWindowContents(Rect inRect)
        {
            Rect inRect1 = inRect.TopPart(0.05f);
            Rect inRect2 = inRect.BottomPart(0.95f);
            Rect Frame = inRect2.ContractedBy(4);
            float width = inRect2.ContractedBy(4).width;
            PreModOptions(listing_Main, inRect1, width, ref menu, ModLoaded());
            Rect Menu = new Rect(Frame.x, Frame.y, width, menu);
            Widgets.DrawMenuSection(Frame);
            Widgets.BeginScrollView(Frame, ref this.pos, Menu, false);
            ModOptions(ref Menu, inRect2, Menu.ContractedBy(4).width, ref menu);
            Widgets.EndScrollView();
            //    PostModOptions(listing_Main, inRect2, width, menu);
        }
        public override void WriteSettings()
        {
            base.WriteSettings();
        }
        #endregion

        #region properties
        public static bool Dev => Prefs.DevMode && SteamUtility.SteamPersonaName.Contains("Ogliss");
        public string ModLoaded() => "Mods Loaded: " + "AdeptusMechanicus.ModName".Translate();

        public int PatchesCount
        {
            get
            {
                if (patchesCount == -1)
                {
                    patchesCount = optionalPatchesCount % 2 == 0 ? optionalPatchesCount / 2 : optionalPatchesCount / 2 + 1;
                }
                return patchesCount;
            }
        }
        public static List<PatchDescription> Patches
        {
            get
            {
                if (patches.NullOrEmpty())
                {
                    patches = new List<PatchDescription>();
                    if (AdeptusIntergrationUtility.enabled_AstraCore)
                    {
                        patches.Add(new PatchDescription("AstraMiliatrumMod_ArmourPatch.xml", "Astra Miliatrum Armour Patch", "Removes the Astra Militarum versions of dupped Armour when active", true));
                        patches.Add(new PatchDescription("AstraMiliatrumMod_WeaponsPatch.xml", "Astra Miliatrum Weapons Patch", "Removes the Astra Militarum versions of dupped Weapons when active", true));
                        if (AdeptusIntergrationUtility.enabled_CombatExtended)
                        {
                            //    patches.Add(new PatchDescription("AstraMiliatrumMod_ArmourPatch.xml", "Astra Miliatrum Armour Patch", "Removes the Astra Militarum versions of dupped Armour when active"));
                            patches.Add(new PatchDescription("Weapons_Imperial_Ranged_Bolt_Astra.xml", "Astra Miliatrum Bolt Weapons CE Patch", "Patches Astra Militarum Bolt Weapons for CE compatability when active", false));
                            patches.Add(new PatchDescription("Weapons_Imperial_Ranged_Plasma_Astra.xml", "Astra Miliatrum Plasma Weapons CE Patch", "Patches Astra Militarum Plasma Weapons for CE compatability when active", false));
                            patches.Add(new PatchDescription("Weapons_Imperial_Ranged_Misc_Astra.xml", "Astra Miliatrum Misc Weapons CE Patch", "Patches Astra Militarum Misc Weapons for CE compatability when active", false));
                        }
                    }
                }
                return patches;
            }
        }

        private float Listing_ArmouryIntergrationLength => Length(this.showArmouryIntergrationOptions, 1, lineheight, 8, 0) + Listing_ArmouryIntergrationContents + intergrationMenuInc;
        private float Listing_ArmouryIntergrationContents => Length(this.showArmouryIntergrationOptions, PatchesCount, lineheight, 0, 0);

        #endregion

        public void IntergrationMenu()
        {
            if (AdeptusIntergrationUtility.enabled_AlienRaces && Dev)
            {
                Listing_StandardExpanding listing_AlienRacesIntergration = listing_Main.BeginSection(110);
                Listing_StandardExpanding listing_ImperialRacesIntergration = listing_AlienRacesIntergration.BeginSection(100);

                listing_AlienRacesIntergration.EndSection(listing_ImperialRacesIntergration);
                Listing_StandardExpanding listing_AeldariRacesIntergration = listing_AlienRacesIntergration.BeginSection(100);

                listing_AlienRacesIntergration.EndSection(listing_AeldariRacesIntergration);
                listing_Main.EndSection(listing_AlienRacesIntergration);
            }
            Listing_StandardExpanding listing_ArmouryIntergration = listing_Main.BeginSection(Listing_ArmouryIntergrationLength, false, 3);
            //   listing_ArmouryIntergration = listing_Main.BeginSection(MenuLengthIntergration, false, 0);
            listing_ArmouryIntergration.Label("Changes to these settings require a restart to take effect." + (Dev ? " patchesCount: " + PatchesCount : ""));
            Listing_StandardExpanding listing_General = listing_ArmouryIntergration.BeginSection(Listing_ArmouryIntergrationContents, true);
            listing_General.ColumnWidth *= 0.488f;
            bool flag = false;
            for (int i = 0; i < Patches.Count; i++)
            {
                var patch = Patches[i];
                if (!patch.optional)
                {
                    continue;
                }
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
            listing_Main.EndSection(listing_ArmouryIntergration);
        }

        public virtual void PreModOptions(Listing_StandardExpanding listing_Main, Rect inRect, float width, ref float menuLength, string label)
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
                tooltip += "\nMenuLengthIntergration: " + Listing_ArmouryIntergrationContents + " Total: " + Listing_ArmouryIntergrationLength + " Show Menu: " + showArmouryIntergrationMenu + " Show Options: " + showArmouryIntergrationOptions;
                tooltip += "\nMainSectionLength: " + listing_Main.CurHeight;
                tooltip += "\nMaxColumnHeightSeen: " + listing_Main.MaxColumnHeightSeen;
                TooltipHandler.TipRegion(inRect.ContractedBy(4), tooltip);
            }
            else
            {
                Widgets.Label(inRect.ContractedBy(4), label);
            }
        }

        public void ModOptions(ref Rect rect, Rect inRect, float width, ref float ml)
        {
            rect.height = 100000f;
            listing_Main.Begin(rect);
            string labelA = "AdeptusMechanicus.ModName".Translate() + " Settings";
            string tooltipA = "AdeptusMechanicus.ShowSpecialRulesDesc".Translate();
            if (listing_Main.ButtonText(labelA, ref settings.ShowArmourySettings, Dev, ref armouryMenuInc, tooltipA))
            {
                ArmouryMenus.ArmouryModOptionsMenu(listing_Main);
                if (AdeptusIntergrationUtility.enabled_VE)
                {
                    listing_Main.CheckboxLabeled("Disable Vanilla Expanded Memes",
                        ref settings.DisableVEMPatch,
                        "Temp option to bypass an issue with Vanilla Expanded Memes erroring on worldgen with certain scenarios");
                }
            }
            
            DisableConflicting();
            if (AdeptusIntergrationUtility.enabled_MagosXenobiologis)
            {
                string labelXB = "AdeptusMechanicus.Xenobiologis.ModName".Translate() + " Settings";
                string tooltipXB = "AdeptusMechanicus.Xenobiologis.ShowOptionsDesc".Translate();
                if (listing_Main.ButtonText(labelXB, ref settings.ShowXenobiologisSettings, Dev, ref XenobiologisMenuInc, tooltipXB))
                {
                    XenobiologisSettings(ref listing_Main, rect, inRect, width, ml);
                }
            }
            else
            {
                ImperialSettings(ref listing_Main, rect, inRect, width, ref raceMenuImperial);
                ChaosSettings(ref listing_Main, rect, inRect, width, ref raceMenuChaos);
                EldarSettings(ref listing_Main, rect, inRect, width, ref raceMenuEldar);
                DarkEldarSettings(ref listing_Main, rect, inRect, width, ref raceMenuDarkEldar);
                OrkSettings(ref listing_Main, rect, inRect, width, ref raceMenuOrk);
                TauSettings(ref listing_Main, rect, inRect, width, ref raceMenuTau);
                NecronSettings(ref listing_Main, rect, inRect, width, ref raceMenuNecron);
                TyranidSettings(ref listing_Main, rect, inRect, width, ref raceMenuTyranid);
            }
            if (showArmouryIntergrationMenu)
            {
                string labelI = "AdeptusMechanicus.IntergrationOptions".Translate();
                string tooltipI = "AdeptusMechanicus.IntergrationOptionsDesc".Translate();
                if (Dev)
                {
                    labelI = "AdeptusMechanicus.IntergrationOptions".Translate() + " Menu Length: " + Listing_ArmouryIntergrationContents + " Total Length: " + Listing_ArmouryIntergrationLength + " " + showArmouryIntergrationOptions + " CurInc: " + intergrationMenuInc;
                }
                if (listing_Main.ButtonText(labelI, ref showArmouryIntergrationOptions, Dev, ref intergrationMenuInc, tooltipI))
                {
                    IntergrationMenu();
                }
            }
            listing_Main.End();
            rect.height = listing_Main.MaxColumnHeightSeen;
            menu = listing_Main.CurHeight;
        }


        public void DisableConflicting()
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
            if (!settings.AllowTyranidWeapons || !settings.AllowTyranid)
            {
                settings.AllowTyranid = false;
                settings.AllowTyranidWeapons = false;
                settings.AllowTyranidInfestation = false;
            }
        }
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
        public float Length(bool setting, float lines, float lineheight, float offset = 8, float linesfallback = 1, float offsetfallback = 0)
        {
            return ((lineheight * (setting ? lines : linesfallback)) + (setting ? offset : offsetfallback));
        }

        public virtual void XenobiologisSettings(ref Listing_StandardExpanding listing_Main, Rect rect, Rect inRect, float num, float xenobiologisMenuLenght)
        {

        }
        public virtual void ImperialSettings(ref Listing_StandardExpanding listing_Main, Rect rect, Rect inRect, float num, ref float num2)
        {
            if (!AdeptusIntergrationUtility.enabled_MagosXenobiologis)
            {
                AstartesSettings(ref listing_Main, rect, inRect, num, ref num2);
                MechanicusSettings(ref listing_Main, rect, inRect, num, ref num2);
                MilitarumSettings(ref listing_Main, rect, inRect, num, ref num2);
            }
            
        }
        public virtual void AstartesSettings(ref Listing_StandardExpanding listing_Main, Rect rect, Rect inRect, float num, ref float num2)
        {

        }
        public virtual void MechanicusSettings(ref Listing_StandardExpanding listing_Main, Rect rect, Rect inRect, float num, ref float num2)
        {

        }
        public virtual void MilitarumSettings(ref Listing_StandardExpanding listing_Main, Rect rect, Rect inRect, float num, ref float num2)
        {

        }
        public virtual void ChaosSettings(ref Listing_StandardExpanding listing_Main, Rect rect, Rect inRect, float num, ref float num2)
        {

        }
        public virtual void EldarSettings(ref Listing_StandardExpanding listing_Main, Rect rect, Rect inRect, float num, ref float num2)
        {

        }
        public virtual void DarkEldarSettings(ref Listing_StandardExpanding listing_Main, Rect rect, Rect inRect, float num, ref float num2)
        {

        }
        public virtual void OrkSettings(ref Listing_StandardExpanding listing_Main, Rect rect, Rect inRect, float num, ref float num2)
        {

        }
        public virtual void TauSettings(ref Listing_StandardExpanding listing_Main, Rect rect, Rect inRect, float num, ref float num2)
        {

        }
        public virtual void NecronSettings(ref Listing_StandardExpanding listing_Main, Rect rect, Rect inRect, float num, ref float num2)
        {

        }
        public virtual void TyranidSettings(ref Listing_StandardExpanding listing_Main, Rect rect, Rect inRect, float num, ref float num2)
        {

        }

        public static Harmony harmony = new Harmony("com.ogliss.rimworld.mod.AdeptusMechanicus");
        public static AMAMod Instance;
        public static AMSettings settings;
        public static float lineheight = (Text.LineHeight + 2f);
        private int optionalPatchesCount = Patches.FindAll(x => x.optional).Count;
        private int patchesCount = -1;
        private float menu = 0;
        private float armouryMenuInc = 0f;
        public float XenobiologisMenuInc = 0f;
        private float intergrationMenuInc = 0f;
        private static float raceMenuImperial = 0;
        private static float raceMenuChaos = 0;
        private static float raceMenuEldar = 0;
        private static float raceMenuDarkEldar = 0;
        private static float raceMenuOrk = 0;
        private static float raceMenuTau = 0;
        private static float raceMenuNecron = 0;
        private static float raceMenuTyranid = 0;
        private static List<PatchDescription> patches;
        private float inc = 0;
        bool showArmouryIntergrationMenu;
        bool showArmouryIntergrationOptions = false;
        public static int IntergrationOptions;
        public static Listing_StandardExpanding listing_Main;
        private Vector2 pos = new Vector2(0f, 0f);
    }
    
}