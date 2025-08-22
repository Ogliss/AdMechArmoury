using RimWorld;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class AdeptusTaleDefOf
    {
        static AdeptusTaleDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AdeptusTaleDefOf));
        }
        public static TaleDef OG_EmergedFromTunnel;

    }
}
