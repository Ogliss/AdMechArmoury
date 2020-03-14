using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    public class Hediff_RegeneratingPart : Hediff_AddedPart
    {
        public override bool ShouldRemove
        {
            get
            {
                return this.Severity >= this.def.maxSeverity;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
        }

        public override string TipStringExtra
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(base.TipStringExtra);
                stringBuilder.AppendLine(Translator.Translate("Efficiency") + ": " + GenText.ToStringPercent
                    (this.def.addedPartProps.partEfficiency));
                stringBuilder.AppendLine("Growth: " + GenText.ToStringPercent(this.Severity));
                return stringBuilder.ToString();
            }
        }

        public override void PostRemoved()
        {
            base.PostRemoved();
            bool flag = this.Severity >= 1f;
            if (flag)
            {
                this.pawn.ReplaceHediffFromBodypart(base.Part, HediffDefOf.MissingBodyPart, OGHediffDefOf.Regenerated_Part_OG);
            }
        }
    }
}
