using RimWorld;
using System.Linq;
using Verse;


namespace AdeptusMechanicus
{
    public class Verb_LaunchProjectileOH : Verb_LaunchProjectile
    {
        public HediffDef HediffToAdd = OGHediffDefOf.PlasmaBurn;
        protected virtual float overheat
        {
            get
            {
                return ownerEquipment.GetStatValue(StatDefOf_OH.overheat);
            }
        }

        public VerbPropertiesOH verbPropsOH
        {
            get
            {
                return verbProps as VerbPropertiesOH;
            }
        }

        protected override bool TryCastShot()
        {
            string overheatString;
            float jamsOn;
            StatPart_Overheat.GetOverheat((ThingDef_GunOH)ownerEquipment, out overheatString, out jamsOn);
            float jamRoll = (Rand.Range(0, 1000))/10f;
            //float jamRoll = Rand.Range(0, 100);
            Pawn launcherPawn = caster as Pawn;
            if (jamRoll < jamsOn)
            {
                string msg = string.Format("{0}'s {1} had a weapon overheat. ({2}/{3})", caster.LabelCap, ownerEquipment.LabelCap, jamRoll, jamsOn);
                Messages.Message(msg, MessageTypeDefOf.SilentInput);
                ownerEquipment.HitPoints--;
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
                }
                return false;
            }
            return base.TryCastShot();
        }
    }
}
