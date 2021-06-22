using RimWorld;

namespace AdeptusMechanicus
{
    [DefOf]
    public static class AdeptusPawnsArrivalModeDefOf
    {
        static AdeptusPawnsArrivalModeDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AdeptusPawnsArrivalModeDefOf));
        }
        public static PawnsArrivalModeDef OG_DeepStrike_DropPod_TargetPower;
        public static PawnsArrivalModeDef OG_DeepStrike_DropPod_TargetEnemies;
        public static PawnsArrivalModeDef OG_DeepStrike_Teleport_TargetPower;
        public static PawnsArrivalModeDef OG_DeepStrike_Teleport_TargetEnemies;
        public static PawnsArrivalModeDef OG_DeepStrike_Tunnel_TargetPower;
        public static PawnsArrivalModeDef OG_DeepStrike_Tunnel_TargetEnemies;
        public static PawnsArrivalModeDef OG_DeepStrike_Fly_TargetPower;
        public static PawnsArrivalModeDef OG_DeepStrike_Fly_TargetEnemies;

    }
}
