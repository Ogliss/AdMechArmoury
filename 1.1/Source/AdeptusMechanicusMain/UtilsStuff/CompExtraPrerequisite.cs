using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x0200024E RID: 590
    public class CompProperties_ExtraPrerequisite : CompProperties
    {
        // Token: 0x06000AB1 RID: 2737 RVA: 0x0005598C File Offset: 0x00053D8C
        public CompProperties_ExtraPrerequisite()
        {
            this.compClass = typeof(CompExtraPrerequisite);
        }

        public List<string> ExtraResarchPrerequisites;
    }
    // Token: 0x02000C69 RID: 3177
    public class CompExtraPrerequisite : ThingComp
    {
        public CompProperties_ExtraPrerequisite Props
        {
            get
            {
                return (CompProperties_ExtraPrerequisite)this.props;
            }
        }

        public List<ResearchProjectDef> ExtraResarchPrerequisites
        {
            get
            {
                List<ResearchProjectDef> list = new List<ResearchProjectDef>();
                foreach (var item in Props.ExtraResarchPrerequisites)
                {
                    ResearchProjectDef Def = DefDatabase<ResearchProjectDef>.GetNamedSilentFail(item);
                    if (Def != null)
                    {
                        list.Add(Def);
                    }
                }
                return list;
            }
        }
    }
}
