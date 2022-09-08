using System;
using Verse;

namespace AdeptusMechanicus
{
	// AdeptusMechanicus.HediffGiver_HealthBelowThreshold
	public class HediffGiver_HealthBelowThreshold : HediffGiver
	{
		public float threshold = 0.5f;

		public override void OnIntervalPassed(Pawn pawn, Hediff cause)
		{
			HediffSet hediffSet = pawn.health.hediffSet;
			if (pawn.health.summaryHealth.SummaryHealthPercent <= this.threshold)
			{
				HealthUtility.AdjustSeverity(pawn, this.hediff, hediffSet.BleedRateTotal * 0.001f);
				return;
			}
			HealthUtility.AdjustSeverity(pawn, this.hediff, -0.00033333333f);
		}
	}
}
