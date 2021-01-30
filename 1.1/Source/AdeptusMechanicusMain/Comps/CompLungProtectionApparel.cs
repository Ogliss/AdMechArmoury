using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace AdeptusMechanicus
{

    public class CompProperties_LungProtectionApparel : CompProperties
    {
        public CompProperties_LungProtectionApparel()
        {
            base.compClass = typeof(CompLungProtectionApparel);
        }
        public float Chance = 0.25f;
    }

    public class CompLungProtectionApparel : ThingComp
    {
        public CompProperties_LungProtectionApparel Props => (CompProperties_LungProtectionApparel)base.props;

        private Pawn lastWearer = null;

        public void MyRemoveComps(Pawn pawn)
        {
            if (pawn != null)
            {
                if (lastWearer.GetComp<CompLungProtection>() != null)
                {
                    IEnumerable<CompLungProtection> list = lastWearer.GetComps<CompLungProtection>();
                    foreach (var c in list)
                    {
                        pawn.AllComps.Remove(c);
                    }
                }
            }
        }

        public bool MyAddComps(Pawn pawn)
        {
            // Sanity test; if our pawn doesn't exist, don't even bother.
            if (pawn == null) return false;

            // Apply our hediffs!
            if (pawn.GetComp<CompLungProtection>() == null)
            {

                pawn.AllComps.Add(new CompLungProtection()
                {
                    chance = Props.Chance
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
    public class CompProperties_LungProtection : CompProperties
    {
        public CompProperties_LungProtection()
        {
            base.compClass = typeof(CompLungProtection);
        }
        public float chance;
    }

    public class CompLungProtection : ThingComp
    {
        public CompProperties_LungProtection Props => (CompProperties_LungProtection)base.props;

        public float chance;
    }
}