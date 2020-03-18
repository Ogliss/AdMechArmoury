using System;
using System.Collections.Generic;
using System.Xml;

namespace Verse
{
	// Token: 0x02000369 RID: 873
	public class PatchOperationFindModByID : PatchOperation
	{
		// Token: 0x060015DF RID: 5599 RVA: 0x000CA0F4 File Offset: 0x000C82F4
		protected override bool ApplyWorker(XmlDocument xml)
		{
			bool flag = false;
			for (int i = 0; i < this.mods.Count; i++)
			{
				if (ModLister.GetModWithIdentifier(this.mods[i],true)!=null)
				{
					flag = true;
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
			else if (this.nomatch != null)
			{
				return this.nomatch.Apply(xml);
			}
			return true;
		}

		// Token: 0x060015E0 RID: 5600 RVA: 0x00015912 File Offset: 0x00013B12
		public override string ToString()
		{
			return string.Format("{0}({1})", base.ToString(), this.mods.ToCommaList(false));
		}

		// Token: 0x040010AD RID: 4269
		private List<string> mods;

		// Token: 0x040010AE RID: 4270
		private PatchOperation match;

		// Token: 0x040010AF RID: 4271
		private PatchOperation nomatch;
	}
}
