using AdeptusMechanicus;
using AdeptusMechanicus.settings;
using System.Collections.Generic;

namespace Verse
{
    public class PatchOperationSequenceOptional : PatchOperationSequence, IOptionalPatch
    {
        private string label;
        private string toolTip;
        private bool optional = true;
        private bool enabledByDefault = true;
        private List<string> mods;

        public bool Optional => optional;

		public bool EnabledByDefault => enabledByDefault;
		public string Label => label;

		public string ToolTip => toolTip;

		public List<string> LinkedModIDs => mods;
	}
}
