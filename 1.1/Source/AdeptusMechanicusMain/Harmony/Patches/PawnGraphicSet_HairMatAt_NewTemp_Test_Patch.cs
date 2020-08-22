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
                if (pawn.apparel.WornApparel.Any(x => x.TryGetComp<CompApparelExtraDrawer>() != null))
                {
                    foreach (var item in pawn.apparel.WornApparel)
                    {
                        CompApparelExtraDrawer extraDrawer = item.TryGetComp<CompApparelExtraDrawer>();
                        if (extraDrawer != null && extraDrawer.hidesHair)
                        {
                            __result = AMConstants.Invisiblegraphics(pawn).hairGraphic.MatSingle;
                            return;
                        }
                    }
                }
            }
        }
    }

}
