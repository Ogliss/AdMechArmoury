using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AdeptusMechanicus
{
    internal class SubTabRecord : TabRecord
	{

		public SubTabRecord(ResearchSubTabDef subTabDef, Action clickedAction, bool selected, string label = null) : base(label, clickedAction, selected)
		{
			this.clickedAction = clickedAction;
			this.selected = selected;
			this.subTabDef = subTabDef;
			this.label = subTabDef.LabelCap;
			this.subTagDef = subTabDef.tagdef;
			this.parentTab = subTabDef.parentTab;
		}
		public SubTabRecord(ResearchSubTabDef subTabDef, Action clickedAction, Func<bool> selected, string label = null) : base(label, clickedAction, selected)
		{
			this.clickedAction = clickedAction;
			this.selectedGetter = selected;
			this.label = subTabDef.LabelCap;
			this.subTabDef = subTabDef;
			this.subTagDef = subTabDef.tagdef;
			this.parentTab = subTabDef.parentTab;
		}
		public ResearchSubTabDef subTabDef;
		public ResearchProjectTagDef subTagDef;
		public ResearchTabDef parentTab;

		public bool SubTabOf(ResearchTabDef tabDef)
        {
			return tabDef == this.subTabDef.parentTab;
        }
	}
}
