using Verse;

namespace AdeptusMechanicus
{
    public class ThingDef_BulletWC : ThingDef
    {
        public float DetonationChance = 0.05f; //The default chance of detonation.
        public float blastRadius = 0.05f; //explosionRadius
        public DamageDef blastdamageDef = null; //damageDef
        public int blastdamageAmount = 0; //damageAmount
        public float blastarmorPenetration = 0.05f; //armorPenetration
        public SoundDef blastsoundExplode = null; //soundExplode
    }
}