using RimWorld;
using System;
using Verse;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class AdeptusMentalStateDefOf
    {
        static AdeptusMentalStateDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AdeptusMentalStateDefOf));
        }
        [MayRequireOrkz]
        public static MentalStateDef OGO_Ork_Scrapping;
        [MayRequireOrkz]
        public static MentalStateDef OGO_Orkoid_Starving;
        [MayRequireXenobiologis]
        public static MentalStateDef OG_MentalState_ChaosDeamon;
    }
}
