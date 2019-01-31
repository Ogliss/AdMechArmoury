using System;
using Verse;

namespace RimWorld
{
    // Token: 0x02000956 RID: 2390
    [DefOf]
    public static class OGHiveLikeDefOf
    {
        // Token: 0x06003781 RID: 14209 RVA: 0x001A8393 File Offset: 0x001A6793
        static OGHiveLikeDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(OGHiveLikeDefOf));
        }

        //    public static HediffDef RRY_FaceHuggerInfection;
        //    public static HediffDef RRY_XenomorphImpregnation;
        public static ThingDef HiveLike;
        public static ThingDef TunnelHiveLikeSpawner; 

        //   public static FactionDef OGChaosDeamonFaction;

        //   public static DamageDef OGCDWarpfire;

        //   public static PawnKindDef ChaosDeamonFlamer;

    }
}
