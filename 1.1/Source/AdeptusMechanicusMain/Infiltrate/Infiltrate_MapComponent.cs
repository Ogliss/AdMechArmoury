using System;
using System.Collections.Generic;
using AdeptusMechanicus.ExtensionMethods;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x02000067 RID: 103
    public class MapComponent_Infiltrate : MapComponent, IThingHolder
    {
        public MapComponent_Infiltrate(Map map) : base(map)
        {
            this.map = map;
            this.innerContainer = new ThingOwner<Thing>(this, false, LookMode.Deep);
        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();
            if (innerContainer.Count > 0)
            {
                if (strikeDelay == -1)
                {
                    strikeDelay = ResultSpawnDelay.RandomInRange;
                    ticksSinceRaid++;
                }
                if (ticksSinceRaid != -1)
                {
                    ticksSinceRaid++;
                }

            //    Log.Message(string.Format("{0}, strikeDelay: {1}, ticksLastRaid: {2}, ticksSinceRaid: {3}", innerContainer.ContentsString, strikeDelay, raidLastTick, ticksSinceRaid));
                if (ticksSinceRaid > strikeDelay)
                {
                    List<Pawn> Infiltrators = new List<Pawn>();
                    foreach (Pawn p in innerContainer)
                    {
                        if (p.canInfiltrate())
                        {
                            Infiltrators.Add(p);
                        }
                    }
                    
                    if (!Infiltrators.NullOrEmpty())
                    {
                        RevealInfiltrators(Infiltrators);
                        Infiltrators.Clear();
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

        public void RevealInfiltrators(List<Pawn> pawns)
        {
            for (int i = 0; i < pawns.Count; i++)
            {
                //    IntVec3 dropCenter = map.listerBuildings.allBuildingsColonist.FindAll(x => x.TryGetComp<CompPowerPlant>() != null).RandomElement().Position;
                IntVec3 dropCenter;
                if (DropCellFinder.TryFindRaidDropCenterClose(out dropCenter, map))
                {
                    if (RCellFinder.TryFindRandomSpotJustOutsideColony(dropCenter, map, out dropCenter))
                    {
                        InfiltrateUtility.DropThingsNear(dropCenter, map, Gen.YieldSingle<Thing>(pawns[i]), 50, true, false, true);
                    }
                    else
                    {
                        InfiltrateUtility.DropThingsNear(dropCenter, map, Gen.YieldSingle<Thing>(pawns[i]), 50, true, false, true);
                    }
                }
                else
                {
                    dropCenter = DropCellFinder.FindRaidDropCenterDistant_NewTemp(map, true);
                    if (RCellFinder.TryFindRandomSpotJustOutsideColony(dropCenter, map, out dropCenter))
                    {
                        InfiltrateUtility.DropThingsNear(dropCenter, map, Gen.YieldSingle<Thing>(pawns[i]), 50, true, false, true);
                    }
                    else
                    {
                        InfiltrateUtility.DropThingsNear(dropCenter, map, Gen.YieldSingle<Thing>(pawns[i]), 50, true, false, true);
                    }
                }

            }
            Find.LetterStack.ReceiveLetter("AMA_Infiltrators_Revealed".Translate(pawns.Find(x => x.Faction != null).Faction.def.pawnSingular), "AMA_Infiltrators_Revealed_Letter".Translate(pawns.Find(x => x.Faction != null).Faction.def.pawnsPlural), LetterDefOf.ThreatBig, pawns, pawns.Find(x => x.Faction != null).Faction, null);
        //    Messages.Message("AMA_Infiltrators_Revealed".Translate(pawns.Find(x=> x.Faction!=null).Faction.Name), pawns, MessageTypeDefOf.ThreatBig);
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
            Scribe_Deep.Look<ThingOwner>(ref this.innerContainer, "innerContainer", new object[]
            {
                this
            });
        }

        private readonly IntRange ResultSpawnDelay = new IntRange(500, 1000);
        public int strikeMaxDelay = 24000;
        public int strikeMinDelay = 6000;
        public int strikeDelay = -1;
        private int strikeLastTick = -1;
        public int raidLastTick = -1;
    }
}
