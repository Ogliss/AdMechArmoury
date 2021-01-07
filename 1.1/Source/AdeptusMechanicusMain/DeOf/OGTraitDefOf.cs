using RimWorld;
using System;
using Verse;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class OGTraitDefOf
    {
        static OGTraitDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(OGTraitDefOf));
        }
        public static readonly TraitDef FastLearner;
        public static readonly TraitDef Nimble;
        public static readonly TraitDef Masochist;

    }
}
