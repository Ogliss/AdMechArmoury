using System;
using Verse;
using Verse.Grammar;

namespace AdeptusMechanicus
{
    // Token: 0x020004D4 RID: 1236
    public class Rule_Ordinal_Number : Rule
	{
		// Token: 0x060024D8 RID: 9432 RVA: 0x000DCFC0 File Offset: 0x000DB1C0
		public override Rule DeepCopy()
		{
			Rule_Ordinal_Number rule_Number = (Rule_Ordinal_Number)base.DeepCopy();
			rule_Number.range = this.range;
			rule_Number.selectionWeight = this.selectionWeight;
			return rule_Number;
		}

		// Token: 0x17000746 RID: 1862
		// (get) Token: 0x060024D9 RID: 9433 RVA: 0x000DCFE5 File Offset: 0x000DB1E5
		public override float BaseSelectionWeight
		{
			get
			{
				return (float)this.selectionWeight;
			}
		}

		// Token: 0x060024DA RID: 9434 RVA: 0x000DCFF0 File Offset: 0x000DB1F0
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
		// Token: 0x060024DB RID: 9435 RVA: 0x000DD010 File Offset: 0x000DB210
		public override string ToString()
		{
			return this.keyword + "->(number: " + this.range.ToString() + ")";
		}

		// Token: 0x0400163D RID: 5693
		private IntRange range = IntRange.zero;

		// Token: 0x0400163E RID: 5694
		public int selectionWeight = 1;
	}
}
