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
        private FleckDef _muzzleFlareDef;
        public FloatRange? muzzleFlareSizeRange;

        public string muzzleSmokeDef = "OG_Mote_SmokeTrail";
        public float muzzleSmokeSize = 0.35f;
        private FleckDef _muzzleSmokeDef;
        public FloatRange? muzzleSmokeSizeRange;

        public float BarrelOffset => barrelOffset;
        public float BarrelLength => barrelLength;
        public float BulletOffset => bulletOffset;
        public float MuzzleSmokeSize => muzzleSmokeSizeRange?.RandomInRange ?? muzzleSmokeSize;
        public float MuzzleFlareSize => muzzleFlareSizeRange?.RandomInRange ?? muzzleFlareSize;
        public FleckDef MuzzleFlareDef => _muzzleFlareDef ??= !muzzleFlareDef.NullOrEmpty() ? DefDatabase<FleckDef>.GetNamedSilentFail(muzzleFlareDef) : null;
        public FleckDef MuzzleSmokeDef => _muzzleSmokeDef ??= !muzzleSmokeDef.NullOrEmpty() ? DefDatabase<FleckDef>.GetNamedSilentFail(muzzleSmokeDef) : null;

    }

}
