using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    public class CompProperties_CompApparel : CompProperties
    {
        public ThingComp comp;

        public CompProperties_CompApparel()
        {
            base.compClass = typeof(CompCompApparel);
        }
    }

    public class CompCompApparel : ThingComp
    {
        private Pawn lastWearer;

        public CompProperties_CompApparel Props => (CompProperties_CompApparel)base.props;

        public void MyRemoveComps(Pawn pawn)
        {
            if (pawn != null)
            {
                List<ThingComp> diffs = pawn.AllComps.Where(d => d == Props.comp).ToList();
                foreach (ThingComp diff in diffs)
                {
                    pawn.AllComps.Remove(diff);
                }
            }
        }

        public bool MyAddComps(Pawn pawn)
        {
            // Sanity test; if our pawn doesn't exist, don't even bother.
            if (pawn == null) return false;

            // Apply our hediffs!
            if (Props.comp.props.compClass.Name=="CompEyeProtectionApparel")
            {

                pawn.AllComps.Add(new CompEyeProtection()
                {
                    chance = 0.25f
                });
            }
            return true;
        }


        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);

            // We've been destroyed, so remove our effects.
            MyRemoveComps(lastWearer);
        }

        public override void CompTick()
        {
            base.CompTick();

            // We know our parent is an Apparel; cast it as such so we can access its Wearer member.
            Apparel parent = base.parent as Apparel;

            // We only need to do something if our wearer has changed.
            if (parent.Wearer != lastWearer)
            {
                // It has, so remove our effects from the last wearer and apply them to the new one.
                MyRemoveComps(lastWearer);
                MyAddComps(parent.Wearer);
                // Update our wearer so we don't run code too often.
                lastWearer = parent.Wearer;
            }

        }
    }
}