using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;

namespace AdeptusMechanicus
{
    // Token: 0x0200092D RID: 2349
    public static class InfestationLikeCellFinder
    {
        // Token: 0x0600368B RID: 13963 RVA: 0x001A0E1C File Offset: 0x0019F21C
        public static bool TryFindCell(out IntVec3 cell, Map map, ThingDef thingDef, FactionDef factionDef = null)
        {
            //Log.Message(string.Format("Tick: 1"));
            InfestationLikeCellFinder.CalculateLocationCandidates(map, thingDef);
            InfestationLikeCellFinder.LocationCandidate locationCandidate;
            if (!InfestationLikeCellFinder.locationCandidates.TryRandomElementByWeight((InfestationLikeCellFinder.LocationCandidate x) => x.score, out locationCandidate))
            {
                cell = IntVec3.Invalid;
                //Log.Message(string.Format("TryFindCell: {0} From !InfestationLikeCellFinder.TryFindCell(out loc, map)", cell));
                return false;
            }
            cell = CellFinder.FindNoWipeSpawnLocNear(locationCandidate.cell, map, thingDef, Rot4.North, 2, (IntVec3 x) => InfestationLikeCellFinder.GetScoreAt(x, map, thingDef) > 0f && x.GetFirstThing(map, thingDef) == null && x.GetFirstThing(map, ((ThingDef_HiveLike)thingDef).TunnelDef) == null);
            return true;
        }

        // Token: 0x0600368C RID: 13964 RVA: 0x001A0EAC File Offset: 0x0019F2AC
        private static float GetScoreAt(IntVec3 cell, Map map, ThingDef thingDef)
        {
            if ((float)InfestationLikeCellFinder.distToColonyBuilding[cell] > 30f)
            {
                return 0f;
            }
            if (!cell.Walkable(map))
            {
                return 0f;
            }
            if (cell.Fogged(map))
            {
                return 0f;
            }
            if (InfestationLikeCellFinder.CellHasBlockingThings(cell, map, thingDef))
            {
                return 0f;
            }
            if (!cell.Roofed(map) || !cell.GetRoof(map).isThickRoof)
            {
                return 0f;
            }
            Region region = cell.GetRegion(map, RegionType.Set_Passable);
            if (region == null)
            {
                return 0f;
            }
            if (InfestationLikeCellFinder.closedAreaSize[cell] < 2)
            {
                return 0f;
            }
            float temperature = cell.GetTemperature(map);
            if (temperature < -17f)
            {
                return 0f;
            }
            float mountainousnessScoreAt = InfestationLikeCellFinder.GetMountainousnessScoreAt(cell, map);
            if (mountainousnessScoreAt < 0.17f)
            {
                return 0f;
            }
            int num = InfestationLikeCellFinder.StraightLineDistToUnroofed(cell, map);
            float num2;
            if (!InfestationLikeCellFinder.regionsDistanceToUnroofed.TryGetValue(region, out num2))
            {
                num2 = (float)num * 1.15f;
            }
            else
            {
                num2 = Mathf.Min(num2, (float)num * 4f);
            }
            num2 = Mathf.Pow(num2, 1.55f);
            float num3 = Mathf.InverseLerp(0f, 12f, (float)num);
            float num4 = Mathf.Lerp(1f, 0.18f, map.glowGrid.GameGlowAt(cell, false));
            float num5 = 1f - Mathf.Clamp(InfestationLikeCellFinder.DistToBlocker(cell, map) / 11f, 0f, 0.6f);
            float num6 = Mathf.InverseLerp(-17f, -7f, temperature);
            float num7 = num2 * num3 * num5 * mountainousnessScoreAt * num4 * num6;
            num7 = Mathf.Pow(num7, 1.2f);
            if (num7 < 7.5f)
            {
                return 0f;
            }
            return num7;
        }

        private static void CalculateLocationCandidates(Map map, ThingDef thingDef)
        {
            InfestationLikeCellFinder.locationCandidates.Clear();
            InfestationLikeCellFinder.CalculateTraversalDistancesToUnroofed(map);
            InfestationLikeCellFinder.CalculateClosedAreaSizeGrid(map);
            InfestationLikeCellFinder.CalculateDistanceToColonyBuildingGrid(map);
            for (int i = 0; i < map.Size.z; i++)
            {
                for (int j = 0; j < map.Size.x; j++)
                {
                    IntVec3 cell = new IntVec3(j, 0, i);
                    float scoreAt = InfestationLikeCellFinder.GetScoreAt(cell, map, thingDef);
                    if (scoreAt > 0f)
                    {
                        InfestationLikeCellFinder.locationCandidates.Add(new InfestationLikeCellFinder.LocationCandidate(cell, scoreAt));
                    }
                }
            }
        }

        private static bool CellHasBlockingThings(IntVec3 cell, Map map, ThingDef thingDef)
        {
            List<Thing> thingList = cell.GetThingList(map);
            for (int i = 0; i < thingList.Count; i++)
            {
                if (thingList[i] is Pawn || thingList[i] is HiveLike || thingList[i] is TunnelHiveLikeSpawner)
                {
                    return true;
                }
                bool flag = thingList[i].def.category == ThingCategory.Building && thingList[i].def.passability == Traversability.Impassable;
                if (flag && GenSpawn.SpawningWipes(thingDef, thingList[i].def))
                {
                    return true;
                }
            }
            return false;
        }

        // Token: 0x06003690 RID: 13968 RVA: 0x001A13CC File Offset: 0x0019F7CC
        private static int StraightLineDistToUnroofed(IntVec3 cell, Map map)
        {
            int num = int.MaxValue;
            int i = 0;
            while (i < 4)
            {
                Rot4 rot = new Rot4(i);
                IntVec3 facingCell = rot.FacingCell;
                int num2 = 0;
                int num3;
                for (; ; )
                {
                    IntVec3 intVec = cell + facingCell * num2;
                    if (!intVec.InBounds(map))
                    {
                        goto Block_1;
                    }
                    num3 = num2;
                    num2++;
                }
                IL_6F:
                if (num3 < num)
                {
                    num = num3;
                }
                i++;
                continue;
                Block_1:
                num3 = int.MaxValue;
                goto IL_6F;
            }
            if (num == 2147483647)
            {
                return map.Size.x;
            }
            return num;
        }

        // Token: 0x06003691 RID: 13969 RVA: 0x001A1478 File Offset: 0x0019F878
        private static float DistToBlocker(IntVec3 cell, Map map)
        {
            int num = int.MinValue;
            int num2 = int.MinValue;
            for (int i = 0; i < 4; i++)
            {
                Rot4 rot = new Rot4(i);
                IntVec3 facingCell = rot.FacingCell;
                int num3 = 0;
                int num4;
                for (; ; )
                {
                    IntVec3 c = cell + facingCell * num3;
                    num4 = num3;
                    if (!c.InBounds(map) || !c.Walkable(map))
                    {
                        break;
                    }
                    num3++;
                }
                if (num4 > num)
                {
                    num2 = num;
                    num = num4;
                }
                else if (num4 > num2)
                {
                    num2 = num4;
                }
            }
            return (float)Mathf.Min(num, num2);
        }

        // Token: 0x06003692 RID: 13970 RVA: 0x001A1520 File Offset: 0x0019F920
        private static bool NoRoofAroundAndWalkable(IntVec3 cell, Map map)
        {
            if (!cell.Walkable(map))
            {
                return false;
            }
            if (cell.Roofed(map))
            {
                return false;
            }
            for (int i = 0; i < 4; i++)
            {
                Rot4 rot = new Rot4(i);
                IntVec3 c = rot.FacingCell + cell;
                if (c.InBounds(map) && c.Roofed(map))
                {
                    return false;
                }
            }
            return true;
        }

        // Token: 0x06003693 RID: 13971 RVA: 0x001A158C File Offset: 0x0019F98C
        private static float GetMountainousnessScoreAt(IntVec3 cell, Map map)
        {
            float num = 0f;
            int num2 = 0;
            for (int i = 0; i < 700; i += 10)
            {
                IntVec3 c = cell + GenRadial.RadialPattern[i];
                if (c.InBounds(map))
                {
                    Building edifice = c.GetEdifice(map);
                    if (edifice != null && edifice.def.category == ThingCategory.Building && edifice.def.building.isNaturalRock)
                    {
                        num += 1f;
                    }
                    else if (c.Roofed(map) && c.GetRoof(map).isThickRoof)
                    {
                        num += 0.5f;
                    }
                    num2++;
                }
            }
            return num / (float)num2;
        }

        // Token: 0x06003694 RID: 13972 RVA: 0x001A1654 File Offset: 0x0019FA54
        private static void CalculateTraversalDistancesToUnroofed(Map map)
        {
            InfestationLikeCellFinder.tempUnroofedRegions.Clear();
            for (int i = 0; i < map.Size.z; i++)
            {
                for (int j = 0; j < map.Size.x; j++)
                {
                    IntVec3 intVec = new IntVec3(j, 0, i);
                    Region region = intVec.GetRegion(map, RegionType.Set_Passable);
                    if (region != null && InfestationLikeCellFinder.NoRoofAroundAndWalkable(intVec, map))
                    {
                        InfestationLikeCellFinder.tempUnroofedRegions.Add(region);
                    }
                }
            }
            Dijkstra<Region>.Run(InfestationLikeCellFinder.tempUnroofedRegions, (Region x) => x.Neighbors, (Region a, Region b) => Mathf.Sqrt((float)a.extentsClose.CenterCell.DistanceToSquared(b.extentsClose.CenterCell)), InfestationLikeCellFinder.regionsDistanceToUnroofed, null);
            InfestationLikeCellFinder.tempUnroofedRegions.Clear();
        }

        // Token: 0x06003695 RID: 13973 RVA: 0x001A1738 File Offset: 0x0019FB38
        private static void CalculateClosedAreaSizeGrid(Map map)
        {
            if (InfestationLikeCellFinder.closedAreaSize == null)
            {
                InfestationLikeCellFinder.closedAreaSize = new ByteGrid(map);
            }
            else
            {
                InfestationLikeCellFinder.closedAreaSize.ClearAndResizeTo(map);
            }
            for (int i = 0; i < map.Size.z; i++)
            {
                for (int j = 0; j < map.Size.x; j++)
                {
                    IntVec3 intVec = new IntVec3(j, 0, i);
                    if (InfestationLikeCellFinder.closedAreaSize[j, i] == 0 && !intVec.Impassable(map))
                    {
                        int area = 0;
                        map.floodFiller.FloodFill(intVec, (IntVec3 c) => !c.Impassable(map), delegate (IntVec3 c)
                        {
                            area++;
                        }, int.MaxValue, false, null);
                        area = Mathf.Min(area, 255);
                        map.floodFiller.FloodFill(intVec, (IntVec3 c) => !c.Impassable(map), delegate (IntVec3 c)
                        {
                            InfestationLikeCellFinder.closedAreaSize[c] = (byte)area;
                        }, int.MaxValue, false, null);
                    }
                }
            }
        }

        // Token: 0x06003696 RID: 13974 RVA: 0x001A188C File Offset: 0x0019FC8C
        private static void CalculateDistanceToColonyBuildingGrid(Map map)
        {
            if (InfestationLikeCellFinder.distToColonyBuilding == null)
            {
                InfestationLikeCellFinder.distToColonyBuilding = new ByteGrid(map);
            }
            else if (!InfestationLikeCellFinder.distToColonyBuilding.MapSizeMatches(map))
            {
                InfestationLikeCellFinder.distToColonyBuilding.ClearAndResizeTo(map);
            }
            InfestationLikeCellFinder.distToColonyBuilding.Clear(byte.MaxValue);
            InfestationLikeCellFinder.tmpColonyBuildingsLocs.Clear();
            List<Building> allBuildingsColonist = map.listerBuildings.allBuildingsColonist;
            for (int i = 0; i < allBuildingsColonist.Count; i++)
            {
                InfestationLikeCellFinder.tmpColonyBuildingsLocs.Add(allBuildingsColonist[i].Position);
            }
            Dijkstra<IntVec3>.Run(InfestationLikeCellFinder.tmpColonyBuildingsLocs, (IntVec3 x) => DijkstraUtility.AdjacentCellsNeighborsGetter(x, map), delegate (IntVec3 a, IntVec3 b)
            {
                if (a.x == b.x || a.z == b.z)
                {
                    return 1f;
                }
                return 1.41421354f;
            }, InfestationLikeCellFinder.tmpDistanceResult, null);
            for (int j = 0; j < InfestationLikeCellFinder.tmpDistanceResult.Count; j++)
            {
                InfestationLikeCellFinder.distToColonyBuilding[InfestationLikeCellFinder.tmpDistanceResult[j].Key] = (byte)Mathf.Min(InfestationLikeCellFinder.tmpDistanceResult[j].Value, 254.999f);
            }
        }

        // Token: 0x04001DE2 RID: 7650
        private static List<InfestationLikeCellFinder.LocationCandidate> locationCandidates = new List<InfestationLikeCellFinder.LocationCandidate>();

        // Token: 0x04001DE3 RID: 7651
        private static Dictionary<Region, float> regionsDistanceToUnroofed = new Dictionary<Region, float>();

        // Token: 0x04001DE4 RID: 7652
        private static ByteGrid closedAreaSize;

        // Token: 0x04001DE5 RID: 7653
        private static ByteGrid distToColonyBuilding;

        // Token: 0x04001DE6 RID: 7654
        private const float MinRequiredScore = 7.5f;

        // Token: 0x04001DE7 RID: 7655
        private const float MinMountainousnessScore = 0.17f;

        // Token: 0x04001DE8 RID: 7656
        private const int MountainousnessScoreRadialPatternIdx = 700;

        // Token: 0x04001DE9 RID: 7657
        private const int MountainousnessScoreRadialPatternSkip = 10;

        // Token: 0x04001DEA RID: 7658
        private const float MountainousnessScorePerRock = 1f;

        // Token: 0x04001DEB RID: 7659
        private const float MountainousnessScorePerThickRoof = 0.5f;

        // Token: 0x04001DEC RID: 7660
        private const float MinCellTempToSpawnHive = -17f;

        // Token: 0x04001DED RID: 7661
        private const float MaxDistanceToColonyBuilding = 30f;

        // Token: 0x04001DEE RID: 7662
    //    private static List<Pair<IntVec3, float>> tmpCachedInfestationChanceCellColors;

        // Token: 0x04001DEF RID: 7663
        private static HashSet<Region> tempUnroofedRegions = new HashSet<Region>();

        // Token: 0x04001DF0 RID: 7664
        private static List<IntVec3> tmpColonyBuildingsLocs = new List<IntVec3>();

        // Token: 0x04001DF1 RID: 7665
        private static List<KeyValuePair<IntVec3, float>> tmpDistanceResult = new List<KeyValuePair<IntVec3, float>>();

        // Token: 0x0200092E RID: 2350
        private struct LocationCandidate
        {
            // Token: 0x0600369C RID: 13980 RVA: 0x001A1A6D File Offset: 0x0019FE6D
            public LocationCandidate(IntVec3 cell, float score)
            {
                this.cell = cell;
                this.score = score;
            }

            // Token: 0x04001DF6 RID: 7670
            public IntVec3 cell;

            // Token: 0x04001DF7 RID: 7671
            public float score;
        }
    }
}
