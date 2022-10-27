using AdeptusMechanicus.settings;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    public abstract class Integration// : IExposable
    {
        public virtual string PackageID => "";
        public bool IsActive => ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == PackageID);

        public virtual string Label => "";
        protected bool Dev => AMAMod.Dev;
        public virtual int Priority => 0;
        protected static AMSettings settings = AMAMod.settings;
        public virtual List<PatchDescription> Patches => null;
        public int PatchesCount => Patches?.Count ?? 0;
        private bool showpatches = false;
        public virtual void DrawPatches(Listing_StandardExpanding listing_ArmouryIntergration) 
        {
            Listing_StandardExpanding listing_XML = listing_ArmouryIntergration.BeginSection(length_PatchesMenu, false, 3);
            if (listing_XML.CheckboxLabeled("Xml Patches", ref showpatches, "Changes to these settings require a restart to take effect.", texChecked: ArmouryMain.collapseTex, texUnchecked: ArmouryMain.expandTex))
            {
                listing_XML.Label("Changes to these settings require a restart to take effect." + (Dev ? $" patchesCount: {PatchesCount}" : ""));
                Listing_StandardExpanding listing_General = listing_XML.BeginSection(length_PatchesMenuContent, true);
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
                    if (!flag && i + 1 > Patches.Count / 2)
                    {
                        listing_General.NewColumn();
                        flag = true;
                    }
                    listing_General.CheckboxLabeled(patch.label, ref status, patch.tooltip);
                    if (settings.PatchDisabled[patch] != status)
                    {
                        IntergrationMenus.restart = true;
                    }
                    settings.PatchDisabled[patch] = status;

                }
                listing_XML.EndSection(listing_General);
                length_PatchesMenuContent = listing_General.MaxColumnHeightSeen;
            }
            listing_ArmouryIntergration.EndSection(listing_XML);
            length_PatchesMenu = listing_XML.MaxColumnHeightSeen;
        }
        public virtual void DrawPatches(ref Rect rect) { }
        public virtual void DrawSettings(Listing_StandardExpanding listing_Main) { }
        public virtual void DrawSettings(ref Rect rect) { }
        public virtual void ScribeSettings()
        {
        }

        public void Reset()
        {
            var type = GetType();
            var defaults = Activator.CreateInstance(type);
            AccessTools.GetFieldNames(this).Do(name =>
            {
                var finfo = AccessTools.Field(type, name);
                finfo.SetValue(this, finfo.GetValue(defaults));
            });
            //    Dialogs.scrollPosition = Vector2.zero;
        }
        protected float length_Menu = 0;
        protected float length_MenuInc = 0;
        protected float length_MenuContent = 0;
        private float length_PatchesMenu = 0;
        private float length_PatchesMenuInc = 0;
        private float length_PatchesMenuContent = 0;
    }

}
