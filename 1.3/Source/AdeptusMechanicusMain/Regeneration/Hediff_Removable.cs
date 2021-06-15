using Verse;

namespace AdeptusMechanicus
{
    public class Hediff_Removable : Hediff
    {
        public override bool ShouldRemove
        {
            get
            {
                return true;
            }
        }

        public Hediff_Removable()
        {
        }
    }
}
