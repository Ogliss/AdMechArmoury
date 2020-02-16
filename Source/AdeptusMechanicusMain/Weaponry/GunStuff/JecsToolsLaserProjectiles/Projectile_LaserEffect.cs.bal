using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    public class Projectile_LaserEffect : Projectile_Laser
    {

        #region Overrides
        protected override void Impact(Thing hitThing)
        {
            base.Impact(hitThing);

            if (Def != null && hitThing != null && hitThing is Pawn hitPawn) //Fancy way to declare a variable inside an if statement. - Thanks Erdelf.
            {
                StatDef ResistHediffStat = Def.ResistHediffStat;
                var rand = Rand.Value; // This is a random percentage between 0% and 100%
                float AddHediffChance = Def.AddHediffChance;
                float ResistHediffChance = Def.ResistHediffChance;
                if (Def.CanResistHediff == true)
                {
                    /*
                    if (Def.ResistHediffChance!=0)
                    {
                        rand = rand + Def.ResistHediffChance;
                    }
                    else */if (Def.ResistHediffStat!=null)
                    {
                        ResistHediffChance = hitPawn.GetStatValue(ResistHediffStat, true);
                    }
                    AddHediffChance = AddHediffChance * ResistHediffChance;
                }
                if (rand <= AddHediffChance) // If the percentage falls under the chance, success!
                {

                    var effectOnPawn = hitPawn?.health?.hediffSet?.GetFirstHediffOfDef(Def.HediffToAdd);
                    var randomSeverity = Rand.Range(0.15f, 0.30f);
                    if (effectOnPawn != null)
                    {

                        effectOnPawn.Severity += randomSeverity;
                    }
                    else
                    {

                        Hediff hediff = HediffMaker.MakeHediff(Def.HediffToAdd, hitPawn, null);
                        hediff.Severity = randomSeverity;
                        hitPawn.health.AddHediff(hediff, null, null);
                    }
                }
                else //failure!
                {

                }
            }
        }
        #endregion Overrides
    }
}