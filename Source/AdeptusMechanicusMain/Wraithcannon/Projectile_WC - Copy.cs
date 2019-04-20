using Verse;
using RimWorld;
using Verse.Sound;

namespace AdeptusMechanicus
{
	public class Projectile_WC2 : Projectile
    {
        #region Properties
        private int ticksToDetonation;

        public ThingDef_BulletWC Def
        {
            get
            {
                return this.def as ThingDef_BulletWC;
            }
        }
        #endregion Properties

        #region Overrides

        protected override void Impact(Thing hitThing)
        {

        }
    
        public override void PreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
        {
            base.PreApplyDamage(ref dinfo, out absorbed);
        }

        #endregion Overrides
    }
}
