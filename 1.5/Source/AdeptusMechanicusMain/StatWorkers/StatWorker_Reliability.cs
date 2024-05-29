using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    public class StatWorker_Reliability : StatWorker
    {
        public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
        {

            return string.Empty;
        }

        public override string GetExplanationFinalizePart(StatRequest req, ToStringNumberSense numberSense, float finalVal)
        {
            StringBuilder sb = new StringBuilder();
            //sb.AppendLine("FinalizeExplanation");
            if (stat.parts != null)
            {
                for (int i = 0; i < stat.parts.Count; i++)
                {
                    string text = stat.parts[i].ExplanationPart(req);
                    if (!text.NullOrEmpty())
                    {
                        sb.AppendLine(text);
                        sb.AppendLine();
                    }
                }
                string reliabilityString = string.Empty;
                if (finalVal < 0.25)
                    reliabilityString = "Extremely Reliable";
                else if (finalVal < 0.5)
                    reliabilityString = "Very Reliable";
                else if (finalVal < 0.75)
                    reliabilityString = "Standard";
                else
                    reliabilityString = "Unreliable";


                sb.AppendLine(string.Format("Reliability: {0}\r\n\r\nFailiure chance: {1}%", reliabilityString, finalVal));
            }
            return sb.ToString();
        }
    }
}
