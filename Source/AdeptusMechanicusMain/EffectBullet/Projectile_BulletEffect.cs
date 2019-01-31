using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    public class Projectile_BulletEffect : Bullet
    {
        #region Properties
        public ThingDef_BulletEffect Def
        {
            get
            {
                return this.def as ThingDef_BulletEffect;
            }
        }
        #endregion Properties

        #region Overrides
        protected override void Impact(Thing hitThing)
        {
            base.Impact(hitThing);

            if (Def != null && hitThing != null && hitThing is Pawn hitPawn) //Fancy way to declare a variable inside an if statement. - Thanks Erdelf.
            {
                var rand = Rand.Value; // This is a random percentage between 0% and 100%

                StatDef ResistHediffStat = Def.ResistHediffStat;
                float AddHediffChance = Def.AddHediffChance;
                float ResistHediffChance = Def.ResistHediffChance;
                if (Def.CanResistHediff == true)
                {
                    /*
                    if (Def.ResistHediffChance!=0)
                    {
                        rand = rand + Def.ResistHediffChance;
                    }
                    else */
                    if (Def.ResistHediffStat != null)
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
                        //If they already have plague, add a random range to its severity.
                        //If severity reaches 1.0f, or 100%, plague kills the target.
                        effectOnPawn.Severity += randomSeverity;
                    }
                    else
                    {
                        //These three lines create a new health differential or Hediff,
                        //put them on the character, and increase its severity by a random amount.
                        Hediff hediff = HediffMaker.MakeHediff(Def.HediffToAdd, hitPawn, null);
                        hediff.Severity = randomSeverity;
                        hitPawn.health.AddHediff(hediff, null, null);
                    }
                }
                else //failure!
                {

                    /*
                    MoteMaker.ThrowText(hitThing.PositionHeld.ToVector3(), hitThing.MapHeld, "TST_PlagueBullet_FailureMote".Translate(Def.AddHediffChance), 12f);
                    */
                }
            }
        }
        #endregion Overrides
    }
}