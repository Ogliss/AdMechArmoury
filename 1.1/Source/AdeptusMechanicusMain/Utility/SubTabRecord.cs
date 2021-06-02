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

		public SubTabRecord(string label, ResearchProjectTagDef subTagDef, Action clickedAction, bool selected) : base(label, clickedAction, selected)
		{
			this.label = label;
			this.clickedAction = clickedAction;
			this.selected = selected;
			this.subTagDef = subTagDef;
		}
		public SubTabRecord(string label, ResearchProjectTagDef subTagDef, Action clickedAction, Func<bool> selected) : base(label, clickedAction, selected)
		{
			this.label = label;
			this.clickedAction = clickedAction;
			this.selectedGetter = selected;
			this.subTagDef = subTagDef;
		}
		public ResearchProjectTagDef subTagDef;
	}
}
