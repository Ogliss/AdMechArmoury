using RimWorld;
using System;
using Verse;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class AdeptusNeedDefOf
    {
        static AdeptusNeedDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AdeptusNeedDefOf));
        }
        public static NeedDef Mood;
        public static NeedDef Outdoors;
        public static NeedDef Comfort;
        public static NeedDef Beauty;

        [MayRequireDarkEldar]
        public static NeedDef OGDE_Soul;
        [MayRequireOrkz]
        public static NeedDef OG_Ork_Fightyness;

    }
}
