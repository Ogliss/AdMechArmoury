using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
	public class Verb_ShootGetsHot : Verb_LaunchProjectile
    {

        public float overheatchance = 0.05f; //The default chance of adding a hediff.
        public HediffDef HediffToAdd = OGHediffDefOf.PlasmaBurn;

        protected override int ShotsPerBurst
		{
			get
			{
				return this.verbProps.burstShotCount;
			}
		}

		public override void WarmupComplete()
		{
			base.WarmupComplete();
			if (base.CasterIsPawn && base.CasterPawn.skills != null)
			{
				float xp = 6f;
				Pawn pawn = this.currentTarget.Thing as Pawn;
				if (pawn != null && pawn.HostileTo(this.caster) && !pawn.Downed)
				{
					xp = 240f;
				}
				base.CasterPawn.skills.Learn(SkillDefOf.Shooting, xp, false);
			}
		}

		protected override bool TryCastShot()
		{
			bool flag = base.TryCastShot();
            Pawn launcherPawn = base.CasterPawn as Pawn;
            if (flag && base.CasterIsPawn)
			{
                var rand = Rand.Value; // This is a random percentage between 0% and 100%
                if (rand <= overheatchance) // If the percentage falls under the chance, success!
                {
                    /*
                     * Messages.Message flashes a message on the top of the screen. 
                     * You may be familiar with this one when a colonist dies, because
                     * it makes a negative sound and mentioneds "So and so has died of _____".
                     * 
                     * Here, we're using the "Translate" function. More on that later in
                     * the localization section.
                     */
                    Messages.Message("TST_BurnerBullet_SuccessMessage".Translate(new object[] {
                        launcherPawn.Label
                    }), MessageTypeDefOf.NeutralEvent);
                    //This checks to see if the character has a heal differential, or hediff on them already.
                    var overheatOnPawn = launcherPawn?.health?.hediffSet?.GetFirstHediffOfDef(HediffToAdd);
                    var randomSeverity = Rand.Range(15.15f, 30.30f);
                    if (overheatOnPawn != null)
                    {
                        //If they already have plague, add a random range to its severity.
                        //If severity reaches 1.0f, or 100%, plague kills the target.
                        overheatOnPawn.Severity += randomSeverity;
                    }
                    else
                    {
                        //These three lines create a new health differential or Hediff,
                        //put them on the character, and increase its severity by a random amount.
                        Hediff hediffL = HediffMaker.MakeHediff(HediffToAdd, launcherPawn, null);
                        hediffL.Severity = randomSeverity;
                        launcherPawn.health.AddHediff(hediffL, launcherPawn.RaceProps.body.AllParts.First((BodyPartRecord record) => record.def.defName == "LeftHand"), null);

                        Hediff hediffR = HediffMaker.MakeHediff(HediffToAdd, launcherPawn, null);
                        hediffR.Severity = randomSeverity;
                        launcherPawn.health.AddHediff(hediffR, launcherPawn.RaceProps.body.AllParts.First((BodyPartRecord record) => record.def.defName == "RightHand"), null);
                        base.CasterPawn.records.Increment(RecordDefOf.ShotsFired);
                    }
                }
            }
			return flag;
		}
	}
}
