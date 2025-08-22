using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    public class GasmaskExtentsion : DefModExtension
    {
        public float Chance = 0.25f;
        public bool qualityBased = true;
    }
    public class CompProperties_LungProtectionApparel : CompProperties
    {
        public CompProperties_LungProtectionApparel()
        {
            base.compClass = typeof(CompLungProtectionApparel);
        }
        public float Chance = 0.25f;
        public bool qualityBased = true;
    }

    public class CompLungProtectionApparel : ThingComp
    {
        public CompProperties_LungProtectionApparel Props => (CompProperties_LungProtectionApparel)base.props;

        private Apparel Apparel => this.parent as Apparel;
        public float Reduction
        {
            get
            {
                if (Apparel != null && Props.qualityBased && Apparel.TryGetQuality(out QualityCategory qc))
                {
                    return ((byte)qc + Props.Chance) / Enum.GetNames(typeof(QualityCategory)).Length;
                }
                return Props.Chance;
            }
        }
    }
}