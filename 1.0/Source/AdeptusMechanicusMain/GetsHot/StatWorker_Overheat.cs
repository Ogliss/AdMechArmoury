using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    public class StatWorker_Overheat : StatWorker
    {
        public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
        {
            
            return string.Empty;
        }

        public override string GetExplanationFinalizePart(StatRequest req, ToStringNumberSense numberSense, float finalVal)
        {
            StringBuilder sb = new StringBuilder();
            //sb.AppendLine("FinalizeExplanation");
            if (this.stat.parts != null)
            {
                for (int i = 0; i < this.stat.parts.Count; i++)
                {
                    string text = this.stat.parts[i].ExplanationPart(req);
                    if (!text.NullOrEmpty())
                    {
                        sb.AppendLine(text);
                        sb.AppendLine();
                    }
                }
                string overheatString = string.Empty;
                if (finalVal < 0.25)
                    overheatString = "Extremely Reliable";
                else if (finalVal < 0.5)
                    overheatString = "Very Reliable";
                else if (finalVal < 1)
                    overheatString = "Standard";
                else
                    overheatString = "Unreliable";


                sb.AppendLine(string.Format("Reliability: {0}\r\n\r\nChance of Overheat: {1}%", overheatString, finalVal));
            }
            return sb.ToString();
        }
    }
}
