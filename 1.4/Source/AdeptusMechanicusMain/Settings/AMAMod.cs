using AdeptusMechanicus.ExtensionMethods;
using AdeptusMechanicus.HarmonyInstance;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus.settings
{
    public class AMAMod : Mod
    {
        public static bool updateFactions_PawnKinds = false;
        public static bool updateFactions_Required = false;
        public static bool updateIncidents_Disabled = false;
        public static bool updateWeapons_Allowed = false;
        public static IEnumerable<Type> Integrations = typeof(Integration).AllSubclasses();
        public static IEnumerable<Integration> integrations;
        public static IEnumerable<Type> AdeptusMenus = typeof(Integration_Adeptus).AllSubclasses();
        public static IEnumerable<Integration_Adeptus> integration_AdeptusMenus;
        public void LoadIntergrations() 
        {
            List<Integration_Adeptus> list_Adeptus = new List<Integration_Adeptus>();
            List<Integration_Exposable> list_Intergrations = new List<Integration_Exposable>();
            foreach (var type in Integrations)
            {
                if (type.IsSubclassOf(typeof(Integration_Adeptus)))
                {
                    if (!list_Adeptus.Any(x => x.GetType() == type))
                    {
                        var menu = (Integration_Adeptus)Activator.CreateInstance(type, null);
                        if (menu.IsActive && (!menu.XenobiologisSub || !AdeptusIntergrationUtility.enabled_MagosXenobiologis))
                        {
                            list_Adeptus.Add(menu);
                            if (Dev) Log.Message($"loading {menu.Label}");
                        }
                    }
                }
                if (type.IsSubclassOf(typeof(Integration_Exposable)))
                {
                    if (!list_Intergrations.Any(x => x.GetType() == type))
                    {
                        var menu = (Integration_Exposable)Activator.CreateInstance(type, null);
                        if (menu.IsActive)
                        {
                            list_Intergrations.Add(menu);
                            StringBuilder sb = new StringBuilder($"loading {menu.Label}");
                            if (Dev) Log.Message(sb.ToString());
                        }
                    }
                }
            }
            integrations = list_Intergrations;
            integration_AdeptusMenus = list_Adeptus.OrderByDescending(x => x.Priority);
        }
        public void FilterPatches(ModContentPack content)
        {
            var allPatches = content.Patches as List<PatchOperation>;
            List<PatchOperation> optionalPatches = new List<PatchOperation>(); 
            if (settings.DisabledPatchSetting == null)
            {
                settings.DisabledPatchSetting = new List<PatchDescription>();
            }
            foreach (var item in content.Patches as List<PatchOperation>)
            {
                if (item is PatchOperationOptional optional)
                {
                    PatchDescription patch = settings.DisabledPatchSetting.FirstOrDefault(x => x.key == optional.PatchName);
                    if (patch == null)
                    {
                        patch = settings.DisabledPatchSetting.FirstOrDefault(x => item.sourceFile.EndsWith(x.file));

                    }
                    if (patch == null)
                    {
                        patch = new PatchDescription(item.sourceFile, optional.PatchName, optional.Label, optional.LinkedModIDs, optional.ToolTip, optional.EnabledByDefault);
                        settings.DisabledPatchSetting.Add(patch);
                    }
                    else
                    {
                        patch.key = optional.PatchName;
                    }
                    patch.LinkedOperation = item;
                }
            }

            /*
            showArmouryIntergrationMenu = !settings.DisabledPatchSetting.NullOrEmpty();
            if (!settings.DisabledPatchSetting.NullOrEmpty())
            {
                IntergrationOptions = (int)Mathf.Round((settings.DisabledPatchSetting.Count / 2) + 0.25f);
                foreach (PatchOperation patch in optionalPatches)
                {
                    if (patch is IOptionalPatch optional)
                    {
                        PatchDescription description = settings.DisabledPatchSetting.FirstOrDefault(x => x.key == optional.PatchName);

                        if (description != null && !description.enabled)
                        {
                            //    if (Dev) Log.Message("RemoveAll XML Patch: " + optional.Label);
                            //    allPatches.RemoveAll(p => p.sourceFile.EndsWith(patch.file));
                            if (Dev) Log.Message("Removed XML Patch: " + optional.Label);
                            allPatches.Remove(patch);
                        }
                        else
                        {
                            if (Dev) Log.Message("Running XML Patch: " + optional.Label);
                        }
                    }
                }
            }
            */
        }
        #region overrides
        public AMAMod(ModContentPack content) : base(content)
        {
            Instance = this;
            settings = GetSettings<AMSettings>();
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            AdeptusHarmonyPatches.PatchPawnsArrivalModeWorker(harmony);
            if (AdeptusIntergrationUtility.enabled_ResearchPal)
            {
                //    harmony.Patch(AccessTools.Method(GenTypes.GetTypeInAnyAssembly("ResearchPal.Tree", "ResearchPal"), "DrawEquipmentAimingPostFix", null, null), new HarmonyMethod(typeof(AM_ResearchProjectDef_get_PrerequisitesCompleted_CommonTech_ResearchPal_Patch), "Postfix", null));
            }
            StringBuilder builder = new StringBuilder($"Adeptus Mechanicus:: Loading {content.Patches.Where(x => x is PatchOperationOptional modID).Count()} Optional XML Patches out of {content.Patches.Count()}");

            if (AMAMod.Dev) Log.Message(builder.ToString());
            LoadIntergrations();
            FilterPatches(content);
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
            LongEventHandler.ExecuteWhenFinished(settings.ApplySettingsStartUp);
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
            settings.ApplySettings();
        }
        #endregion

        #region properties
        public static bool Dev => Prefs.DevMode && SteamUtility.SteamPersonaName.Contains("Ogliss");
        public string ModLoaded() => "Mods Loaded: " + "AdeptusMechanicus.ModName".Translate();


        #endregion


        public virtual void PreModOptions(Listing_StandardExpanding listing_Main, Rect inRect, float width, ref float menuLength, string label)
        {
            Widgets.Label(inRect.RightPart(0.95f).ContractedBy(4), label);
            if (Widgets.ButtonText(inRect.RightPart(0.05f).ContractedBy(4), label))
            {
                ResetMenu();
            }
        }

        public void ModOptions(ref Rect rect, Rect inRect, float width, ref float ml)
        {
            rect.height = 100000f;
            listing_Main.Begin(rect);
            string labelA = "AdeptusMechanicus.ModName".Translate() + " Settings";
            string tooltipA = "AdeptusMechanicus.ShowSpecialRulesDesc".Translate();
            if (listing_Main.ButtonText(labelA, ref settings.ShowArmourySettings, Dev, ref armouryMenuInc, tooltipA, true))
            {

                ArmouryMenus.ArmouryModOptionsMenu(listing_Main);
                /*
                if (AdeptusIntergrationUtility.enabled_VE)
                {
                    listing_Main.CheckboxLabeled("Disable Vanilla Expanded Memes",
                        ref settings.DisableVEMPatch,
                        "Temp option to bypass an issue with Vanilla Expanded Memes erroring on worldgen with certain scenarios");
                }
                */
            }
            
            DisableConflicting();
            foreach (var menu in integration_AdeptusMenus)
            {
                menu.DrawSettings(listing_Main);
            }
            string labelI = "AdeptusMechanicus.IntergrationOptions".Translate();
            string tooltipI = "AdeptusMechanicus.IntergrationOptionsDesc".Translate();
            if (listing_Main.ButtonText(labelI, ref showArmouryIntergrationOptions, Dev, ref intergrationMenuInc, tooltipI))
            {
                IntergrationMenus.DrawMenu(listing_Main);
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
            settings.ShowAeldari = false;
            settings.ShowAeldari = false;
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

        public static Harmony harmony = new Harmony("com.ogliss.rimworld.mod.AdeptusMechanicus");
        public static AMAMod Instance;
        public static AMSettings settings;
        public static float lineheight = (Text.LineHeight + 2f);
        private int patchesCount = -1;
        private float menu = 0;
        public float armouryMenuInc = 0f;
        public float XenobiologisMenuInc = 0f;
        private float intergrationMenuInc = 0f;
        public static List<PatchDescription> patches = new List<PatchDescription>();
        private float inc = 0;
        bool showArmouryIntergrationMenu;
        bool showArmouryIntergrationOptions = false;
        public static int IntergrationOptions;
        public static Listing_StandardExpanding listing_Main;
        private Vector2 pos = new Vector2(0f, 0f);
    }
    
}