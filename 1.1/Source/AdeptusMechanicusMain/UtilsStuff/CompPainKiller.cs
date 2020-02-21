using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    public class CompProperties_PainKiller : CompProperties
    {
        public CompProperties_PainKiller()
        {
            this.compClass = typeof(CompPainKiller);
        }
        public float painOffset = 0.5f;


    }
    // Token: 0x02000002 RID: 2
    public class CompPainKiller : ThingComp
    {
        public CompProperties_PainKiller Props => (CompProperties_PainKiller)this.props;

    }
}
