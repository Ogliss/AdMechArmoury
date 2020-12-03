using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse.AI.Group;
using Verse.Profile;
using Verse.Sound;

namespace Verse
{
    // Token: 0x0200053D RID: 1341
    public static class AdMechDebugActionsMisc
	{
		/*
		[DebugAction("General", null, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		private static void DestroyAllStrangeFungus()
		{
			foreach (Thing thing in Find.CurrentMap.listerThings.AllThings.ToList<Thing>())
			{
				if (thing is Plant && thing.def.defName.Contains(XenomorphDefOf.AvP_Plant_Neomorph_Fungus.defName))
				{
					thing.Destroy(DestroyMode.Vanish);
				}
			}
		}
		*/

		// Verse.DebugToolsSpawning
		[DebugAction("General", "Destroy All things of Def...", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		private static void DestroyAllThingsOf()
		{
			List<DebugMenuOption> list = new List<DebugMenuOption>();
			foreach (Thing t in from kd in Find.CurrentMap.thingGrid.ThingsAt(UI.MouseCell()).ToList<Thing>()
								orderby kd.def.defName
								select kd)
			{
				list.Add(new DebugMenuOption(t.def.LabelCap, DebugMenuOptionMode.Action, delegate ()
				{
					foreach (Thing thing in Find.CurrentMap.listerThings.AllThings.ToList<Thing>())
					{
						if (thing.def == t.def)
						{
							thing.Destroy(DestroyMode.Vanish);
						}
					}
				}));
			}
			Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
		}
		// Verse.DebugToolsSpawning
		[DebugAction("General", "Replace All things of Def...", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		private static void ReplaceAllThingsOf()
		{
			List<DebugMenuOption> list = new List<DebugMenuOption>();
			foreach (Thing t in from kd in Find.CurrentMap.thingGrid.ThingsAt(UI.MouseCell()).ToList<Thing>()
								orderby kd.def.defName
								select kd)
			{
				list.Add(new DebugMenuOption(t.def.LabelCap, DebugMenuOptionMode.Action, delegate ()
				{
					List<DebugMenuOption> list2 = new List<DebugMenuOption>();
					foreach (ThingDef localDef3 in from def in DefDatabase<ThingDef>.AllDefs
												   where DebugThingPlaceHelper.IsDebugSpawnable(def, false)
												   select def)
					{
						ThingDef localDef = localDef3;
						list2.Add(new DebugMenuOption(localDef.LabelCap, DebugMenuOptionMode.Action, delegate ()
						{
							foreach (Thing thing in Find.CurrentMap.listerThings.AllThings.ToList<Thing>())
							{
								if (thing.def == t.def)
								{
									IntVec3 pos = thing.Position;
									int count = thing.stackCount;
									thing.Destroy(DestroyMode.Vanish);
									DebugThingPlaceHelper.DebugSpawn(localDef, pos, count);
								}
							}
						}));
						Find.WindowStack.Add(new Dialog_DebugOptionListLister(list2));
					}
				}));
			}
			Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
		}

	}
}
