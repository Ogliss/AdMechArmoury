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
        public static ThingDef OG_ProteinMash;
        public static ThingDef OG_NutrientSolution;

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
        // Astartes ThingDefs
        [MayRequireAstartes]
        public static ThingDef OG_Astartes_Geneseed;

        [MayRequireAstartes]
        public static ThingDef OG_Zygote_Organ_SecondaryHeart;
        [MayRequireAstartes]
        public static ThingDef OG_Zygote_Organ_Ossmodula;
        [MayRequireAstartes]
        public static ThingDef OG_Zygote_Organ_Biscopea;
        [MayRequireAstartes]
        public static ThingDef OG_Zygote_Organ_Haemastamen;
        [MayRequireAstartes]
        public static ThingDef OG_Zygote_Organ_LarramansOrgan;
        [MayRequireAstartes]
        public static ThingDef OG_Zygote_Organ_Catalepsean;
        [MayRequireAstartes]
        public static ThingDef OG_Zygote_Organ_Preomnor;
        [MayRequireAstartes]
        public static ThingDef OG_Zygote_Organ_Omophagea;
        [MayRequireAstartes]
        public static ThingDef OG_Zygote_Organ_MultiLung;
        [MayRequireAstartes]
        public static ThingDef OG_Zygote_Organ_Occulobe;
        [MayRequireAstartes]
        public static ThingDef OG_Zygote_Organ_LymanEar;
        [MayRequireAstartes]
        public static ThingDef OG_Zygote_Organ_SusanMembrane;
        [MayRequireAstartes]
        public static ThingDef OG_Zygote_Organ_Melanochrome;
        [MayRequireAstartes]
        public static ThingDef OG_Zygote_Organ_OoliticKidney;
        [MayRequireAstartes]
        public static ThingDef OG_Zygote_Organ_Neuroglottis;
        [MayRequireAstartes]
        public static ThingDef OG_Zygote_Organ_Mucranoid;
        [MayRequireAstartes]
        public static ThingDef OG_Zygote_Organ_BetchersGland;
        [MayRequireAstartes]
        public static ThingDef OG_Zygote_Organ_ProgenoidGlands;
        [MayRequireAstartes]
        public static ThingDef OG_Zygote_Organ_BlackCarapace;

        #endregion

        #region Chaos
        /*
        // Humanlike Race Defs
        [MayRequireXenobiologis]
        public static ThingDef OG_Alien_Ork;

        // Tool User Race Defs
        [MayRequireXenobiologis]
        public static ThingDef OG_Alien_Ork;

        // Animal Race Defs
        [MayRequireXenobiologis]
        public static ThingDef OG_Squig;

        // Plant Defs
        [MayRequireOrkz]
        public static ThingDef OG_Plant_Orkoid_Cocoon;

        // Blood Defs
        [MayRequireXenobiologis
        public static ThingDef OG_FilthBlood_Orkoid;


        // Item Defs
        [MayRequireOrkz, MayRequireUniversalFermenter]
        public static ThingDef OG_Ork_Waart;
        [MayRequireOrkz, MayRequireUniversalFermenter]
        public static ThingDef OG_Ork_Grog;
        */
        // Building Defs
        [MayRequireXenobiologis]
        public static ThingDef OG_Chaos_Deamon_WarpTunnel;
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

        // Blood Defs
        [MayRequireXenobiologis, MayRequireTau]
        public static ThingDef OG_FilthBlood_Tau;
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

        // Blood Defs
        [MayRequireXenobiologis, MayRequireTau]
        public static ThingDef OG_FilthBlood_Vespid;
        #endregion

        #region Necron
        // Humanlike Race Defs
        [MayRequireNecrons]
        public static ThingDef OG_Alien_Necron;
        [MayRequireXenobiologis, MayRequireNecrons]
        public static ThingDef MonolithIncoming;
        [MayRequireXenobiologis, MayRequireNecrons]
        public static ThingDef OG_FilthBlood_Necron;

        // Tool User Race Defs
        [MayRequireXenobiologis, MayRequireNecrons]
        public static ThingDef Necron_ScarabSwarm;
        [MayRequireXenobiologis, MayRequireNecrons]
        public static ThingDef Necron_FlayedOne;
        [MayRequireXenobiologis, MayRequireNecrons]
        public static ThingDef Necron_Warrior;
        [MayRequireXenobiologis, MayRequireNecrons]
        public static ThingDef Necron_Wraith;
        [MayRequireXenobiologis, MayRequireNecrons]
        public static ThingDef Necron_Immortal;
        [MayRequireXenobiologis, MayRequireNecrons]
        public static ThingDef Necron_TombSpyder;
        [MayRequireXenobiologis, MayRequireNecrons]
        public static ThingDef Necron_Destroyer;
        [MayRequireXenobiologis, MayRequireNecrons]
        public static ThingDef Necron_HeavyDestroyer;
        [MayRequireXenobiologis, MayRequireNecrons]
        public static ThingDef Necron_Lord;
        #endregion

        #region Tyranids
        // Humanlike Race Defs
        [MayRequireTyranids]
        public static ThingDef OG_Alien_Tyranid;

        // Tool User Race Defs
        [MayRequireXenobiologis, MayRequireTyranids]
        public static ThingDef Tyranid_SporeMine_HE;

        [MayRequireXenobiologis, MayRequireTyranids]
        public static ThingDef OG_FilthBlood_Tyranid;
        #endregion
    }
}
