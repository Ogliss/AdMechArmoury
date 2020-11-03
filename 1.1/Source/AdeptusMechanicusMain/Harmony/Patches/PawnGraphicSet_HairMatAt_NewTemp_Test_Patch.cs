using System.Linq;
using Verse;
using UnityEngine;

namespace AdeptusMechanicus.HarmonyInstance
{
    public static class PawnGraphicSet_HairMatAt_NewTemp_Test_Patch
    {
        public static void Postfix(PawnGraphicSet __instance, ref Material __result)
        {
            Pawn pawn = __instance.pawn;
            if (pawn.apparel.AnyApparel)
            {
                if (pawn.apparel.WornApparel.Any(x => x.TryGetComp<CompApparelExtraPartDrawer>() != null))
                {
                    foreach (var item in pawn.apparel.WornApparel)
                    {
                        CompApparelExtraPartDrawer extraDrawer = item.TryGetComp<CompApparelExtraPartDrawer>();
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
