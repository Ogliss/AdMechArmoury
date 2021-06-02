using AdeptusMechanicus.ExtensionMethods;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
    public enum DeepStrikeType
    {
        DropPod,
        DropPara,
        DropShip,
        Fly,
        Teleport,
        Tunnel,
    }
    public static class DeepStrikeUtility
    {
        public static void DropThingGroupsNear_NewTmp(IntVec3 dropCenter, Map map, List<List<Thing>> thingsGroups, int openDelay = 110, bool instaDrop = false, bool leaveSlag = false, bool canRoofPunch = true, bool forbid = true, bool allowFogged = true)
        {
            foreach (List<Thing> thingsGroup in thingsGroups)
            {
                if (!DropCellFinder.TryFindDropSpotNear(dropCenter, map, out IntVec3 result, allowFogged, canRoofPunch) && (canRoofPunch || !DropCellFinder.TryFindDropSpotNear(dropCenter, map, out result, allowFogged, canRoofPunch: true)))
                {
                    Log.Warning("DropThingsNear failed to find a place to drop " + thingsGroup.FirstOrDefault() + " near " + dropCenter + ". Dropping on random square instead.");
                    result = CellFinderLoose.RandomCellWith((IntVec3 c) => c.Walkable(map), map);
                }
                if (forbid)
                {
                    for (int i = 0; i < thingsGroup.Count; i++)
                    {
                        thingsGroup[i].SetForbidden(value: true, warnOnFail: false);
                    }
                }
                if (instaDrop)
                {
                    foreach (Thing item in thingsGroup)
                    {
                        GenPlace.TryPlaceThing(item, result, map, ThingPlaceMode.Near);
                    }
                }
                else
                {
                    ActiveDropPodInfo activeDropPodInfo = new ActiveDropPodInfo();
                    foreach (Thing item2 in thingsGroup)
                    {
                        activeDropPodInfo.innerContainer.TryAdd(item2);
                    }
                    activeDropPodInfo.openDelay = openDelay;
                    activeDropPodInfo.leaveSlag = leaveSlag;
                    MakeDropPodAt(result, map, activeDropPodInfo);
                }
            }
        }
        public static void DropThingsNear(IntVec3 dropCenter, Map map, IEnumerable<Thing> things, int openDelay = 110, bool canInstaDropDuringInit = false, bool leaveSlag = false, bool canRoofPunch = true, DeepStrikeType strikeType = DeepStrikeType.DropPod, bool scatters = true)
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
            DeepStrikeUtility.DropThingGroupsNear(dropCenter, map, DeepStrikeUtility.tempList, openDelay, canInstaDropDuringInit, leaveSlag, canRoofPunch, strikeType, scatters);
            DeepStrikeUtility.tempList.Clear();
        }

        public static void DropThingGroupsNear(IntVec3 dropCenter, Map map, List<List<Thing>> thingsGroups, int openDelay = 110, bool instaDrop = false, bool leaveSlag = false, bool canRoofPunch = true, DeepStrikeType strikeType = DeepStrikeType.DropPod, bool scatters = true)
        {
            foreach (List<Thing> list in thingsGroups)
            {
                List<Thing> list2 = list.Where(x => x.def.thingClass == typeof(Pawn) && (x.Faction != null && x.Faction.def.HasModExtension<FactionDefExtension>())).ToList();
                FactionDefExtension extension = list2.NullOrEmpty() ? null : list2.RandomElement().Faction.def.GetModExtensionFast<FactionDefExtension>();
                if (!DropCellFinder.TryFindDropSpotNear(dropCenter, map, out IntVec3 intVec, true, canRoofPunch) || !scatters)
                {
                    if (scatters)
                    {
                        Log.Warning(string.Concat(new object[]
                        {
                        "DropThingsNear failed to find a place to drop ",
                        list.FirstOrDefault<Thing>(),
                        " near ",
                        dropCenter,
                        ". Dropping on random square instead."
                        }), false);
                    }
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

                    switch (strikeType)
                    {
                        /*
                        case DeepStrikeType.DropPara:
                            DeepStrikeUtility.MakeDropParaAt(intVec, map, activeDropPodInfo, extension);
                            break;
                        case DeepStrikeType.DropShip:
                            DeepStrikeUtility.MakeDropShipLandAt(intVec, map, activeDropPodInfo, extension);
                            break;
                        */
                        case DeepStrikeType.Fly:
                            DeepStrikeUtility.MakeFlyerLandAt(intVec, map, activeDropPodInfo, extension);
                            break;
                        case DeepStrikeType.Teleport:
                            DeepStrikeUtility.MakeTeleportAt(intVec, map, activeDropPodInfo, extension);
                            break;
                        case DeepStrikeType.Tunnel:
                            DeepStrikeUtility.MakeTunnelAt(intVec, map, activeDropPodInfo, extension);
                            break;
                        default:
                            DeepStrikeUtility.MakeDropPodAt(intVec, map, activeDropPodInfo, extension);
                            break;
                    }
                }
            }
        }

        public static void MakeDropPodAt(IntVec3 c, Map map, ActiveDropPodInfo info, FactionDefExtension extension = null)
        {
            ThingDef ActiveDropPod = ThingDefOf.ActiveDropPod;
            ThingDef DropPodIncoming = ThingDefOf.DropPodIncoming;
            if (extension != null)
            {
                if (extension.ActiveDropPod != null)
                {
                    ActiveDropPod = extension.ActiveDropPod;
                }
                if (extension.DropPodIncoming != null)
                {
                    DropPodIncoming = extension.DropPodIncoming;
                }
            }
            ActiveDropPod activeDropPod = (ActiveDropPod)ThingMaker.MakeThing(ActiveDropPod, null);
            activeDropPod.Contents = info;
            SkyfallerMaker.SpawnSkyfaller(DropPodIncoming, activeDropPod, c, map);
        }

        public static void MakeFlyerLandAt(IntVec3 c, Map map, ActiveDropPodInfo info, FactionDefExtension extension = null)
        {
            ThingDef ActiveDropPod = DefDatabase<ThingDef>.GetNamed("OG_Active_DeepStrike_Flyer");
            ThingDef DropPodIncoming = DefDatabase<ThingDef>.GetNamed("OG_DeepStrike_Flyer_Incoming");
            ActiveFlyer activeDropPod = (ActiveFlyer)ThingMaker.MakeThing(ActiveDropPod, null);
            activeDropPod.Contents = info;
            activeDropPod.Contents.leaveSlag = false;
            SkyfallerMaker.SpawnSkyfaller(DropPodIncoming, activeDropPod, c, map);
        }

        public static void MakeTunnelAt(IntVec3 c, Map map, ActiveDropPodInfo info, FactionDefExtension extension = null)
        {
            ThingDef TunnelDef = DefDatabase<ThingDef>.GetNamed("OG_AMA_Tunneler");
            TunnelSpawner tunnelSpawner = (TunnelSpawner)ThingMaker.MakeThing(TunnelDef, null);
            foreach (Thing item in info.innerContainer)
            {
                tunnelSpawner.GetDirectlyHeldThings().TryAddOrTransfer(item, false);
            }
            GenSpawn.Spawn(tunnelSpawner, c, map);
        }

        public static void MakeTeleportAt(IntVec3 c, Map map, ActiveDropPodInfo info, FactionDefExtension extension = null)
        {
            /*
            WeatherEvent @event = new WeatherEvent_DeepStrike_Teleport(map, c, boltstring: (extension !=null ? extension.TeleportBoltTexPath : ""));
            map.weatherManager.eventHandler.AddEvent(@event);
            */
            ThingDef TunnelDef = DefDatabase<ThingDef>.GetNamed("OG_AMA_Teleporter");
            TeleportSpawner teleportSpawner = (TeleportSpawner)ThingMaker.MakeThing(TunnelDef, null);
            foreach (Thing item in info.innerContainer)
            {
                teleportSpawner.GetDirectlyHeldThings().TryAddOrTransfer(item, false);
            }
            teleportSpawner.extFaction = extension;
            GenSpawn.Spawn(teleportSpawner, c, map);
        }

        public static string DeepstrikeArrivalmode(DeepStrikeType pawnsArrivalMode)
        {
            switch (pawnsArrivalMode)
            {
                case DeepStrikeType.DropPara:
                    return "AdeptusMechanicus.DeepStrikeType_DropPara".Translate() + " - NOT YET IMPLEMENTED";
                case DeepStrikeType.Fly:
                    return "AdeptusMechanicus.DeepStrikeType_Flight".Translate();
                case DeepStrikeType.Teleport:
                    return "AdeptusMechanicus.DeepStrikeType_Teleport".Translate();
                case DeepStrikeType.Tunnel:
                    return "AdeptusMechanicus.DeepStrikeType_Tunnel".Translate();
                case DeepStrikeType.DropShip:
                    return "AdeptusMechanicus.DeepStrikeType_DropShip".Translate() + " - NOT YET IMPLEMENTED";
                default:
                    return "AdeptusMechanicus.DeepStrikeType_DropPod".Translate();
            }
        }

        // Token: 0x04001640 RID: 5696
        private static List<List<Thing>> tempList = new List<List<Thing>>();
    }
}
