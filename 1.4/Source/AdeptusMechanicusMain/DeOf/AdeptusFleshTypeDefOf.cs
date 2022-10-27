using RimWorld;
using System;
using Verse;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class AdeptusFleshTypeDefOf
    {
        static AdeptusFleshTypeDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AdeptusFleshTypeDefOf));
        }
        [MayRequireXenobiologis, MayRequireEldar]
        public static FleshTypeDef OG_Flesh_Construct_Eldar;
        [MayRequireXenobiologis, MayRequireDarkEldar]
        public static FleshTypeDef OG_Flesh_Construct_DarkEldar;
        [MayRequireXenobiologis]
        public static FleshTypeDef OG_Flesh_Chaos_Deamon;
        [MayRequireXenobiologis]
        public static FleshTypeDef OG_Flesh_Construct_Necron;
        [MayRequireXenobiologis, MayRequireTyranids]
        public static FleshTypeDef OG_Flesh_Tyranid;

    }
}
