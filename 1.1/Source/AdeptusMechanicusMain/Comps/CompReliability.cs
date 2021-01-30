using RimWorld;
using System;
using Verse;

namespace AdeptusMechanicus
{
	public class CompProperties_Reliability : CompProperties
	{
		public CompProperties_Reliability()
		{
			this.compClass = typeof(CompReliability);
		}
		Reliability reliability = Reliability.NA;
	}
	// Token: 0x02000D7C RID: 3452
	public class CompReliability : ThingComp
	{
		// Token: 0x17000EF6 RID: 3830
		// (get) Token: 0x0600541B RID: 21531 RVA: 0x001C15D5 File Offset: 0x001BF7D5
		public Reliability Reliability
		{
			get
			{
				return this.reliabilityInt;
			}
		}

		// Token: 0x0600541C RID: 21532 RVA: 0x001C15E0 File Offset: 0x001BF7E0
		public void SetReliability(Reliability q)
		{
			this.reliabilityInt = q;
		}

		// Token: 0x0600541D RID: 21533 RVA: 0x001C160A File Offset: 0x001BF80A
		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look<Reliability>(ref this.reliabilityInt, "reliability", Reliability.NA, false);
		}

		// Token: 0x06005421 RID: 21537 RVA: 0x001C1670 File Offset: 0x001BF870
		public override string CompInspectStringExtra()
		{
			return "ReliabilityIs".Translate(this.Reliability.GetLabel().CapitalizeFirst());
		}

		// Token: 0x04002E5F RID: 11871
		private Reliability reliabilityInt = Reliability.NA;
	}
	public enum Reliability : short
	{
		UR = 80,
		ST = 55,
		VR = 30,
		NA = 0
	}

	public static class ReliabilityUtility
	{
		public static string GetLabel(this Reliability cat)
		{
			switch (cat)
			{
				case Reliability.NA:
					return null;
				case Reliability.VR:
					return "Reliability_VeryReliable".Translate();
				case Reliability.ST:
					return "Reliability_Standard".Translate();
				case Reliability.UR:
					return "Reliability_Unreliable".Translate();
				default:
					throw new ArgumentException();
			}
		}
	}
}
