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
        public static readonly TraitDef Masochist;
        public static readonly TraitDef Nimble;
        public static readonly TraitDef PsychicSensitivity;
        public static readonly TraitDef SlowLearner;
        public static readonly TraitDef Tough;
        public static readonly TraitDef Cannibal;
        public static readonly TraitDef Beauty;
        public static readonly TraitDef TooSmart;
        public static readonly TraitDef Nerves;

    }
}
