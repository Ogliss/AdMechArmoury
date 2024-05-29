using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace AdeptusMechanicus
{
    public class HediffCompProperties_RejuvTreatment : HediffCompProperties
    {
        public HediffCompProperties_RejuvTreatment()
        {
            this.compClass = typeof(HediffComp_RejuvTreatment);
        }
        public float LifeExpectancyIncrease = 5f;
        public bool PerSeverityIncrease = true;
    }

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
