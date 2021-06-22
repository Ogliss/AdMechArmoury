using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
    // Token: 0x02000014 RID: 20
    [StaticConstructorOnStartup]
    public class ProtectiveEyeWear : Apparel
    {

        // Token: 0x06000059 RID: 89 RVA: 0x00003CB0 File Offset: 0x00001EB0
        public override bool CheckPreAbsorbDamage(DamageInfo dinfo)
        {
            //Log.Message(string.Format("{0}", dinfo.Def.label));
            if (dinfo.Instigator == base.Wearer)
            {
                return true;
            }
            if (dinfo.Instigator != null || dinfo.Def.isExplosive)
            {
                //Log.Message(string.Format("Instigator:{0} isExplosive:{1}", dinfo.Instigator, dinfo.Def.isExplosive));
                if (dinfo.Def.defName.Contains("InEyes")||dinfo.Def.defName == "OGGrenadePhoton")
                {
                    //Log.Message(string.Format("absorbing {0}", dinfo.Def.defName));
                   // this.AbsorbedDamage(dinfo);
                    return true;
                }
                else
                {
                    //Log.Message(string.Format("allowing {0}", dinfo.Def.defName));
                    return false;
                }
            }
            if (dinfo.Instigator != null)
            {
                return false;
            }
            this.AbsorbedDamage(dinfo);
            return true;

            /*
        //    log.message(string.Format("{0}", dinfo.Def.label));
            if (dinfo.Def.defName.Contains("InEyes"))
            {
            //    log.message(string.Format("{0}", dinfo.Def.defName));
                this.AbsorbedDamage(dinfo);
                return true;
            }
            else if (dinfo.HitPart.def.defName == "Eye" && dinfo.Def.defName == "OGGrenadePhoton")
            {
            //    log.message(string.Format("{0}", dinfo.Def.defName));
                this.AbsorbedDamage(dinfo);
                return true;
            }
            else return base.CheckPreAbsorbDamage(dinfo);
             */
        }

        // Token: 0x0600005B RID: 91 RVA: 0x00003DCC File Offset: 0x00001FCC
        private void AbsorbedDamage(DamageInfo dinfo)
        {
            SoundDefOf.EnergyShield_AbsorbDamage.PlayOneShot(new TargetInfo(base.Wearer.Position, base.Wearer.Map, false));

            float num = Mathf.Min(10f, 2f + (float)dinfo.Amount / 10f);
            AdeptusMoteMaker.MakeStaticMote(base.Wearer.DrawPos, base.Wearer.Map, ThingDefOf.Mote_ExplosionFlash, num);
            int num2 = (int)num;
            for (int i = 0; i < num2; i++)
            {
                Rand.PushState();
                AdeptusMoteMaker.ThrowDustPuff(base.Wearer.DrawPos, base.Wearer.Map, Rand.Range(0.8f, 1.2f));
                Rand.PopState();
            }
        }
    }
}
