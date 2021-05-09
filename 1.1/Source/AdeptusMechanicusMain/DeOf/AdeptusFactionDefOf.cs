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

        #region Kroot
        [MayRequireTau]
        public static FactionDef OG_Kroot_Faction;
        [MayRequireTau]
        public static FactionDef OG_Kroot_PlayerColony;
        [MayRequireTau]
        public static FactionDef OG_Kroot_PlayerTribe;
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

    }
}
