using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    public class CompProperties_AddedBulletEffect : CompProperties
    {
        public CompProperties_AddedBulletEffect()
        {
            this.compClass = typeof(CompAddedBulletEffect);
        }
        public int FragmentCount = 0;
        public ThingDef Fragmentdef = null;


    }

    // Token: 0x02000002 RID: 2
    public class CompAddedBulletEffect : ThingComp
    {

    }
}
