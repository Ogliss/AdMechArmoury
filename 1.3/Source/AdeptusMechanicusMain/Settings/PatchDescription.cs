namespace AdeptusMechanicus.settings
{
    public struct PatchDescription
    {
        public string file;
        public string label;
        public string tooltip;
        public bool optional;
        public bool enabledByDefault;

        public PatchDescription(string file, string label, string tooltip = null, bool optional = true, bool enabledByDefault = true)
        {
            this.file = file;
            this.label = label;
            this.tooltip = tooltip;
            this.optional = optional;
            this.enabledByDefault = enabledByDefault;
        }
    }
}