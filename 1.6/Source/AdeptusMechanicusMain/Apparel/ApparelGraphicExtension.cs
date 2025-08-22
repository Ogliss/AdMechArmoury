using System.Collections.Generic;
using Verse;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.ApparelGraphicExtension
    public class ApparelGraphicExtension : DefModExtension
    {
        public string defaultLabel = "Default";
        public string keyLabel = "Alternate Graphic";
        public List<AlternateApparelGraphic> alternateGraphics = new List<AlternateApparelGraphic>();
        public bool gizmoOnWorn = false;
        public bool randomizeInital = false;
        private bool? qualityControled;
        public bool QualityControled
        {
            get
            {
                if (qualityControled == null)
                {
                    qualityControled = alternateGraphics.Any(x => x.QualityControled);
                }
                return qualityControled.Value;
            }
        }
        public override IEnumerable<string> ConfigErrors()
        {
            foreach (string text in base.ConfigErrors())
            {
                yield return text;
            }


            yield break;
        }
    }
}