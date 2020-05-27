using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    public class CompProperties_EyeProtectionApparel : CompProperties
    {
        public CompProperties_EyeProtectionApparel()
        {
            base.compClass = typeof(CompEyeProtectionApparel);
        }
        public float Chance = 0.25f;
    }

    public class CompEyeProtectionApparel : ThingComp
    {
        public CompProperties_EyeProtectionApparel Props => (CompProperties_EyeProtectionApparel)base.props;

        private Pawn lastWearer = null;

        public void MyRemoveComps(Pawn pawn)
        {
            if (pawn != null)
            {
                if (lastWearer.GetComp<CompEyeProtection>()!=null)
                {
                    IEnumerable<CompEyeProtection> list = lastWearer.GetComps<CompEyeProtection>();
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
            if (pawn.GetComp<CompEyeProtection>() == null)
            {

                pawn.AllComps.Add(new CompEyeProtection()
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
    public class CompProperties_EyeProtection : CompProperties
    {
        public CompProperties_EyeProtection()
        {
            base.compClass = typeof(CompEyeProtection);
        }
        public float chance;
    }

    public class CompEyeProtection : ThingComp
    {
        public CompProperties_EyeProtection Props => (CompProperties_EyeProtection)base.props;

        public float chance;
        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.PostPostApplyDamage(dinfo, totalDamageDealt);
        }
        public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
        {
            if (dinfo.Def.defName.Contains("InEyes") || dinfo.Def.defName == "OGGrenadePhoton")
            {
                absorbed = true;
            }
            else
            base.PostPreApplyDamage(dinfo, out absorbed);
        }
    }
}