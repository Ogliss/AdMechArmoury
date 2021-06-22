using System;
using Verse;

namespace AdeptusMechanicus
{
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
