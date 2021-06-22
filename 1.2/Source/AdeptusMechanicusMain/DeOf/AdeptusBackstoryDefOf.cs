using RimWorld;
using System;
using Verse;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class AdeptusBackstoryDefOf
    {
        static AdeptusBackstoryDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AdeptusBackstoryDefOf));
        }
        [MayRequireAstartes]
        public static BackstoryDef OG_Astartes_BrotherNew;

    }
}
