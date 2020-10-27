using RimWorld;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class OGTaleDefOf
    {
        static OGTaleDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(OGTaleDefOf));
        }
        public static TaleDef OG_EmergedFromTunnel;

    }
}
