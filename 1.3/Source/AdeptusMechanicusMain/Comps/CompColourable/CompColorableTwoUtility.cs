using AdeptusMechanicus.ExtensionMethods;
using HarmonyLib;
using RimWorld;
using System;
using System.Reflection;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x0200031D RID: 797
    public static class CompColorableTwoUtility
	{
		public static void SetColors(this Thing t, Color newColorOne, Color? newColorTwo = null, bool setFaction = false, FactionDef factionDef = null, Graphic graphic = null, bool reportFailure = true)
		{
			ThingWithComps twc = t as ThingWithComps;
			if (twc == null)
			{
				if (reportFailure)
				{
					Log.Error("SetColor on non-ThingWithComps " + t);
				}
				return;
			}
			t.SetColorOne(newColorOne);
            if (newColorTwo.HasValue)
			{
				t.SetColorTwo(newColorTwo.Value);
			}
            if (setFaction)
			{
				CompColorableTwoFaction comp = twc.TryGetCompFast<CompColorableTwoFaction>();
				if (comp != null && factionDef != null)
				{
					comp.FactionDef = factionDef;
                    if (graphic != null)
					{
						FieldInfo subgraphic = typeof(Thing).GetField("graphicInt", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
						Traverse traverse = Traverse.Create(t);
						subgraphic.SetValue(t, graphic);
					}
				}
			}
            if (t is Apparel apparel && apparel.Wearer != null && !apparel.def.apparel.wornGraphicPath.NullOrEmpty())
            {
				apparel.Wearer.Drawer.renderer.graphics.ResolveApparelGraphics();
            }
		}
		public static void SetColorOne(this Thing t, Color newColor, bool reportFailure = true)
		{
			ThingWithComps thingWithComps = t as ThingWithComps;
			if (thingWithComps == null)
			{
				if (reportFailure)
				{
					Log.Error("SetColor on non-ThingWithComps " + t);
				}
				return;
			}
			CompColorableTwo comp = thingWithComps.GetComp<CompColorableTwo>();
			if (comp == null)
			{
				t.SetColor(newColor);
				if (reportFailure)
				{
					Log.Error("SetColor on Thing without CompColorableTwo " + t);
				}
				return;
			}
			if (comp.Color != newColor)
			{
				comp.Color = newColor;
			}
		}

		public static void SetColorTwo(this Thing t, Color newColor, bool reportFailure = true)
		{
			ThingWithComps thingWithComps = t as ThingWithComps;
			if (thingWithComps == null)
			{
				if (reportFailure)
				{
					Log.Error("SetColor on non-ThingWithComps " + t);
				}
				return;
			}
			CompColorableTwo comp = thingWithComps.GetComp<CompColorableTwo>();
			if (comp == null)
			{
				if (reportFailure)
				{
					Log.Error("SetColor on Thing without CompColorableTwo " + t);
				}
				return;
			}
			if (comp.ColorTwo != newColor)
			{
				comp.ColorTwo = newColor;
			}
		}
	}
}
