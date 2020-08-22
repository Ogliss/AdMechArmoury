using Verse;
using HarmonyLib;
using UnityEngine;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Pawn), "get_DrawPos")]
    public static class Pawn_get_DrawPos_CompFloater_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn __instance, ref Vector3 __result)
        {
            CompFloating floater = __instance.TryGetComp<CompFloating>();
            if (floater!=null)
            {
            //    Log.Message("get_DrawPos patch for floater " + __instance);
                if (floater.Props.useZOffset)
                {
                //    Log.Message("get_DrawPos modified by " + floater.Props.zOffset + " for floater " + __instance);
                    __result.z += floater.Props.zOffset;
                }
            }

        }
    }
    
}
