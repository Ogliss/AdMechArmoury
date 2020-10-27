using RimWorld;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class OGStatDefOf
    {
        static OGStatDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(OGStatDefOf));
        }
        public static readonly StatDef reliability = StatDef.Named("reliability"); // for gun reliability

    }
}
