using Verse;

namespace AdeptusMechanicus
{
    public class VerbPropertiesOGJam : VerbProperties
    {
        public Reliability reliability = Reliability.ST;

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
                    ", reliability=",
                    this.reliability.ToString()
                });
            }
            return "VerbProperties(" + str + ")";
        }
    }

    public enum Reliability : short
    {
        UR = 80,
        ST = 55,
        VR = 30,
        NA = 0
    }

}
