using System;
using System.Collections.Generic;
using System.Xml;

namespace Verse
{
	// Token: 0x02000215 RID: 533
	public class PatchOperationFindModID : PatchOperation
	{
		// Token: 0x06000F11 RID: 3857 RVA: 0x00055788 File Offset: 0x00053988
		protected override bool ApplyWorker(XmlDocument xml)
		{
			bool flag = false;
			for (int i = 0; i < this.mods.Count; i++)
			{
				if (ModLister.HasActiveModWithName(this.mods[i]))
				{
					flag = true;
					Log.Message("Found Named " + this.mods[i]);
					break;
				}
				if (ModLister.GetActiveModWithIdentifier(this.mods[i]) != null)
				{
					flag = true;
					Log.Message("Found ID " + this.mods[i]);
					break;
				}
			}
			if (flag)
			{
				if (this.match != null)
				{
					return this.match.Apply(xml);
				}
			}
            else
			{
				if (this.nomatch != null)
				{
					return this.nomatch.Apply(xml);
				}
			}
			return true;
		}

		// Token: 0x06000F12 RID: 3858 RVA: 0x000557F2 File Offset: 0x000539F2
		public override string ToString()
		{
			return string.Format("{0}({1})", base.ToString(), this.mods.ToCommaList(false));
		}

		// Token: 0x04000B37 RID: 2871
		private List<string> mods;

		// Token: 0x04000B38 RID: 2872
		private PatchOperation match;

		// Token: 0x04000B39 RID: 2873
		private PatchOperation nomatch;
	}
}
