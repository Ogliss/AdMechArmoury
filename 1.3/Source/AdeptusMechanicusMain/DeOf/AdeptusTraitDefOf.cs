using RimWorld;
using System;
using Verse;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class AdeptusTraitDefOf
    {
        static AdeptusTraitDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AdeptusTraitDefOf));
        }
        public static readonly TraitDef FastLearner;
        public static readonly TraitDef Nimble;
        public static readonly TraitDef Masochist;
        public static readonly TraitDef SlowLearner;

    }
}
