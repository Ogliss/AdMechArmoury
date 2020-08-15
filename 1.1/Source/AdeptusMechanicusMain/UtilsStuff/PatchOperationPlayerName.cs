using RimWorld;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Verse
{
    // Token: 0x02000D37 RID: 3383
    public class PatchOperationPlayerName : PatchOperation
    {
        // Token: 0x06004B25 RID: 19237 RVA: 0x00231378 File Offset: 0x0022F778
        protected override bool ApplyWorker(XmlDocument xml)
        {
            bool flag = false;
            for (int i = 0; i < this.names.Count; i++)
            {
                if (SteamUtility.SteamPersonaName == (this.names[i]))
                {
                    
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                if (this.match != null)
                {
                //    Log.Message(SteamUtility.SteamPersonaName + " Playing Loading Test Scenario");
                    return this.match.Apply(xml);
                }
            }
            else if (this.nomatch != null)
            {
                return this.nomatch.Apply(xml);
            }
            return true;
        }

        // Token: 0x06004B26 RID: 19238 RVA: 0x002313FC File Offset: 0x0022F7FC
        public override string ToString()
        {
            return string.Format("{0}({1})", base.ToString(), this.names.ToCommaList(false));
        }

        // Token: 0x040032F2 RID: 13042
        private List<string> names;

        // Token: 0x040032F3 RID: 13043
        private PatchOperation match;

        // Token: 0x040032F4 RID: 13044
        private PatchOperation nomatch;
    }
}
