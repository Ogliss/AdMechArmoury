using Verse;
using System.Collections.Generic;

namespace AdeptusMechanicus.HarmonyInstance
{
    public abstract class CompWearable : ThingComp
    {
        public virtual IEnumerable<Gizmo> CompGetGizmosWorn()
        {
            // return no Gizmos
            return new List<Gizmo>();
        }
    }

}