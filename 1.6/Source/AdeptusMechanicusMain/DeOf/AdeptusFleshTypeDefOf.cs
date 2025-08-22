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
        [MayRequireEldar]
        public static FleshTypeDef OG_Flesh_Construct_Eldar;
        [MayRequireDarkEldar]
        public static FleshTypeDef OG_Flesh_Construct_DarkEldar;
        [MayRequireChaos]
        public static FleshTypeDef OG_Flesh_Chaos_Deamon;
        [MayRequireNecrons]
        public static FleshTypeDef OG_Flesh_Construct_Necron;
        [MayRequireTyranids]
        public static FleshTypeDef OG_Flesh_Tyranid;

    }
}
