using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    public class ThingDef_BulletEffect : ThingDef
    {
        public float AddHediffChance = 0.05f; //The default chance of adding a hediff.
        public HediffDef HediffToAdd = OGHediffDefOf.RadiationPoisioning;
        public bool CanResistHediff = false; //The default chance of adding a hediff.
        public float ResistHediffChance = 0.00f; //The default chance of adding a hediff.
        public StatDef ResistHediffStat = null; //The default chance of adding a hediff.
    }
}