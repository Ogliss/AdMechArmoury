using RimWorld;
using System;
using Verse;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class AdeptusBodyDefOf
    {
        static AdeptusBodyDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AdeptusBodyDefOf));
        }

        #region Kroot
        [MayRequireXenobiologis, MayRequireTau]
        public static BodyDef OG_Kroot_Body;
        [MayRequireXenobiologis, MayRequireTau]
        public static BodyDef OG_Kroothound_Body;
        [MayRequireXenobiologis, MayRequireTau]
        public static BodyDef OG_KrootOx_Body;
        [MayRequireXenobiologis, MayRequireTau]
        public static BodyDef OG_Knarloc_Body;
        #endregion

        #region Tau
        [MayRequireXenobiologis, MayRequireTau]
        public static BodyDef OG_Tau_Body;
        #endregion

        #region Vespid
        [MayRequireXenobiologis, MayRequireTau]
        public static BodyDef OG_Vespid_Body;
        #endregion

    }
}
