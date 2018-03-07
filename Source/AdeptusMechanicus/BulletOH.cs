using RimWorld;
using System;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
    public class BulletOH : Projectile
    {
        protected override void Impact(Thing hitThing)
        {
            Map map = base.Map;
            base.Impact(hitThing);
            if (hitThing != null)
            {
                int damageAmountMax = this.def.projectile.damageAmountBase;
                int damageAmount = Rand.Range(0, damageAmountMax);

                ThingDef equipmentDef = this.equipmentDef;
                DamageInfo dinfo = new DamageInfo(this.def.projectile.damageDef, damageAmount, this.ExactRotation.eulerAngles.y, this.launcher, null, equipmentDef);
                hitThing.TakeDamage(dinfo);
            }
            else
            {
                SoundDefOf.BulletImpactGround.PlayOneShot(new TargetInfo(base.Position, map, false));
                MoteMaker.MakeStaticMote(this.ExactPosition, map, ThingDefOf.Mote_ShotHit_Dirt, 1f);
            }
        }
    }
}
