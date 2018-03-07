using RimWorld;
using System;
using Verse;

namespace AdeptusMechanicus
{
    public class ThingDef_GunCP : ThingWithComps
    {
        public Reliability reliability
        {
            get
            {
                foreach (VerbProperties v in def.Verbs)
                {
                    if (v.GetType() == Type.GetType("AdeptusMechanicus.VerbPropertiesCP"))
                    {
                        return ((VerbPropertiesCP)v).reliability;
                    }
                }
                return Reliability.NA;
            }
        }
        public override string GetInspectString()
        {
            string result = base.GetInspectString();
            string reliabilityString;
            float jamsOn;
            StatPart_Reliability.GetReliability(this, out reliabilityString, out jamsOn);

            result += string.Format("\r\nReliability: {0}\r\nChance of jam: {1}%", reliabilityString, jamsOn);
            return result;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            string reliabilityString;
            float jamsOn;
            StatPart_Reliability.GetReliability(this, out reliabilityString, out jamsOn);

            Scribe_Values.Look<string>(ref reliabilityString, "reliability", "NA", false);
        }
    }
}
