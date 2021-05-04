using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class AdeptusThingDefOf
    {
        static AdeptusThingDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AdeptusThingDefOf));
        }
        //    public static ThingDef HiveLike; RimlaserPrism
        public static ThingDef OG_AMA_Tunneler;
        public static ThingDef OG_AMA_Teleporter;
        public static ThingDef OG_Warpfire;
        public static ThingDef OG_WarpSpark;
        public static ThingDef OG_Mote_MicroSparksWarp;
        public static ThingDef OG_Mote_WarpFireGlow;

        [MayRequireXenobiologis]
        public static ThingDef OG_Human_Mechanicus;
        [MayRequireXenobiologis]
        public static ThingDef OG_Abhuman_Ogryn;
        [MayRequireXenobiologis]
        public static ThingDef OG_Abhuman_Ratlin;
        [MayRequireXenobiologis]
        public static ThingDef OG_Abhuman_Beastman;
        [MayRequireAstartes]
        public static ThingDef OG_Human_Astartes;
        [MayRequireDarkEldar]
        public static ThingDef OG_Alien_DarkEldar;
        [MayRequireEldar]
        public static ThingDef OG_Alien_Eldar;
        /*
        [MayRequireEldar]
        public static ThingDef OG_Alien_Wraithguard;
        */
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
