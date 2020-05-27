using System;
using System.Collections.Generic;
using AdeptusMechanicus.ExtensionMethods;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x02000067 RID: 103
    public class MapComponent_DeepStrike : MapComponent, IThingHolder
    {
        public MapComponent_DeepStrike(Map map) : base(map)
        {
            this.map = map;
            this.innerContainer = new ThingOwner<Thing>(this, false, LookMode.Deep);
        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();
            if (innerContainer.Count > 0)
            {
                if (strikeDelay==-1)
                {
                    strikeDelay = ResultSpawnDelay.RandomInRange;
                    ticksSinceRaid++;
                }
                if (ticksSinceRaid!=-1)
                {
                    ticksSinceRaid++;
                }

            //    Log.Message(string.Format("{0}, strikeDelay: {1}, ticksLastRaid: {2}, ticksSinceRaid: {3}", innerContainer.ContentsString, strikeDelay, raidLastTick, ticksSinceRaid));
                if (ticksSinceRaid > strikeDelay)
                {
                    foreach (Pawn p in innerContainer)
                    {
                        if (p.DeepStrike().pawnsArrivalMode == DeepStrikeType.Drop)
                        {
                            Droppers.Add(p);
                        }
                        if (p.DeepStrike().pawnsArrivalMode == DeepStrikeType.Fly)
                        {
                            Flyers.Add(p);
                        }
                        if (p.DeepStrike().pawnsArrivalMode == DeepStrikeType.Teleport)
                        {
                            Teleporters.Add(p);
                        }
                        if (p.DeepStrike().pawnsArrivalMode == DeepStrikeType.Tunnel)
                        {
                            Tunnellers.Add(p);
                        }

                    }

                    if (!innerContainer.NullOrEmpty())
                    {
                        DoStrike();
                    }
                    strikeLastTick = Find.TickManager.TicksGame;
                }
            }
            else
            {
                raidLastTick = -1;
                ticksSinceRaid = -1;
                strikeDelay = -1;
            }
        }

        public void DoStrike()
        {
            string str = string.Empty;
            List<Pawn> pawns = new List<Pawn>();
            if (!Droppers.NullOrEmpty())
            {
                str += str == string.Empty ? DeepStrikeUtility.DeepstrikeArrivalmode(DeepStrikeType.Drop) : ", "+ DeepStrikeUtility.DeepstrikeArrivalmode(DeepStrikeType.Drop);
                pawns.AddRange(Droppers);
                ArriveDropPod(Droppers);
            }
            if (!Flyers.NullOrEmpty())
            {
                str += str == string.Empty ? DeepStrikeUtility.DeepstrikeArrivalmode(DeepStrikeType.Fly) : ", " + DeepStrikeUtility.DeepstrikeArrivalmode(DeepStrikeType.Fly);
                pawns.AddRange(Flyers);
                ArriveFly(Flyers);
            }
            if (!Teleporters.NullOrEmpty())
            {
                str += str == string.Empty ? DeepStrikeUtility.DeepstrikeArrivalmode(DeepStrikeType.Teleport) : ", " + DeepStrikeUtility.DeepstrikeArrivalmode(DeepStrikeType.Teleport);
                pawns.AddRange(Teleporters);
                ArriveTeleport(Teleporters);
            }
            if (!Tunnellers.NullOrEmpty())
            {
                str += str == string.Empty ? DeepStrikeUtility.DeepstrikeArrivalmode(DeepStrikeType.Tunnel) : ", " + DeepStrikeUtility.DeepstrikeArrivalmode(DeepStrikeType.Tunnel);
                pawns.AddRange(Tunnellers);
                ArriveTunnel(Tunnellers);
            }
            Find.LetterStack.ReceiveLetter("AMA_DeepStrike_Incomming".Translate(pawns.Find(x => x.Faction != null).Faction.def.pawnSingular), "AMA_DeepStrike_Incomming_Letter".Translate(pawns.Find(x => x.Faction != null).Faction.def.pawnsPlural, str), LetterDefOf.ThreatBig, pawns, pawns.Find(x => x.Faction != null).Faction, null);
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

        // Token: 0x060024F3 RID: 9459 RVA: 0x00116CE3 File Offset: 0x001150E3
        public ThingOwner GetDirectlyHeldThings()
        {
            return this.innerContainer;
        }

        // Token: 0x060024F4 RID: 9460 RVA: 0x00116CEB File Offset: 0x001150EB
        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, this.GetDirectlyHeldThings());
        }

        public IThingHolder ParentHolder
        {
            get
            {
                return map;
            }
        }
        protected ThingOwner innerContainer;
        
        public override void ExposeData()
        {

            base.ExposeData();
            Scribe_Values.Look<int>(ref this.strikeLastTick, "strikeLastTick", -1, false);
            Scribe_Values.Look<int>(ref this.strikeDelay, "strikeDelay", -1, false);
            Scribe_Values.Look<int>(ref this.raidLastTick, "ticksLastRaid", -1, false);
            Scribe_Collections.Look(ref this.Strikes, "StrikesGroups", LookMode.Deep);
            Scribe_Deep.Look<ThingOwner>(ref this.innerContainer, "innerContainer", new object[]
            {
                this
            });
        }

        public List<DeepStrikeEntry> Strikes = new List<DeepStrikeEntry>();

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

    public class DeepStrikeEntry
    {
        private List<Pawn> StrikeGroup = new List<Pawn>();
        public DeepStrikeType InsetionMethod = DeepStrikeType.Drop;
        public float StrikeDelay = 0f;
        public float QuedTick = 0f;
    }

}
