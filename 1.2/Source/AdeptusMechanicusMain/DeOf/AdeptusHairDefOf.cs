using RimWorld;
using System;
using Verse;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class AdeptusHairDefOf
    {
        static AdeptusHairDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AdeptusHairDefOf));
        }
        public static HairDef Shaved;
        public static HairDef Topdog;
    }
}
