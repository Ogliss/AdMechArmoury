using System;
using Verse;
using Verse.Grammar;

namespace AdeptusMechanicus
{
    public class Rule_Current_World : Rule
	{
		public override Rule DeepCopy()
		{
			Rule_Current_World rule = (Rule_Current_World)base.DeepCopy();
			return rule;
		}

        public override float BaseSelectionWeight => 1f;

        public override string Generate()
		{
			return Find.World.info.name;
		}

		public override string ToString()
		{
			return this.keyword + "->(WorldName: " + Find.World.info.name + ")";
		}

	}
}
