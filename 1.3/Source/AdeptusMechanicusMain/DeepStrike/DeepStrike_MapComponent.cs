using System;
using System.Collections.Generic;
using AdeptusMechanicus.ExtensionMethods;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x02000067 RID: 103
    public class MapComponent_DeepStrike : MapComponent
    {
        public MapComponent_DeepStrike(Map map) : base(map)
        {
            this.map = map;
            this.Strikes = new List<DeepStrike>();

        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();
            if (Strikes.Count > 0)
            {
                foreach (DeepStrike item in Strikes)
                {
                    if (!item.StrikeNow)
                    {
                        item.delay--;
                    }
                    else
                    {
                        DoStrike(item);
                        strikeLastTick = Find.TickManager.TicksGame;
                    }
                }
                Strikes.RemoveAll(x => x.struck);
            }
            else
            {
                raidLastTick = -1;
                ticksSinceRaid = -1;
                strikeDelay = -1;
            }
        }

        public void DoStrike(DeepStrike strike)
        {
            string str = string.Empty;
            foreach (var item in strike.Members)
            {
                switch (item.DeepStrike().pawnsArrivalMode)
                {
                    case DeepStrikeType.DropPod:
                        Droppers.Add(item);
                        break;
                    case DeepStrikeType.DropPara:
                        break;
                    case DeepStrikeType.DropShip:
                        break;
                    case DeepStrikeType.Fly:
                        Flyers.Add(item);
                        break;
                    case DeepStrikeType.Teleport:
                        Teleporters.Add(item);
                        break;
                    case DeepStrikeType.Tunnel:
                        Tunnellers.Add(item);
                        break;
                    default:
                        break;
                }
            }
            List<Pawn> pawns = new List<Pawn>();
            if (!Droppers.NullOrEmpty())
            {
                str += str == string.Empty ? DeepStrikeUtility.DeepstrikeArrivalmode(DeepStrikeType.DropPod) : ", "+ DeepStrikeUtility.DeepstrikeArrivalmode(DeepStrikeType.DropPod);
                pawns.AddRange(Droppers);
                ArriveDropPod(Droppers);
                Droppers.Clear();
            }
            if (!Flyers.NullOrEmpty())
            {
                str += str == string.Empty ? DeepStrikeUtility.DeepstrikeArrivalmode(DeepStrikeType.Fly) : ", " + DeepStrikeUtility.DeepstrikeArrivalmode(DeepStrikeType.Fly);
                pawns.AddRange(Flyers);
                ArriveFly(Flyers);
                Flyers.Clear();
            }
            if (!Teleporters.NullOrEmpty())
            {
                str += str == string.Empty ? DeepStrikeUtility.DeepstrikeArrivalmode(DeepStrikeType.Teleport) : ", " + DeepStrikeUtility.DeepstrikeArrivalmode(DeepStrikeType.Teleport);
                pawns.AddRange(Teleporters);
                ArriveTeleport(Teleporters);
                Teleporters.Clear();
            }
            if (!Tunnellers.NullOrEmpty())
            {
                str += str == string.Empty ? DeepStrikeUtility.DeepstrikeArrivalmode(DeepStrikeType.Tunnel) : ", " + DeepStrikeUtility.DeepstrikeArrivalmode(DeepStrikeType.Tunnel);
                pawns.AddRange(Tunnellers);
                ArriveTunnel(Tunnellers);
                Tunnellers.Clear();
            }
            Find.LetterStack.ReceiveLetter("AdeptusMechanicus.DeepStrike_Incomming".Translate(pawns.Find(x => x.Faction != null).Faction.def.pawnSingular), "AdeptusMechanicus.DeepStrike_Incomming_Letter".Translate(pawns.Find(x => x.Faction != null).Faction.def.pawnsPlural, str), LetterDefOf.ThreatBig, pawns, pawns.Find(x => x.Faction != null).Faction, null);
            strike.struck = true;
        }

        public void ArriveDropPod(List<Pawn> pawns)
        {
            for (int i = 0; i < pawns.Count; i++)
            {
                //    IntVec3 dropCenter = map.listerBuildings.allBuildingsColonist.FindAll(x => x.TryGetCompFast<CompPowerPlant>() != null).RandomElement().Position;
                IntVec3 dropCenter;
                if (DropCellFinder.TryFindRaidDropCenterClose(out dropCenter, map))
                {
                    DeepStrikeUtility.DropThingsNear(dropCenter, map, Gen.YieldSingle<Thing>(pawns[i]), 50, false, false, true, DeepStrikeType.DropPod);
                }
            }
            Droppers.Clear();
        }
        public void ArriveFly(List<Pawn> pawns)
        {
            for (int i = 0; i < pawns.Count; i++)
            {
                //    IntVec3 dropCenter = map.listerBuildings.allBuildingsColonist.FindAll(x => x.TryGetCompFast<CompPowerPlant>() != null).RandomElement().Position;
                IntVec3 dropCenter;
                if (DropCellFinder.TryFindRaidDropCenterClose(out dropCenter, map))
                {
                    DeepStrikeUtility.DropThingsNear(dropCenter, map, Gen.YieldSingle<Thing>(pawns[i]), 0, false, false, true, DeepStrikeType.Fly);
                }
            }
            Flyers.Clear();
        }

        public void ArriveTunnel(List<Pawn> pawns)
        {
            for (int i = 0; i < pawns.Count; i++)
            {
                //    IntVec3 dropCenter = map.listerBuildings.allBuildingsColonist.FindAll(x => x.TryGetCompFast<CompPowerPlant>() != null).RandomElement().Position;
                IntVec3 dropCenter;
                if (DropCellFinder.TryFindRaidDropCenterClose(out dropCenter, map))
                {
                    DeepStrikeUtility.DropThingsNear(dropCenter, map, Gen.YieldSingle<Thing>(pawns[i]), 50, false, false, true, DeepStrikeType.Tunnel);
                }
            }
            Tunnellers.Clear();
        }

        public void ArriveTeleport(List<Pawn> pawns)
        {
            for (int i = 0; i < pawns.Count; i++)
            {
                //    IntVec3 dropCenter = map.listerBuildings.allBuildingsColonist.FindAll(x => x.TryGetCompFast<CompPowerPlant>() != null).RandomElement().Position;
                IntVec3 dropCenter;
                if (DropCellFinder.TryFindRaidDropCenterClose(out dropCenter, map))
                {
                    DeepStrikeUtility.DropThingsNear(dropCenter, map, Gen.YieldSingle<Thing>(pawns[i]), 0, true, false, true, DeepStrikeType.Teleport);
                }
            }
            Teleporters.Clear();
        }

        public int ticksSinceRaid = -1;
        public int TicksSinceRaid
        {
            get
            {
                if (raidLastTick != -1)
                {
                    return Find.TickManager.TicksGame - raidLastTick;
                }
                return ticksSinceRaid;
            }
        }

        public override void ExposeData()
        {

            base.ExposeData();
            Scribe_Values.Look<int>(ref this.strikeLastTick, "strikeLastTick", -1, false);
            Scribe_Values.Look<int>(ref this.strikeDelay, "strikeDelay", -1, false);
            Scribe_Values.Look<int>(ref this.raidLastTick, "ticksLastRaid", -1, false);
            Scribe_Collections.Look(ref this.Strikes, "StrikesGroups", LookMode.Deep);
        }

        public List<DeepStrike> Strikes;

        private List<Pawn> Droppers = new List<Pawn>();
        private List<Pawn> Flyers = new List<Pawn>();
        private List<Pawn> Teleporters = new List<Pawn>();
        private List<Pawn> Tunnellers = new List<Pawn>();
        private readonly IntRange ResultSpawnDelay = new IntRange(500, 1000);
        public int strikeMaxDelay = 24000;
        public int strikeMinDelay = 6000;
        public int strikeDelay = -1;
        private int strikeLastTick = -1;
        public int raidLastTick = -1;
    }


}
