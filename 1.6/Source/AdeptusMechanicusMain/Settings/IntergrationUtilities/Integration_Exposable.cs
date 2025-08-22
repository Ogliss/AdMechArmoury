using Verse;

namespace AdeptusMechanicus
{
    public abstract class Integration_Exposable : Integration, IExposable
    {
        public virtual void ExposeData()
        { }
    }

}
