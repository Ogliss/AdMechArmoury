using RimWorld;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class AdeptusThoughtDefOf
    {
        static AdeptusThoughtDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AdeptusThoughtDefOf));
        }
        public static ThoughtDef OG_ObservedWarpBeing;

    }
}
