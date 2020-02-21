using System;
using System.Linq;
using RimWorld;
using Verse;

namespace Technocytes
{
    public class HediffCompProperties_AddSecondHediff : HediffCompProperties
    {
        public HediffCompProperties_AddSecondHediff()
        {
            this.compClass = typeof(HediffComp_AddSecondHediff);
        }
        public HediffDef ExtraHediff = null;
        public float ChanceToAdd = 1f;
        public string ExtraBodyPart = null;
    }

    // Token: 0x02000016 RID: 22
    public class HediffComp_AddSecondHediff : HediffComp
    {
        public HediffCompProperties_AddSecondHediff Props
        {
            get
            {
                return this.props as HediffCompProperties_AddSecondHediff;
            }
        }

        string bodyPart;
        public override void CompPostMake()
        {
            if (Props.ExtraHediff==null)
            {
                Log.Error("ExtraHediff is null");
                return;
            }
            bodyPart = Props.ExtraBodyPart;
            BodyPartRecord part = this.parent.Part;
            if (!this.bodyPart.NullOrEmpty())
            {
                if (Pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined, null, null).Any((BodyPartRecord bpr) => bpr.untranslatedCustomLabel == this.bodyPart || bpr.def.defName == this.bodyPart))
                {
                    part = Pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined, null, null).Where((BodyPartRecord bpr) => bpr.untranslatedCustomLabel == this.bodyPart || bpr.def.defName == this.bodyPart).First();
                }
                else
                {
                    Log.Error(string.Format("ExtraBodyPart: {0} is missing for ExtraHediff: {1}", Props.ExtraBodyPart, Props.ExtraHediff.LabelCap));
                    return;
                }
            }
            if (Rand.Chance(Props.ChanceToAdd))
            {
                if (Props.ExtraHediff == HediffDefOf.WoundInfection)
                {
                    part = this.parent.Part.parent;
                }
                this.Pawn.health.AddHediff(Props.ExtraHediff, part);
            }
            base.CompPostMake();
        }
    }
}
