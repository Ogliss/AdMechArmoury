using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    public class CompProperties_HediffWeapon : CompProperties
    {
        public HediffDef hediffDef;
        public List<BodyPartDef> partsToAffect;
        public List<BodyPartGroupDef> groupsToAffect;
        public bool severityBasedOnDurability = false;

        public CompProperties_HediffWeapon()
        {
            base.compClass = typeof(CompHediffWeapon);
        }
    }

    public class CompHediffWeapon : ThingComp
    {
        private float lastDurability;
        private Pawn lastWearer;
        private List<Hediff> addedHediffs = new List<Hediff>();
        private Pawn pawn;

        public CompProperties_HediffWeapon Props => (CompProperties_HediffWeapon)base.props;

        public void MyRemoveHediffs(Pawn pawn)
        {
            if (addedHediffs.Any())
            {
                if (pawn != null)
                {
                    for (int i = 0; i < addedHediffs.Count; i++)
                    {
                        pawn.health.RemoveHediff(addedHediffs[i]);
                    }
                }
                addedHediffs.Clear();
            }
        }


        public bool MyAddHediffs(Pawn pawn)
        {
            // Sanity test; if our pawn doesn't exist, don't even bother.
            if (pawn == null) return false;

            // Special case; if we're not told to apply to anything in particular, apply to the Whole Body.
            if (Props.partsToAffect.NullOrEmpty() && Props.groupsToAffect.NullOrEmpty())
            {
                return HediffGiverUtility.TryApply(pawn, Props.hediffDef, null, false, 1, addedHediffs);
            }

            IEnumerable<BodyPartRecord> source = pawn.health.hediffSet.GetNotMissingParts();
            List<BodyPartDef> partsToAffect = new List<BodyPartDef>();
            int countToAffect;

            if (!Props.partsToAffect.NullOrEmpty())
            {
                partsToAffect.AddRange(from p in source where Props.partsToAffect.Contains(p.def) select p.def);
            }

            if (!Props.groupsToAffect.NullOrEmpty())
            {
                partsToAffect.AddRange(from p in source where Props.groupsToAffect.Intersect(p.groups).Any() select p.def);
            }

            countToAffect = partsToAffect.Count();
            partsToAffect.RemoveDuplicates();

            return HediffGiverUtility.TryApply(pawn, Props.hediffDef, partsToAffect, false, countToAffect, addedHediffs);
        }

        public void MyUpdateSeverity()
        {
            // Get our current durability as a percentage.
            float currentDurability = (float)parent.HitPoints / parent.MaxHitPoints;

            // Only update if our durability has changed.
            if (lastDurability != currentDurability)
            {
                // Set the severity for each of our hediffs.
                foreach (Hediff item in addedHediffs)
                {
                    item.Severity = currentDurability;
                }

                // Update our durability so we don't run code too often.
                lastDurability = currentDurability;
            }
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);

            // We've been destroyed, so remove our effects.
            MyRemoveHediffs(lastWearer);
        }

        public override void CompTick()
        {
            base.CompTick();

            foreach (CompEquippable c in base.parent.GetComps<CompEquippable>())
            {
                pawn = (Pawn)c.verbTracker.PrimaryVerb.caster;
            }
            if (pawn != null)
            {
            //    Log.Message(string.Format("10"));
                // We only need to do something if our wearer has changed.
                if (pawn != lastWearer)
                {
                    // It has, so remove our effects from the last wearer and apply them to the new one.
                    MyRemoveHediffs(lastWearer);
                    MyAddHediffs(pawn);
                    // Update our wearer so we don't run code too often.
                    lastWearer = pawn;
                    // Set our last recorded durability to some impossible value to force an update.
                    lastDurability = -1;
                }
            }
            
            // Check to see if we should update our severity.
            if (Props.severityBasedOnDurability)
            {
               // Log.Message(string.Format("9"));
                MyUpdateSeverity();
            }
           // Log.Message(string.Format("10"));
        }
    }
}