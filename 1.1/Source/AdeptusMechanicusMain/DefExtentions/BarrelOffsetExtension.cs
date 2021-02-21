using Verse;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.BarrelOffsetExtension
    public class BarrelOffsetExtension : DefModExtension, IMuzzlePosition
    {
        public float barrelLength = 1f;
        public float barrelOffset = 0f;
        public float bulletOffset = 0.2f;

        public string muzzleFlareDef = "Mote_SparkFlash";
        public float muzzleFlareSize = 1f;
        private ThingDef _muzzleFlareDef;
        public FloatRange? muzzleFlareSizeRange;

        public string muzzleSmokeDef = "OG_Mote_SmokeTrail";
        public float muzzleSmokeSize = 0.35f;
        private ThingDef _muzzleSmokeDef;
        public FloatRange? muzzleSmokeSizeRange;

        public float BarrelOffset => barrelOffset;
        public float BarrelLength => barrelLength;
        public float BulletOffset => bulletOffset;
        public float MuzzleSmokeSize => muzzleSmokeSizeRange?.RandomInRange ?? muzzleSmokeSize;
        public float MuzzleFlareSize => muzzleFlareSizeRange?.RandomInRange ?? muzzleFlareSize;
        public ThingDef MuzzleFlareDef => _muzzleFlareDef ??= !muzzleFlareDef.NullOrEmpty() ? DefDatabase<ThingDef>.GetNamedSilentFail(muzzleFlareDef) : null;
        public ThingDef MuzzleSmokeDef => _muzzleSmokeDef ??= !muzzleSmokeDef.NullOrEmpty() ? DefDatabase<ThingDef>.GetNamedSilentFail(muzzleSmokeDef) : null;

    }

}
