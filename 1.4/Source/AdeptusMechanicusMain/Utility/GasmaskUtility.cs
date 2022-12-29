using AdeptusMechanicus.ExtensionMethods;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    [StaticConstructorOnStartup]
    public static class GasmaskUtility
    {

        public static bool WearingGasmask(Pawn p, out Apparel a)
        {
            a = null;
            if (p.apparel != null)
            {
                for (int i = 0; i < p.apparel.WornApparelCount; i++)
                {
                    if (p.apparel.WornApparel[i] is Apparel apparel && apparel.def.apparel.immuneToToxGasExposure && GasmaskUtility.GasMasks.Contains(apparel.def))
                    {
                        a = apparel;
                        return true;
                    }
                }
            }
            return false;
        }

        private static List<ThingDef> gasMasks;
        public static List<ThingDef> GasMasks
        {
            get
            {
                if (gasMasks.NullOrEmpty())
                {
                    if (gasMasks == null)
                    {
                        gasMasks = new List<ThingDef>();
                        gasMasks.AddRange(DefDatabase<ThingDef>.AllDefs.Where(x => x.IsApparel && x.HasModExtension<GasmaskExtentsion>()));
                    }
                }
                return gasMasks;
            }
        }
    }
    
}
