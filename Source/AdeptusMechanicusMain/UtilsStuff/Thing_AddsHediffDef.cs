using System;
using Verse;

namespace AdeptusMechanicus
{   // Token: 0x02000005 RID: 5
    public class Thing_AddAcidDamageDef : ThingDef
    {
    }
    // Token: 0x02000007 RID: 7
    public class Thing_AddsHediffDef : ThingDef
    {
        // Token: 0x0400001A RID: 26
        public HediffDef addHediff;

        // Token: 0x0400001B RID: 27
        public float hediffAddChance = 1f;

        // Token: 0x0400001C RID: 28
        public float hediffSeverity = 0.05f;

        // Token: 0x0400001D RID: 29
        public int tickUpdateSpeed = 250;

        // Token: 0x0400001E RID: 30
        public bool onlyAffectLungs = true;

        // Token: 0x0400001F RID: 31
        public bool isAcid = false;
    }
}
