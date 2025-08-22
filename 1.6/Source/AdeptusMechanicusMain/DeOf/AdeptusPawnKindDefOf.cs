using RimWorld;
using System;
using Verse;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class AdeptusPawnKindDefOf
    {
        static AdeptusPawnKindDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AdeptusPawnKindDefOf));
        }
        // Astartes PawnKindDefs
        // Player
        [MayRequireAstartes]
        public static PawnKindDef OG_Astartes_Neophyte;
        [MayRequireAstartes]
        public static PawnKindDef OG_Astartes_Brother;
        [MayRequireAstartes]
        public static PawnKindDef OG_Astartes_Sargent;
        [MayRequireAstartes]
        public static PawnKindDef OG_Astartes_Captain;


        // Eldar PawnKindDefs
        [MayRequireXenobiologis, MayRequireEldar]
        public static PawnKindDef OG_Eldar_Guardian;
        [MayRequireXenobiologis, MayRequireEldar]
        public static PawnKindDef OG_Eldar_GuardianStorm;
        [MayRequireXenobiologis, MayRequireEldar]
        public static PawnKindDef OG_Eldar_GuardianSupport;
        [MayRequireXenobiologis, MayRequireEldar]
        public static PawnKindDef OG_Eldar_Ranger;
        [MayRequireXenobiologis, MayRequireEldar]
        public static PawnKindDef OG_Eldar_DireAvenger;
        [MayRequireXenobiologis, MayRequireEldar]
        public static PawnKindDef OG_Eldar_DireAvenger_Exarch;
        [MayRequireXenobiologis, MayRequireEldar]
        public static PawnKindDef OG_Eldar_HowlingBanshee;
        [MayRequireXenobiologis, MayRequireEldar]
        public static PawnKindDef OG_Eldar_HowlingBanshee_Exarch;
        [MayRequireXenobiologis, MayRequireEldar]
        public static PawnKindDef OG_Eldar_SwoopingHawk;
        [MayRequireXenobiologis, MayRequireEldar]
        public static PawnKindDef OG_Eldar_SwoopingHawk_Exarch;
        [MayRequireXenobiologis, MayRequireEldar]
        public static PawnKindDef OG_Eldar_FireDragon;
        [MayRequireXenobiologis, MayRequireEldar]
        public static PawnKindDef OG_Eldar_FireDragon_Exarch;
        [MayRequireXenobiologis, MayRequireEldar]
        public static PawnKindDef OG_Eldar_DarkReaper;
        [MayRequireXenobiologis, MayRequireEldar]
        public static PawnKindDef OG_Eldar_DarkReaper_Exarch;
        [MayRequireXenobiologis, MayRequireEldar]
        public static PawnKindDef OG_Eldar_StrikingScorpion;
        [MayRequireXenobiologis, MayRequireEldar]
        public static PawnKindDef OG_Eldar_StrikingScorpion_Exarch;
        [MayRequireXenobiologis, MayRequireEldar]
        public static PawnKindDef OG_Eldar_WarpSpider;
        [MayRequireXenobiologis, MayRequireEldar]
        public static PawnKindDef OG_Eldar_WarpSpider_Exarch;
        [MayRequireXenobiologis, MayRequireEldar]
        public static PawnKindDef OG_Eldar_Warlock;
        [MayRequireXenobiologis, MayRequireEldar]
        public static PawnKindDef OG_Eldar_Farseer;
        [MayRequireXenobiologis, MayRequireEldar]
        public static PawnKindDef OG_Eldar_Farseer_Faction;
        [MayRequireXenobiologis, MayRequireEldar]
        public static PawnKindDef OG_Eldar_Wraithguard;
        [MayRequireXenobiologis, MayRequireEldar]
        public static PawnKindDef OG_Eldar_Wraithblade;

        /*
        [MayRequireXenobiologis, MayRequireEldar]
        public static PawnKindDef OG_Eldar_Wraithlord;
        [MayRequireXenobiologis, MayRequireEldar]
        public static PawnKindDef OG_Eldar_Avatar;
        */

        // Deamon PawnKindDefs
        // Deamons of Tzeentch
        [MayRequireChaos]
        public static PawnKindDef OG_Chaos_Deamon_Flamer_Exalted;
        [MayRequireChaos]
        public static PawnKindDef OG_Chaos_Deamon_Flamer;
        [MayRequireChaos]
        public static PawnKindDef OG_Chaos_Deamon_Lessar_Horror_Pink;
        [MayRequireChaos]
        public static PawnKindDef OG_Chaos_Deamon_Lessar_Horror_Blue;
        [MayRequireChaos]
        public static PawnKindDef OG_Chaos_Deamon_Lessar_Horror_Brimstone;
        [MayRequireChaos]
        public static PawnKindDef OG_Chaos_Deamon_Lessar_Screamer;
        [MayRequireChaos]
        public static PawnKindDef OG_Chaos_Deamon_Lord_of_Change;

        // Deamons of Nurgle
        [MayRequireChaos]
        public static PawnKindDef OG_Chaos_Deamon_Lessar_Nurgling;
        [MayRequireChaos]
        public static PawnKindDef OG_Chaos_Deamon_Plague_Bearer;
        [MayRequireChaos]
        public static PawnKindDef OG_Chaos_Deamon_Great_Unclean_One;

        // Deamons of Slaanesh
        [MayRequireChaos]
        public static PawnKindDef OG_Chaos_Deamon_Lessar_Deamonette;
        //   public static PawnKindDef OG_Chaos_Deamon_Mounted_Deamonette;
        [MayRequireChaos]
        public static PawnKindDef OG_Chaos_Deamon_Keeper_of_Secrets;

        // Deamons of Khrone
        //   public static PawnKindDef OG_Chaos_Deamon_Hound_of_Khorne;
        [MayRequireChaos]
        public static PawnKindDef OG_Chaos_Deamon_Lessar_Bloodletter;
        //   public static PawnKindDef OG_Chaos_Deamon_Bloodthirster;
        //   public static PawnKindDef OG_Chaos_Deamon_Juggernaught_of_Khorne;

        #region Greenskins
        // Humanlike PawnKind Defs
        [MayRequireOrkz]
        public static PawnKindDef OG_Grot_Wild;
        [MayRequireOrkz]
        public static PawnKindDef Tribesperson_OG_Grot;
        [MayRequireOrkz]
        public static PawnKindDef Colonist_OG_Grot;
        [MayRequireOrkz]
        public static PawnKindDef OG_Ork_Wild;
        [MayRequireOrkz]
        public static PawnKindDef Tribesperson_OG_Ork_Boy;
        [MayRequireOrkz]
        public static PawnKindDef Tribesperson_OG_Ork_Nob;
        [MayRequireOrkz]
        public static PawnKindDef Tribesperson_OG_Ork_Warboss;
        [MayRequireOrkz]
        public static PawnKindDef Colonist_OG_Ork_Boy;
        [MayRequireOrkz]
        public static PawnKindDef Colonist_OG_Ork_Nob;
        [MayRequireOrkz]
        public static PawnKindDef Colonist_OG_Ork_Warboss;

        // Animal PawnKind Defs
        [MayRequireXenobiologis, MayRequireOrkz]
        public static PawnKindDef OG_Squig;
        [MayRequireOrkz]
        public static PawnKindDef OG_Snotling;

        // Tool User PawnKind Defs
        [MayRequireXenobiologis, MayRequireOrkz]
        public static PawnKindDef OG_Squig_Ork;

        #endregion

        [MayRequireNecrons]
        public static PawnKindDef OG_Necron_Scarab_Swarm;
        [MayRequireNecrons]
        public static PawnKindDef OG_Necron_Flayed_One;
        [MayRequireNecrons]
        public static PawnKindDef OG_Necron_Warrior;
        [MayRequireNecrons]
        public static PawnKindDef OG_Necron_Wraith;
        [MayRequireNecrons]
        public static PawnKindDef OG_Necron_Immortal;
        [MayRequireNecrons]
        public static PawnKindDef OG_Necron_Tomb_Spyder;
        [MayRequireNecrons]
        public static PawnKindDef OG_Necron_Destroyer;
        [MayRequireNecrons]
        public static PawnKindDef OG_Necron_Destroyer_Heavy;
        [MayRequireNecrons]
        public static PawnKindDef OG_Necron_Lord;

        [MayRequireTyranids]
        public static PawnKindDef OG_Tyranid_Ripper_Swarm;
        [MayRequireTyranids]
        public static PawnKindDef OG_Tyranid_Hormagaunt;
        [MayRequireTyranids]
        public static PawnKindDef OG_Tyranid_Termagant;
        [MayRequireTyranids]
        public static PawnKindDef OG_Tyranid_Genestealer;
        [MayRequireTyranids]
        public static PawnKindDef OG_Tyranid_Warrior;
        [MayRequireTyranids]
        public static PawnKindDef OG_Tyranid_Gargoyle;
        [MayRequireTyranids]
        public static PawnKindDef OG_Tyranid_Lictor;
        [MayRequireTyranids]
        public static PawnKindDef OG_Tyranid_Ravener;
        [MayRequireTyranids]
        public static PawnKindDef OG_Tyranid_Hive_Tyrant;
        [MayRequireTyranids]
        public static PawnKindDef OG_Tyranid_Carnifex;
        [MayRequireTyranids]
        public static PawnKindDef OG_Tyranid_Zoanthrope;
        [MayRequireTyranids]
        public static PawnKindDef OG_Tyranid_Biovore;
        [MayRequireTyranids]
        public static PawnKindDef OG_Tyranid_SporeMine;
    }
}
