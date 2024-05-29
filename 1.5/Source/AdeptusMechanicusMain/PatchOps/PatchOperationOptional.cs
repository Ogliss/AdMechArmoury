using AdeptusMechanicus;
using AdeptusMechanicus.settings;
using System.Collections.Generic;
using System.Xml;

namespace Verse
{
    public class PatchOperationOptional : PatchOperation
    {
        private List<string> mods;
        private PatchOperation enabled;
        private PatchOperation disabled;
        private string patchName;
        private string label;
        private string toolTip;
        private bool enabledByDefault = true;
        private bool log = false;
        private PatchOperation lastFailedOperation;
        PatchDescription setting;
        PatchDescription Setting
        {
            get 
            {
                if (setting == null)
                {
                    if (log) Log.Message($"No cached setting found for {PatchName}, searching");
                    PatchDescription descript = AMSettings.Instance.DisabledPatchSetting.FirstOrDefault(x => x.key == PatchName);
                    if (descript == null)
                    {
                        if (log) Log.Message($"No setting found for {PatchName}, making new");
                        descript = new PatchDescription(sourceFile, PatchName, Label, LinkedModIDs, ToolTip, EnabledByDefault);
                        if (log) Log.Message($"new setting for {PatchName}, adding to list");
                        AMSettings.Instance.DisabledPatchSetting.Add(descript);
                        if (log) Log.Message($"added new setting for {PatchName} to list");
                    }
                    else
                    {
                        if (log) Log.Message($"found setting found for {PatchName}");
                        descript.LinkedOperation = this;
                        descript.label = Label;
                        descript.tooltip = ToolTip;
                        descript.enabledByDefault = EnabledByDefault;
                        if (!LinkedModIDs.NullOrEmpty()) descript.linkedModID.AddRange(this.LinkedModIDs);
                    }
                    setting = descript;
                }
                return setting; 
            }
        }
        public bool Enabled => Setting == null || Setting.enabled;

		public bool EnabledByDefault => enabledByDefault;
        public string PatchName => patchName ?? label;
        public string Label => label;

        public string ToolTip => toolTip;

		public List<string> LinkedModIDs => mods;

        public override bool ApplyWorker(XmlDocument xml)
        {
            if (Enabled)
            {
                if (this.enabled != null)
                {
                    if (!this.enabled.Apply(xml))
                    {
                        this.lastFailedOperation = enabled;
                        return false;
                    }
                    return true;
                }
            }
            else if (this.disabled != null)
            {
                if (!this.disabled.Apply(xml))
                {
                    this.lastFailedOperation = disabled;
                    return false;
                }
                return true;
            }
            return this.enabled != null || this.disabled != null;
        }

        public override IEnumerable<string> ConfigErrors()
        {
            foreach (var item in base.ConfigErrors())
            {
                yield return item;
            }
            if (patchName.NullOrEmpty())
            {
                yield return $"patchName is null on {Label} in {sourceFile}";
            }
            yield break;
        }

        public override string ToString()
        {
            string text = string.Format("{0}", base.ToString());
            if (this.lastFailedOperation != null)
            {
                text = text + ", FailedOperation=" + this.lastFailedOperation.ToString();
            }
            return text + ")";
        }
    }
}
