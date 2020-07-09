using System;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
	// Token: 0x0200031D RID: 797
	public static class CompFactionColorableUtility
	{
		// Token: 0x06001748 RID: 5960 RVA: 0x0008564C File Offset: 0x0008384C
		public static void SetFactionColorTwo(this Thing t, Color newColor, bool reportFailure = true)
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
			CompFactionColorable comp = thingWithComps.GetComp<CompFactionColorable>();
			if (comp == null)
			{
				if (reportFailure)
				{
					Log.Error("SetColor on Thing without CompFactionColorable " + t, false);
				}
				return;
			}
			if (comp.Color != newColor)
			{
				comp.Color = newColor;
			}
		}

		public static void SetFactionColor(this Thing t, Color newColor, bool reportFailure = true)
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
			CompFactionColorable comp = thingWithComps.GetComp<CompFactionColorable>();
			if (comp == null)
			{
				if (reportFailure)
				{
					Log.Error("SetColor on Thing without CompFactionColorable " + t, false);
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
