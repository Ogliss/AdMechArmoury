using Verse;
using HarmonyLib;
using UnityEngine;
using RimWorld;
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Pawn), "get_DrawPos")]
    public static class Pawn_get_DrawPos_CompFloater_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn __instance, ref Vector3 __result)
        {
            /*
            CompFloating floater = __instance.TryGetCompFast<CompFloating>();
            if (floater!=null && !__instance.Dead && !__instance.Downed && __instance.Awake())
            {
            //    Log.Message("get_DrawPos patch for floater " + __instance);
                if (floater.Props.useZOffset)
                {
                //    Log.Message("get_DrawPos modified by " + floater.Props.zOffset + " for floater " + __instance);
                    __result.z += floater.Props.zOffset;
                }
            }
            */
            FloatingPawnExtension floater = __instance.def.GetModExtensionFast<FloatingPawnExtension>();
            if (floater != null && !__instance.Dead && !__instance.Downed && __instance.Awake())
            {
                //    Log.Message("get_DrawPos patch for floater " + __instance);
                if (floater.useZOffset)
                {
                    //    Log.Message("get_DrawPos modified by " + floater.Props.zOffset + " for floater " + __instance);
                    __result.z += floater.zOffset;
                }
            }


        }
    }
    
}
