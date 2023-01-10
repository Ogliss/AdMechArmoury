using System.Collections.Generic;

namespace AdeptusMechanicus
{
    public interface IOptionalPatch
    {
        public bool Optional
        {
            get;
        }
        public bool EnabledByDefault
        {
            get;
        }
        public string Label
        {
            get;
        }
        public string ToolTip
        {
            get;
        }
        
        public List<string> LinkedModIDs
        {
            get;
        }

    }

}
