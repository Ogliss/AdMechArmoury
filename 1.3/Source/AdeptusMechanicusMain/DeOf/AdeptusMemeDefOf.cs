using RimWorld;
using System;
using Verse;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class AdeptusMemeDefOf
    {
        static AdeptusMemeDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AdeptusMemeDefOf));
        }
        public static readonly MemeDef OG_Imperial_Structure_TheistEmbodied;

    }
}
