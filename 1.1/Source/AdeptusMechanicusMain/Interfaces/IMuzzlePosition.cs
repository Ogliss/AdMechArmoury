using Verse;

namespace AdeptusMechanicus
{
    public interface IMuzzlePosition
    {
        float BarrelLength
        {
            get;
        }
        float BulletOffset
        {
            get;
        }
        float BarrelOffset
        {
            get;
        }
        public float MuzzleSmokeSize
        {
            get;
        }
        public float MuzzleFlareSize
        {
            get;
        }
        ThingDef MuzzleFlareDef
        {
            get;
        }
        ThingDef MuzzleSmokeDef
        {
            get;
        }
    }

}
