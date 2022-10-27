using RimWorld;
using System;
using Verse;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class AdeptusBodyPartGroupDefOf
    {
        static AdeptusBodyPartGroupDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AdeptusBodyPartGroupDefOf));
        }
        public static readonly BodyPartGroupDef Mouth;
        public static readonly BodyPartGroupDef Neck;

    }
}
