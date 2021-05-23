using System;
using Verse;

namespace AdeptusMechanicus
{
	public class HediffGiver_HealthBelowThreshold : HediffGiver
	{
		public override void OnIntervalPassed(Pawn pawn, Hediff cause)
		{
			HediffSet hediffSet = pawn.health.hediffSet;
			if (pawn.health.summaryHealth.SummaryHealthPercent <= this.thershold)
			{
				HealthUtility.AdjustSeverity(pawn, this.hediff, hediffSet.BleedRateTotal * 0.001f);
				return;
			}
			HealthUtility.AdjustSeverity(pawn, this.hediff, -0.00033333333f);
		}
		float thershold = 0.5f;
	}
}
