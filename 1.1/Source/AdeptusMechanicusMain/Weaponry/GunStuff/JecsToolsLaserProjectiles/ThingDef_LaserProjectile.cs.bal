using System.Collections.Generic;
using Verse;
using RimWorld;

namespace AdeptusMechanicus
{
    //Originally code borrowed from a super cool dude.
    public class ThingDef_LaserProjectile : ThingDef
    {
        public float preFiringInitialIntensity = 0f;
        public float preFiringFinalIntensity = 0f;
        public float postFiringInitialIntensity = 0f;
        public float postFiringFinalIntensity = 0f;
        public string warmupGraphicPathSingle = null;
        public int preFiringDuration = 0;
        public int postFiringDuration = 0;
        public float StartFireChance;
        public bool CanStartFire = false;
        public bool IsMelta = false;
        public List<Projectile_LaserConfig> graphicSettings = null;
        public bool cycleThroughFiringPositions = false;
        public bool createsExplosion = false;
        public float AddHediffChance = 0.00f; //The default chance of adding a hediff.
        public bool CanResistHediff = false; //The default chance of adding a hediff.
        public float ResistHediffChance = 0.00f; //The default chance of adding a hediff.
        public StatDef ResistHediffStat = null; //The default chance of adding a hediff.
        public HediffDef HediffToAdd = null;
    }
}