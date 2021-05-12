using RimWorld;
using System;
using Verse;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class AdeptusOf
    {
        static AdeptusOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AdeptusOf));
        }

    }
}
