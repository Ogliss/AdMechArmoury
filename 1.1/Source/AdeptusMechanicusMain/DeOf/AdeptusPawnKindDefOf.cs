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
        [MayRequireXenobiologis]
        public static PawnKindDef OG_Ork_Snotling;

        #endregion

    }
}
