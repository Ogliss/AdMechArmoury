using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

#pragma warning disable IDE1006 // Naming Styles

namespace AdeptusMechanicus
{
    public class CompProperties_HediffApparel : CompProperties
    {
        public HediffDef hediffDef;
        public List<string> partsToAffect;
        public List<BodyPartGroupDef> groupsToAffect;
        public List<string> filterTerms;
        public FilterMode filterMode = FilterMode.Contains;
        public SeverityMode severityMode = SeverityMode.None;
        public bool global;
        public bool dropOnPartLost = false;

        public CompProperties_HediffApparel()
        {
            base.compClass = typeof(CompHediffApparel);
        }
    }

    public class CompHediffApparel : ThingComp
    {
        public const string AddHediffsToPawnSignal = "HediffApparel_AddHediffsToPawnSignalAM";
        public const string RemoveHediffsFromPawnSignal = "HediffApparel_RemoveHediffsFromPawnSignalAM";

        public CompProperties_HediffApparel Props => (CompProperties_HediffApparel)base.props;

        private float lastSeverity;
        private Pawn lastWearer;

        public List<Hediff> MyGetHediffs(Pawn pawn)
        {
            IEnumerable<BodyPartRecord> partRecords = MyGetPartsToAffect(pawn);
            // Return only hediffs on parts we would have added them to.
            return pawn.health.hediffSet.hediffs.Where(d => d.def == Props.hediffDef && (Props.global || partRecords.Contains(d.Part))).ToList();
        }

        public IEnumerable<BodyPartRecord> MyGetPartsToAffect(Pawn pawn)
        {
            List<BodyPartRecord> partsToAffect = new List<BodyPartRecord>();

            // The bulk of our code is only needed if we were told to apply our hediff to something specific.
            if (Props.partsToAffect.NullOrEmpty() && Props.groupsToAffect.NullOrEmpty())
            {
                // We weren't, so add a null value; this represents the Whole Body.
                partsToAffect.Add(null);
            }
            else
            {
                IEnumerable<BodyPartRecord> source = pawn.health.hediffSet.GetNotMissingParts();

                // Filter our source by the filterTerms.
                if (!Props.filterTerms.NullOrEmpty())
                {
                    switch (Props.filterMode)
                    {
                        case FilterMode.Contains: source = source.Where(r => Props.filterTerms.Any(f => r.Label.Contains(f))); break;
                        case FilterMode.StartsWith: source = source.Where(r => Props.filterTerms.Any(f => r.Label.StartsWith(f))); break;
                        case FilterMode.EndsWith: source = source.Where(r => Props.filterTerms.Any(f => r.Label.EndsWith(f))); break;
                        case FilterMode.Equals: source = source.Where(r => Props.filterTerms.Any(f => r.Label.Equals(f))); break;
                        case FilterMode.Excludes: source = source.Where(r => !Props.filterTerms.Any(f => r.Label.Contains(f))); break;
                    }
                }

                // Add the specified parts, if they exist, to our list of parts to affect.
                if (!Props.partsToAffect.NullOrEmpty())
                {
                    partsToAffect.AddRange(source.Where(p => Props.partsToAffect.Any(x => x == p.def.defName) || Props.partsToAffect.Any(x => x == p.customLabel)));
                }

                // Now do it for all the parts in the specified groups.
                if (!Props.groupsToAffect.NullOrEmpty())
                {
                    partsToAffect.AddRange(source.Where(p => Props.groupsToAffect.Intersect(p.groups).Any()));
                }
            }
            // Return only distinct parts, discarding duplicates.
            return partsToAffect.Distinct();
        }

        private void MyRemoveHediffs(Pawn pawn)
        {
            // Sanity test; if our pawn doesn't exist, don't even bother.
            if (pawn.DestroyedOrNull()) return;

            foreach (Hediff diff in MyGetHediffs(pawn))
            {
                pawn.health.RemoveHediff(diff);
            }
        }

        private void MyAddHediffs(Pawn pawn)
        {
            // Sanity test; if our pawn doesn't exist, don't even bother.
            if (pawn.DestroyedOrNull()) return;

            IEnumerable<BodyPartRecord> partsToAffect = MyGetPartsToAffect(pawn);

            // Only start applying hediffs if there are any parts to affect.
            if (partsToAffect.Any())
            {
                foreach (BodyPartRecord partRecord in partsToAffect)
                {
                    // Don't apply hediffs to parts that already have them.
                    if (!pawn.health.hediffSet.HasHediff(Props.hediffDef, partRecord))
                    {
                        pawn.health.AddHediff(HediffMaker.MakeHediff(Props.hediffDef, pawn, partRecord));
                    }
                }
                // Update our severity when done (even if our hediffs didn't change).
                MyUpdateSeverity(pawn, SeverityMode.Quality);
            }
        }

        public void MyUpdateSeverity(Pawn pawn, SeverityMode severityMode)
        {
            // Mode test; if our input mode is not the mode this comp uses, return.
            if (severityMode != Props.severityMode) return;

            // Sanity test; if our pawn doesn't exist, don't even bother.
            if (pawn.DestroyedOrNull()) return;

            float currentSeverity = Props.hediffDef.initialSeverity;

            switch (severityMode)
            {
                case SeverityMode.Durability:
                    // Severity DECREASES as durability is depleted.
                    currentSeverity = (float)parent.HitPoints / parent.MaxHitPoints;
                    break;
                case SeverityMode.Quality:
                    // Severity INCREASES as quality is improved.
                    if (!parent.TryGetQuality(out QualityCategory qc))
                    {
                        // Although a non-fatal error, this is probably not intentional behavior.
                        Log.Warning($"CompHediffApparel.MyUpdateSeverity: severityMode = Quality but {parent.Label} has no quality.");
                    }
                    currentSeverity = ((byte)qc + 1f) / Enum.GetNames(typeof(QualityCategory)).Length;
                    break;
            }

            // Round our current severity to the ingame display limit. One part in a thousand is precise enough.
            currentSeverity = (float)Math.Round(currentSeverity, 3);

            // Only update if our durability has changed.
            if (lastSeverity != currentSeverity)
            {
                // Set the severity for each of our hediffs.
                foreach (Hediff diff in MyGetHediffs(pawn))
                {
                    diff.Severity = currentSeverity;
                }
                // Update our durability so we don't run code too often.
                lastSeverity = currentSeverity;
            }
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);

            // We've been destroyed, so remove our effects.
            MyRemoveHediffs(lastWearer);
        }

        public override void ReceiveCompSignal(string signal)
        {
            switch (signal)
            {
                case AddHediffsToPawnSignal:
                    // When the AddHediffsToPawnSignal unique signal is received, add hediff to the new wearer
                    this.lastWearer = (parent as Apparel)?.Wearer;
                    this.MyAddHediffs(this.lastWearer);
                    break;
                case RemoveHediffsFromPawnSignal:
                    // When the RemoveHediffsFromPawnSignal unique signal is received, remove all applied hediffs
                    this.MyRemoveHediffs(this.lastWearer);
                    this.lastWearer = null;
                    break;
                default:
                    // Handle the non-HediffApparel signals
                    base.ReceiveCompSignal(signal);
                    break;
            }
        }

        public override void CompTick()
        {
            base.CompTick();

            // We know our parent is an Apparel; cast it as such so we can access its Wearer member.
            Apparel apparel = parent as Apparel;
            // Apparel has no signal for when it is first worn, so we check for this on CompTick().
            // We only need to do something if our wearer has changed, though.
            if (apparel.Wearer != lastWearer)
            {
                // It has, so remove our effects from the last wearer and apply them to the new one.
                MyRemoveHediffs(lastWearer);
                MyAddHediffs(apparel.Wearer);
                // Update our wearer so we don't run code too often.
                lastWearer = apparel.Wearer;
                // Set our last recorded durability to some impossible value to force an update.
                lastSeverity = -1;
            }

            // Update our severity every 60 ticks if our wearer is valid.
            if (!apparel.Wearer.DestroyedOrNull() && apparel.Wearer.IsHashIntervalTick(60))
            {
                MyUpdateSeverity(apparel.Wearer, SeverityMode.Durability);
            }
        }
    }
    public enum SeverityMode : byte
    {
        None, Durability, Quality
    }
    public enum FilterMode : byte
    {
        Contains, StartsWith, EndsWith, Equals, Excludes
    }
}