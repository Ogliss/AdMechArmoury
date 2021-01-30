using System;
using Verse;

namespace AdeptusMechanicus
{   
    public class Thing_AddsHediffDef : ThingDef
    {
        public HediffDef addHediff;
        public float hediffAddChance = 1f;
        public float hediffSeverity = 0.05f;
        public int tickUpdateSpeed = 250;
        public bool onlyAffectLungs = true;
        public bool isAcid = false;
    }
}
