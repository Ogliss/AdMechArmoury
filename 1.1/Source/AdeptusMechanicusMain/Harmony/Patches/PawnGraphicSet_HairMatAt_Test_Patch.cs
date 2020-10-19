using System.Linq;
using Verse;
using UnityEngine;

namespace AdeptusMechanicus.HarmonyInstance
{
    //    [HarmonyPatch(typeof(PawnGraphicSet), "HairMatAt_NewTemp")]
    public static class PawnGraphicSet_HairMatAt_Test_Patch
    {
        public static void Postfix(PawnGraphicSet __instance, ref Material __result)
        {
            Pawn pawn = __instance.pawn;
            if (pawn.apparel.AnyApparel)
            {
                if (pawn.apparel.WornApparel.Any(x=> x.def.defName == "OGT_Apparel_XV25Battlesuit"))
                {
                    __result.SetTexture(AMConstants.InvisibleGraphics(pawn).headGraphic.MatSingle.name, AMConstants.InvisibleGraphics(pawn).headGraphic.MatSingle.mainTexture);
                    __result.shader = ShaderDatabase.Cutout;
                    return;
                }
            }
        }
    }

}
