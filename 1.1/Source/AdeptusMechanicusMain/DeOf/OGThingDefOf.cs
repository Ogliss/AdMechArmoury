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

        [MayRequireAstartes]
        public static ThingDef OG_Human_Astartes;
        [MayRequireDarkEldar]
        public static ThingDef OG_Alien_DarkEldar;
        [MayRequireEldar]
        public static ThingDef OG_Alien_Eldar;
        [MayRequireOrkz]
        public static ThingDef OG_Alien_Ork;
        [MayRequireOrkz]
        public static ThingDef OG_Alien_Grot;
        [MayRequireTau]
        public static ThingDef OG_Alien_Tau;
        [MayRequireTau]
        public static ThingDef OG_Alien_Kroot;
        [MayRequireTau]
        public static ThingDef OG_Alien_Vespid;
        [MayRequireNecrons]
        public static ThingDef OG_Alien_Necron;
        [MayRequireTyranids]
        public static ThingDef OG_Alien_Tyranid;
    }
}
