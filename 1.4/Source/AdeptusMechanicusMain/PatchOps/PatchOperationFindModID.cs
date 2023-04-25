using AdeptusMechanicus;
using AdeptusMechanicus.settings;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Verse
{
	// Token: 0x02000215 RID: 533
	public class PatchOperationFindModID : PatchOperation
    {
		public bool log = false;
		public override bool ApplyWorker(XmlDocument xml)
		{
			bool flag = false;
			for (int i = 0; i < this.mods.Count; i++)
			{
				if (ModLister.HasActiveModWithName(this.mods[i]))
				{
					flag = true;
					if (log && AMAMod.Dev)
					{
						string text = string.Format("Name:: [{0}] Found by {1} ", this.mods[i], this);
						if (!string.IsNullOrEmpty(this.sourceFile))
						{
							text = text + "\nfile: " + this.sourceFile;
						}
						Log.Message(text);
					}
					break;
				}
				if (ModLister.GetActiveModWithIdentifier(this.mods[i]) != null)
				{
					flag = true;
					if (log && AMAMod.Dev)
					{
						string text = string.Format("ID:: [{0}] Found by {1} ", this.mods[i], this);
						if (!string.IsNullOrEmpty(this.sourceFile))
						{
							text = text + "\nfile: " + this.sourceFile;
						}
						Log.Message(text);
					}
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
