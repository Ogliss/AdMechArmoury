using System.Linq;
using Verse;
using UnityEngine;
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{
    //   [HarmonyPatch(typeof(PawnGraphicSet), "HeadMatAt")]
    public static class PawnGraphicSet_HeadMatAt_Test_Patch
    {
        public static void Postfix(PawnGraphicSet __instance, ref bool stump, ref Material __result)
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
                            if (extraDrawer != null && extraDrawer.hidesHead)
                            {
                                __result.SetTexture(AMConstants.InvisibleGraphics(pawn).headGraphic.MatSingle.name, AMConstants.InvisibleGraphics(pawn).headGraphic.MatSingle.mainTexture);
                                __result.shader = ShaderDatabase.Cutout;
                                return;
                            }
                        }
                    }
                }
            }
        }
    }

}
