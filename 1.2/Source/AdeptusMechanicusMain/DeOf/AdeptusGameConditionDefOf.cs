using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class AdeptusGameConditionDefOf
    {
        static AdeptusGameConditionDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AdeptusGameConditionDefOf));
        }
        public static GameConditionDef OG_Condition_Warpstorm;
    }
}
