using RimWorld;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class AdeptusStatDefOf
    {
        static AdeptusStatDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AdeptusStatDefOf));
        }
        public static readonly StatDef reliability = StatDef.Named("reliability"); // for gun reliability

    }
}
