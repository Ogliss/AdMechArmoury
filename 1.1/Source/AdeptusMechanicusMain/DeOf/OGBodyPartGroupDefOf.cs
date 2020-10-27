using RimWorld;
using System;
using Verse;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class OGBodyPartGroupDefOf
    {
        static OGBodyPartGroupDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(OGBodyPartGroupDefOf));
        }
        public static readonly BodyPartGroupDef Mouth;
        public static readonly BodyPartGroupDef Neck;

    }
}
