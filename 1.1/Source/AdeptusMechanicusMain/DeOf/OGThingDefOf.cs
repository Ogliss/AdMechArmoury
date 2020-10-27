using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class OGThingDefOf
    {
        static OGThingDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(OGThingDefOf));
        }
        //    public static ThingDef HiveLike; RimlaserPrism
        public static ThingDef OG_AMA_Tunneler;
        public static ThingDef OG_AMA_Teleporter;
        public static ThingDef OG_Warpfire;
        public static ThingDef OG_WarpSpark;
        public static ThingDef OG_Mote_MicroSparksWarp;
        public static ThingDef OG_Mote_WarpFireGlow;

    }
}
