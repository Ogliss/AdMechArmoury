using RimWorld;
using System;
using Verse;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class AdeptusFactionDefOf
    {
        static AdeptusFactionDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AdeptusFactionDefOf));
        }

        [MayRequireAstartes]
        public static FactionDef OG_Astartes_Faction;

        #region Mechanicus
        [MayRequireXenobiologis]
        public static FactionDef OG_Mechanicus_Faction;
        #endregion
        
        #region Militarum
        [MayRequireXenobiologis]
        public static FactionDef OG_Militarum_Cadian_Faction;
        [MayRequireXenobiologis]
        public static FactionDef OG_Militarum_PlayerColony;
        #endregion
        
        #region Chaos
        [MayRequireXenobiologis]
        public static FactionDef OG_Chaos_Deamon_Faction;
        #endregion

        #region Tau
        [MayRequireXenobiologis, MayRequireTau]
        public static FactionDef OG_Tau_Faction;
        #endregion

        #region Kroot
        [MayRequireTau]
        public static FactionDef OG_Kroot_Faction;
        [MayRequireTau]
        public static FactionDef OG_Kroot_PlayerColony;
        [MayRequireTau]
        public static FactionDef OG_Kroot_PlayerTribe;
        #endregion
        
        #region Eldar
        [MayRequireXenobiologis, MayRequireEldar]
        public static FactionDef OG_Eldar_Craftworld_Faction;
        /*
        [MayRequireXenobiologis, MayRequireEldar]
        public static FactionDef OG_Eldar_Exodite_Faction;
        [MayRequireXenobiologis, MayRequireEldar]
        public static FactionDef OG_Eldar_Harlequinn_Faction;
        [MayRequireXenobiologis, MayRequireEldar]
        public static FactionDef OG_Eldar_Corsair_Faction;
        */
        [MayRequireEldar]
        public static FactionDef OG_Eldar_Craftworld_PlayerColony;
        /*
        [MayRequireEldar]
        public static FactionDef OG_Eldar_Exodite_PlayerTribe;
        */
        #endregion

        #region Orkz
        [MayRequireXenobiologis, MayRequireOrkz]
        public static FactionDef OG_Ork_Tek_Faction;
        [MayRequireXenobiologis, MayRequireOrkz]
        public static FactionDef OG_Ork_Feral_Faction;
        [MayRequireXenobiologis, MayRequireOrkz]
        public static FactionDef OG_Ork_Rok;
        [MayRequireXenobiologis, MayRequireOrkz]
        public static FactionDef OG_Ork_Hulk;
        [MayRequireOrkz]
        public static FactionDef OG_Ork_PlayerColony;
        [MayRequireOrkz]
        public static FactionDef OG_Ork_PlayerTribe;
        #endregion

        #region Necron
        [MayRequireXenobiologis]
        public static FactionDef OG_Necron_Faction;
        #endregion

        #region Tyranid
        [MayRequireXenobiologis]
        public static FactionDef OG_Tyranid_Faction;
        #endregion
    }
}
