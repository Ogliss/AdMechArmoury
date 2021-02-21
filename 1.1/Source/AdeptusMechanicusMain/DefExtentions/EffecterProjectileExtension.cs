using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.EffecterProjectileExtension
    public class EffecterProjectileExtension : DefModExtension
    {
        public float AddHediffChance = 0.05f; //The default chance of adding a hediff.
        public HediffDef HediffToAdd = null;
        public bool CanResistHediff = false; //The default chance of adding a hediff.
        public float ResistHediffChance = 0.00f; //The default chance of adding a hediff.
        public StatDef ResistHediffStat = null; //The default chance of adding a hediff.

        public void ApplyEffect(Thing hitThing)
        {

            if (hitThing != null && hitThing is Pawn hitPawn)
            {
                if (HediffToAdd != null)
                {
                    Rand.PushState();
                    var rand = Rand.Value; // This is a random percentage between 0% and 100%
                    Rand.PopState();
                    if (CanResistHediff == true)
                    {
                        /*
                        if (ResistHediffChance!=0)
                        {
                            rand = rand + ResistHediffChance;
                        }
                        else */
                        if (ResistHediffStat != null)
                        {
                            ResistHediffChance = hitPawn.GetStatValue(ResistHediffStat, true);
                        }
                        AddHediffChance = AddHediffChance * ResistHediffChance;
                    }

                    if (rand <= AddHediffChance)
                    {
                        var effectOnPawn = hitPawn?.health?.hediffSet?.GetFirstHediffOfDef(HediffToAdd);
                        Rand.PushState();
                        var randomSeverity = Rand.Range(0.15f, 0.30f);
                        Rand.PopState();
                        if (effectOnPawn != null)
                        {
                            effectOnPawn.Severity += randomSeverity;
                        }
                        else
                        {
                            Hediff hediff = HediffMaker.MakeHediff(HediffToAdd, hitPawn, null);
                            hediff.Severity = randomSeverity;
                            hitPawn.health.AddHediff(hediff, null, null);
                        }
                    }
                    /*
                    else
                    {
                        MoteMaker.ThrowText(hitThing.PositionHeld.ToVector3(), hitThing.MapHeld, "FailureMote".Translate(Def.AddHediffChance), 12f);
                    }
                    */
                }
            }
        }
    }

}
