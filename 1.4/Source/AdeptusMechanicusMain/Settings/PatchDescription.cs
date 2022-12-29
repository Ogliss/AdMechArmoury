using HarmonyLib;
using System.Collections.Generic;
using Verse;

namespace AdeptusMechanicus.settings
{
    public class PatchDescriptionDef : Def
    {
        public string file;
        public string linkedModID;
        public string tooltip;
        public bool optional = true;
        public bool enabledByDefault = true;

        public override IEnumerable<string> ConfigErrors()
        {
            IEnumerable<string> strings = base.ConfigErrors();
            if (file.NullOrEmpty())
            {
                strings.AddItem($"file null for PatchDescriptionDef {defName}");
            }
            if (label.NullOrEmpty())
            {
                strings.AddItem($"label null for PatchDescriptionDef {defName}");
            }
            return strings;
        }

    }

    public struct PatchDescription
    {
        public string file;
        public string linkedModID;
        public string label;
        public string tooltip;
        public bool optional;
        public bool enabledByDefault;

        public PatchDescription(string file, string label, string linkedModID = null, string tooltip = null, bool optional = true, bool enabledByDefault = true)
        {
            this.file = file;
            this.linkedModID = linkedModID;
            this.label = label;
            this.tooltip = tooltip;
            this.optional = optional;
            this.enabledByDefault = enabledByDefault;
        }
        public PatchDescription(PatchDescriptionDef def)
        {
            this.file = def.defName;
            if (!file.EndsWith(".xml"))
            {
                this.file += ".xml";
            }
            this.label = def.label;
            this.linkedModID = def.linkedModID;
            this.tooltip = def.tooltip;
            this.optional = def.optional;
            this.enabledByDefault = def.enabledByDefault;
        }
        public bool DrawOption
        {
            get
            {
                return this.optional;
            }
        }
    }
}