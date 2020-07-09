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
            this.Strikes = new List<DeepStrikeEntry>();

        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();
            if (Strikes.Count > 0)
            {
                foreach (DeepStrikeEntry item in Strikes)
                {
                    if (item.delay>0)
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

        public void DoStrike(DeepStrikeEntry strike)
        {
            string str = string.Empty;
            Log.Message("0");
            foreach (var item in strike.members)
            {
                Log.Message("0 a");
                if (item.DeepStrike().pawnsArrivalMode == DeepStrikeType.Drop)
                {
                    Droppers.Add(item);
                }
                Log.Message("0 b");
                if (item.DeepStrike().pawnsArrivalMode == DeepStrikeType.Fly)
                {
                    Flyers.Add(item);
                }
                Log.Message("0 c");
                if (item.DeepStrike().pawnsArrivalMode == DeepStrikeType.Teleport)
                {
                    Teleporters.Add(item);
                }
                Log.Message("0 d");
                if (item.DeepStrike().pawnsArrivalMode == DeepStrikeType.Tunnel)
                {
                    Tunnellers.Add(item);
                }
                Log.Message("0 e");
            }
            Log.Message("1");
            List<Pawn> pawns = new List<Pawn>();
            Log.Message("2");
            if (!Droppers.NullOrEmpty())
            {
                Log.Message("2 a");
                str += str == string.Empty ? DeepStrikeUtility.DeepstrikeArrivalmode(DeepStrikeType.Drop) : ", "+ DeepStrikeUtility.DeepstrikeArrivalmode(DeepStrikeType.Drop);
                pawns.AddRange(Droppers);
                ArriveDropPod(Droppers);
                Droppers.Clear();
            }
            Log.Message("3");
            if (!Flyers.NullOrEmpty())
            {
                Log.Message("3 a");
                str += str == string.Empty ? DeepStrikeUtility.DeepstrikeArrivalmode(DeepStrikeType.Fly) : ", " + DeepStrikeUtility.DeepstrikeArrivalmode(DeepStrikeType.Fly);
                pawns.AddRange(Flyers);
                ArriveFly(Flyers);
                Flyers.Clear();
            }
            Log.Message("4");
            if (!Teleporters.NullOrEmpty())
            {
                Log.Message("4 a");
                str += str == string.Empty ? DeepStrikeUtility.DeepstrikeArrivalmode(DeepStrikeType.Teleport) : ", " + DeepStrikeUtility.DeepstrikeArrivalmode(DeepStrikeType.Teleport);
                pawns.AddRange(Teleporters);
                ArriveTeleport(Teleporters);
                Teleporters.Clear();
            }
            Log.Message("5");
            if (!Tunnellers.NullOrEmpty())
            {
                Log.Message("5 a");
                str += str == string.Empty ? DeepStrikeUtility.DeepstrikeArrivalmode(DeepStrikeType.Tunnel) : ", " + DeepStrikeUtility.DeepstrikeArrivalmode(DeepStrikeType.Tunnel);
                pawns.AddRange(Tunnellers);
                ArriveTunnel(Tunnellers);
                Tunnellers.Clear();
            }
            Log.Message("6");
            Find.LetterStack.ReceiveLetter("AMA_DeepStrike_Incomming".Translate(pawns.Find(x => x.Faction != null).Faction.def.pawnSingular), "AMA_DeepStrike_Incomming_Letter".Translate(pawns.Find(x => x.Faction != null).Faction.def.pawnsPlural, str), LetterDefOf.ThreatBig, pawns, pawns.Find(x => x.Faction != null).Faction, null);

            Log.Message("7");
            strike.struck = true;
        }

        public void ArriveDropPod(List<Pawn> pawns)
        {
            for (int i = 0; i < pawns.Count; i++)
            {
                //    IntVec3 dropCenter = map.listerBuildings.allBuildingsColonist.FindAll(x => x.TryGetComp<CompPowerPlant>() != null).RandomElement().Position;
                IntVec3 dropCenter;
                if (DropCellFinder.TryFindRaidDropCenterClose(out dropCenter, map))
                {
                    DeepStrikeUtility.DropThingsNear(dropCenter, map, Gen.YieldSingle<Thing>(pawns[i]), 50, false, false, true, DeepStrikeType.Drop);
                }
            }
            Droppers.Clear();
        }
        public void ArriveFly(List<Pawn> pawns)
        {
            for (int i = 0; i < pawns.Count; i++)
            {
                //    IntVec3 dropCenter = map.listerBuildings.allBuildingsColonist.FindAll(x => x.TryGetComp<CompPowerPlant>() != null).RandomElement().Position;
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
                //    IntVec3 dropCenter = map.listerBuildings.allBuildingsColonist.FindAll(x => x.TryGetComp<CompPowerPlant>() != null).RandomElement().Position;
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
                //    IntVec3 dropCenter = map.listerBuildings.allBuildingsColonist.FindAll(x => x.TryGetComp<CompPowerPlant>() != null).RandomElement().Position;
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

        public List<DeepStrikeEntry> Strikes;

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
