using RimWorld;
using RimWorld.BaseGen;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    public class GenStep_EscapePortal_Webway : GenStep_Scatterer
	{
		// Token: 0x17000D3D RID: 3389
		// (get) Token: 0x06004C34 RID: 19508 RVA: 0x00194703 File Offset: 0x00192903
		public override int SeedPart
		{
			get
			{
				return 860042045;
			}
		}

		// Token: 0x06004C35 RID: 19509 RVA: 0x0019470C File Offset: 0x0019290C
		public override bool CanScatterAt(IntVec3 c, Map map)
		{
			if (!base.CanScatterAt(c, map))
			{
				return false;
			}
			if (!c.Standable(map))
			{
				return false;
			}
			if (c.Roofed(map))
			{
				return false;
			}
			if (!map.reachability.CanReachMapEdge(c, TraverseParms.For(TraverseMode.PassDoors, Danger.Deadly, false, false, false)))
			{
				return false;
			}
			CellRect cellRect = new CellRect(c.x - GenStep_EscapePortal_Webway.EscapeShipSizeWidth.min / 2, c.z - GenStep_EscapePortal_Webway.EscapeShipSizeHeight.min / 2, GenStep_EscapeShip.EscapeShipSizeWidth.min, GenStep_EscapeShip.EscapeShipSizeHeight.min);
			if (!cellRect.FullyContainedWithin(new CellRect(0, 0, map.Size.x, map.Size.z)))
			{
				return false;
			}
			foreach (IntVec3 c2 in cellRect)
			{
				TerrainDef terrainDef = map.terrainGrid.TerrainAt(c2);
				if (!terrainDef.affordances.Contains(TerrainAffordanceDefOf.Heavy) && (terrainDef.driesTo == null || !terrainDef.driesTo.affordances.Contains(TerrainAffordanceDefOf.Heavy)))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06004C36 RID: 19510 RVA: 0x00194840 File Offset: 0x00192A40
		public override void ScatterAt(IntVec3 c, Map map, GenStepParams parms, int stackCount = 1)
		{
			int randomInRange = GenStep_EscapePortal_Webway.EscapeShipSizeWidth.RandomInRange;
			int randomInRange2 = GenStep_EscapePortal_Webway.EscapeShipSizeHeight.RandomInRange;
			CellRect rect = new CellRect(c.x - randomInRange / 2, c.z - randomInRange2 / 2, randomInRange, randomInRange2);
			rect.ClipInsideMap(map);
			foreach (IntVec3 c2 in rect)
			{
				if (!map.terrainGrid.TerrainAt(c2).affordances.Contains(TerrainAffordanceDefOf.Heavy))
				{
					CompTerrainPumpDry.AffectCell(map, c2);
					for (int i = 0; i < 8; i++)
					{
						Vector3 b = Rand.InsideUnitCircleVec3 * 3f;
						IntVec3 c3 = IntVec3.FromVector3(c2.ToVector3Shifted() + b);
						if (c3.InBounds(map))
						{
							CompTerrainPumpDry.AffectCell(map, c3);
						}
					}
				}
			}
			ResolveParams resolveParams = default(ResolveParams);
			resolveParams.rect = rect;
			BaseGen.globalSettings.map = map;
			BaseGen.symbolStack.Push("portal_webway_core", resolveParams, null);
			BaseGen.Generate();
		}

		// Token: 0x04002E23 RID: 11811
		private static readonly IntRange EscapeShipSizeWidth = new IntRange(20, 28);

		// Token: 0x04002E24 RID: 11812
		private static readonly IntRange EscapeShipSizeHeight = new IntRange(34, 42);
	}
}
