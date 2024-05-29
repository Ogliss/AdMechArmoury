using Verse;
using System.Collections.Generic;

namespace AdeptusMechanicus
{
    public class InheritRecipesDefExtension : DefModExtension
    {
        public List<ThingDef> workbenches = new List<ThingDef>();

        public override IEnumerable<string> ConfigErrors()
        {
            if (true)
            {

            }
            return base.ConfigErrors();
        }
    }
}
