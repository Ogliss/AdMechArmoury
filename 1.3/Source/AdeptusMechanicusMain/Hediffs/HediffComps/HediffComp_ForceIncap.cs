using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.HediffCompProperties_ForceIncap
    public class HediffCompProperties_ForceIncap : HediffCompProperties
    {
        // Token: 0x06000AB1 RID: 2737 RVA: 0x0005598C File Offset: 0x00053D8C
        public HediffCompProperties_ForceIncap()
        {
            this.compClass = typeof(HediffComp_ForceIncap);
        }
        public float chance = 1f;
    }
    // Token: 0x02000C69 RID: 3177
    public class HediffComp_ForceIncap : HediffComp
    {
        public HediffCompProperties_ForceIncap Props
        {
            get
            {
                return (HediffCompProperties_ForceIncap)this.props;
            }
        }
    }
}
