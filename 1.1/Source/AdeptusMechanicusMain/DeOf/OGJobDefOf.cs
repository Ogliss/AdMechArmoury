using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class OGJobDefOf
    {
        static OGJobDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(OGJobDefOf));
        }
        public static JobDef OG_Job_ChangeLaserColor;

    }
}
