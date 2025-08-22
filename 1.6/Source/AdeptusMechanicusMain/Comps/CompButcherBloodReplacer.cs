using System;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;

namespace AdeptusMechanicus
{
    public class CompProperties_ButcherBloodReplacer : CompProperties
    {
        public CompProperties_ButcherBloodReplacer()
        {
            this.compClass = typeof(CompButcherBloodReplacer);
        }
        public ThingDef bloodDef = null;
        public float timeAfterDeath = 0f;
    }

    public class CompButcherBloodReplacer : ThingComp
    {
        public CompProperties_ButcherBloodReplacer Props
        {
            get
            {
                return (CompProperties_ButcherBloodReplacer)this.props;
            }
        }

        public ThingDef bloodDef
        {
            get
            {
                return Props.bloodDef;
            }
        }
        public float timeAfterDeath
        {
            get
            {
                return Props.timeAfterDeath.SecondsToTicks();
            }
        }
    }
}
