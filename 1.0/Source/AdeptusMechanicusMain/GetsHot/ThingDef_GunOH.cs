using RimWorld;
using System;
using Verse;

namespace AdeptusMechanicus
{
    public class ThingDef_GunOH : ThingWithComps
    {
        public Overheat overheat
        {
            get
            {
                foreach (VerbProperties v in def.Verbs)
                {
                    if (v.GetType() == Type.GetType("AdeptusMechanicus.VerbPropertiesOH"))
                    {
                        return ((VerbPropertiesOH)v).overheat;
                    }
                }
                return Overheat.NA;
            }
        }
        public override string GetInspectString()
        {
            string result = base.GetInspectString();
            string overheatString;
            float jamsOn;
            StatPart_Overheat.GetOverheat(this, out overheatString, out jamsOn);

            result += string.Format("\r\nOverheat: {0}\r\nChance of overheat: {1}%", overheatString, jamsOn);
            return result;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            string overheatString;
            float jamsOn;
            StatPart_Overheat.GetOverheat(this, out overheatString, out jamsOn);

            Scribe_Values.Look<string>(ref overheatString, "overheat", "NA", false);
        }
    }
}
