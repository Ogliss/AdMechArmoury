using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    public class Verb_LaunchProjectileNG : Verb_LaunchProjectile
    {
        public float AddHediffChance = 0.020f;
        public HediffDef HediffToAdd = OGHediffDefOf.RadiationPoisioning;
        protected override bool TryCastShot()
        {
            if (caster.def.category == ThingCategory.Pawn)
            {
                Pawn launcherPawn = caster as Pawn;
                AddHediffChance = AddHediffChance * CasterPawn.GetStatValue(StatDefOf.ToxicSensitivity, true);
                var rand = Rand.Value; // This is a random percentage between 0% and 100%
                if (rand <= AddHediffChance) // If the percentage falls under the chance, success!
                {
                    var randomSeverity = Rand.Range(0.05f, 0.15f);
                    var effectOnPawn = launcherPawn?.health?.hediffSet?.GetFirstHediffOfDef(HediffToAdd);
                    if (effectOnPawn != null)
                    {
                        effectOnPawn.Severity += randomSeverity;
                    }
                    else
                    {
                        Hediff hediff = HediffMaker.MakeHediff(HediffToAdd, launcherPawn, null);
                        hediff.Severity = randomSeverity;
                        launcherPawn.health.AddHediff(hediff, null, null);
                    }
                }
            }
            return base.TryCastShot();
        }
    }
}
