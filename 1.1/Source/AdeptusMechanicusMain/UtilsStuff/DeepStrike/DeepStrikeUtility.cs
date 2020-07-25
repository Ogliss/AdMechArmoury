using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
    // Token: 0x0200070C RID: 1804
    public static class DeepStrikeUtility
    {
        public static string DeepstrikeArrivalmode (DeepStrikeType pawnsArrivalMode)
        {
            string str = string.Empty;
            if (pawnsArrivalMode == DeepStrikeType.Drop)
            {
                str = "AMA_DeepStrikeType_Drop".Translate();
            }
            if (pawnsArrivalMode == DeepStrikeType.Fly)
            {
                str = "AMA_DeepStrikeType_Flight".Translate();
            }
            if (pawnsArrivalMode == DeepStrikeType.Tunnel)
            {
                str = "AMA_DeepStrikeType_Tunnel".Translate();
            }
            if (pawnsArrivalMode == DeepStrikeType.Teleport)
            {
                str = "AMA_DeepStrikeType_Teleport".Translate();
            }
            return str;
        }

        // Token: 0x06002762 RID: 10082 RVA: 0x0012C458 File Offset: 0x0012A858
        public static void MakeDropPodAt(IntVec3 c, Map map, ActiveDropPodInfo info, FactionDefExtension extension)
        {
            ThingDef ActiveDropPod = ThingDefOf.ActiveDropPod;
            ThingDef DropPodIncoming = ThingDefOf.DropPodIncoming;
            if (extension!=null)
            {
                if (extension.ActiveDropPod != null)
                {
                //    Log.Message("MakeDropPodAt ActiveDropPod " + extension.ActiveDropPod);
                    ActiveDropPod = extension.ActiveDropPod;
                }
                if (extension.DropPodIncoming != null)
                {
                //    Log.Message("MakeDropPodAt DropPodIncoming " + extension.DropPodIncoming);
                    DropPodIncoming = extension.DropPodIncoming;
                }
            }
        //    Log.Message("MakeDropPodAt 3");
            ActiveDropPod activeDropPod = (ActiveDropPod)ThingMaker.MakeThing(ActiveDropPod, null);
       //     Log.Message("MakeDropPodAt 4");
            activeDropPod.Contents = info;
        //    Log.Message("MakeDropPodAt 5");
            SkyfallerMaker.SpawnSkyfaller(DropPodIncoming, activeDropPod, c, map);
        //    Log.Message("MakeDropPodAt 6");
        }
        // Token: 0x06002762 RID: 10082 RVA: 0x0012C458 File Offset: 0x0012A858

        public static void MakeFlyerLandAt(IntVec3 c, Map map, ActiveDropPodInfo info, FactionDefExtension extension)
        {
            ThingDef ActiveDropPod = DefDatabase<ThingDef>.GetNamed("OG_Active_DeepStrike_Flyer");
            ThingDef DropPodIncoming = DefDatabase<ThingDef>.GetNamed("OG_DeepStrike_Flyer_Incoming");
            ActiveFlyer activeDropPod = (ActiveFlyer)ThingMaker.MakeThing(ActiveDropPod, null);
            activeDropPod.Contents = info;
            activeDropPod.Contents.leaveSlag = false;
            SkyfallerMaker.SpawnSkyfaller(DropPodIncoming, activeDropPod, c, map);
        }

        // Token: 0x06002762 RID: 10082 RVA: 0x0012C458 File Offset: 0x0012A858
        public static void MakeTunnelAt(IntVec3 c, Map map, ActiveDropPodInfo info, FactionDefExtension extension)
        {
            ThingDef TunnelDef = DefDatabase<ThingDef>.GetNamed("OG_AMA_Tunneler");
            //    Log.Message(string.Format("making tunnelSpawner: {0}, @: {1}, {2}, {3}", TunnelDef, c, map, info.innerContainer.ContentsString));
            TunnelSpawner tunnelSpawner = (TunnelSpawner)ThingMaker.MakeThing(TunnelDef, null);
            foreach (Thing item in info.innerContainer)
            {
                tunnelSpawner.GetDirectlyHeldThings().TryAddOrTransfer(item, false);
            }
            GenSpawn.Spawn(tunnelSpawner, c, map);
            //--    SkyfallerMaker.SpawnSkyfaller(ThingDefOf.DropPodIncoming, tunnelSpawner, c, map);
        }

        // Token: 0x06002762 RID: 10082 RVA: 0x0012C458 File Offset: 0x0012A858
        public static void MakeTeleportAt(IntVec3 c, Map map, ActiveDropPodInfo info, FactionDefExtension extension)
        {
            WeatherEvent @event = new WeatherEvent_DeepStrike_Teleport(map, c, boltstring: (extension !=null ? extension.TeleportBoltTexPath : ""));
            map.weatherManager.eventHandler.AddEvent(@event);
            ThingDef TunnelDef = DefDatabase<ThingDef>.GetNamed("OG_AMA_Teleporter");
            //    Log.Message(string.Format("making teleportSpawner: {0}, @: {1}, {2}, {3}", TunnelDef, c, map, info.innerContainer.ContentsString));
            TeleportSpawner teleportSpawner = (TeleportSpawner)ThingMaker.MakeThing(TunnelDef, null);
            foreach (Thing item in info.innerContainer)
            {
                teleportSpawner.GetDirectlyHeldThings().TryAddOrTransfer(item, false);
            }
            GenSpawn.Spawn(teleportSpawner, c, map);
            //--    SkyfallerMaker.SpawnSkyfaller(ThingDefOf.DropPodIncoming, tunnelSpawner, c, map);
        }

        // Token: 0x06002763 RID: 10083 RVA: 0x0012C48C File Offset: 0x0012A88C
        public static void DropThingsNear(IntVec3 dropCenter, Map map, IEnumerable<Thing> things, int openDelay = 110, bool canInstaDropDuringInit = false, bool leaveSlag = false, bool canRoofPunch = true, DeepStrikeType strikeType = DeepStrikeType.Drop)
        {
            DeepStrikeUtility.tempList.Clear();
            foreach (Thing item in things)
            {
                List<Thing> list = new List<Thing>
                {
                    item
                };
                DeepStrikeUtility.tempList.Add(list);
            }
            DeepStrikeUtility.DropThingGroupsNear(dropCenter, map, DeepStrikeUtility.tempList, openDelay, canInstaDropDuringInit, leaveSlag, canRoofPunch, strikeType);
            DeepStrikeUtility.tempList.Clear();
        }

        // Token: 0x06002764 RID: 10084 RVA: 0x0012C518 File Offset: 0x0012A918
        public static void DropThingGroupsNear(IntVec3 dropCenter, Map map, List<List<Thing>> thingsGroups, int openDelay = 110, bool instaDrop = false, bool leaveSlag = false, bool canRoofPunch = true, DeepStrikeType strikeType = DeepStrikeType.Drop)
        {
            foreach (List<Thing> list in thingsGroups)
            {
                List<Thing> list2 = list.Where(x => x.def.thingClass == typeof(Pawn) && (x.Faction != null && x.Faction.def.HasModExtension<FactionDefExtension>())).ToList();
                FactionDefExtension extension = list2.RandomElement().Faction.def.GetModExtension<FactionDefExtension>();
                IntVec3 intVec;
                if (!DropCellFinder.TryFindDropSpotNear(dropCenter, map, out intVec, true, canRoofPunch))
                {
                    Log.Warning(string.Concat(new object[]
                    {
                        "DropThingsNear failed to find a place to drop ",
                        list.FirstOrDefault<Thing>(),
                        " near ",
                        dropCenter,
                        ". Dropping on random square instead."
                    }), false);
                    intVec = CellFinderLoose.RandomCellWith((IntVec3 c) => c.Walkable(map) && (c.Roofed(map) && c.GetRoof(map) != RoofDefOf.RoofRockThick), map, 1000);
                }
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].SetForbidden(true, false);
                }
                if (instaDrop)
                {
                    foreach (Thing thing in list)
                    {
                        GenPlace.TryPlaceThing(thing, intVec, map, ThingPlaceMode.Near, null, null);
                    }
                }
                else
                {
                    ActiveDropPodInfo activeDropPodInfo = new ActiveDropPodInfo();
                    foreach (Thing item in list)
                    {
                        activeDropPodInfo.innerContainer.TryAddOrTransfer(item, true);
                    }
                    activeDropPodInfo.openDelay = openDelay;
                    activeDropPodInfo.leaveSlag = leaveSlag;
                    
                    if (strikeType == DeepStrikeType.Fly)
                    {
                        DeepStrikeUtility.MakeFlyerLandAt(intVec, map, activeDropPodInfo, extension);
                    }
                    else if (strikeType == DeepStrikeType.Tunnel)
                    {
                        DeepStrikeUtility.MakeTunnelAt(intVec, map, activeDropPodInfo, extension);
                    }
                    else if (strikeType == DeepStrikeType.Teleport)
                    {
                        DeepStrikeUtility.MakeTeleportAt(intVec, map, activeDropPodInfo, extension);
                    }
                    else
                    {
                        DeepStrikeUtility.MakeDropPodAt(intVec, map, activeDropPodInfo, extension);
                    }
                }
            }
        }

        // Token: 0x06002762 RID: 10082 RVA: 0x0012C458 File Offset: 0x0012A858
        public static void MakeTunnelAt(IntVec3 c, Map map, List<Thing> info)
        {
            ThingDef TunnelDef = DefDatabase<ThingDef>.GetNamed("OG_AMA_Tunneler");
            //    Log.Message(string.Format("making tunnel: {0}, @: {1}, {2}, {3}", TunnelDef, c, map, info));
            TunnelSpawner tunnelSpawner = (TunnelSpawner)ThingMaker.MakeThing(TunnelDef, null);
            foreach (Thing item in info)
            {
                tunnelSpawner.GetDirectlyHeldThings().TryAddOrTransfer(item, false);
            }
            GenSpawn.Spawn(tunnelSpawner, c, map);
            //--    SkyfallerMaker.SpawnSkyfaller(ThingDefOf.DropPodIncoming, tunnelSpawner, c, map);
        }

        /*
        // Token: 0x06002763 RID: 10083 RVA: 0x0012C48C File Offset: 0x0012A88C
        public static void TunnelThingsNear(IntVec3 dropCenter, Map map, IEnumerable<Thing> things, int openDelay = 110, bool canInstaDropDuringInit = false, bool leaveSlag = false, bool canRoofPunch = true)
        {
            DeepStrikeUtility.tempList.Clear();
            foreach (Thing item in things)
            {
                List<Thing> list = new List<Thing>
                {
                    item
                };
                DeepStrikeUtility.tempList.Add(list);
            }
            DeepStrikeUtility.TunnelThingGroupsNear(dropCenter, map, DeepStrikeUtility.tempList, openDelay, canInstaDropDuringInit, leaveSlag, canRoofPunch);
            DeepStrikeUtility.tempList.Clear();
        }

        // Token: 0x06002764 RID: 10084 RVA: 0x0012C518 File Offset: 0x0012A918
        public static void TunnelThingGroupsNear(IntVec3 dropCenter, Map map, List<List<Thing>> thingsGroups, int openDelay = 110, bool instaDrop = false, bool leaveSlag = false, bool canRoofPunch = true)
        {
            foreach (List<Thing> list in thingsGroups)
            {
                IntVec3 intVec;
                if (!DropCellFinder.TryFindDropSpotNear(dropCenter, map, out intVec, true, canRoofPunch))
                {
                    Log.Warning(string.Concat(new object[]
                    {
                        "TunnelThingGroupsNear failed to find a place to drop ",
                        list.FirstOrDefault<Thing>(),
                        " near ",
                        dropCenter,
                        ". Tunneling on random square instead."
                    }), false);
                    intVec = CellFinderLoose.RandomCellWith((IntVec3 c) => c.Walkable(map), map, 1000);
                }
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].SetForbidden(true, false);
                }
                if (instaDrop)
                {
                    foreach (Thing thing in list)
                    {
                        GenPlace.TryPlaceThing(thing, intVec, map, ThingPlaceMode.Near, null, null);
                    }
                }
                else
                {
                    DeepStrikeUtility.MakeTunnelAt(intVec, map, list);
                }
            }
        }
        */
        // Token: 0x04001640 RID: 5696
        private static List<List<Thing>> tempList = new List<List<Thing>>();
    }
}
