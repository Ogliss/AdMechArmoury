using System;
using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    public class VerbPropertiesOGSG : VerbProperties
    {
        public int pelletCount = 1;

    }
    // Token: 0x02000007 RID: 7
    public class Verb_ShootShotgun : Verb_Shoot
    {
        public VerbPropertiesOGSG VerbPropsOGSG
        {
            get
            {
                return verbProps as VerbPropertiesOGSG;
            }
        }
        protected override bool TryCastShot()
        {
            bool flag = base.TryCastShot();
            bool flag2 = flag && VerbPropsOGSG.pelletCount - 1 > 0;
            bool flag3 = flag2;
            if (flag3)
            {
                for (int i = 0; i < VerbPropsOGSG.pelletCount - 1; i++)
                {
                    base.TryCastShot();
                }
            }
            return flag;
        }
    }
}
