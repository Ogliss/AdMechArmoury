using Verse;

namespace AdeptusMechanicus
{
    static class UtilTieredAndroids
    {
        public static bool TieredAndroid = false;
        public static ModContentPack modContent = null;
        static UtilTieredAndroids()
        {
            foreach (ModContentPack p in LoadedModManager.RunningMods)
            {
                if (p.PackageIdPlayerFacing == ("Atlas.AndroidTiers"))
                {
                    modContent = p;
                //    log.message(string.Format("{0}: PackageId: {1}, PackageIdPlayerFacing: {2}", p.Name, p.PackageId, p.PackageIdPlayerFacing));
                    TieredAndroid = true;
                }
            }
        }

        public static bool isAtlasAndroid(PawnKindDef pawn)
        {

            bool Result = false;
            if (pawn.race.modContentPack == modContent)
            {
                Result = true;
            }
            return Result;
        }
        public static bool isAtlasAndroid(Pawn pawn)
        {
            bool Result = false;
            if (pawn.def.modContentPack == modContent)
            {
                Result = true;
            }
            return Result;
        }
        public static bool isAtlasAndroid(ThingDef td)
        {
            bool Result = false;
            if (td.modContentPack == modContent)
            {
                Result = true;
            }
            return Result;
        }
    }
}