using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace AdeptusMechanicus
{
    // Token: 0x0200024E RID: 590
    public class HediffCompProperties_RejuvTreatment : HediffCompProperties
    {
        // Token: 0x06000AB1 RID: 2737 RVA: 0x0005598C File Offset: 0x00053D8C
        public HediffCompProperties_RejuvTreatment()
        {
            this.compClass = typeof(HediffComp_RejuvTreatment);
        }
        public float LifeExpectancyIncrease = 5f;
        public bool PerSeverityIncrease = true;
    }
    // Token: 0x02000C69 RID: 3177
    public class HediffComp_RejuvTreatment : HediffComp
    {
        public HediffCompProperties_RejuvTreatment Props
        {
            get
            {
                return (HediffCompProperties_RejuvTreatment)this.props;
            }
        }

        public float LifeExpectancyIncrease
        {
            get
            {
                if (Props.PerSeverityIncrease)
                {
                    return Props.LifeExpectancyIncrease * parent.Severity;
                }
                else
                {
                    return Props.LifeExpectancyIncrease;
                }
            }
        }
    }
}
