using RimWorld;
using Verse;


namespace AdeptusMechanicus
{
    public class Verb_LaunchProjectileCP : Verb_LaunchProjectile
    {
        protected virtual float reliable
        {
            get
            {
                return ownerEquipment.GetStatValue(StatDefOf_CP.reliability);
            }
        }

        public VerbPropertiesCP verbPropsCP
        {
            get
            {
                return verbProps as VerbPropertiesCP;
            }
        }

        protected override bool TryCastShot()
        {
            string reliabilityString;
            float jamsOn;
            StatPart_Reliability.GetReliability((ThingDef_GunCP)ownerEquipment, out reliabilityString, out jamsOn);
            float jamRoll = (Rand.Range(0, 1000))/10f;
            //float jamRoll = Rand.Range(0, 100);
            if (jamRoll < jamsOn)
            {
                string msg = string.Format("{0}'s {1} had a weapon jam. ({2}/{3})", caster.LabelCap, ownerEquipment.LabelCap, jamRoll, jamsOn);
                Messages.Message(msg, MessageTypeDefOf.SilentInput);
                ownerEquipment.HitPoints--;
                return false;
            }
            return base.TryCastShot();
        }
    }
}
