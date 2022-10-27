using System;
using Verse;
using Verse.Grammar;

namespace AdeptusMechanicus
{
    public class Rule_Ordinal_Number : Rule
	{
		public override Rule DeepCopy()
		{
			Rule_Ordinal_Number rule = (Rule_Ordinal_Number)base.DeepCopy();
			rule.range = this.range;
			rule.selectionWeight = this.selectionWeight;
			return rule;
		}

		public override float BaseSelectionWeight
		{
			get
			{
				return (float)this.selectionWeight;
			}
		}

		public override string Generate()
		{
			return AddOrdinal(this.range.RandomInRange);
		}
		public static string AddOrdinal(int num)
		{
			if (num <= 0) return num.ToString();

			switch (num % 100)
			{
				case 11:
				case 12:
				case 13:
					return num + "th";
			}

			switch (num % 10)
			{
				case 1:
					return num + "st";
				case 2:
					return num + "nd";
				case 3:
					return num + "rd";
				default:
					return num + "th";
			}
		}
		public override string ToString()
		{
			return this.keyword + "->(number: " + this.range.ToString() + ")";
		}

		private IntRange range = IntRange.zero;

		public int selectionWeight = 1;
	}
}
