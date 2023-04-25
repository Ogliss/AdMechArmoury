using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    public static class DebugToolsGeneral
    {

		[DebugAction("General", null, false, false, actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		private static void SetDualColor()
		{
			List<FloatMenuOption> colorOne = new List<FloatMenuOption>();
			List<FloatMenuOption> colorTwo = new List<FloatMenuOption>();
			IntVec3 cell = UI.MouseCell();
			foreach (Ideo i in Find.IdeoManager.IdeosListForReading)
			{
				colorOne.Add(new FloatMenuOption(i.name, delegate
				{
					SetColor_All(i.Color);
				}, i.Icon, i.Color));
				colorTwo.Add(new FloatMenuOption(i.name, delegate
				{
					SetColor_All(i.Color);
				}, i.Icon, i.Color));
			}
			foreach (ColorDef c in DefDatabase<ColorDef>.AllDefs)
			{
				colorOne.Add(new FloatMenuOption(c.defName, delegate
				{
					SetColor_All(c.color);
				}, BaseContent.WhiteTex, c.color));
				colorTwo.Add(new FloatMenuOption(c.defName, delegate
				{
					SetColor_All(c.color);
				}, BaseContent.WhiteTex, c.color));
			}
			if (colorOne.Any())
			{
				Find.WindowStack.Add(new FloatMenu(colorOne));
			}
			void SetColor_All(Color color)
			{
				foreach (Thing item in Find.CurrentMap.thingGrid.ThingsAt(cell).ToList())
				{
					Pawn pawn;
					if ((pawn = (item as Pawn)) != null && pawn.apparel != null)
					{
						foreach (Apparel item2 in pawn.apparel.WornApparel)
						{
							item2.SetColor(color, reportFailure: false);
						}
					}
					else
					{
						item.SetColor(color, reportFailure: false);
					}
				}
			}
		}
	}
    class MechanicusDebugViewSettings
    {
        public static bool drawMuzzlePosition = false;
    }
}
