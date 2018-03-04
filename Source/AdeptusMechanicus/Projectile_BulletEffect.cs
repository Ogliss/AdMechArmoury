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
        //


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

            /*
             * Null checking is very important in RimWorld.
             * 99% of errors reported are from NullReferenceExceptions (NREs).
             * Make sure your code checks if things actually exist, before they
             * try to use the code that belongs to said things.
             */
            if (Def != null && hitThing != null && hitThing is Pawn hitPawn) //Fancy way to declare a variable inside an if statement. - Thanks Erdelf.
            {
                var rand = Rand.Value; // This is a random percentage between 0% and 100%
                if (rand <= Def.AddHediffChance) // If the percentage falls under the chance, success!
                {
                    /*
                     * Messages.Message flashes a message on the top of the screen. 
                     * You may be familiar with this one when a colonist dies, because
                     * it makes a negative sound and mentioneds "So and so has died of _____".
                     * 
                     * Here, we're using the "Translate" function. More on that later in
                     * the localization section.
                     */

                     /*
                    Messages.Message("TST_PlagueBullet_SuccessMessage".Translate(new object[] {
                        this.launcher.Label, hitPawn.Label
                    }), MessageTypeDefOf.NeutralEvent);
                    */

                    //This checks to see if the character has a heal differential, or hediff on them already.
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
                     * Motes handle all the smaller visual effects in RimWorld.
                     * Dust plumes, symbol bubbles, and text messages floating next to characters.
                     * This mote makes a small text message next to the character.
                     */

                    /*
                    MoteMaker.ThrowText(hitThing.PositionHeld.ToVector3(), hitThing.MapHeld, "TST_PlagueBullet_FailureMote".Translate(Def.AddHediffChance), 12f);
                    */
                }
            }
        }
        #endregion Overrides
    }
}