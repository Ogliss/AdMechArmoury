using System;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
	// Token: 0x0200031D RID: 797
	public static class CompColorableTwoUtility
	{
		public static void SetColorTwo(this Thing t, Color newColor, bool reportFailure = true)
		{
			ThingWithComps thingWithComps = t as ThingWithComps;
			if (thingWithComps == null)
			{
				if (reportFailure)
				{
					Log.Error("SetColor on non-ThingWithComps " + t, false);
				}
				return;
			}
			CompColorableTwo comp = thingWithComps.GetComp<CompColorableTwo>();
			if (comp == null)
			{
				if (reportFailure)
				{
					Log.Error("SetColor on Thing without CompColorableTwo " + t, false);
				}
				return;
			}
			if (comp.Color != newColor)
			{
				comp.Color = newColor;
			}
		}
	}
}
