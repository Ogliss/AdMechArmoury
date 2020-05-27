using Verse;

namespace AdeptusMechanicus
{
    public class VerbPropertiesOH : VerbProperties
    {
        public Overheat overheat = Overheat.ST;

        protected float weaponDamageMultiplier = 1f;

        public int Damage
        {
            get
            {
                return this.defaultProjectile.projectile.GetDamageAmount(this.defaultProjectile.projectile.damageDef.defaultDamage, null);
            }
        }


        public override string ToString()
        {
            string str;
            if (!this.label.NullOrEmpty())
            {
                str = this.label;
            }
            else
            {
                str = string.Concat(new object[]
                {
                    "range=",
                    this.range,
                    ", projectile=",
                    (this.defaultProjectile == null) ? "null" : this.defaultProjectile.defName,
                    ", overheat=",
                    this.overheat.ToString()
                });
            }
            return "VerbProperties(" + str + ")";
        }
    }

    public enum Overheat : short
    {
        UR = 80,
        ST = 55,
        VR = 30,
        NA = 0
    }

}
