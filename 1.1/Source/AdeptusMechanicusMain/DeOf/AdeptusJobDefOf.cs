using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class AdeptusJobDefOf
    {
        static AdeptusJobDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AdeptusJobDefOf));
        }
        public static JobDef OG_Job_ChangeLaserColor;

    }
}
