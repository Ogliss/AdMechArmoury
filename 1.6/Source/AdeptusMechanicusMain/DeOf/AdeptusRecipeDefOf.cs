using RimWorld;
using System;
using Verse;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class AdeptusRecipeDefOf
    {
        static AdeptusRecipeDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AdeptusRecipeDefOf));
        }

        [MayRequireAstartes]
        public static RecipeDef OG_Install_AstartePart;
    }
}
