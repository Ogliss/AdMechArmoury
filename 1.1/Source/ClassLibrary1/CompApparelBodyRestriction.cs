using Verse;
using RimWorld;
using System.Collections.Generic;

namespace Momu
{
    public class CompProperties_ApparelBodyRestriction : CompProperties
    {
        public CompProperties_ApparelBodyRestriction()
        {
            this.compClass = typeof(CompApparelBodyRestriction);
        }
        public List<BodyTypeDef> AllowedBodyTypes = new List<BodyTypeDef>();
    }

    public class CompApparelBodyRestriction : ThingComp
    {
        public CompProperties_ApparelBodyRestriction Props => (CompProperties_ApparelBodyRestriction)props;
    }
}
