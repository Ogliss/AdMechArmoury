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
        public static ThingDef OG_AMA_Tunneler;
        public static ThingDef OG_AMA_Teleporter;
        public static ThingDef OG_Warpfire;
        public static ThingDef OG_WarpSpark;
        public static ThingDef OG_Mote_MicroSparksWarp;
        public static ThingDef OG_Mote_WarpFireGlow;

        #region Mechanicus
        // Humanlike Race Defs
        [MayRequireXenobiologis]
        public static ThingDef OG_Human_Mechanicus;
        #endregion

        #region Imperial
        // Humanlike Race Defs
        [MayRequireXenobiologis]
        public static ThingDef OG_Abhuman_Ogryn;
        [MayRequireXenobiologis]
        public static ThingDef OG_Abhuman_Ratlin;
        [MayRequireXenobiologis]
        public static ThingDef OG_Abhuman_Beastman;
        #endregion

        #region Astartes
        // Humanlike Race Defs
        [MayRequireAstartes]
        public static ThingDef OG_Human_Astartes;
        #endregion

        #region Eldar
        // Humanlike Race Defs
        [MayRequireXenobiologis, MayRequireEldar]
        public static ThingDef OG_Alien_Eldar;
        [MayRequireEldar]
        public static ThingDef OG_Alien_Wraithguard;
        [MayRequireXenobiologis]
        public static ThingDef OG_Eldar_Wraithguard_Race;
        #endregion

        #region Dark Eldar
        // Humanlike Race Defs
        [MayRequireDarkEldar]
        public static ThingDef OG_Alien_DarkEldar;
        #endregion

        #region Greenskins
        // Humanlike Race Defs
        [MayRequireXenobiologis, MayRequireOrkz]
        public static ThingDef OG_Alien_Ork;
        [MayRequireXenobiologis, MayRequireOrkz]
        public static ThingDef OG_Alien_Grot;

        // Animal Race Defs
        [MayRequireOrkz]
        public static ThingDef OG_Snotling;
        [MayRequireXenobiologis, MayRequireOrkz]
        public static ThingDef OG_Squig;
        [MayRequireXenobiologis, MayRequireOrkz]
        public static ThingDef OG_Squig_Ork;

        // Plant Defs
        [MayRequireOrkz]
        public static ThingDef OG_Plant_Orkoid_Cocoon;
        [MayRequireOrkz]
        public static ThingDef OG_Plant_Orkoid_Fungus;

        // Blood Defs
        [MayRequireXenobiologis, MayRequireOrkz]
        public static ThingDef OG_FilthBlood_Orkoid;
        /*
        // Building Defs
        [MayRequireOrkz, MayRequireUniversalFermenter]
        public static ThingDef OG_Ork_FermentingBarrel;

        // Item Defs
        [MayRequireOrkz, MayRequireUniversalFermenter]
        public static ThingDef OG_Ork_Waart;
        [MayRequireOrkz, MayRequireUniversalFermenter]
        public static ThingDef OG_Ork_Grog;
        */
        #endregion

        #region Tau
        // Humanlike Race Defs
        [MayRequireXenobiologis, MayRequireTau]
        public static ThingDef OG_Alien_Tau;
        #endregion

        #region Kroot
        // Humanlike Race Defs
        [MayRequireXenobiologis, MayRequireTau]
        public static ThingDef OG_Alien_Kroot;

        // Animal Race Defs
        [MayRequireXenobiologis, MayRequireTau]
        public static ThingDef OG_Kroothound;
        [MayRequireXenobiologis, MayRequireTau]
        public static ThingDef OG_KrootOx;
        [MayRequireXenobiologis, MayRequireTau]
        public static ThingDef OG_Knarloc;

        // Tool User Race Defs
        [MayRequireXenobiologis, MayRequireTau]
        public static ThingDef OG_Kroothound_Kindred;
        [MayRequireXenobiologis, MayRequireTau]
        public static ThingDef OG_KrootOx_Kindred;
        [MayRequireXenobiologis, MayRequireTau]
        public static ThingDef OG_Knarloc_Kindred;

        // Blood Defs
        [MayRequireXenobiologis, MayRequireTau]
        public static ThingDef OG_FilthBlood_Kroot;
        #endregion

        #region Vespid
        // Humanlike Race Defs
        [MayRequireXenobiologis, MayRequireTau]
        public static ThingDef OG_Alien_Vespid;
        #endregion

        #region Necron
        // Humanlike Race Defs
        [MayRequireNecrons]
        public static ThingDef OG_Alien_Necron;

        // Tool User Race Defs
        #endregion

        #region Tyranids
        // Humanlike Race Defs
        [MayRequireTyranids]
        public static ThingDef OG_Alien_Tyranid;

        // Tool User Race Defs
        #endregion
    }
}
