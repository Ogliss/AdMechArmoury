using System.Linq;
using Verse;
using UnityEngine;
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{
    public static class PawnGraphicSet_HairMatAt_NewTemp_Test_Patch
    {
        public static void Postfix(PawnGraphicSet __instance, ref Material __result)
        {
            Pawn pawn = __instance.pawn;
            if (pawn == null || !pawn.RaceProps.Humanlike)
            {
                return;
            }
            if (pawn.apparel.AnyApparel)
            {
                if (pawn.apparel.WornApparel.Any(x => x.def.HasComp(typeof(CompApparelExtraPartDrawer))))
                {
                    foreach (var item in pawn.apparel.WornApparel)
                    {
                        if (item.def.HasComp(typeof(CompApparelExtraPartDrawer)))
                        {
                            CompApparelExtraPartDrawer extraDrawer = item.TryGetCompFast<CompApparelExtraPartDrawer>();
                            if (extraDrawer != null && extraDrawer.hidesHair)
                            {
                                __result = AMConstants.InvisibleGraphics(pawn).hairGraphic.MatSingle;
                                return;
                            }
                        }
                    }
                }
            }
        }
    }

}
