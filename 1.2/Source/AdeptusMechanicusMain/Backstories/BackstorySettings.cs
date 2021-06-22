using System.Collections.Generic;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x0200000A RID: 10
    [StaticConstructorOnStartup]
	public class BackstorySettings : Def
	{
		// Token: 0x0600008F RID: 143 RVA: 0x00009A4C File Offset: 0x00007C4C
		static BackstorySettings()
		{
		}

		// Token: 0x04000046 RID: 70
		public List<BackstoryTagItem> backstoryTagInsertion;
	}

}
