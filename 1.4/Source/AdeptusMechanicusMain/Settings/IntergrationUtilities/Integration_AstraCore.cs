using AdeptusMechanicus.settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    public class Integration_AstraCore : Integration_Exposable
    {
        public override string PackageID => "QX.AstraMilitarum";
        public override string Label => "Astra Miliatrum:: Core";
        public bool CE => AdeptusIntergrationUtility.enabled_CombatExtended;
        private List<PatchDescription> patches = null;
        public override List<PatchDescription> Patches
        {
            get
            {
                if (patches == null)
                {

                    patches = new List<PatchDescription>();
                    /*
                    {
                    new PatchDescription("AstraMiliatrumMod_ArmourPatch.xml", "Astra Miliatrum Armour Patch", "Removes the Astra Militarum versions of dupped Armour when active", true),
                    new PatchDescription("AstraMiliatrumMod_WeaponsPatch.xml", "Astra Miliatrum Weapons Patch", "Removes the Astra Militarum versions of dupped Weapons when active", true)
                    };
                    if (CE)
                    {
                        //    patches.Add(new PatchDescription("AstraMiliatrumMod_ArmourPatch.xml", "Astra Miliatrum Armour Patch", "Removes the Astra Militarum versions of dupped Armour when active"));
                        list.Add(new PatchDescription("Weapons_Imperial_Ranged_Bolt_Astra.xml", "Astra Miliatrum Bolt Weapons CE Patch", "Patches Astra Militarum Bolt Weapons for CE compatability when active", false));
                        list.Add(new PatchDescription("Weapons_Imperial_Ranged_Plasma_Astra.xml", "Astra Miliatrum Plasma Weapons CE Patch", "Patches Astra Militarum Plasma Weapons for CE compatability when active", false));
                        list.Add(new PatchDescription("Weapons_Imperial_Ranged_Misc_Astra.xml", "Astra Miliatrum Misc Weapons CE Patch", "Patches Astra Militarum Misc Weapons for CE compatability when active", false));
                    }
                    patches = list;
                    */
                    if (!settings.DisabledPatchSetting.NullOrEmpty())
                    {
                        Log.Message($"Optional patches setup for {Label} checking total of {settings.DisabledPatchSetting.Count} Patches");
                        foreach (var item in settings.DisabledPatchSetting)
                        {
                            if (!item.linkedModID.NullOrEmpty() && !PackageID.NullOrEmpty())
                            {
                            //    Log.Message($"Checking {item.label} setup for {Label}");
                                if (item.linkedModID.Contains(PackageID))
                                {
                             //       Log.Message($"Optional patch {item.label} setup for {Label}");
                                    patches.Add(item);
                                }
                            }
                        }
                    }
                }
                return patches;
            }
        }

        public bool ShowSettings = false;

        public override void DrawSettings(Listing_StandardExpanding listing_Main)
        {

            Listing_StandardExpanding listing_XML = listing_Main.BeginSection(length_Menu, false, 3);
            if (listing_XML.CheckboxLabeled(Label + " Options", ref ShowSettings, texChecked: ArmouryMain.collapseTex, texUnchecked: ArmouryMain.expandTex))
            {
                DrawPatches(listing_XML);
            }
            listing_Main.EndSection(listing_XML);
            length_Menu = listing_XML.MaxColumnHeightSeen;
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref this.ShowSettings, "ShowSettings", false);
        }
    }
    public class Integration_Core : Integration_Exposable
    {
        public override string PackageID => "Ludeon.RimWorld";
        public override string Label => "Vanilla";
        public bool CE => AdeptusIntergrationUtility.enabled_CombatExtended;
        private List<PatchDescription> patches;
        public override List<PatchDescription> Patches
        {
            get
            {
                if (patches == null)
                {

                    List<PatchDescription> list = new List<PatchDescription>();
                    /*
                    {
                    new PatchDescription("AstraMiliatrumMod_ArmourPatch.xml", "Astra Miliatrum Armour Patch", "Removes the Astra Militarum versions of dupped Armour when active", true),
                    new PatchDescription("AstraMiliatrumMod_WeaponsPatch.xml", "Astra Miliatrum Weapons Patch", "Removes the Astra Militarum versions of dupped Weapons when active", true)
                    };
                    if (CE)
                    {
                        //    patches.Add(new PatchDescription("AstraMiliatrumMod_ArmourPatch.xml", "Astra Miliatrum Armour Patch", "Removes the Astra Militarum versions of dupped Armour when active"));
                        list.Add(new PatchDescription("Weapons_Imperial_Ranged_Bolt_Astra.xml", "Astra Miliatrum Bolt Weapons CE Patch", "Patches Astra Militarum Bolt Weapons for CE compatability when active", false));
                        list.Add(new PatchDescription("Weapons_Imperial_Ranged_Plasma_Astra.xml", "Astra Miliatrum Plasma Weapons CE Patch", "Patches Astra Militarum Plasma Weapons for CE compatability when active", false));
                        list.Add(new PatchDescription("Weapons_Imperial_Ranged_Misc_Astra.xml", "Astra Miliatrum Misc Weapons CE Patch", "Patches Astra Militarum Misc Weapons for CE compatability when active", false));
                    }
                    patches = list;
                    */
                    if (!settings.DisabledPatchSetting.NullOrEmpty())
                    {
                        Log.Message($"Optional patches setup for {Label} checking total of {settings.DisabledPatchSetting.Count} Patches");
                        foreach (var item in AMAMod.settings.DisabledPatchSetting)
                        {
                            Log.Message($"checking {item.file}");
                            if (item.linkedModID.NullOrEmpty() || item.linkedModID.Contains(PackageID))
                            {
                                Log.Message($"Option patch  {item.label} setup for {Label}");
                                list.Add(item);
                            }
                        }
                    }
                    patches = list;
                }
                return patches;
            }
        }

        public bool ShowSettings = false;

        public override void DrawSettings(Listing_StandardExpanding listing_Main)
        {

            Listing_StandardExpanding listing_XML = listing_Main.BeginSection(length_Menu, false, 3);
            if (listing_XML.CheckboxLabeled(Label + " Options", ref ShowSettings, texChecked: ArmouryMain.collapseTex, texUnchecked: ArmouryMain.expandTex))
            {
                DrawPatches(listing_XML);
            }
            listing_Main.EndSection(listing_XML);
            length_Menu = listing_XML.MaxColumnHeightSeen;
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref this.ShowSettings, "ShowSettings", false);
        }
    }
}
