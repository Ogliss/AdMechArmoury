﻿using RimWorld;
using RimWorld.BaseGen;
using System.Linq;
using Verse;

namespace AdeptusMechanicus
{
    public class SymbolResolver_Portal_Webway_Spine : SymbolResolver
	{
		public override void Resolve(ResolveParams rp)
		{
			ThingDef ship_Beam = ThingDefOf.Ship_Beam;
			Map map = BaseGen.globalSettings.map;
			if (rp.thingRot == null && rp.thrustAxis == null)
			{
				rp.thrustAxis = new Rot4?(Rot4.Random);
				rp.thingRot = rp.thrustAxis;
			}
			IntVec2 invalid = IntVec2.Invalid;
			IntVec2 invalid2 = IntVec2.Invalid;
			IntVec2 toIntVec = rp.thingRot.Value.FacingCell.ToIntVec2;
			int num = 0;
			for (; ; )
			{
				if (rp.thingRot.Value.IsHorizontal)
				{
					int newZ = Rand.Range(rp.rect.minZ + 1, rp.rect.maxZ - 2);
					invalid = new IntVec2((rp.thingRot.Value == Rot4.East) ? rp.rect.minX : rp.rect.maxX, newZ);
					invalid2 = new IntVec2((rp.thingRot.Value == Rot4.East) ? rp.rect.maxX : rp.rect.minX, newZ);
				}
				else
				{
					int newX = Rand.Range(rp.rect.minX + 1, rp.rect.maxX - 2);
					invalid = new IntVec2(newX, (rp.thingRot.Value == Rot4.North) ? rp.rect.minZ : rp.rect.maxZ);
					invalid2 = new IntVec2(newX, (rp.thingRot.Value == Rot4.North) ? rp.rect.maxZ : rp.rect.minZ);
				}
				if (rp.allowPlacementOffEdge ?? true)
				{
					goto IL_22B;
				}
				if ((invalid - toIntVec).ToIntVec3.GetThingList(map).Any((Thing thing) => thing.def == ThingDefOf.Ship_Beam))
				{
					goto IL_22B;
				}
				if (num == 20)
				{
					break;
				}
				num++;
			}
			return;
			IL_22B:
			int magnitudeManhattan = (invalid - invalid2).MagnitudeManhattan;
			if ((invalid - invalid2).Magnitude < (float)ship_Beam.Size.z)
			{
				return;
			}
			int num4;
			int num5;
			do
			{
				int num2 = (rp.allowPlacementOffEdge ?? true) ? Rand.Range(0, 7) : 0;
				int num3 = Rand.Range(0, 7);
				num4 = 0;
				num3 = 0;
				num5 = (magnitudeManhattan - num4 - num3) / ship_Beam.Size.z;
			}
			while (num5 <= 0);
			IntVec2 a = invalid + toIntVec * (num4 + ship_Beam.Size.z / 2 - 1);
			Thing t = null;
			for (int i = 0; i < num5; i++)
			{
				Thing thing2 = ThingMaker.MakeThing(ship_Beam, null);
				thing2.SetFaction(rp.faction, null);
				t = GenSpawn.Spawn(thing2, a.ToIntVec3, map, rp.thingRot.Value, WipeMode.Vanish, false);
				a += toIntVec * ship_Beam.Size.z;
			}
			if (rp.allowPlacementOffEdge ?? true)
			{
				BaseGen.symbolStack.Push("ship_populate", rp, null);
			}
			CellRect rect;
			Rot4 value;
			CellRect rect2;
			Rot4 value2;
			if (rp.thingRot.Value.IsHorizontal)
			{
				rect = rp.rect;
				rect.minZ = t.OccupiedRect().maxZ + 1;
				value = Rot4.North;
				rect2 = rp.rect;
				rect2.maxZ = t.OccupiedRect().minZ - 1;
				value2 = Rot4.South;
			}
			else
			{
				rect = rp.rect;
				rect.maxX = t.OccupiedRect().minX - 1;
				value = Rot4.West;
				rect2 = rp.rect;
				rect2.minX = t.OccupiedRect().maxX + 1;
				value2 = Rot4.East;
			}
			if ((rp.allowPlacementOffEdge ?? true) || Rand.Value < 0.3f)
			{
				ResolveParams resolveParams = rp;
				resolveParams.rect = rect;
				resolveParams.thingRot = new Rot4?(value);
				resolveParams.allowPlacementOffEdge = new bool?(false);
				BaseGen.symbolStack.Push("ship_spine", resolveParams, null);
			}
			if ((rp.allowPlacementOffEdge ?? true) || Rand.Value < 0.3f)
			{
				ResolveParams resolveParams2 = rp;
				resolveParams2.rect = rect2;
				resolveParams2.thingRot = new Rot4?(value2);
				resolveParams2.allowPlacementOffEdge = new bool?(false);
				BaseGen.symbolStack.Push("ship_spine", resolveParams2, null);
			}
			ResolveParams resolveParams3 = rp;
			resolveParams3.floorDef = TerrainDefOf.Concrete;
			BaseGen.symbolStack.Push("floor", resolveParams3, null);
		}
	}
}
