using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.HediffCompProperties_GiveHediffsInRangeInterval
    public class HediffCompProperties_GiveHediffsInRangeInterval : HediffCompProperties
    {
        public HediffCompProperties_GiveHediffsInRangeInterval()
        {
            this.compClass = typeof(HediffComp_GiveHediffsInRangeInterval);
        }

        public float range;
        public TargetingParameters targetingParameters;
        public HediffDef hediff;
        public ThingDef mote;
        public bool hideMoteWhenNotDrafted;
        public float initialSeverity = 1f;
        public bool onlyPawnsInSameFaction = true;
        public int intervalTicks = 300;
    }

    public class HediffComp_GiveHediffsInRangeInterval : HediffComp
    {
        public HediffCompProperties_GiveHediffsInRangeInterval Props
        {
            get
            {
                return (HediffCompProperties_GiveHediffsInRangeInterval)this.props;
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (!this.parent.pawn.Awake() || this.parent.pawn.health == null || this.parent.pawn.health.InPainShock || !this.parent.pawn.Spawned)
            {
                return;
            }
            if (this.ticks > 0)
            {
                ticks--;
                return;
            }
            else this.ticks = Props.intervalTicks;
            if (!this.Props.hideMoteWhenNotDrafted || this.parent.pawn.Drafted)
            {
                if (this.Props.mote != null && (this.mote == null || this.mote.Destroyed))
                {
                    this.mote = MoteMaker.MakeAttachedOverlay(this.parent.pawn, this.Props.mote, Vector3.zero, 1f, -1f);
                }
                if (this.mote != null)
                {
                    this.mote.Maintain();
                }
            }
            IReadOnlyList<Pawn> readOnlyList;
            if (this.Props.onlyPawnsInSameFaction && this.parent.pawn.Faction != null)
            {
                readOnlyList = this.parent.pawn.Map.mapPawns.SpawnedPawnsInFaction(this.parent.pawn.Faction);
            }
            else
            {
                readOnlyList = this.parent.pawn.Map.mapPawns.AllPawnsSpawned;
            }
            foreach (Pawn pawn in readOnlyList)
            {
                if (pawn.RaceProps.Humanlike && !pawn.Dead && pawn.health != null && pawn != this.parent.pawn && pawn.Position.DistanceTo(this.parent.pawn.Position) <= this.Props.range && this.Props.targetingParameters.CanTarget(pawn, null))
                {
                    Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(this.Props.hediff, false);
                    if (hediff == null)
                    {
                        hediff = pawn.health.AddHediff(this.Props.hediff, pawn.health.hediffSet.GetBrain(), null, null);
                        hediff.Severity = this.Props.initialSeverity;
                        HediffComp_Link hediffComp_Link = hediff.TryGetComp<HediffComp_Link>();
                        if (hediffComp_Link != null)
                        {
                            hediffComp_Link.drawConnection = true;
                            hediffComp_Link.other = this.parent.pawn;
                        }
                    }
                    HediffComp_Disappears hediffComp_Disappears = hediff.TryGetComp<HediffComp_Disappears>();
                    if (hediffComp_Disappears == null)
                    {
                        Log.Error("HediffComp_GiveHediffsInRangeInterval has a hediff in props which does not have a HediffComp_Disappears");
                    }
                    else
                    {
                        hediffComp_Disappears.ticksToDisappear = 5 + Props.intervalTicks;
                    }
                }
            }
        }
        private int ticks = 0;
        private Mote mote;
    }
}
