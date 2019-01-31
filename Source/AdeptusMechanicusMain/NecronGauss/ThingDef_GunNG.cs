using RimWorld;
using System;
using Verse;

namespace AdeptusMechanicus
{
    public class ThingDef_GunNG : ThingDef
    {
        public Boolean CanRapidFire = false;
        public Boolean AddHediffToCaster = false;
        public HediffDef CasterHediffToAdd = null;
        public int CasterHediffchance = 0;

    }
}
