using HarmonyLib;
using RimWorld.BaseGen;
using System.Collections.Generic;
using Verse;

namespace AdeptusMechanicus.settings
{
    public class PatchDescription : IExposable
    {
        public string file;
        public string key;
        public List<string> linkedModID;
        public string label;
        public string tooltip;
        public bool enabledByDefault = true;
        public bool enabled;
        public PatchOperation linkedOperation;
        public PatchOperation LinkedOperation
        {
            get
            {
                return linkedOperation;
            }
            set
            {
                linkedOperation = value;
                if (linkedOperation is PatchOperationOptional optional)
                {
                    key = optional.PatchName;
                    label = optional.Label;
                    tooltip = optional.ToolTip;
                    enabledByDefault = optional.EnabledByDefault;
                    if (!optional.LinkedModIDs.NullOrEmpty()) this.linkedModID.AddRange(optional.LinkedModIDs);

                }
            }
        }
        public PatchDescription() { }
        public PatchDescription(string file, string key = null, string label = null, List<string> linkedModIDs = null, string tooltip = null, bool enabledByDefault = true)
        {
            this.file = file.Substring(file.LastIndexOf("\\")+1);
            this.linkedModID = new List<string>();
            this.key = key;
            if (!linkedModIDs.NullOrEmpty()) this.linkedModID.AddRange(linkedModIDs);
            this.label = label ?? this.file;
            this.tooltip = tooltip;
            this.enabledByDefault = enabledByDefault;
            if (enabledByDefault)
            {
                enabled = true;
            }
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref this.file, "file");
            Scribe_Values.Look(ref this.key, "key");
            Scribe_Values.Look(ref this.label, "label");
            Scribe_Values.Look(ref this.tooltip, "tooltip");
            Scribe_Values.Look(ref this.enabledByDefault, "enabledByDefault");
            Scribe_Values.Look(ref this.enabled, "enabled", this.enabledByDefault);
            Scribe_Collections.Look(ref this.linkedModID, "linkedModIDs");
        }
    }
}