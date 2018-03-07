using Verse;

namespace AdeptusMechanicus
{
    public class ThingDef_BulletEffect : ThingDef
    {
        public float AddHediffChance = 0.05f; //The default chance of adding a hediff.
        public HediffDef HediffToAdd = OGHediffDefOf.RadiationPoisioning;
    }
}