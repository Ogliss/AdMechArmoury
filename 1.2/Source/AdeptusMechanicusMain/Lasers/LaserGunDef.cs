using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace AdeptusMechanicus.Lasers
{
    // 	AdeptusMechanicus.Lasers.LaserGunDef
    public class LaserGunDef : ThingDef
    {
        public static LaserGunDef defaultObj = new LaserGunDef();

        public float barrelLength = 0.9f;
        public float barrelOffset = 0.0f;
        public float bulletOffset = 0.2f;
        public string muzzleFlareDef;
        public float muzzleFlareSize = 1f;
        public FloatRange? muzzleFlareSizeRange;

        public string muzzleSmokeDef;
        public float muzzleSmokeSize = 0.35f;
        public FloatRange? muzzleSmokeSizeRange;
        public bool supportsColors = false;

        public LaserGunDef()
        {
            this.thingClass = typeof(LaserGun);
        }
    }
}
