using System;
using RimWorld;
using Verse;
using Verse.AI;

namespace AdeptusMechanicus
{
    public class OrdnanceStrikeCellFinder
    {
		public static bool TryFindStrikeLocNear(IntVec3 center, Map map, out IntVec3 result, bool allowFogged, bool canRoofPunch, bool allowIndoors = true, IntVec2? size = null)
		{
			if (DebugViewSettings.drawDestSearch)
			{
				map.debugDrawer.FlashCell(center, 1f, "center");
			}
			Room centerRoom = center.GetRoom(map);
			Predicate<IntVec3> validator = delegate (IntVec3 c)
			{
				if (size.HasValue)
				{
					foreach (IntVec3 item in GenAdj.OccupiedRect(c, Rot4.North, size.Value))
					{
						if (!IsGoodDropSpot(item, map, allowFogged, canRoofPunch, allowIndoors))
						{
							return false;
						}
					}
				}
				else if (!IsGoodDropSpot(c, map, allowFogged, canRoofPunch, allowIndoors))
				{
					return false;
				}
				return map.reachability.CanReach(center, c, PathEndMode.OnCell, TraverseMode.PassDoors, Danger.Deadly) ? true : false;
			};
			if ((allowIndoors & canRoofPunch) && centerRoom != null && !centerRoom.PsychologicallyOutdoors)
			{
				Predicate<IntVec3> v2 = (IntVec3 c) => validator(c) && c.GetRoom(map) == centerRoom;
				if (TryFindCell(v2, out result))
				{
					return true;
				}
				Predicate<IntVec3> v3 = delegate (IntVec3 c)
				{
					if (!validator(c))
					{
						return false;
					}
					Room room = c.GetRoom(map);
					return room != null && !room.PsychologicallyOutdoors;
				};
				if (TryFindCell(v3, out result))
				{
					return true;
				}
			}
			return TryFindCell(validator, out result);
			bool TryFindCell(Predicate<IntVec3> v, out IntVec3 r)
			{
				int num = 5;
				do
				{
					if (CellFinder.TryFindRandomCellNear(center, map, num, v, out r))
					{
						return true;
					}
					num += 3;
				}
				while (num <= 16);
				r = center;
				return false;
			}
		}

		public static bool IsGoodDropSpot(IntVec3 c, Map map, bool allowFogged, bool canRoofPunch, bool allowIndoors = true)
		{
			if (!c.InBounds(map) /*|| !c.Standable(map)*/ )
			{
				return false;
			}
			if (!DropCellFinder.CanPhysicallyDropInto(c, map, canRoofPunch, allowIndoors))
			{
				if (DebugViewSettings.drawDestSearch)
				{
					map.debugDrawer.FlashCell(c, 0f, "phys", 50);
				}
				return false;
			}
			if (Current.ProgramState == ProgramState.Playing && !allowFogged && c.Fogged(map))
			{
				return false;
			}
			/*
			List<Thing> thingList = c.GetThingList(map);
			for (int i = 0; i < thingList.Count; i++)
			{
				Thing thing = thingList[i];
				if (thing is IActiveDropPod || thing is Skyfaller)
				{
					return false;
				}
				if (thing.def.IsEdifice())
				{
					return false;
				}
				if (thing.def.preventSkyfallersLandingOn)
				{
					return false;
				}
				if (thing.def.category != ThingCategory.Plant && GenSpawn.SpawningWipes(ThingDefOf.ActiveDropPod, thing.def))
				{
					return false;
				}
			}
			*/
			return true;
		}
	}
}
