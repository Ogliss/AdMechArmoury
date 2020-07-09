using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus.HarmonyInstance
{
	// Token: 0x020001AF RID: 431
	[HarmonyPatch(typeof(ApparelGraphicRecordGetter), "TryGetGraphicApparel")]
	public static class AM_ApparelGraphicRecordGetter_TryGetGraphicApparel_AdvancedGraphics_Patch
	{

		// Token: 0x060008F6 RID: 2294 RVA: 0x0004A904 File Offset: 0x00048B04
		[HarmonyPostfix]
		public static void Postfix(Apparel apparel, BodyTypeDef bodyType, ref ApparelGraphicRecord rec)
		{
			bool adv = apparel.TryGetComp<CompPauldronDrawer>() != null;
			if (adv)
			{
			//	Log.Message("Updating pad graphics for "+apparel.LabelShortCap);
				for (int i = 0; i < apparel.GetComps<CompPauldronDrawer>().Count(); i++)
				{
				//	Log.Message("Pauldron drawer "+(i+1));
					CompPauldronDrawer comp = apparel.GetComps<CompPauldronDrawer>().ElementAt(i);
					if (!comp.activeEntries.NullOrEmpty())
					{
						for (int i2 = 0; i2 < comp.activeEntries.Count; i2++)
						{
						//	Log.Message("Entry drawer " + (i2 + 1));
							comp.activeEntries[i2].UpdatePadGraphic();
						}
					}
				}
			}
		}

	}
}
