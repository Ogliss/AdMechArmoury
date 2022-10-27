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
    public enum ReserveDeploymentType
    {
        DropPod,
        DropPara,
        DropShip,
        Fly,
        Infiltrate,
        Teleport,
        Tunnel
    }
    public static class DeepStrikeUtility
    {
        public static bool TryGenerateStrikeInfo(IncidentParms parms, out List<Pawn> pawns, bool debugTest = false)
        {
            pawns = null;
            ResolveStrikePoints(parms);
            Log.Message($"DeepStrikeUtility TryGenerateStrikeInfo Post ResolveStrikePoints");
            if (!TryResolveStrikeFaction(parms))
            {
                return false;
            }
            Log.Message($"DeepStrikeUtility TryGenerateStrikeInfo Post TryResolveStrikeFaction");
            PawnGroupKindDef combat = PawnGroupKindDefOf.Combat;
            ResolveStrikeStrategy(parms, combat);
            Log.Message($"DeepStrikeUtility TryGenerateStrikeInfo Post ResolveStrikeStrategy");
            if (!debugTest)
            {
                parms.raidStrategy.Worker.TryGenerateThreats(parms);
            }
            Log.Message($"DeepStrikeUtility TryGenerateStrikeInfo Post TryGenerateThreats");
            if (!debugTest && !parms.raidArrivalMode.Worker.TryResolveRaidSpawnCenter(parms))
            {
                return false;
            }
            Log.Message($"DeepStrikeUtility TryGenerateStrikeInfo Post TryResolveRaidSpawnCenter");
            float points = parms.points;
            parms.points = AdjustedStrikePoints(parms.points, parms.raidArrivalMode, parms.raidStrategy, parms.faction, combat);
            Log.Message($"DeepStrikeUtility TryGenerateStrikeInfo Post AdjustedStrikePoints");
            if (!debugTest)
            {
                pawns = parms.raidStrategy.Worker.SpawnThreats(parms);
            }
            Log.Message($"DeepStrikeUtility TryGenerateStrikeInfo Post SpawnThreats");
            if (pawns == null)
            {
                PawnGroupMakerParms defaultPawnGroupMakerParms = IncidentParmsUtility.GetDefaultPawnGroupMakerParms(combat, parms);
                Log.Message($"DeepStrikeUtility TryGenerateStrikeInfo Post defaultPawnGroupMakerParms");
                pawns = PawnGroupMakerUtility.GeneratePawns(defaultPawnGroupMakerParms).ToList();
                Log.Message($"DeepStrikeUtility TryGenerateStrikeInfo Post pawns");
                if (pawns.Count == 0)
                {
                    Log.Error("Got no pawns spawning raid from parms " + parms);
                    return false;
                }
                if (!debugTest)
                {
                //    parms.raidArrivalMode.Worker.Arrive(pawns, parms);
                }
                Log.Message($"DeepStrikeUtility TryGenerateStrikeInfo Post Arrive");
            }
            Log.Message($"DeepStrikeUtility TryGenerateStrikeInfo Post pawns == null");
            if (debugTest)
            {
                parms.target.StoryState.lastRaidFaction = parms.faction;
            }
            else
            {
                GenerateStrikeLoot(parms, points, pawns);
            }
            Log.Message($"DeepStrikeUtility TryGenerateStrikeInfo END");
            return true;
        }

        static bool TryResolveStrikeFaction(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            return (parms.faction != null && parms.faction.HostileTo(Faction.OfPlayer)) || PawnGroupMakerUtility.TryGetRandomFactionForCombatPawnGroupWeighted(parms, out parms.faction, (Faction f) => FactionCanBeGroupSource(f, map, false), true, true, true, true) || PawnGroupMakerUtility.TryGetRandomFactionForCombatPawnGroupWeighted(parms, out parms.faction, (Faction f) => FactionCanBeGroupSource(f, map, true), true, true, true, true);
        }
        static bool FactionCanBeGroupSource(Faction f, Map map, bool desperate = false)
        {
            if (f.IsPlayer)
            {
                return false;
            }
            if (f.defeated)
            {
                return false;
            }
            if (f.temporary)
            {
                return false;
            }
            if (!desperate && (!f.def.allowedArrivalTemperatureRange.Includes(map.mapTemperature.OutdoorTemp) || !f.def.allowedArrivalTemperatureRange.Includes(map.mapTemperature.SeasonalTemp)))
            {
                return false;
            }
            if (!desperate)
            {
                return (float)GenDate.DaysPassedSinceSettle >= f.def.earliestRaidDays;
            }
            return true;
        }

        static void ResolveStrikePoints(IncidentParms parms)
        {
            if (parms.points <= 0f)
            {
                Log.Error("RaidEnemy is resolving raid points. They should always be set before initiating the incident.");
                parms.points = StorytellerUtility.DefaultThreatPointsNow(parms.target);
            }
        }

        static void ResolveStrikeStrategy(IncidentParms parms, PawnGroupKindDef groupKind)
        {
            if (parms.raidStrategy == null)
            {
                Map map = (Map)parms.target;
                DefDatabase<RaidStrategyDef>.AllDefs.Where((RaidStrategyDef d) => d.Worker.CanUseWith(parms, groupKind) && (parms.raidArrivalMode != null || (d.arriveModes != null && d.arriveModes.Any((PawnsArrivalModeDef x) => x.Worker.CanUseWith(parms))))).TryRandomElementByWeight((RaidStrategyDef d) => d.Worker.SelectionWeightForFaction(map, parms.faction, parms.points), out RaidStrategyDef result);
                parms.raidStrategy = result;
                if (parms.raidStrategy == null)
                {
                    Log.Error("No raid stategy found, defaulting to ImmediateAttack. Faction=" + parms.faction.def.defName + ", points=" + parms.points + ", groupKind=" + groupKind + ", parms=" + parms);
                    parms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
                }
            }
        }

        static float AdjustedStrikePoints(float points, PawnsArrivalModeDef raidArrivalMode, RaidStrategyDef raidStrategy, Faction faction, PawnGroupKindDef groupKind)
        {
            if (raidArrivalMode.pointsFactorCurve != null)
            {
                points *= raidArrivalMode.pointsFactorCurve.Evaluate(points);
            }
            if (raidStrategy.pointsFactorCurve != null)
            {
                points *= raidStrategy.pointsFactorCurve.Evaluate(points);
            }
            points = Mathf.Max(points, raidStrategy.Worker.MinimumPoints(faction, groupKind) * 1.05f);
            return points;
        }
        static void GenerateStrikeLoot(IncidentParms parms, float raidLootPoints, List<Pawn> pawns)
        {
            if (parms.faction.def.raidLootMaker == null || !pawns.Any<Pawn>())
            {
                return;
            }
            raidLootPoints *= Find.Storyteller.difficulty.EffectiveRaidLootPointsFactor;
            float num = parms.faction.def.raidLootValueFromPointsCurve.Evaluate(raidLootPoints);
            if (parms.raidStrategy != null)
            {
                num *= parms.raidStrategy.raidLootValueFactor;
            }
            ThingSetMakerParams parms2 = default(ThingSetMakerParams);
            parms2.totalMarketValueRange = new FloatRange?(new FloatRange(num, num));
            parms2.makingFaction = parms.faction;
            List<Thing> loot = parms.faction.def.raidLootMaker.root.Generate(parms2);
            new RaidLootDistributor(parms, pawns, loot).DistributeLoot();
        }


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
        public static void DropThingsNear(IntVec3 dropCenter, Map map, IEnumerable<Thing> things, int openDelay = 110, bool canInstaDropDuringInit = false, bool leaveSlag = false, bool canRoofPunch = true, ReserveDeploymentType strikeType = ReserveDeploymentType.DropPod, bool scatters = true)
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

        public static void DropThingGroupsNear(IntVec3 dropCenter, Map map, List<List<Thing>> thingsGroups, int openDelay = 110, bool instaDrop = false, bool leaveSlag = false, bool canRoofPunch = true, ReserveDeploymentType strikeType = ReserveDeploymentType.DropPod, bool scatters = true)
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
                        }));
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
                        case ReserveDeploymentType.Fly:
                            DeepStrikeUtility.MakeFlyerLandAt(intVec, map, activeDropPodInfo, extension);
                            break;
                        case ReserveDeploymentType.Teleport:
                            DeepStrikeUtility.MakeTeleportAt(intVec, map, activeDropPodInfo, extension);
                            break;
                        case ReserveDeploymentType.Tunnel:
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

        public static string DeepstrikeArrivalmode(ReserveDeploymentType pawnsArrivalMode)
        {
            switch (pawnsArrivalMode)
            {
                case ReserveDeploymentType.DropPara:
                    return "AdeptusMechanicus.ReserveDeploymentType_DropPara".Translate() + " - NOT YET IMPLEMENTED";
                case ReserveDeploymentType.Fly:
                    return "AdeptusMechanicus.ReserveDeploymentType_Flight".Translate();
                case ReserveDeploymentType.Teleport:
                    return "AdeptusMechanicus.ReserveDeploymentType_Teleport".Translate();
                case ReserveDeploymentType.Tunnel:
                    return "AdeptusMechanicus.ReserveDeploymentType_Tunnel".Translate();
                case ReserveDeploymentType.DropShip:
                    return "AdeptusMechanicus.ReserveDeploymentType_DropShip".Translate() + " - NOT YET IMPLEMENTED";
                case ReserveDeploymentType.Infiltrate:
                    return "AdeptusMechanicus.ReserveDeploymentType_Infiltrate".Translate();
                default:
                    return "AdeptusMechanicus.ReserveDeploymentType_DropPod".Translate();
            }
        }

        private static List<List<Thing>> tempList = new List<List<Thing>>();

    }
}
