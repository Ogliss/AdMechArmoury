using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
    // Token: 0x020006EB RID: 1771
    public class Apparel_ShockCollarold : Apparel
    {
        public override void Tick()
        {
            base.Tick();
            if (Wearer!=null)
            {
                if (Wearer.MentalState!=null)
                {
                    if (Wearer.MentalState.def.defName == "Slaughterer")
                    {
                        DamageInfo dinfo = new DamageInfo();
                        dinfo.Def = OGShockCollarDefOf.OGElectrical;

                        Wearer.TakeDamage(dinfo);
                    }
                }
            }
        }
    }

    [DefOf]
    public static class OGShockCollarDefOf
    {
        public static DamageDef OGElectrical;
    }
}
